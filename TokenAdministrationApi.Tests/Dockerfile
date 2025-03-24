FROM mcr.microsoft.com/dotnet/sdk:8.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./TokenAdministrationApi.sln ./
COPY ./TokenAdministrationApi/TokenAdministrationApi.csproj ./TokenAdministrationApi/
COPY ./TokenAdministrationApi.Tests/TokenAdministrationApi.Tests.csproj ./TokenAdministrationApi.Tests/

RUN dotnet restore ./TokenAdministrationApi/TokenAdministrationApi.csproj
RUN dotnet restore ./TokenAdministrationApi.Tests/TokenAdministrationApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out TokenAdministrationApi.Tests/TokenAdministrationApi.Tests.csproj

CMD dotnet test
