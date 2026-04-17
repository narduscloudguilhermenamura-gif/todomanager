using Petrobras.TodoManager.Models;

namespace Petrobras.TodoManager.BLL;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public TodoService(ITodoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TodoItem> CreateAsync(CreateTodoRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCreateRequest(request);

        var item = new TodoItem
        {
            Titulo = request.Titulo.Trim(),
            Descricao = request.Descricao?.Trim(),
            Status = TodoStatus.Pendente,
            DataVencimento = request.DataVencimento.Date,
            DataAlteracao = DateTime.UtcNow,
        };

        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            await _repository.AddAsync(item, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return item;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public Task<IReadOnlyCollection<TodoItem>> ListAsync(
        TodoStatus? status,
        DateTime? dataVencimento,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(status, dataVencimento?.Date, page, pageSize, cancellationToken);
    }

    public Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<TodoItem?> UpdateAsync(int id, UpdateTodoRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return null;
        }

        if (request.Titulo is not null)
        {
            if (string.IsNullOrWhiteSpace(request.Titulo))
            {
                throw new ArgumentException("Título é obrigatório.");
            }

            item.Titulo = request.Titulo.Trim();
        }

        if (request.Descricao is not null)
        {
            item.Descricao = request.Descricao.Trim();
        }

        if (request.DataVencimento.HasValue)
        {
            if (request.DataVencimento.Value.Date < DateTime.UtcNow.Date)
            {
                throw new ArgumentException("Regra Petrobras: Vencimento não pode ser retroativo.");
            }

            item.DataVencimento = request.DataVencimento.Value.Date;
        }

        if (request.Status.HasValue)
        {
            if (item.Status == TodoStatus.Concluido && request.Status.Value != TodoStatus.Concluido)
            {
                throw new ArgumentException("Tarefa concluída não pode retornar para estado anterior.");
            }

            item.Status = request.Status.Value;
        }

        item.DataAlteracao = DateTime.UtcNow;

        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            await _repository.UpdateAsync(item, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return item;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            await _repository.DeleteAsync(item, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static void ValidateCreateRequest(CreateTodoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Titulo))
        {
            throw new ArgumentException("Título é obrigatório.");
        }

        if (request.DataVencimento.Date < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Regra Petrobras: Vencimento não pode ser retroativo.");
        }
    }
}
