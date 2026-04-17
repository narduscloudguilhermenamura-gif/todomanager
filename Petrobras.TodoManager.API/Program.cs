using Petrobras.TodoManager.BLL;
using Petrobras.TodoManager.DAL;
using Petrobras.TodoManager.API.Middlewares;
using Petrobras.TodoManager.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var connectionString = builder.Configuration.GetConnectionString("OracleDb")
    ?? throw new InvalidOperationException("Connection string 'OracleDb' não foi configurada.");

builder.Services.AddTodoDataAccess(connectionString);
builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

app.UseStandardErrorHandling();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
