FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
WORKDIR /app

ENV MEDIA_PATH="/Media"
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ARG BUILD_CONFIGURATION=Release

# TODO: find a way to export the dev cert and import/reference it from final (runtime only)
#       we can then enable https (or use certbot instead somehow...)
RUN dotnet dev-certs https

WORKDIR /src
COPY ["OSPhoto.sln", "/src"]
COPY ["OSPhoto.Common.Tests/OSPhoto.Common.Tests.csproj", "OSPhoto.Common.Tests/"]
COPY ["OSPhoto.Common/OSPhoto.Common.csproj", "OSPhoto.Common/"]
COPY ["OSPhoto.Api/OSPhoto.Api.csproj", "OSPhoto.Api/"]
RUN dotnet restore "OSPhoto.sln"

COPY . .

WORKDIR "/src/OSPhoto.Api"
RUN dotnet build "OSPhoto.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OSPhoto.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

#COPY --from=build /src/Media /Media
VOLUME ["/AppData"]
VOLUME ["/Media"]

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OSPhoto.Api.dll"]
