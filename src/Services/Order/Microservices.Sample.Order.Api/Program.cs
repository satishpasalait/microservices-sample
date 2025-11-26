using FluentValidation;
using MassTransit;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Microservices.Sample.Order.Infrastructure.Data;
using Microservices.Sample.Order.Infrastructure.Repositories;
using Microservices.Sample.Order.Domain.Repositories;
using Microservices.Sample.Order.Application.Services;
using Microservices.Sample.Order.Application.Commands;
using Microservices.Sample.Order.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Microservices.Sample.Common.Resilience;

var builder = WebApplication.CreateBuilder(args);

//Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) 
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

//Add Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Database
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderConnection")));

//Repositories with Resilience
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<IOrderRepository>(sp =>
{
    var respository = sp.GetRequiredService<IOrderRepository>();
    var logger = sp.GetRequiredService<ILogger<ResilientOrderRepository>>();
    return new ResilientOrderRepository(respository, logger);
});

//External Services with Resilience Polocies (Retry + Circuit Breaker)
builder.Services.AddHttpClient<IProductService, ProductService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:ProductService"] ?? "https://localhost:5001");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler((serviceProvider, request) =>
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    return ResiliencePolicies.GetHttpResiliencePolicy(logger);
});

//MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

//AutoMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

//MassTransit with RabbitMQ and Retry Polocies
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        //Configure retry polocies for message consumption
        cfg.UseMessageRetry(r =>
        {
            r.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5));
        });
        cfg.UseInMemoryOutbox();
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

//Ensure Database is Created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();

