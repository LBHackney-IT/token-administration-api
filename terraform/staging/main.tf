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
  version = "~> 2.0"
}
data "aws_caller_identity" "current" {}
data "aws_region" "current" {}
locals {
  application_name = "auth token generator api"
   parameter_store = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
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
    Name = "vpc-staging-apis-staging"  
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
  vpc_id = data.aws_vpc.staging_vpc.id
  db_engine = "postgres"
  db_engine_version = "11.1"
  db_identifier = "auth-token-generator-staging-db"
  db_instance_class = "db.t2.micro"
  db_name = "auth_token_generator_db"
  db_port  = 5102
  db_username = data.aws_ssm_parameter.auth_token_generator_postgres_username.value
  db_password = data.aws_ssm_parameter.auth_token_generator_postgres_password.value
  subnet_ids = data.aws_subnet_ids.staging.ids
  db_allocated_storage = 20
  maintenance_window ="sun:10:00-sun:10:30"
  storage_encrypted = false
  multi_az = false //only true if production deployment
  publicly_accessible = false
  project_name = "platform apis"
}