using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;

using Microsoft.OpenApi.Models;

using System.Text;

using VexaDriveAPI.Context;

using VexaDriveAPI.JWT;

using VexaDriveAPI.Repository.OwnerServices;

using VexaDriveAPI.Repository.VehicleServices;
 
var builder = WebApplication.CreateBuilder(args);
 
// Controllers

builder.Services.AddControllers();
 
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

builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();

builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
 
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

        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

        ValidAudience = builder.Configuration["JwtSettings:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(

            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)),

        ClockSkew = TimeSpan.Zero

    };

});
 
// AutoMapper

builder.Services.AddAutoMapper(typeof(Program));
 
var app = builder.Build();
 
// Swagger

if (app.Environment.IsDevelopment())

{

    app.UseSwagger();

    app.UseSwaggerUI(c =>

    {

        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VexaDrive API v1");

        c.RoutePrefix = string.Empty; // swagger at root

    });

}
 
app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();
 
app.MapControllers();
 
app.Run();

 