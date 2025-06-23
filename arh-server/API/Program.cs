using System.Data.Common;
using API.Extensions;
using API.Helpers;
using API.Middleware;
using Core.Entities.Identity;
using FluentValidation.AspNetCore;
using Infrastructure.Data;

using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Builder;
//using Scalar.AspNetCore;
using Npgsql;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Core.Interfaces;
using AutoMapper;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

//using AutoMapper.Extensions.Microsoft.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);

var _config = builder.Configuration;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMastersService, MastersService>();
builder.Services.AddScoped<IPaValidator, PaValidator>();
builder.Services.AddScoped<ITokenService, TokenService>();
//AutoMapper.Extensions.Microsoft.DependencyInjection.ServiceCollectionExtensions
//    .AddAutoMapper(builder.Services, typeof(MappingProfiles));
builder.Services.AddAutoMapper(typeof(MappingProfiles)); builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    //// 🔐 Enable JWT Bearer authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and the JWT token.\r\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6..."
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
var services = builder.Services;
services.AddDistributedMemoryCache();

services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = false;
});

services.AddControllers();
services.AddDbContext<AppIdentityDbContext>(options =>
{
options.UseNpgsql(_config.GetConnectionString("IdentityConnection"));
});
services.AddDbContext<DBServerContext>(options =>
{
    options.UseNpgsql(_config.GetConnectionString("DefaultConnection"));
});
services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();
services.AddApplicationServices();
//services.AddIdentityServices(_config);

// //  This will enable Emailer service
// if (!string.IsNullOrEmpty(_config["AllowMailer"]))
// {
//     if (_config["AllowMailer"].ToString().ToUpper() == "YES")
//     {
//         services.AddHostedService<TimerService>();
//     }
// }

//  services.AddSwaggerDocumentation();

services.AddMvc();
    // .AddJsonOptions(options => {
    //     options.JsonSerializerOptions.PropertyNamingPolicy = null;
    // })
services.AddFluentValidationAutoValidation();

services.AddCors(opt => 
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"])),
                        ValidIssuer = _config["Token:Issuer"],
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        // NameClaimType = ClaimTypes.NameIdentifier,
                        NameClaimType = "nameid",
                        RoleClaimType = ClaimTypes.Role
                    };
                });
var serviceProvider = services.BuildServiceProvider();
using (var scope = serviceProvider.CreateScope())
{
    var dbCheck = scope.ServiceProvider.GetRequiredService<DBServerContext>();
}

var app = builder.Build();


//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    //app.MapOpenApi();

//    app.MapScalarApiReference(options =>
//    {
//        // Fluent API
//        options
//            .WithTitle("Server API")
//            .WithSidebar(true);

//        // Object initializer
//        options.Title = "Server API";
//        options.ShowSidebar = true;
//    });
//    app.UseSwaggerUI(options =>
//    {
//        options.SwaggerEndpoint("/openapi/v1.json", "API");
//    });
//}
if (app.Environment.IsDevelopment())
{
    // Remove Scalar if you don't use it
    // app.MapOpenApi(); 
    // app.MapScalarApiReference(...)

    app.UseSwagger(); // ✅ generates /swagger/v1/swagger.json
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"); // ✅ match this
        c.RoutePrefix = "swagger"; // access it via /swagger
    });
}

app.UseMiddleware<ExceptionMiddleware>();
            
app.UseStatusCodePagesWithReExecute("/errors/{0}");

//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Content")
    ), 
    RequestPath = "/content"
});
app.UseRouting();
if (!string.IsNullOrEmpty(_config["AllowCORS"]))
{
    if (_config["AllowCORS"].ToString().ToUpper() == "YES")
    {
        //  app.UseCors("CorsPolicy");
        app.UseCors("CorsPolicy");
    }
}

//app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

if (!string.IsNullOrEmpty(_config["AllowAPIAccess"]))
{
    if (_config["AllowAPIAccess"].ToString().ToUpper() == "YES")
    {
        //  app.UseSwaggerDocumentation();
    }
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
  //  endpoints.MapFallbackToController("Index", "Fallback");
});
app.Use(async (context, next) =>
{
    var isAuth = context.User.Identity?.IsAuthenticated ?? false;
    Console.WriteLine($"Is Authenticated: {isAuth}");
    await next();
});
using (var scope = app.Services.CreateScope())
{
    var srx = scope.ServiceProvider;
    var loggerFactory = srx.GetRequiredService<ILoggerFactory>();

    try
    {
        var userManager = srx.GetRequiredService<UserManager<AppUser>>();
        var identityContext = srx.GetRequiredService<AppIdentityDbContext>();

        await identityContext.Database.MigrateAsync();
        await AppIdentityDbContextSeed.SeedUserAsync(userManager);

        // var context = srx.GetRequiredService<ServerContext>();
        // await context.Database.MigrateAsync();
        // await SServerSeed.SeedAsync(context, loggerFactory, identityContext, userManager);
    }
    catch(PostgresException ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "SQL Exception");
        Console.WriteLine("SQL Exception - " + ex.Message);
    }
    catch(DbException ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "DB Exception");
        Console.WriteLine("DB Exception - " + ex.Message);
    }
    catch(Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occured during migration");
    }
}

app.Run();
