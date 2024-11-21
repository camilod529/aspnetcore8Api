using ApiMovies.Data;
using ApiMovies.MoviesMappers;
using ApiMovies.Repository;
using ApiMovies.Repository.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetValue<string>("ApiSettings:secret");

// version support
var apiVersioningBuilder = builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new ApiVersion(1, 0);
    option.ReportApiVersions = true;
    //option.ApiVersionReader = ApiVersionReader.Combine(
    //    new QueryStringApiVersionReader("api-version")//?api-version=1.0
    ///* new HeaderApiVersionReader("api-version")
    // new MediaTypeApiVersionReader("api-version)*/
    //);
});

apiVersioningBuilder.AddApiExplorer(option =>
{
    option.GroupNameFormat = "'v'VVV";
    option.SubstituteApiVersionInUrl = true;
});

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

// cache soport
builder.Services.AddResponseCaching();

//Add repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//Add autommaper
builder.Services.AddAutoMapper(typeof(MoviesMapper));

// Configuracion de autenticacion
builder.Services.AddAuthentication(
    x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
    ).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false; // Solo para desarrollo
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddControllers(option =>
{
    // Cache profile, cache global
    option.CacheProfiles.Add("DefaultCache", new CacheProfile() { Duration = 30 });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \r\n\r\n" +
            "\"Bearer qerqweqwewqedads2123\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Scheme = "Bearer",
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1.0",
            Title = "MoviesAPI v1",
            Description = "API de peliculas",
            TermsOfService = new Uri("https://render2web.com/promociones"),
            Contact = new OpenApiContact
            {
                Name = "Camilo Duran",
                Url = new Uri("https://render2web.com/promociones"),
            },
            License = new OpenApiLicense
            {
                Name = "Licencia personal",
                Url = new Uri("https://render2web.com/promociones"),
            }
        });
        options.SwaggerDoc("v2", new OpenApiInfo
        {
            Version = "v2.0",
            Title = "MoviesAPI v2",
            Description = "API de peliculas",
            TermsOfService = new Uri("https://render2web.com/promociones"),
            Contact = new OpenApiContact
            {
                Name = "Camilo Duran",
                Url = new Uri("https://render2web.com/promociones"),
            },
            License = new OpenApiLicense
            {
                Name = "Licencia personal",
                Url = new Uri("https://render2web.com/promociones"),
            }
        });
    }
);

// Soporte para CORS
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    })
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(option =>
    {
        option.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiMoviesV1");
        option.SwaggerEndpoint("/swagger/v2/swagger.json", "ApiMoviesV2");
    });
}

// CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Middleware para manejar autenticacion
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
