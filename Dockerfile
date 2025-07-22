# Сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем проектные файлы
COPY *.csproj ./

# Восстанавливаем зависимости
RUN dotnet restore

# Копируем всё остальное (включая Program.cs и другие файлы)
COPY . ./

# Публикация проекта
RUN dotnet publish -c Release -o /app/out

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "SmartBot.dll"]
