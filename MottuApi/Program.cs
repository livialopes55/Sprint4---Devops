using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuApi.Data;
using System.Text.Json.Serialization;
using Asp.Versioning;                 // <- versionamento novo
using Asp.Versioning.ApiExplorer;     // <- explorer p/ swagger
using Microsoft.OpenApi.Models;       // <- tipos do swagger

var builder = WebApplication.CreateBuilder(args);

// DB (SQLite)
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? $"Data Source={Path.Combine(builder.Environment.ContentRootPath, "mottu.db")}";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(cs));

// ===== Swagger + JWT no Swagger (1x só) =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    var xml = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
    if (File.Exists(xmlPath)) opt.IncludeXmlComments(xmlPath);

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira: Bearer {seu_token}"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ===== JWT =====
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev-key-muito-secreta-para-sprint";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "MottuApi";
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opt =>
    {
        opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

// ===== HealthChecks =====
builder.Services.AddHealthChecks();

// ===== DI (ML) =====
builder.Services.AddSingleton<MottuApi.ML.SentimentModel>();

// ===== Controllers + JSON =====
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// ===== Versionamento (novo formato) =====
builder.Services
    .AddApiVersioning(o =>
    {
        o.AssumeDefaultVersionWhenUnspecified = true;
        o.DefaultApiVersion = new ApiVersion(1, 0);
        o.ReportApiVersions = true;
    })
    .AddApiExplorer(setup =>
    {
        setup.GroupNameFormat = "'v'VVV";
        setup.SubstituteApiVersionInUrl = true;
    });

var app = builder.Build();

// cria banco no primeiro run
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ===== Middlewares =====
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mottu API - v1");
    c.RoutePrefix = "";
});

app.UseHttpsRedirection();

app.UseAuthentication();   // <- faltava
app.UseAuthorization();    // <- faltava

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/db");

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
