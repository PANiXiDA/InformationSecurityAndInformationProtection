# Используем официальный образ .NET SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем файлы проекта и восстанавливаем зависимости
COPY PasswordSystem.API/PasswordSystem.API.csproj PasswordSystem.API/
COPY BL/BL.Standard/BL.Standard.csproj BL.Standard/
COPY BL/BL.Interfaces/BL.Interfaces.csproj BL.Interfaces/
COPY Common/Common.csproj Common/
COPY DAL/DAL.DbModels/DAL.DbModels.csproj DAL.DbModels/
COPY DAL/DAL.Interfaces/DAL.Interfaces.csproj DAL.Interfaces/
COPY DAL/DAL.SQL/DAL.SQL.csproj DAL.SQL/
COPY Entities/Entities.csproj Entities/
RUN dotnet restore "PasswordSystem.API/PasswordSystem.API.csproj"

# Копируем остальные файлы и собираем проект
COPY . .
WORKDIR "/app/PasswordSystem.API"
RUN dotnet publish -c Release -o out

# Используем официальный образ .NET для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/PasswordSystem.API/out .
ENTRYPOINT ["dotnet", "PasswordSystem.API.dll"]
