# Stage 1: Build the C# application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проектов и восстанавливаем зависимости
COPY *.sln .
COPY Account.Application/*.csproj Account.Application/
COPY Account.DAL/*.csproj Account.DAL/
COPY Account.Domain/*.csproj Account.Domain/
COPY Account.api/*.csproj Account.api/
RUN dotnet restore Account.sln

# Копируем остальной исходный код
COPY . .

# Публикуем C# API проект
WORKDIR /src/Account.api
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8999

# Копируем опубликованное приложение
COPY --from=build /app/publish .

# Устанавливаем переменные окружения для подключения к БД
ENV POSTGRES_HOST=db
ENV POSTGRES_PORT=5432
ENV POSTGRES_USER=postgres
ENV POSTGRES_PASSWORD=postgres
ENV POSTGRES_DB=lk_case2_db

# Точка входа
ENTRYPOINT ["dotnet", "Account.api.dll"]
