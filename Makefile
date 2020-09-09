.PHONY: setup
setup:
	docker-compose build

.PHONY: build
build:
	docker-compose build TokenAdministrationApi

.PHONY: serve
serve:
	docker-compose build TokenAdministrationApi && docker-compose up TokenAdministrationApi

.PHONY: shell
shell:
	docker-compose run TokenAdministrationApi bash

.PHONY: test
test:
	docker-compose up test-database & docker-compose build TokenAdministrationApi-test && docker-compose up TokenAdministrationApi-test

.PHONY: lint
lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format
