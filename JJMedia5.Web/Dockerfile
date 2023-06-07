#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["JJMedia5.Web/JJMedia5.Web.csproj", "JJMedia5.Web/"]
RUN dotnet restore "JJMedia5.Web/JJMedia5.Web.csproj"
COPY . .
WORKDIR "/src/JJMedia5.Web"
RUN dotnet build "JJMedia5.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JJMedia5.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JJMedia5.Web.dll"]