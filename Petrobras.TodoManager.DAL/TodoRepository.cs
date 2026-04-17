using Dapper;
using NHibernate;
using Petrobras.TodoManager.Models;
using System.Data;

namespace Petrobras.TodoManager.DAL;

public class TodoRepository : NhRepositoryBase<TodoItem, int>, ITodoRepository
{
    private readonly IDbConnection _dbConnection;

    public TodoRepository(ISession session, IDbConnection dbConnection)
        : base(session)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IReadOnlyCollection<TodoItem>> GetAllAsync(
        TodoStatus? status,
        DateTime? dataVencimento,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var offset = (page - 1) * pageSize;

        var sql = @"
SELECT
    CD_TAREFA AS Id,
    DS_TITULO AS Titulo,
    DS_DESCRICAO AS Descricao,
    ST_STATUS AS Status,
    DT_VENCIMENTO AS DataVencimento,
    DT_CRIACAO AS DataCriacao,
    DT_ALTERACAO AS DataAlteracao
FROM TB_PETRO_TODO
WHERE 1 = 1";

        var parameters = new DynamicParameters();

        if (status.HasValue)
        {
            sql += " AND ST_STATUS = :status";
            parameters.Add("status", (int)status.Value);
        }

        if (dataVencimento.HasValue)
        {
            sql += " AND TRUNC(DT_VENCIMENTO) = :dataVencimento";
            parameters.Add("dataVencimento", dataVencimento.Value.Date);
        }

        sql += " ORDER BY DT_VENCIMENTO ASC, CD_TAREFA ASC OFFSET :offset ROWS FETCH NEXT :pageSize ROWS ONLY";
        parameters.Add("offset", offset);
        parameters.Add("pageSize", pageSize);

        var result = await _dbConnection.QueryAsync<TodoItem>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        return result.ToList();
    }

}