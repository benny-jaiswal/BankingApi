using Microsoft.EntityFrameworkCore;
using RegisterAPI.Infrastructure.Data;
using RegisterAPI.Infrastructure.Services;
using RegisterAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var azureAdConfig = builder.Configuration.GetSection("AzureAd");
string tenantId = azureAdConfig.GetValue<string>("TenantId");
string apiClientId = azureAdConfig.GetValue<string>("ClientId");


// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer( options =>
    { 
        options.Authority = $"https://login.microsoftonline.com/{tenantId}";
        options.Audience = $"api://{apiClientId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddAuthorization(options =>
{
    // Define a policy for read-only access
    options.AddPolicy("ReadOnlyAccess", policy =>
        policy.RequireClaim("scope", $"api://{apiClientId}/api.readonly"));

    // Define a policy for read-write access
    options.AddPolicy("ReadWriteAccess", policy =>
        policy.RequireClaim("scope", "api://{apiClientId}/api.readwrite"));
}); ;

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
        maxRetryCount: 3,
        maxRetryDelay: TimeSpan.FromSeconds(5),
        errorNumbersToAdd: null
        )
    )
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:4200")  //  the allowed origin
                          .AllowAnyMethod()  // Allows all methods
                          .AllowAnyHeader()
                          .AllowCredentials()); 
});
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
