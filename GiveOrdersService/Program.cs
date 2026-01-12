using GiveOrdersService.Extensions;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.ConfigureRabbitMQ();
builder.Services.AddSingleton<Task<IConnection>>(sp =>
{
    var factory = new ConnectionFactory
    {
        HostName = builder.Configuration["MessageBroker:Host"],
        UserName = builder.Configuration["MessageBroker:Username"],
        Password = builder.Configuration["MessageBroker:Password"]
    };
    return factory.CreateConnectionAsync();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
