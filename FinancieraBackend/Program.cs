using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using FinancieraBackend.Infrastructure;
using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Application.Services;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// ── OpenAPI / Swagger ─────────────────────────────────────────────────────────
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Financiera Backend", Version = "v1" });

    const string bearerSchemeId = "Bearer";

    // Habilitar el botón "Authorize" en Swagger UI para enviar el JWT
    // Nota: Swashbuckle 10.x + Microsoft.OpenApi 2.x usan delegate en AddSecurityRequirement
    // y los tipos están en el namespace raíz Microsoft.OpenApi (sin .Models)
    c.AddSecurityDefinition(bearerSchemeId, new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",       // minúsculas según RFC 7235
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Ingrese el token JWT. Ejemplo: Bearer {token}"
    });

    c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference(bearerSchemeId),
            new List<string>()
        }
    });
});

builder.Services.AddOpenApi();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// ── Base de datos ─────────────────────────────────────────────────────────────
builder.Services.AddDbContext<DBConnection>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))));

// ── Servicios de aplicación ───────────────────────────────────────────────────
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFinancialGroupService, FinancialGroupService>();
builder.Services.AddScoped<IGroupMemberService, GroupMemberService>();
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IProjectionService, DeepSeekProjectionService>();

// ── Autenticación JWT ─────────────────────────────────────────────────────────
// La clave secreta se lee desde JwtSettings:Key.
// En producción, configúrala en User Secrets o en variables de entorno,
// NUNCA en el appsettings.json del repositorio.
//
// El secret de AWS IAM se almacena en User Secrets bajo la clave "IAMSecret".
// Cuando UseAwsIam = true en appsettings.json, AuthService delega la
// autenticación a AWS IAM usando esa clave.
var jwtKey    = builder.Configuration["JwtSettings:Key"]!;
var issuer    = builder.Configuration["JwtSettings:Issuer"]!;
var audience  = builder.Configuration["JwtSettings:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = issuer,
            ValidAudience            = audience,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew                = TimeSpan.Zero   // Sin margen de tiempo extra
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("=== JWT AUTH FAILED ===");
                Console.WriteLine(context.Exception.ToString());
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.Configure<RouteOptions>(options =>
    options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",   // Vite dev server
                "http://localhost:4173",   // Vite preview
                "http://localhost:3000",   // fallback dev port
                "https://financierafrontend.d-xos.com" // production frontend
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// ── Ejecutar Migraciones Pendientes (Para Producción / Docker) ────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DBConnection>();
        if (context.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("Aplicando migraciones pendientes a la base de datos...");
            context.Database.Migrate();
            Console.WriteLine("Migraciones aplicadas correctamente.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al aplicar migraciones: {ex.Message}");
    }
}

// ── Pipeline HTTP ─────────────────────────────────────────────────────────────
app.UseMiddleware<FinancieraBackend.Middlewares.GlobalExceptionMiddleware>();
app.UseCors("AllowFrontend");

// Swagger siempre disponible (útil también en despliegues)
app.UseSwagger();
app.UseSwaggerUI();

// IMPORTANTE: el orden es: Authentication → Authorization → Controllers
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ── Endpoint de prueba de conexión ────────────────────────────────────────────
app.MapGet("/test-connection", async (IConfiguration config) =>
{
    var connectionString = config.GetConnectionString("MySqlConnection");
    try
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        return "realizado con exito";
    }
    catch (Exception ex)
    {
        return $"Error: {ex.Message}";
    }
});

app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
