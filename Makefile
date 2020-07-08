.PHONY: setup
setup:
	docker-compose build

.PHONY: build
build:
	docker-compose build tenancy-information-api

.PHONY: serve
serve:
	docker-compose build tenancy-information-api && docker-compose up tenancy-information-api

.PHONY: shell
shell:
	docker-compose run tenancy-information-api bash

.PHONY: test
test:
	docker-compose up test-database & docker-compose build tenancy-information-api-test && docker-compose up tenancy-information-api-test

.PHONY: lint
lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format
