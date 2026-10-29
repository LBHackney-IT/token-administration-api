#!/usr/bin/env python3
"""Detach this API's Lambda from its pre-production security group.

The AWS provider waits at least 45 minutes when deleting a security group that
still has a Lambda ENI, and it ignores a shorter delete timeout. This script
removes that ENI first and gives up after a few minutes if something else is
still holding it.
"""

import argparse
import json
import subprocess
import sys
import time

REGION = "eu-west-2"
POLL_SECONDS = 15
OUR_FUNCTION_PREFIX = "token-administration-api-"


def aws(args, ignore_not_found=False):
    result = subprocess.run(
        ["aws", *args, "--region", REGION, "--output", "json"],
        capture_output=True,
        text=True,
    )
    if result.returncode != 0:
        message = (result.stderr or result.stdout).strip()
        if ignore_not_found and "ResourceNotFoundException" in message:
            return None
        raise RuntimeError(message)
    if not result.stdout.strip():
        return {}
    return json.loads(result.stdout)


def aws_allow_fail(args):
    try:
        return aws(args)
    except RuntimeError as error:
        print(error)
        return None


def security_group_for_stage(serverless_yml, stage):
    lines = serverless_yml.splitlines()
    in_stage = False
    stage_indent = None
    for line in lines:
        stripped = line.strip()
        indent = len(line) - len(line.lstrip(" "))
        if stripped == f"{stage}:":
            in_stage = True
            stage_indent = indent
            continue
        if not in_stage:
            continue
        if stripped.endswith(":") and indent <= stage_indent:
            break
        token = stripped.lstrip("- ").strip()
        if token.startswith("sg-"):
            return token
    raise RuntimeError(f"No security group found for stage {stage}")


def paginate(command, collection_key):
    marker = None
    while True:
        args = list(command)
        if marker:
            args.extend(["--marker", marker])
        page = aws(args)
        yield from page.get(collection_key, [])
        marker = page.get("NextMarker")
        if not marker:
            break


def security_groups_of(function_config):
    return (function_config.get("VpcConfig") or {}).get("SecurityGroupIds") or []


def functions_using_security_group(security_group_id):
    holders = []
    for function in paginate(["lambda", "list-functions"], "Functions"):
        name = function["FunctionName"]
        for version in paginate(
            ["lambda", "list-versions-by-function", "--function-name", name],
            "Versions",
        ):
            if security_group_id in security_groups_of(version):
                holders.append(f"{name}:{version.get('Version', '$LATEST')}")
    return holders


def delete_our_function(function_name):
    mappings = aws(
        ["lambda", "list-event-source-mappings", "--function-name", function_name],
        ignore_not_found=True,
    )
    if not mappings:
        print(f"Lambda function {function_name} is already gone")
        return
    for mapping in mappings.get("EventSourceMappings", []):
        print(f"Deleting event source mapping {mapping['UUID']} on {function_name}")
        aws(["lambda", "delete-event-source-mapping", "--uuid", mapping["UUID"]])
    print(f"Deleting Lambda function {function_name}")
    aws(["lambda", "delete-function", "--function-name", function_name], ignore_not_found=True)


def network_interfaces(security_group_id):
    described = aws(
        [
            "ec2",
            "describe-network-interfaces",
            "--filters",
            f"Name=group-id,Values={security_group_id}",
        ]
    )
    return described.get("NetworkInterfaces", [])


def default_security_group(vpc_id):
    described = aws(
        [
            "ec2",
            "describe-security-groups",
            "--filters",
            f"Name=vpc-id,Values={vpc_id}",
            "Name=group-name,Values=default",
        ]
    )
    groups = described.get("SecurityGroups", [])
    if not groups:
        return None
    return groups[0]["GroupId"]


def release_network_interface(eni, security_group_id):
    eni_id = eni["NetworkInterfaceId"]
    attachment = eni.get("Attachment") or {}
    status = eni.get("Status")
    print(
        f"ENI {eni_id} status={status} description={eni.get('Description')} "
        f"owner={attachment.get('InstanceOwnerId')} attachment={attachment.get('AttachmentId')}"
    )
    if status == "available":
        print(f"Deleting available ENI {eni_id}")
        aws_allow_fail(["ec2", "delete-network-interface", "--network-interface-id", eni_id])
        return

    default_group = default_security_group(eni.get("VpcId"))
    if default_group and default_group != security_group_id:
        print(f"Moving {eni_id} off {security_group_id} onto {default_group}")
        aws_allow_fail(
            [
                "ec2",
                "modify-network-interface-attribute",
                "--network-interface-id",
                eni_id,
                "--groups",
                default_group,
            ]
        )


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--serverless-yml", default="TokenAdministrationApi/serverless.yml")
    parser.add_argument("--stage", default="pre-production")
    parser.add_argument("--function-name", default="token-administration-api-pre-production")
    parser.add_argument("--timeout-seconds", type=int, default=300)
    args = parser.parse_args()

    with open(args.serverless_yml, encoding="utf-8") as handle:
        security_group_id = security_group_for_stage(handle.read(), args.stage)
    print(f"Releasing ENIs from {security_group_id} (timeout {args.timeout_seconds}s)")

    if not args.function_name.startswith(OUR_FUNCTION_PREFIX):
        raise RuntimeError(f"Refusing to delete unexpected function {args.function_name}")

    if not network_interfaces(security_group_id):
        print(f"No ENIs are attached to {security_group_id}")
        delete_our_function(args.function_name)
        return 0

    holders = functions_using_security_group(security_group_id)
    foreign = [holder for holder in holders if not holder.startswith(args.function_name + ":")]
    ours = [holder for holder in holders if holder.startswith(args.function_name + ":")]
    if ours:
        print(f"{security_group_id} is used by {', '.join(ours)}")
    delete_our_function(args.function_name)
    if foreign:
        print(f"{security_group_id} is still used by other Lambda functions: {', '.join(foreign)}")
        return 1

    deadline = time.time() + args.timeout_seconds
    while True:
        attached = network_interfaces(security_group_id)
        if not attached:
            print(f"No ENIs are attached to {security_group_id}")
            return 0
        for eni in attached:
            release_network_interface(eni, security_group_id)
        if time.time() >= deadline:
            print(
                f"{security_group_id} is still attached to Lambda ENIs after {args.timeout_seconds}s. "
                "Failing now so Terraform does not wait its built-in 45 minute ENI timeout."
            )
            return 1
        print(f"Waiting {POLL_SECONDS}s for {security_group_id} to be released")
        time.sleep(POLL_SECONDS)


if __name__ == "__main__":
    sys.exit(main())
