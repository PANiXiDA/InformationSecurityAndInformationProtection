using DAL.SQL;
using BL.Standard;
using Common.Configurations;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DAL.DbModels;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var environment = builder.Environment.EnvironmentName;
var isDocker = environment == "Docker";

var connectionString = isDocker
    ? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    : builder.Configuration.GetConnectionString("DefaultConnectionString");

builder.Services.AddDbContext<DefaultDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddBusinessLogicLayer();
builder.Services.AddDataAccessLayer();
builder.Services.AddControllers();

if (jwtSettings != null)
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });
}
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

