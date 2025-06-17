FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 8080

COPY PaymentsService.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /scr
COPY --from=build /app .

ENTRYPOINT ["dotnet", "PaymentsService.dll"]