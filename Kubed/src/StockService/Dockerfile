FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as build

WORKDIR /build

COPY ./Kubed.sln .
COPY ./src/StockService/StockService.csproj ./src/StockService/
COPY ./src/StockWeb/StockWeb.csproj ./src/StockWeb/

RUN dotnet restore

COPY . .

RUN dotnet publish --no-restore -c Release -o /published src/StockService/StockService.csproj

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0

WORKDIR /app

COPY --from=build /published .

ENTRYPOINT [ "dotnet", "StockService.dll" ]
