FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/OSPhoto.Web"
RUN dotnet build "OSPhoto.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OSPhoto.Web.csproj" -c Release -o /app/publish

FROM base AS final
CMD ["mkdir", "/Media"]
COPY --from=build /src/Media /Media
VOLUME ["/Media"]
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OSPhoto.Web.dll"]
