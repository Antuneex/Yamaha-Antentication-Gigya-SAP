FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["/NuGet.Config", "/"]
COPY ["/src/IntegrationHubApi/IntegrationHubApi.csproj", "src/IntegrationHubApi/"]
COPY ["/src/IntegrationHubApi.Domain/IntegrationHubApi.Domain.csproj", "src/IntegrationHubApi.Domain/"]
COPY ["/src/IntegrationHubApi.Infra/IntegrationHubApi.Infra.csproj", "src/IntegrationHubApi.Infra/"]
COPY ["/src/IntegrationHubApi.Services/IntegrationHubApi.Services.csproj", "src/IntegrationHubApi.Services/"]
COPY ["/src/IntegrationHubApi.ExternalServices/IntegrationHubApi.ExternalServices.csproj", "src/IntegrationHubApi.ExternalServices/"]
RUN dotnet restore "src/IntegrationHubApi/IntegrationHubApi.csproj"
COPY . .
WORKDIR "/src/IntegrationHubApi"
RUN dotnet build "/src/src/IntegrationHubApi/IntegrationHubApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "/src/src/IntegrationHubApi/IntegrationHubApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY ["/src/IntegrationHubApi/IntegrationHubApi.xml", "IntegrationHubApi.xml"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IntegrationHubApi.dll"]