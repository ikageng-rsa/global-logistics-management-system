FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["GLMS.Web/GLMS.Web.csproj", "GLMS.Web/"]
RUN dotnet restore "GLMS.Web/GLMS.Web.csproj"
COPY . .

WORKDIR "/src/GLMS.Web"
RUN dotnet build "GLMS.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GLMS.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GLMS.Web.dll"]