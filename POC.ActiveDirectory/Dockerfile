#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 389

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["POC.ActiveDirectory/POC.ActiveDirectory.csproj", "POC.ActiveDirectory/"]
RUN dotnet restore "POC.ActiveDirectory/POC.ActiveDirectory.csproj"
COPY . .
WORKDIR "/src/POC.ActiveDirectory"
RUN dotnet build "POC.ActiveDirectory.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "POC.ActiveDirectory.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "POC.ActiveDirectory.dll"]