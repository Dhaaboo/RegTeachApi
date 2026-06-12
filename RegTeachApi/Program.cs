using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegTeachApi.Data;
using RegTeachApi.Data.Models;
using RegTeachApi.Middleware;
using RegTeachApi.Services;
using System.Text;

var _blder = WebApplication.CreateBuilder(args);
var _Src = _blder.Services;

// Add services to the container.
_Src.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
_Src.AddOpenApi();

var Ctr = _blder.Configuration.GetConnectionString("MYRDBCS") ?? throw new InvalidOperationException("Connection string 'In Your Project' not found.");
_Src.AddDbContext<APPDBC>(options => options.UseSqlServer(Ctr));
_Src.AddEndpointsApiExplorer();
_Src.AddSwaggerGen();
_Src.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models
        .OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type =
                Microsoft.OpenApi.Models
                .SecuritySchemeType.Http,

            Scheme = "bearer",
            BearerFormat = "JWT",

            In =
                Microsoft.OpenApi.Models
                .ParameterLocation.Header
        });

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models
        .OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models
                .OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models
                        .OpenApiReference
                        {
                            Type =
                                Microsoft.OpenApi.Models
                                .ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});

_Src.AddScoped<JwtService>();

_Src.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _blder.Configuration["Jwt:Issuer"],
                ValidAudience = _blder.Configuration["Jwt:Audience"],
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_blder.Configuration["Jwt:Key"]!))
            };
    });

_Src.AddAuthorization();
_Src.AddAuthorization(options =>
{
    options.AddPolicy(
        "AdminOnly",
        policy =>
            policy.RequireRole("Admin"));

    options.AddPolicy(
        "VerifiedUser",
        policy =>
            policy.RequireClaim(
                "EmailVerified",
                "true"));
});

_Src.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        });
});

var app = _blder.Build();

using var scope = app.Services.CreateScope();

var _db = scope.ServiceProvider.GetRequiredService<APPDBC>();

if (!_db.RegTeachUsers.Any())
{
    _db.RegTeachUsers.Add(new RegTeachUsers
    {
        Username = "admin",
        Email = "admin@admin.com",
        FirstName = "System",
        LastName = "Admin",
        Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
        Role = "Admin",
        IsEmailVerified = true
    });
    _db.SaveChanges();
}

app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseCors("AllowFrontend");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
