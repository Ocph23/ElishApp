#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update && apt-get install -y libgdiplus

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ApsWebApp/ApsWebApp.csproj", "ApsWebApp/"]
COPY ["WebApp/ShareModels/ShareModels.csproj", "WebApp/ShareModels/"]
RUN dotnet restore "ApsWebApp/ApsWebApp.csproj"
COPY . .
WORKDIR "/src/ApsWebApp"
RUN dotnet build "ApsWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApsWebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApsWebApp.dll"]