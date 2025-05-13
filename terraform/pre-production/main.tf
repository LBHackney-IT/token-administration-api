provider "aws" {
  region  = "eu-west-2"
  version = "~> 2.0"
}

data "aws_caller_identity" "current" {}
data "aws_region" "current" {}

locals {
  environment      = "pre-production"
  application_name = "auth token generator api"
  parameter_store  = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
}

terraform {
  backend "s3" {
    bucket         = "housing-pre-production-terraform-state"
    encrypt        = true
    region         = "eu-west-2"
    key            = "services/auth-token-generator-api/state"
    dynamodb_table = "housing-pre-production-terraform-state-lock"
  }
}

/*    POSTGRES SET UP    */
data "aws_vpc" "pre_production_vpc" {
  tags = {
    Name = "housing-pre-prod-pre-prod"
  }
}

data "aws_subnet_ids" "pre_production" {
  vpc_id = data.aws_vpc.pre_production_vpc.id
  filter {
    name   = "tag:Type"
    values = ["private"]
  }
}

data "aws_ssm_parameter" "auth_token_generator_postgres_password" {
  name = "/api-auth-token-generator/${local.environment}/postgres-password"
}

data "aws_ssm_parameter" "auth_token_generator_postgres_username" {
  name = "/api-auth-token-generator/${local.environment}/postgres-username"
}

module "postgres_db_pre_production" {
  source                         = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/postgres"
  environment_name               = local.environment
  vpc_id                         = data.aws_vpc.pre_production_vpc.id
  db_engine                      = "postgres"
  db_engine_version              = "16.4"
  db_allow_major_version_upgrade = true
  db_identifier                  = "auth-token-generator-pre-prod-db"
  db_instance_class              = "db.t3.micro"
  db_name                        = "auth_token_generator_db"
  db_port                        = 5103
  db_username                    = data.aws_ssm_parameter.auth_token_generator_postgres_username.value
  db_password                    = data.aws_ssm_parameter.auth_token_generator_postgres_password.value
  subnet_ids                     = data.aws_subnet_ids.pre_production.ids
  db_allocated_storage           = 20
  maintenance_window             = "sun:10:00-sun:10:30"
  storage_encrypted              = true
  multi_az                       = false
  publicly_accessible            = false
  project_name                   = "pre-prod platform apis"
  deletion_protection            = true
  copy_tags_to_snapshot          = true
  additional_tags = {
    BackupPolicy    = "Stg"
    Environment     = "prod"
    Application     = "MTFH Housing Pre-Production"
    TeamEmail       = "developementteam@hackney.gov.uk"
    Confidentiality = "Internal"
  }
}
