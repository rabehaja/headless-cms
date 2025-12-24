# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ContentModels.sln ./
COPY ContentModels.Domain/ContentModels.Domain.csproj ContentModels.Domain/
COPY Assets.Api/Assets.Api.csproj Assets.Api/
RUN dotnet restore Assets.Api/Assets.Api.csproj

COPY . .
RUN dotnet publish Assets.Api/Assets.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Assets.Api.dll"]
