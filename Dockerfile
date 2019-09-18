FROM microsoft/dotnet:2.2-sdk-alpine AS build
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

FROM microsoft/dotnet:2.2-aspnetcore-runtime-alpine AS runtime
WORKDIR /app
COPY --from=build /app/GatefailBot/out ./
RUN mkdir db
ENTRYPOINT ["dotnet", "GatefailBot.dll"]
