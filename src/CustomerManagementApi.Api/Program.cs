using HealthChecks.UI.Client;
using CustomerManagementApi.Api.Configurations;
using CustomerManagementApi.Api.HealthChecks;
using CustomerManagementApi.Api.Middlewares;
using CustomerManagementApi.CrossCutting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCorrelationIdGenerator();

builder.Services.AddLoggingService();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddMvc().AddJsonOptions(options => 
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddHealthChecks()
    .AddCheck<MongoDbHealthCheck>("mongodb", tags: new[] { "readiness" })
    .AddCheck<KafkaHealthCheck>("kafka", tags: new[] { "readiness" });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "customer-management-api", 
        Description = "API para gerenciamento dos dados de Clientes.", 
        Version = "v1" 
    });

    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CustomerManagementApi.Api.xml"));
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CustomerManagementApi.Application.xml"));
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CustomerManagementApi.Domain.xml"));

    c.OperationFilter<AppOriginHeaderOperationFilter>();
});

builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.EnableForHttps = true;
});

builder.Services.AddDependencies();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false }).AllowAnonymous();
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("readiness"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).AllowAnonymous();
app.MapControllers();
app.UsePathBase("/customer-management-api");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/customer-management-api/swagger/v1/swagger.json", "customer-management-api v1");
});
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseResponseCompression();

app.UseMiddleware<ExceptionMiddleware>();
//app.UseMiddleware<LoggingMiddleware>();

app.Run();