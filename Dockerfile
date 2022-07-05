FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY global.json dotnet-test-project.csproj ./
RUN dotnet restore dotnet-test-project.csproj
COPY Api/ Api/
COPY BLL/ BLL/
COPY Models/ Models/
COPY View/ View/
COPY Program.cs ./
RUN dotnet publish dotnet-test-project.csproj -c Release --no-restore -o /app

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app ./
USER $APP_UID
ENTRYPOINT ["dotnet", "dotnet-test-project.dll"]
