# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ContentModels.sln ./
COPY ContentModels.Api/ContentModels.Api.csproj ContentModels.Api/
COPY ContentModels.Domain/ContentModels.Domain.csproj ContentModels.Domain/
COPY ContentModels.Domain.Tests/ContentModels.Domain.Tests.csproj ContentModels.Domain.Tests/
COPY GlobalFields.Api/GlobalFields.Api.csproj GlobalFields.Api/
RUN dotnet restore GlobalFields.Api/GlobalFields.Api.csproj

COPY . .
RUN dotnet publish GlobalFields.Api/GlobalFields.Api.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "GlobalFields.Api.dll"]
