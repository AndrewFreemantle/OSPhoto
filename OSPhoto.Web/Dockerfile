FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OSPhoto.Web/OSPhoto.Web.csproj", "OSPhoto.Web/"]
RUN dotnet restore "OSPhoto.Web/OSPhoto.Web.csproj"
COPY . .
WORKDIR "/src/OSPhoto.Web"
RUN dotnet build "OSPhoto.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OSPhoto.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OSPhoto.Web.dll"]
