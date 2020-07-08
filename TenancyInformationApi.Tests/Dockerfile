FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./TenancyInformationApi.sln ./
COPY ./TenancyInformationApi/TenancyInformationApi.csproj ./TenancyInformationApi/
COPY ./TenancyInformationApi.Tests/TenancyInformationApi.Tests.csproj ./TenancyInformationApi.Tests/

RUN dotnet restore ./TenancyInformationApi/TenancyInformationApi.csproj
RUN dotnet restore ./TenancyInformationApi.Tests/TenancyInformationApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out TenancyInformationApi.Tests/TenancyInformationApi.Tests.csproj

CMD dotnet test
