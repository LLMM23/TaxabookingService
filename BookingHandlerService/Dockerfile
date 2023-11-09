FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine as build
WORKDIR /app

COPY . .
RUN dotnet restore BookingHandlerService/BookingHandlerService.csproj
RUN dotnet publish BookingHandlerService/BookingHandlerService.csproj -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine as runtime
WORKDIR /app
COPY --from=build /app/published-app /app
ENTRYPOINT ["dotnet", "/app/BookingHandlerService.dll"]