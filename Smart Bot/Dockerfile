# Используем официальный .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем файлы проекта и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем оставшиеся файлы и собираем проект
COPY . ./
RUN dotnet publish -c Release -o out

# Финальный образ для запуска
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "MyTelegramBot.dll"]
