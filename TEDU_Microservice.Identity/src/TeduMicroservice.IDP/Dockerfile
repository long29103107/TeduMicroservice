FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TeduMicroservice.IDP/TeduMicroservice.IDP.csproj", "TeduMicroservice.IDP/"]
COPY ["TeduMicroservice.IDP.Presentation/TeduMicroservice.IDP.Presentation.csproj", "TeduMicroservice.IDP.Presentation/"]
COPY ["TeduMicroservice.IDP.Infrastructure/TeduMicroservice.IDP.Infrastructure.csproj", "TeduMicroservice.IDP.Infrastructure/"]
RUN dotnet restore "TeduMicroservice.IDP/TeduMicroservice.IDP.csproj"
COPY . .
WORKDIR "/src/TeduMicroservice.IDP"
RUN dotnet build "TeduMicroservice.IDP.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeduMicroservice.IDP.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeduMicroservice.IDP.dll"]