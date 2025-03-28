.PHONY: setup
setup:
	docker compose build

.PHONY: build
build:
	docker compose build token-administration-api

.PHONY: serve
serve:
	docker compose build token-administration-api && docker compose up token-administration-api

.PHONY: shell
shell:
	docker compose run token-administration-api bash

.PHONY: test
test:
	docker compose up test-database & docker compose build token-administration-api-test && docker compose up token-administration-api-test

.PHONY: lint
lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format
