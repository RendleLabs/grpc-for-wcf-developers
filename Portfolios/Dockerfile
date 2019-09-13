FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as build

WORKDIR /src

COPY . .

RUN dotnet restore

RUN dotnet publish -c Release -o /published src/TraderSys.Portfolios/TraderSys.Portfolios.csproj

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0

WORKDIR /app

COPY --from=build /published .

ENTRYPOINT [ "dotnet", "TraderSys.Portfolios.dll" ]
