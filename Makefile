.PHONY: setup build serve shell test lint install-ef-tool migrate-local-dev-database serve-local-dev-database serve-local-dev-api

setup:
	docker compose build

build:
	docker compose build token-administration-api

serve:
	docker compose build token-administration-api && docker compose up token-administration-api

shell:
	docker compose run token-administration-api bash

test:
	docker compose up test-database & docker compose build token-administration-api-test && docker compose up token-administration-api-test

serve-local-dev-database:
	docker compose up local-dev-database

migrate-local-dev-database:
	CONNECTION_STRING="Host=127.0.0.1;Port=5433;Username=postgres;Password=mypassword;Database=devdb" dotnet ef database update -p TokenAdministrationApi

serve-local-dev-api:
	docker compose up token-administration-api-dev

lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format

install-ef-tool:
	@if ! dotnet tool list --global | grep -q "dotnet-ef"; then \
		dotnet tool install --global dotnet-ef --version 8.0.11; \
	fi
