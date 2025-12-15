using System.Text;
using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.BAL.Services;
using CommunityEventsApi.DAL.Interfaces;
using CommunityEventsApi.DAL.Repositories;
using CommunityEventsApi.Data;
using CommunityEventsApi.Helpers;
using CommunityEventsApi.Middleware;
using CommunityEventsApi.Observability;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Community Events API",
        Version = "v1",
        Description = "API for managing community events and user interactions"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
// Temporarily disabled until models are fixed:
// builder.Services.AddScoped<ICommunityRepository, CommunityRepository>();
// builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
// Temporarily disabled until models are fixed:
// builder.Services.AddScoped<ICommunityService, CommunityService>();
// builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICommentService, CommentService>();

// Register Helpers
builder.Services.AddSingleton<TokenGenerator>();

// Configure Logging - basic built-in logging
builder.Services.AddLogging();

// Configure OpenTelemetry (optional) - commented out for now
// OpenTelemetryConfig.ConfigureOpenTelemetry(builder);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Community Events API V1");
    });
}

// Use custom middleware - commented out temporarily due to model mismatches
// app.UseMiddleware<ExceptionMiddleware>();
// app.UseMiddleware<LoggingMiddleware>();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Configure Prometheus (optional)
// PrometheusMetrics.ConfigurePrometheus(app);

app.Run();
