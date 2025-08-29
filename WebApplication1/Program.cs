using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WebApplication1.Services;
using WebApplication1.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
// Configurar Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sistema de Atendimento nas Inspetorias",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<IAtendimentoInterface, AtendimentoService>();
builder.Services.AddScoped<IAtendenteInterface, AtendenteService>();
builder.Services.AddScoped<IAtendimentoInterface, AtendimentoService>();
builder.Services.AddScoped<IFeedbackInterface, FeedbackService>();
builder.Services.AddScoped<IInspetoriaInterface, InspetoriaService>();
builder.Services.AddScoped<IProfissionalInterface, ProfissionalService>();


//Program como parametro diz respeito a toda a aplicação
builder.Services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
         policyBuilder =>
         {
             policyBuilder
                 .WithOrigins(
                     "http://localhost:3000",
                     "http://localhost:5173",
                     "http://localhost:5500",
                     "http://localhost:5501",
                     "https://localhost:3000",
                     "https://localhost:5173",
                     "https://irispatrimonio.creaba.org.br"
                 )
                 .AllowAnyHeader()
                 .AllowAnyMethod();
                 //.AllowCredentials(); // Importante para cookies
         });
});

// Configurar autenticação para validar tokens do Entra ID
// No Program.cs, adicione mais logs no JWT Bearer Events
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var azureAdConfig = builder.Configuration.GetSection("AzureAd");
        var tenantId = azureAdConfig["TenantId"];
        var clientId = azureAdConfig["ClientId"];

        options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
        options.Audience = clientId;


        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = $"https://login.microsoftonline.com/{tenantId}/v2.0",
            ValidAudience = clientId,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

                // Log do header Authorization
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                logger.LogInformation($"Authorization Header: {authHeader}");

                // Tenta pegar o token do header
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                    logger.LogInformation($"Token extraído: {(context.Token?.Length > 20 ? context.Token.Substring(0, 20) : context.Token)}...");
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError($"Autenticação falhou: {context.Exception.Message}");
                logger.LogError($"Exception Type: {context.Exception.GetType().Name}");

                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation($"Token validado com sucesso para: {context.Principal?.Identity?.Name}");
                return Task.CompletedTask;
            }
        };
    });

// Adicionar autorização
builder.Services.AddAuthorization();

// Adicionar logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();


var app = builder.Build();

// Configurar o pipeline




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ?? Aqui é o ponto importante:
// UseRouting precisa vir antes de UseCors
app.UseRouting();

// ORDEM IMPORTANTE: CORS antes de Authentication/Authorization
app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Habilita resposta para OPTIONS
app.MapMethods("{*path}", new[] { "OPTIONS" }, () => Results.Ok());

app.Run();


/* NÃO APAGAR
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WebApplication1.Services;
using WebApplication1.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAtendimentoInterface, AtendimentoService>();
builder.Services.AddScoped<IAtendenteInterface, AtendenteService>();
builder.Services.AddScoped<IAtendimentoInterface, AtendimentoService>();
builder.Services.AddScoped<IFeedbackInterface, FeedbackService>();
builder.Services.AddScoped<IInspetoriaInterface, InspetoriaService>();
builder.Services.AddScoped<IProfissionalInterface, ProfissionalService>();

//Program como parametro diz respeito a toda a aplicação
builder.Services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();
*/