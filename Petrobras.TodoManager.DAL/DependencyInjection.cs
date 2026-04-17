using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using Oracle.ManagedDataAccess.Client;
using Petrobras.TodoManager.Models;
using System.Data;

namespace Petrobras.TodoManager.DAL;

public static class DependencyInjection
{
    public static IServiceCollection AddTodoDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<ISessionFactory>(_ =>
            Fluently.Configure()
                .Database(OracleManagedDataClientConfiguration.Oracle10.ConnectionString(connectionString))
                .Mappings(mapping => mapping.FluentMappings.AddFromAssemblyOf<TodoMap>())
                .BuildSessionFactory());

        services.AddScoped<ISession>(provider => provider.GetRequiredService<ISessionFactory>().OpenSession());
        services.AddScoped<IDbConnection>(_ => new OracleConnection(connectionString));
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}