#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NotificationApi/NotificationApi.csproj", "NotificationApi/"]
RUN dotnet restore "NotificationApi/NotificationApi.csproj"
COPY . .
WORKDIR "/src/NotificationApi"
RUN dotnet build "NotificationApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotificationApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationApi.dll"]