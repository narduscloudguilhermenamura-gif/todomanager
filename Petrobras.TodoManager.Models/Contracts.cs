namespace Petrobras.TodoManager.Models;

public interface IRepository<TEntity, in TKey> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface ITodoRepository : IRepository<TodoItem, int>
{
    Task<IReadOnlyCollection<TodoItem>> GetAllAsync(
        TodoStatus? status,
        DateTime? dataVencimento,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

public interface IUnitOfWork : IDisposable
{
    Task BeginAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}

public interface ITodoService
{
    Task<TodoItem> CreateAsync(CreateTodoRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TodoItem>> ListAsync(
        TodoStatus? status,
        DateTime? dataVencimento,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<TodoItem?> UpdateAsync(int id, UpdateTodoRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}