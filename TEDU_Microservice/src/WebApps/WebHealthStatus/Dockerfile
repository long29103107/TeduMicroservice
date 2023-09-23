FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebApps/WebHealthStatus/WebHealthStatus.csproj","WebApps/WebHealthStatus/"]
RUN dotnet restore "WebApps/WebHealthStatus/WebHealthStatus.csproj"
COPY . .
WORKDIR "/src/WebApps/WebHealthStatus"
RUN dotnet build "WebHealthStatus.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "WebHealthStatus.csproj" -c Release -o /app/publish

FROM base as Final
WORKDIR /APP
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet","WebHealthStatus.dll"]