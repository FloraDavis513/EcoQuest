FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
EXPOSE 80
COPY [".", "EcoQuest/"]
RUN dotnet restore "EcoQuest/EcoQuest.csproj"
WORKDIR "/src/EcoQuest"
RUN dotnet build "EcoQuest.csproj" -c Release -o /app/build

FROM build AS publish
EXPOSE 80
RUN dotnet publish "EcoQuest.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcoQuest.dll"]