using FluentValidation;
using Library.Api.Auth;
using Library.Api.Data;
using Library.Api.Endpoints.Internal;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AnyOrigin", x => x.AllowAnyOrigin());
    });

    builder.Services.Configure<JsonOptions>(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.IncludeFields = true;
    });

    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

    builder.Services.AddAuthentication(ApiKeySchemeConstants.SchemeName)
    .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(ApiKeySchemeConstants.SchemeName, _ => { });
    builder.Services.AddAuthorization();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
        new SqliteConnectionFactory(builder.Configuration.GetConnectionString("Default")!));

    builder.Services.AddEndpoints<Program>(builder.Configuration);

    builder.Services.AddSingleton<DatabaseInitializer>();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();
}

var app = builder.Build();
{
    app.UseCors();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints<Program>();

    var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
    await databaseInitializer.InitializeAsync();

    app.Run();
}


