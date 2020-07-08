.PHONY: setup
setup:
	docker-compose build

.PHONY: build
build:
	docker-compose build TenancyInformationApi

.PHONY: serve
serve:
	docker-compose build TenancyInformationApi && docker-compose up TenancyInformationApi

.PHONY: shell
shell:
	docker-compose run TenancyInformationApi bash

.PHONY: test
test:
	docker-compose up test-database & docker-compose build TenancyInformationApi-test && docker-compose up TenancyInformationApi-test

.PHONY: lint
lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format
