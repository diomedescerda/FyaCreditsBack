FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY ["src/FyaCredits.Domain/FyaCredits.Domain.csproj", "src/FyaCredits.Domain/"]
COPY ["src/FyaCredits.Application/FyaCredits.Application.csproj", "src/FyaCredits.Application/"]
COPY ["src/FyaCredits.Infrastructure/FyaCredits.Infrastructure.csproj", "src/FyaCredits.Infrastructure/"]
COPY ["src/FyaCredits.WebApi/FyaCredits.WebApi.csproj", "src/FyaCredits.WebApi/"]
RUN dotnet restore "src/FyaCredits.WebApi/FyaCredits.WebApi.csproj"

COPY . .
RUN dotnet publish "src/FyaCredits.WebApi/FyaCredits.WebApi.csproj" \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
RUN apk add --no-cache curl icu-libs krb5-libs
ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "FyaCredits.WebApi.dll"]
