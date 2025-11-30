using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VexaDriveAPI.Context;
using Microsoft.AspNetCore.Diagnostics;
using VexaDriveAPI.JWT;
using VexaDriveAPI.Repository.VehicleServices;
using VexaDriveAPI.Repository.ServiceRequestServices;
using VexaDriveAPI.Repository.NotificationServices;
using VexaDriveAPI.Repository.BillServices;
using VexaDriveAPI.Services.Lifecycle;
using VexaDriveAPI.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
// HttpContext accessor for correlation id and other services
builder.Services.AddHttpContextAccessor();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        b => b.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// Swagger + JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VexaDrive API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter 'Bearer {token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
    
    // Add operation filter to handle IFormFile parameters
    options.OperationFilter<FormFileOperationFilter>();
    
    // Include XML comments if present (helps Swagger examples)
    try
    {
        var xmlFile = System.IO.Path.ChangeExtension(System.Reflection.Assembly.GetExecutingAssembly().Location, ".xml");
        if (System.IO.File.Exists(xmlFile)) options.IncludeXmlComments(xmlFile);
    }
    catch { }
});

// JWT Generator
builder.Services.AddScoped<JwtTokenGenerator>();

// DbContext
builder.Services.AddDbContext<VexaDriveDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<VexaDriveDbContext>()
    .AddDefaultTokenProviders();

// Repositories
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IBillRepository, BillRepository>();

// Authentication (JWT)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        ),
        ClockSkew = TimeSpan.Zero,
        // Ensure the JWT claim types map to the expected ASP.NET identity claim types
        NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
// Lifecycle service
builder.Services.AddSingleton<IServiceLifecycle, ServiceLifecycle>();

var app = builder.Build();

// Run identity seeding (roles + default admin) - idempotent
try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var config = services.GetRequiredService<IConfiguration>();
        VexaDriveAPI.Services.Seed.IdentitySeeder.SeedAsync(services, config).GetAwaiter().GetResult();
    }
}
catch (Exception ex)
{
    // Log seeding failure to console; global exception handler will surface other errors
    Console.WriteLine($"Identity seeding failed: {ex}");
}

// Swagger
// Always enable swagger in this feature branch for easier testing.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "VexaDrive API v1");
    c.RoutePrefix = string.Empty; // swagger at root
});

// Global exception handler that returns ProblemDetails with a correlation id
app.UseExceptionHandler(errApp =>
{
    errApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        var ex = feature?.Error;
        var traceId = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;

        var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Title = "An unexpected error occurred.",
            Status = StatusCodes.Status500InternalServerError,
            Detail = app.Environment.IsDevelopment() ? ex?.ToString() : "Please contact support with the correlation id.",
            Instance = feature?.Path
        };

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        context.Response.Headers["X-Correlation-ID"] = traceId;

        var writer = new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsJsonAsync(new { correlationId = traceId, problem }, writer);
    });
});

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
