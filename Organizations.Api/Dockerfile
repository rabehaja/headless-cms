# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ContentModels.sln ./
COPY ContentModels.Domain/ContentModels.Domain.csproj ContentModels.Domain/
COPY Organizations.Api/Organizations.Api.csproj Organizations.Api/
RUN dotnet restore Organizations.Api/Organizations.Api.csproj

COPY . .
RUN dotnet publish Organizations.Api/Organizations.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Organizations.Api.dll"]
