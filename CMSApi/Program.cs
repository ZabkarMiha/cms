using System.Text;
using AutoMapper;
using CMSApi;
using CMSApi.Business;
using CMSApi.Configurations;
using CMSApi.Services;
using CMSApi.Repository;
using Core;
using Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Core.Services;
using Core.Configurations;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddControllers(options => options.Filters.Add<GlobalExceptionFilter>());

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
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
        }
    );
});

builder.Services.AddDbContext<CoreDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"));
});

builder.Services
    .AddIdentity<ProfileModel, IdentityRole<Guid>>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Lockout.AllowedForNewUsers = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 4;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<CoreDbContext>();

builder.Services.Configure<DataProtectionTokenProviderOptions>(
    opt => opt.TokenLifespan = TimeSpan.FromHours(2)
);

builder.Services.Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<AzureAccount>(configuration.GetSection("AzureAccount"));
builder.Services.Configure<Mail>(configuration.GetSection("Mail"));
builder.Services.Configure<JWT>(configuration.GetSection("JWT"));

builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddScoped<IAssignRepository, AssignRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddTransient<IProfileBusinessLayer, ProfileBusinessLayer>();
builder.Services.AddTransient<IAssignBusinessLayer, AssignBusinessLayer>();
builder.Services.AddTransient<ICarBusinessLayer, CarBusinessLayer>();
builder.Services.AddTransient<IRoleBusinessLayer, RoleBusinessLayer>();
builder.Services.AddTransient<IJWTSecurityTokenService, JWTSecurityTokenService>();
builder.Services.AddTransient<IBlobBusinessLayer, BlobBusinessLayer>();
builder.Services.AddTransient<IFileService, FileService>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = configuration["JWT:ValidAudience"],
            ValidIssuer = configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JWT:Secret"])
            )
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetService<UserManager<ProfileModel>>();
        var mapper = scope.ServiceProvider.GetService<IMapper>();
        StartupSeedService.IdentitySeed(userManager, roleManager, mapper);
    }
    catch (Exception) { }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");

        options.DocumentTitle = "My API Documentation";
        options.DocExpansion(DocExpansion.None);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
