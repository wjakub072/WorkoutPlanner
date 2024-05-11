using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkoutPlanner.Api;
using WorkoutPlanner.Api.Utils.Security;
using Serilog;
using Serilog.Events;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using WorkoutPlanner.Api.Data.Settings;
using WorkoutPlanner.Api.Utils;
using WorkoutPlanner.Database.Auth;
using WorkoutPlanner.Models.Auth;
using WorkoutPlanner.Database.Application;
using WorkoutPlanner.Api.Repositories;
using WorkoutPlanner.Api.Validators.RequestPayloads;
using FluentValidation;
using WorkoutPlanner.Api.Services;

string ProductionEnviromentCorsPolicy = "_WorkoutPlannerCorsPolicy";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(formatProvider: CultureInfo.CurrentCulture)
    .CreateBootstrapLogger();

try
{
    bool seed = SeedData.HasCalled(args);
    if (seed)
        args = SeedData.ClearArguments(args);

    var builder = WebApplication.CreateBuilder(args);
    
    builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("Authentication"));
    builder.Services.Configure<ConnectionStringsSettings>(builder.Configuration.GetSection("ConnectionStrings"));

    builder.Services.AddCors(
        options => options.AddPolicy(ProductionEnviromentCorsPolicy, policy =>
            policy.WithOrigins(
                    "") // hard hyped ip origins
                .AllowAnyHeader()
                .AllowAnyMethod()));


    // logging
    builder.Host.UseSerilog((ctx, setup) => setup
        .ReadFrom.Configuration(builder.Configuration));

    // auth database
    builder.Services.AddDbContext<IdentityDatabaseContext>((config) => {
        config.UseSqlServer(builder.Configuration.GetConnectionString("AuthDatabase"));
    });
    // app database
    builder.Services.AddDbContext<WorkoutPlannerDatabaseContext>((config) => {
        config.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDatabase"));

        if(builder.Environment.IsDevelopment())
        {
            config.EnableDetailedErrors();
            config.EnableSensitiveDataLogging();
        }
    });
    
    builder.Services.AddControllers();
    
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    // identity 
    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddEntityFrameworkStores<IdentityDatabaseContext>() 
        .AddDefaultTokenProviders();

    builder.Services.Configure<IdentityOptions>(options =>
    {
        if (builder.Environment.IsDevelopment())
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 3;
            options.Password.RequiredUniqueChars = 3;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        }
        else
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 3;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
        }

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        string issuer = builder.Configuration.GetValue<string>("Authentication:JwtIssuer") ?? "";
        options.Tokens.AuthenticatorIssuer = issuer;

        // cSpell: disable-next-line
        options.User.AllowedUserNameCharacters = "aąbcćdeęfghijklłmnńoópqrsśtuvwxyzźżAĄBCĆDEĘFGHIJKLŁMNŃOÓPQRSŚTUVWXYZŹŻ0123456789-._@+ ";

        options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
        options.ClaimsIdentity.UserNameClaimType = ClaimTypes.Name;
        options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
    });

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            AuthenticationSettings authenticationSettings = new AuthenticationSettings();
            builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = authenticationSettings.Issuer,
                ValidAudience = authenticationSettings.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.Key)),
                NameClaimType = ClaimTypes.Name,
                ClockSkew = TimeSpan.Zero
            };
        });
    
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("IsUser", policy =>
            policy.RequireAuthenticatedUser()
                .AddAuthenticationSchemes(
                   IdentityConstants.ApplicationScheme,
                   JwtBearerDefaults.AuthenticationScheme)
                .RequireRole("user", "admin"));

        options.AddPolicy("IsAdmin", policy =>
            policy.RequireAuthenticatedUser()
                .AddAuthenticationSchemes(
                   IdentityConstants.ApplicationScheme,
                   JwtBearerDefaults.AuthenticationScheme)
                .RequireRole("admin"));
    });

    // Utils 
    // builder.Services.AddScoped<IAuthorizationHandler, UserAuthorizationHandler>();
    builder.Services.AddScoped<ErrorHandlingMiddleware>();

    // // Converters
    // builder.Services.AddScoped<TimeParamsConverter>();
    // builder.Services.AddScoped<Base64Converter>();

    // // Services
    builder.Services.AddScoped<IAuthService, AuthService>();
    // builder.Services.AddScoped<IiCalService, ICalService>();
    // builder.Services.AddScoped<ICurrencyService, CurrencyService>();

    // // HttpClients 
    // builder.Services.AddScoped<CurrencyClient>();
    
    // // Repositories
    builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
    // builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
    // builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
    
    // // Providers 
    // builder.Services.AddScoped<ISummaryProvider, SummaryProvider>();

    // Validatiors
    builder.Services.AddValidatorsFromAssemblyContaining<SignUpRequestValidator>();
    
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddMvc(options =>
    {
        options.SuppressAsyncSuffixInActionNames = false;
    });

    var app = builder.Build();

    if (seed)
    {
        await SeedData.EnsureSeedDataAsync(app.Services);
        return;
    }

    app.UseSerilogRequestLogging();
    app.UseMiddleware<ErrorHandlingMiddleware>();
    
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.Headers.Append("Cache-Control", "public, max-age=31536000");
            context.Response.Headers.Append("Expires", DateTime.UtcNow.AddYears(1).ToString("R"));
        }

        await next();
    });

    if (!builder.Environment.IsDevelopment())
        app.UseCors(ProductionEnviromentCorsPolicy);
    else
        app.UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

    app.UseAuthentication();
    app.UseAuthorization();
    
    app.MapControllers();
    
    app.Run();
    
}
catch (Exception e)
{
    Log.Fatal("Major error during startup: {exception}", e.Message);
}
finally 
{
    Log.CloseAndFlush();
}