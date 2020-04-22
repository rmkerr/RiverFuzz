#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["WebLauncher/WebLauncher.csproj", "WebLauncher/"]
COPY ["Fuzz/Fuzz.csproj", "Fuzz/"]
COPY ["CaptureParse/CaptureParse.csproj", "CaptureParse/"]
COPY ["Database/Database.csproj", "Database/"]
COPY ["HttpTokenize/HttpTokenize.csproj", "HttpTokenize/"]
COPY ["PopulationManager/Population.csproj", "PopulationManager/"]
COPY ["Generators/Generators.csproj", "Generators/"]
COPY ["ProjectSpecific/ProjectSpecific.csproj", "ProjectSpecific/"]
COPY ["Configuration/Configuration.csproj", "Configuration/"]

RUN dotnet restore "WebLauncher/WebLauncher.csproj"
COPY . .
WORKDIR "/src/WebLauncher"
RUN dotnet build "WebLauncher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebLauncher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebLauncher.dll"]