# INSTRUCTIONS:
# 1) ENSURE YOU POPULATE THE LOCALS
# 2) ENSURE YOU REPLACE ALL INPUT PARAMETERS, THAT CURRENTLY STATE 'ENTER VALUE', WITH VALID VALUES
# 3) YOUR CODE WOULD NOT COMPILE IF STEP NUMBER 2 IS NOT PERFORMED!
# 4) ENSURE YOU CREATE A BUCKET FOR YOUR STATE FILE AND YOU ADD THE NAME BELOW - MAINTAINING THE STATE OF THE INFRASTRUCTURE YOU CREATE IS ESSENTIAL - FOR APIS, THE BUCKETS ALREADY EXIST
# 5) THE VALUES OF THE COMMON COMPONENTS THAT YOU WILL NEED ARE PROVIDED IN THE COMMENTS
# 6) IF ADDITIONAL RESOURCES ARE REQUIRED BY YOUR API, ADD THEM TO THIS FILE
# 7) ENSURE THIS FILE IS PLACED WITHIN A 'terraform' FOLDER LOCATED AT THE ROOT PROJECT DIRECTORY

terraform {
  backend "s3" {
    bucket  = "terraform-state-development-apis"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/auth-token-generator-api/state"
  }

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 2.0"
    }
  }
}

provider "aws" {
  region  = "eu-west-2"
}

data "aws_caller_identity" "current" {}
data "aws_region" "current" {}

locals {
  application_name = "auth token generator api"
  parameter_store = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"

  token_db_port = 5101
}

/*    POSTGRES SET UP    */
data "aws_vpc" "development_vpc" {
  tags = {
    Name = "apis-dev"
  }
}

data "aws_subnet_ids" "development" {
  vpc_id = data.aws_vpc.development_vpc.id
  filter {
    name   = "tag:Type"
    values = ["private"]
  }
}

data "aws_ssm_parameter" "auth_token_generator_postgres_password" {
  name = "/api-auth-token-generator/development/postgres-password"
}

data "aws_ssm_parameter" "auth_token_generator_postgres_username" {
  name = "/api-auth-token-generator/development/postgres-username"
}

module "postgres_db_development" {
  source = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/postgres"

  environment_name = "development"
  project_name = "platform apis"
  db_identifier = "auth-token-generator-dev-db"

  vpc_id = data.aws_vpc.development_vpc.id
  subnet_ids = data.aws_subnet_ids.development.ids
  multi_az = false
  publicly_accessible = false

  db_instance_class = "db.t3.micro"
  db_allocated_storage = 20
  storage_encrypted = false

  db_engine = "postgres"
  db_engine_version = "16.8"
  db_username = data.aws_ssm_parameter.auth_token_generator_postgres_username.value
  db_password = data.aws_ssm_parameter.auth_token_generator_postgres_password.value
  db_name = "auth_token_generator_db"
  db_port  = locals.token_db_port

  copy_tags_to_snapshot = true
  maintenance_window ="sun:10:00-sun:10:30"

  additional_tags = {
    BackupPolicy = "Dev"
  }
}

resource "aws_ssm_parameter" "postgres_hostname" {
  name  = "/api-auth-token-generator/development/postgres-hostname"
  type  = "String" 
  
  value = module.postgres_db_development.db_instance_endpoint 

  tags = {
    Project     = "platform apis"
  }
}

resource "aws_security_group_rule" "allow_jumpbox_traffic" {
  type              = "ingress"
  from_port         = locals.token_db_port
  to_port           = locals.token_db_port
  protocol          = "tcp"
  source_security_group_id = "sg-0a457bf4e6eda31de"
  security_group_id = module.postgres_db_development.db_security_group_id
}
