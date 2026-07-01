locals {
  rds_proxy_name                     = "auth-token-generator-prod-proxy"
  db_instance_identifier             = "auth-token-generator-prod-db-db-production"
  rds_proxy_secret_name              = "/api-auth-token-generator/production/rds-proxy-db-credentials"
  rds_proxy_secret_placeholder_value = "tbc"
  token_db_port                      = 5100
  token_db_name                      = "auth_token_generator_db"
  new_authorizer_sg_id               = "sg-01a5769a96ffcfa25"
}

resource "aws_secretsmanager_secret" "rds_proxy_db_credentials" {
  name = local.rds_proxy_secret_name

  tags = {
    Project     = "platform apis"
    Environment = "prod"
  }
}

resource "aws_secretsmanager_secret_version" "rds_proxy_db_credentials" {
  secret_id = aws_secretsmanager_secret.rds_proxy_db_credentials.id
  secret_string = jsonencode({
    username = local.rds_proxy_secret_placeholder_value
    password = local.rds_proxy_secret_placeholder_value
    engine   = "postgres"
    host     = local.rds_proxy_secret_placeholder_value
    port     = local.token_db_port
    dbname   = local.token_db_name
  })

  lifecycle {
    ignore_changes = [secret_string]
  }
}

resource "aws_security_group" "rds_proxy" {
  name_prefix = "auth-token-generator-prod-proxy-"
  vpc_id      = data.aws_vpc.production_vpc.id
  description = "Security group for auth token generator RDS proxy"

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name        = "auth-token-generator-prod-proxy-production"
    Project     = "platform apis"
    Environment = "prod"
  }
}

// RDS proxy for Postgres only ever exposes the default port 5432. It cannot be changed
resource "aws_security_group_rule" "allow_authorizer_to_proxy" {
  type                     = "ingress"
  from_port                = 5432
  to_port                  = 5432
  protocol                 = "tcp"
  source_security_group_id = local.new_authorizer_sg_id
  security_group_id        = aws_security_group.rds_proxy.id
  description              = "Allow authorizer Lambda to connect to RDS Proxy"
}

resource "aws_security_group_rule" "allow_proxy_to_db" {
  type                     = "ingress"
  from_port                = local.token_db_port
  to_port                  = local.token_db_port
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.rds_proxy.id
  security_group_id        = module.postgres_db_production.db_security_group_id
  description              = "Allow RDS Proxy to connect to the database"
}

resource "aws_iam_role" "rds_proxy" {
  name = "${local.rds_proxy_name}-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          Service = "rds.amazonaws.com"
        }
        Action = "sts:AssumeRole"
      }
    ]
  })

  tags = {
    Project     = "platform apis"
    Environment = "prod"
  }
}

resource "aws_iam_role_policy" "rds_proxy" {
  name = "${local.rds_proxy_name}-policy"
  role = aws_iam_role.rds_proxy.id

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "secretsmanager:GetSecretValue",
          "secretsmanager:DescribeSecret"
        ]
        Resource = aws_secretsmanager_secret.rds_proxy_db_credentials.arn
      },
      {
        Effect = "Allow"
        Action = [
          "kms:Decrypt"
        ]
        Resource = data.aws_kms_key.secretsmanager.arn
        Condition = {
          StringEquals = {
            "kms:ViaService" = "secretsmanager.${data.aws_region.current.name}.amazonaws.com"
          }
        }
      },
      {
        Effect = "Allow"
        Action = [
          "rds:DescribeDBInstances",
          "rds:DescribeDBClusters"
        ]
        Resource = "*"
      }
    ]
  })
}

data "aws_kms_key" "secretsmanager" {
  key_id = "alias/aws/secretsmanager"
}

resource "aws_db_proxy" "auth_token_generator_production" {
  name                   = local.rds_proxy_name
  engine_family          = "POSTGRESQL"
  role_arn               = aws_iam_role.rds_proxy.arn
  vpc_subnet_ids         = data.aws_subnet_ids.production.ids
  vpc_security_group_ids = [aws_security_group.rds_proxy.id]
  require_tls            = true

  auth {
    auth_scheme = "SECRETS"
    iam_auth    = "DISABLED"
    secret_arn  = aws_secretsmanager_secret.rds_proxy_db_credentials.arn
  }

  tags = {
    Project     = "platform apis"
    Environment = "prod"
  }

  depends_on = [
    module.postgres_db_production,
    aws_iam_role_policy.rds_proxy
  ]
}

resource "aws_db_proxy_default_target_group" "auth_token_generator_production" {
  db_proxy_name = aws_db_proxy.auth_token_generator_production.name

  connection_pool_config {
    # db.t3.medium PostgreSQL (4GB memory)
    # SHOW max_connections; returns 402 -> 70% ≈ 281 pooled, 121 reserved for direct access
    max_connections_percent      = 70
    max_idle_connections_percent = 40 //Keeps some warm connections for bursty Lambda traffic
    connection_borrow_timeout    = 5  //Timeout for getting a connection from the pool, after that Lambda receives an error. Current Lambda timeout is set to 10s
  }
}

resource "aws_db_proxy_target" "auth_token_generator_production" {
  db_proxy_name          = aws_db_proxy.auth_token_generator_production.name
  target_group_name      = aws_db_proxy_default_target_group.auth_token_generator_production.name
  db_instance_identifier = local.db_instance_identifier
}
