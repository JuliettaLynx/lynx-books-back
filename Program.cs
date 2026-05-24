using LynxBooks.Backend.Data;
using LynxBooks.Backend.Middleware;
using LynxBooks.Backend.Services;
using LynxBooks.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// База данных: SQLite или PostgreSQL
var usePostgres = builder.Configuration.GetValue<bool>("UsePostgres");
if (usePostgres)
{
    var connectionString = builder.Configuration.GetConnectionString("PostgresConnection")
                           ?? Environment.GetEnvironmentVariable("DATABASE_URL");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseNpgsql(connectionString));
}
else
{
    var sqliteConnection = builder.Configuration.GetConnectionString("SqliteConnection");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlite(sqliteConnection));
}



// JWT аутентификация
var jwtKey = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
        };
    });

builder.Services.AddAuthorization();

// Контроллеры, Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Введите JWT токен",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Регистрация сервисов (IoC)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<JwtService>();
// builder.Services.AddHttpClient<GoogleAuthService>();
builder.Services.AddHttpContextAccessor();

// CORS – разрешаем фронтенд (значение из конфигурации)
var frontendUrl = builder.Configuration["FrontendUrl"];
if (string.IsNullOrEmpty(frontendUrl))
{
    frontendUrl = builder.Environment.IsDevelopment() 
        ? "http://localhost:5173" 
        : "https://lynx-books.vercel.app";
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(frontendUrl, "http://10.70.146.189:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Автоматическое создание БД и применение миграций
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Конвейер обработки запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();