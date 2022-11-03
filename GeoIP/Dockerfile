FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
ARG CONFIGURATION=Release

COPY . .
WORKDIR GeoIP
RUN dotnet restore

RUN dotnet build -c $CONFIGURATION -o /app

FROM builder AS publish
ARG CONFIGURATION=Release
RUN dotnet publish -c $CONFIGURATION -o /app

FROM builder AS testrunner
WORKDIR /GeoIP.UnitTest
COPY . .
ENTRYPOINT ["dotnet", "test", "--logger:trx"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=publish /app .
EXPOSE 55688

ENTRYPOINT ["dotnet", "GeoIP.dll"]