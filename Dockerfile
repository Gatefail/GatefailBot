FROM  mcr.microsoft.com/dotnet/core/sdk:3.0.100-alpine3.9 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY src/GatefailBot/*.csproj ./src/GatefailBot/
COPY NuGet.config .
RUN dotnet restore

# copy and publish app and libraries
COPY src/GatefailBot/. ./GatefailBot/
WORKDIR /app/GatefailBot
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-alpine3.9 AS runtime
WORKDIR /app
COPY --from=build /app/GatefailBot/out ./
RUN mkdir db
ENTRYPOINT ["dotnet", "GatefailBot.dll"]
