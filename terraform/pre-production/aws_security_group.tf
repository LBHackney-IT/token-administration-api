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

}
