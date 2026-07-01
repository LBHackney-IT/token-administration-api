# INSTRUCTIONS:
# 1) ENSURE YOU POPULATE THE LOCALS
# 2) ENSURE YOU REPLACE ALL INPUT PARAMETERS, THAT CURRENTLY STATE 'ENTER VALUE', WITH VALID VALUES
# 3) YOUR CODE WOULD NOT COMPILE IF STEP NUMBER 2 IS NOT PERFORMED!
# 4) ENSURE YOU CREATE A BUCKET FOR YOUR STATE FILE AND YOU ADD THE NAME BELOW - MAINTAINING THE STATE OF THE INFRASTRUCTURE YOU CREATE IS ESSENTIAL - FOR APIS, THE BUCKETS ALREADY EXIST
# 5) THE VALUES OF THE COMMON COMPONENTS THAT YOU WILL NEED ARE PROVIDED IN THE COMMENTS
# 6) IF ADDITIONAL RESOURCES ARE REQUIRED BY YOUR API, ADD THEM TO THIS FILE
# 7) ENSURE THIS FILE IS PLACED WITHIN A 'terraform' FOLDER LOCATED AT THE ROOT PROJECT DIRECTORY

provider "aws" {
  region  = "eu-west-2"
  version = "~> 3.0"
}

data "aws_caller_identity" "current" {}
data "aws_region" "current" {}

locals {
  application_name = "auth token generator api"
  parameter_store  = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"

  token_db_port        = 5102
  token_db_name        = "auth_token_generator_db"
  jumpbox_sg_id        = "sg-04bc7744258d113d2"
  token_api_sg_id      = "sg-0c935c1e1df0a4d4c"
  new_authorizer_sg_id = "sg-01002f004e60edfe6"
}

terraform {
  backend "s3" {
    bucket  = "terraform-state-staging-apis"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/auth-token-generator-api/state"
  }
}

/*    POSTGRES SET UP    */
data "aws_vpc" "staging_vpc" {
  tags = {
    Name = "apis-stg"
  }
}

data "aws_subnet_ids" "staging" {
  vpc_id = data.aws_vpc.staging_vpc.id
  filter {
    name   = "tag:Type"
    values = ["private"]
  }
}

data "aws_ssm_parameter" "auth_token_generator_postgres_password" {
  name = "/api-auth-token-generator/staging/postgres-password"
}

data "aws_ssm_parameter" "auth_token_generator_postgres_username" {
  name = "/api-auth-token-generator/staging/postgres-username"
}

module "postgres_db_staging" {
  source = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/postgres"

  environment_name = "staging"
  project_name     = "platform apis"
  db_identifier    = "auth-token-generator-staging-db"

  vpc_id              = data.aws_vpc.staging_vpc.id
  subnet_ids          = data.aws_subnet_ids.staging.ids
  multi_az            = false
  publicly_accessible = false

  db_instance_class    = "db.t3.micro"
  db_allocated_storage = 20
  storage_encrypted    = false

  db_engine         = "postgres"
  db_engine_version = "16.13"
  db_username       = data.aws_ssm_parameter.auth_token_generator_postgres_username.value
  db_password       = data.aws_ssm_parameter.auth_token_generator_postgres_password.value
  db_name           = "auth_token_generator_db"
  db_port           = 5102

  copy_tags_to_snapshot = true
  maintenance_window    = "sun:10:00-sun:10:30"

  additional_tags = {
    BackupPolicy = "Stg"
  }
}

resource "aws_security_group_rule" "allow_jumpbox_traffic" {
  type                     = "ingress"
  from_port                = local.token_db_port
  to_port                  = local.token_db_port
  protocol                 = "tcp"
  source_security_group_id = local.jumpbox_sg_id
  security_group_id        = module.postgres_db_staging.db_security_group_id
}

resource "aws_security_group_rule" "allow_token_api_traffic" {
  type                     = "ingress"
  from_port                = local.token_db_port
  to_port                  = local.token_db_port
  protocol                 = "tcp"
  source_security_group_id = local.token_api_sg_id
  security_group_id        = module.postgres_db_staging.db_security_group_id
}

resource "aws_security_group_rule" "allow_new_authorizer_traffic" {
  type                     = "ingress"
  from_port                = local.token_db_port
  to_port                  = local.token_db_port
  protocol                 = "tcp"
  source_security_group_id = local.new_authorizer_sg_id
  security_group_id        = module.postgres_db_staging.db_security_group_id
}
