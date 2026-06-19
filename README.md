# Lynx Books - Backend

ASP.NET Core 9 API для книжной библиотеки с поддержкой JWT аутентификации, Google Sign-In и офлайн-синхронизации.

## Технологии

- **ASP.NET Core 9** (C# 12)
- **Entity Framework Core 9**
- **SQLite / PostgreSQL** (на выбор)
- **JWT Bearer** аутентификация
- **Swagger** API документация
- **BCrypt** хеширование паролей
- **Google Identity Services** OAuth2

## Требования

- .NET 9 SDK или новее
- SQLite (встроенная) или PostgreSQL

## Установка

```bash
dotnet restore
```

## Настройка окружения

Создайте `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "SqliteConnection": "Data Source=lynxbooks.db",
    "PostgresConnection": "Host=localhost;Database=lynxbooks;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your-256-bit-secret-key-with-at-least-32-characters-long",
    "Issuer": "LynxBooks",
    "Audience": "LynxBooksClient"
  },
  "Google": {
    "ClientId": "your-google-client-id.apps.googleusercontent.com"
  },
  "FrontendUrl": "http://localhost:5173",
  "UsePostgres": false
}
```

## Запуск

```bash
dotnet run                     # Development mode
dotnet run --configuration Production  # Production mode
dotnet run --urls="http://localhost:5000"  # С указанием порта
```

## Структура проекта

```
├── Controllers/
│   ├── AuthController.cs       # Авторизация, регистрация, Google Sign-In
│   ├── BooksController.cs      # CRUD для книг
│   ├── SessionsController.cs   # Сессии чтения
│   ├── WishlistController.cs   # Виш-лист
│   ├── CommunityController.cs  # Сообщество, подписки
│   └── UsersController.cs      # Профиль, аватар, настройки
├── Data/
│   └── AppDbContext.cs         # EF Core контекст БД
├── DTOs/                       # Data Transfer Objects
├── Middleware/
│   └── ErrorHandlingMiddleware.cs  # Глобальная обработка ошибок
├── Models/                     # Модели данных
└── Services/                   # Бизнес-логика
```

## API Endpoints

### Аутентификация

| Метод | Endpoint | Описание |
|-------|----------|----------|
| POST | `/api/auth/register` | Регистрация |
| POST | `/api/auth/login` | Вход по email/паролю |
| POST | `/api/auth/google` | Вход через Google |
| POST | `/api/auth/refresh` | Обновление токена |
| GET | `/api/auth/me` | Текущий пользователь |
| POST | `/api/auth/change-password` | Смена пароля |
| DELETE | `/api/auth/account` | Удаление аккаунта |

### Библиотека

| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | `/api/books` | Получить все книги |
| GET | `/api/books/{id}` | Получить книгу по ID |
| POST | `/api/books` | Добавить книгу |
| PUT | `/api/books/{id}` | Обновить книгу |
| DELETE | `/api/books/{id}` | Удалить книгу |

### Сессии чтения

| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | `/api/sessions` | Получить сессии |
| POST | `/api/sessions` | Создать сессию |
| PUT | `/api/sessions/{id}` | Обновить сессию |
| DELETE | `/api/sessions/{id}` | Удалить сессию |

### Виш-лист

| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | `/api/wishlist` | Получить виш-лист |
| POST | `/api/wishlist` | Добавить в виш-лист |
| DELETE | `/api/wishlist/{id}` | Удалить из виш-листа |

### Сообщество

| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | `/api/community/users/search` | Поиск пользователей |
| POST | `/api/community/subscribe` | Подписаться |
| DELETE | `/api/community/subscribe/{userId}` | Отписаться |
| GET | `/api/community/subscriptions` | Мои подписки |

### Пользователь

| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | `/api/users/profile` | Получить профиль |
| PUT | `/api/users/profile` | Обновить профиль |
| PUT | `/api/users/avatar` | Обновить аватар |
| PUT | `/api/users/daily-goal` | Установить дневную цель |

## Аутентификация

Все защищённые endpoints требуют JWT токен в заголовке:

```
Authorization: Bearer <token>
```

### JWT Token Structure

- **Access Token:** 1 час жизни
- **Refresh Token:** 7 дней жизни

## База данных

### По умолчанию: SQLite

```bash
# База создаётся автоматически при первом запуске
# Файл: lynxbooks.db
```

### PostgreSQL (опционально)

```json
{
  "UsePostgres": true,
  "ConnectionStrings": {
    "PostgresConnection": "Host=localhost;Database=lynxbooks;Username=postgres;Password=..."
  }
}
```

### Миграции

```bash
dotnet ef migrations add MigrationName  # Создать миграцию
dotnet ef database update              # Применить миграции
dotnet ef migrations remove            # Удалить последнюю миграцию
```

## Swagger API Documentation

После запуска откройте Swagger:

- Development: `http://localhost:5000/swagger`
- Production: `https://yourdomain.com/swagger`

## Нашли ошибку или появилось предложение?

https://docs.google.com/forms/d/e/1FAIpQLSdSlanCQfeF_4zFU9pj1IdYPxGSJBN4pGtslnWWvsdZfO4lgQ/viewform?usp=dialog

## Лицензия

Private project
