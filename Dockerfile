# ---------- Angular Build Stage ----------
FROM node:20 AS angular-build

WORKDIR /app

# Copy Angular project
COPY AngularApp/ ./AngularApp/

WORKDIR /app/AngularApp

RUN npm ci
RUN npm run build -- --configuration production


# ---------- .NET Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dotnet-build

ENV RUNNING_IN_DOCKER=true

WORKDIR /src

# Copy solution
COPY . .

# Copy Angular dist into wwwroot BEFORE publish
COPY --from=angular-build /app/AngularApp/dist/AngularApp/browser ./AngularApp/wwwroot

WORKDIR /src/src/AngularApp

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish


# ---------- Runtime Stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=dotnet-build /app/publish .

ENTRYPOINT ["dotnet", "AngularApp.dll"]