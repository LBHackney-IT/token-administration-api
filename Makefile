.PHONY: setup build serve shell test lint install-ef-tool

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

lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format

install-ef-tool:
	@if ! dotnet tool list --global | grep -q "dotnet-ef"; then \
		dotnet tool install --global dotnet-ef --version 8.0.11; \
	fi
