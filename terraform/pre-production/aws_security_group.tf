resource "aws_security_group" "lambda_sg" {
  vpc_id = data.aws_vpc.pre_production_vpc.id
  name   = "auth-token-generator-api-pre-production"

  ingress {
    cidr_blocks = ["0.0.0.0/0"]
    from_port   = 5103
    to_port     = 5103
    protocol    = "tcp"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # Used when no Lambda ENI matches the provider's forced 45 minute waiter.
  # Lambda ENIs are released before destroy so this delete stays on the short timeout.
  timeouts {
    delete = "5m"
  }
}
