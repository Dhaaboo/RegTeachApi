using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegTeachApi.Data;
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

var app = _blder.Build();

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
