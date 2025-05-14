resource "aws_ssm_parameter" "jump_box_pem_key" {
  name  = "/platform-apis-jump-box-pem-key"
  type  = "String"
  value = "to_be_set_manually"

  lifecycle {
    ignore_changes = [
      value,
    ]
  }
}

resource "aws_ssm_parameter" "jump_box_instance_name" {
  name  = "/platform-apis-jump-box-instance-name"
  type  = "String"
  value = "to_be_set_manually"

  lifecycle {
    ignore_changes = [
      value,
    ]
  }
}
