using System.Text;
using backend.clinicalbackend.Data;
using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Dto.validators.AuthValidators;
using backend.clinicalbackend.Dto.validators.DoctorAvailabilityValidators;
using backend.clinicalbackend.Dto.validators.DoctorValidators;
using backend.clinicalbackend.Dto.validators.DepartmentValidators;
using backend.clinicalbackend.Dto.validators.PatientValidators;
using backend.clinicalbackend.exceptions;
using backend.clinicalbackend.Infrastructure.Implementations;
using backend.clinicalbackend.Infrastructure.Interfaces;
using backend.clinicalbackend.repositories.Implementations;
using backend.clinicalbackend.repositories.Interfaces;
using backend.clinicalbackend.Services.Implementations;
using backend.clinicalbackend.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// -------------------------------------
// Configuration
// -------------------------------------

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT audience is missing.");

var allowedOrigins =
    builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>()
    ?? throw new InvalidOperationException(
        "CORS allowed origins are missing."
    );

// -------------------------------------
// Controllers
// -------------------------------------

builder.Services.AddControllers();

// -------------------------------------
// Database
// -------------------------------------

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"
        )
    )
);

// -------------------------------------
// Repositories
// -------------------------------------

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository,RefreshTokenRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<
    IDoctorAvailabilityRepository,
    DoctorAvailabilityRepository
>();



// -------------------------------------
// Services
// -------------------------------------

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<
    IDoctorAvailabilityService,
    DoctorAvailabilityService
>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// -------------------------------------
// Validators
// -------------------------------------

builder.Services.AddScoped<
    IValidator<RegisterDto>,
    RegisterDtoValidator
>();

builder.Services.AddScoped<
    IValidator<LoginDto>,
    LoginDtoValidator
>();

builder.Services.AddScoped<
    IValidator<CreateDepartmentDto>,
    CreateDepartmentDtoValidator
>();

builder.Services.AddScoped<
    IValidator<UpdateDepartmentDto>,
    UpdateDepartmentDtoValidator
>();

builder.Services.AddScoped<
    IValidator<CreateDoctorDto>,
    CreateDoctorDtoValidator
>();

builder.Services.AddScoped<
    IValidator<UpdateDoctorDto>,
    UpdateDoctorDtoValidator
>();

builder.Services.AddScoped<
    IValidator<UpdatePatientDto>,
    UpdatePatientDtoValidator
>();

builder.Services.AddScoped<
    IValidator<CreateDoctorAvailabilityDto>,
    CreateDoctorAvailabilityDtoValidator
>();

builder.Services.AddScoped<
    IValidator<UpdateDoctorAvailabilityDto>,
    UpdateDoctorAvailabilityDtoValidator
>();

// -------------------------------------
// Exception Handling
// -------------------------------------
// Exception Handling
builder.Services.AddExceptionHandler<AppExceptionHandler>();
builder.Services.AddProblemDetails();

// -------------------------------------
// JWT Authentication
// -------------------------------------

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme
    )
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    ),

                ClockSkew = TimeSpan.Zero
            };
    });

// -------------------------------------
// Authorization
// -------------------------------------

builder.Services.AddAuthorization();

// -------------------------------------
// CORS
// -------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// -------------------------------------
// Middleware
// -------------------------------------

app.UseExceptionHandler();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.Run();
