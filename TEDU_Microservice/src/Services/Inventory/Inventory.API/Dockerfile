FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Inventory/Inventory.API/Inventory.API.csproj","Services/Inventory/Inventory.API/"]
COPY ["BuildingBlocks/Common.Logging/Common.Logging.csproj","BuildingBlocks/Common.Logging/"]
COPY ["BuildingBlocks/Contracts/Contracts.csproj","BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj","BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Shared/Shared.csproj","BuildingBlocks/Shared/"]
RUN dotnet restore "Services/Inventory/Inventory.API/Inventory.API.csproj"
COPY . .
WORKDIR "/src/Services/Inventory/Inventory.API"
RUN dotnet build "Inventory.API.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "Inventory.API.csproj" -c Release -o /app/publish

FROM base as Final
WORKDIR /APP
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet","Inventory.API.dll"]