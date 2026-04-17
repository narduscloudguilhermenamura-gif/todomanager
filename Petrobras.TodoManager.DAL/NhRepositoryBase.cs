using NHibernate;
using Petrobras.TodoManager.Models;

namespace Petrobras.TodoManager.DAL;

public abstract class NhRepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class
{
    protected readonly ISession Session;

    protected NhRepositoryBase(ISession session)
    {
        Session = session;
    }

    public virtual Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return Session.GetAsync<TEntity>(id, cancellationToken);
    }

    public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return Session.SaveAsync(entity, cancellationToken);
    }

    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return Session.UpdateAsync(entity, cancellationToken);
    }

    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return Session.DeleteAsync(entity, cancellationToken);
    }
}