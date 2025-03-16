using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using ShopApp.Application.Profiles;
using ShopApp.Application.Services;
using ShopApp.Application.Validators;
using ShopApp.Domain.Models;
using ShopApp.Infrastructure.Jobs;
using ShopApp.Infrastructure.Services;
using ShopAppV2.Modules;

var builder = WebApplication.CreateBuilder(args);

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });

// Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your valid JWT token."
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


// Add Autofac and register modules
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Register Autofac modules
    containerBuilder.RegisterModule<RepositoryModule>();
    containerBuilder.RegisterModule<ServiceModule>();
    containerBuilder.RegisterModule<DataModule>();
});


// Add FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<ProductFormDtoValidator>(); // Register validators
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // Format FluentValidation errors into a single response
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .Select(e => e.Value.Errors.First().ErrorMessage)
                .ToList();

            return new BadRequestObjectResult(
                new Response<List<string>>("fail", "Validation error(s) occured!", errors));
        };
    });

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(ProductProfile));

// Add http context accessor
builder.Services.AddHttpContextAccessor();

// Add Quartz jobs
builder.Services.AddQuartz(q =>
{
    // Automatically register all jobs via IJobConfig
    var jobConfigType = typeof(IJobConfig);
    var jobConfigs = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(s => s.GetTypes())
        .Where(p => jobConfigType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
        .Select(Activator.CreateInstance)
        .Cast<IJobConfig>();

    foreach (var configurator in jobConfigs) configurator.ConfigureJob(q);
});

builder.Services.AddQuartzHostedService(q =>
{
    q.WaitForJobsToComplete = true; // Wait for jobs to complete on shutdown
});

// Add memory caching
builder.Services.AddMemoryCache();

builder.Services.AddControllers();

builder.Services.AddHttpClient<IImageUploaderService, RemoteImageUploaderService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5215"); // Replace with the ImageUploader API URL
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();