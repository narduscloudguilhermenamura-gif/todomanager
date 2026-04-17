namespace Petrobras.TodoManager.Models;

public class TodoItem
{
    public virtual int Id { get; protected set; }

    public virtual string Titulo { get; set; } = string.Empty;

    public virtual string? Descricao { get; set; }

    public virtual TodoStatus Status { get; set; } = TodoStatus.Pendente;

    public virtual DateTime DataVencimento { get; set; }

    public virtual DateTime DataCriacao { get; protected set; } = DateTime.UtcNow;

    public virtual DateTime? DataAlteracao { get; set; }
}