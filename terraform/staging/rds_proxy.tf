locals {
  rds_proxy_name                     = "auth-token-generator-stg-proxy"
  db_instance_identifier             = "auth-token-generator-staging-db-db-staging"
  rds_proxy_secret_name              = "/api-auth-token-generator/staging/rds-proxy-db-credentials"
  rds_proxy_secret_placeholder_value = "tbc"
}

resource "aws_secretsmanager_secret" "rds_proxy_db_credentials" {
  name = local.rds_proxy_secret_name

  tags = {
    Project     = "platform apis"
    Environment = "stg"
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
  name_prefix = "auth-token-generator-stg-proxy-"
  vpc_id      = data.aws_vpc.staging_vpc.id
  description = "Security group for auth token generator RDS proxy"

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name        = "auth-token-generator-stg-proxy-staging"
    Project     = "platform apis"
    Environment = "stg"
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
  security_group_id        = module.postgres_db_staging.db_security_group_id
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
    Environment = "stg"
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

resource "aws_db_proxy" "auth_token_generator_staging" {
  name                   = local.rds_proxy_name
  engine_family          = "POSTGRESQL"
  role_arn               = aws_iam_role.rds_proxy.arn
  vpc_subnet_ids         = data.aws_subnet_ids.staging.ids
  vpc_security_group_ids = [aws_security_group.rds_proxy.id]
  require_tls            = true

  auth {
    auth_scheme = "SECRETS"
    iam_auth    = "DISABLED"
    secret_arn  = aws_secretsmanager_secret.rds_proxy_db_credentials.arn
  }

  tags = {
    Project     = "platform apis"
    Environment = "stg"
  }

  depends_on = [
    module.postgres_db_staging,
    aws_iam_role_policy.rds_proxy
  ]
}

resource "aws_db_proxy_default_target_group" "auth_token_generator_staging" {
  db_proxy_name = aws_db_proxy.auth_token_generator_staging.name

  connection_pool_config {
    # db.t3.micro PostgreSQL (1GB memory)
    # rough estimate for max connections: LEAST({DBInstanceClassMemory/9531392}, 5000)
    # 1,073,741,824 / 9,531,392 ≈ 112.6 → 112 connections
    # Actual: SHOW max_connections; returns 80 -> 70% ≈ 56 pooled, 24 reserved for direct access
    # 80 × 9,531,392 = 762,511,360 bytes (~728 MiB) available for connections on the host
    max_connections_percent      = 70
    max_idle_connections_percent = 40 //Keeps some warm connections for bursty Lambda traffic
    connection_borrow_timeout    = 5  //Timeout for getting a connection from the pool, after that Lambda receives an error. Current Lambda timeout is set to 10s
  }
}

resource "aws_db_proxy_target" "auth_token_generator_staging" {
  db_proxy_name          = aws_db_proxy.auth_token_generator_staging.name
  target_group_name      = aws_db_proxy_default_target_group.auth_token_generator_staging.name
  db_instance_identifier = local.db_instance_identifier
}
