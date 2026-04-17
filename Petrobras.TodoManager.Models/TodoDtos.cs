using System.ComponentModel.DataAnnotations;

namespace Petrobras.TodoManager.Models;

public class CreateTodoRequest
{
    [Required(ErrorMessage = "Título é obrigatório.")]
    [MaxLength(100, ErrorMessage = "Título deve ter no máximo 100 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres.")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "Data de vencimento é obrigatória.")]
    public DateTime DataVencimento { get; set; }
}

public class UpdateTodoRequest
{
    [MaxLength(100, ErrorMessage = "Título deve ter no máximo 100 caracteres.")]
    public string? Titulo { get; set; }

    [MaxLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres.")]
    public string? Descricao { get; set; }

    public TodoStatus? Status { get; set; }

    public DateTime? DataVencimento { get; set; }
}

public class ListTodoRequest
{
    public TodoStatus? Status { get; set; }

    public DateTime? Vencimento { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "O parâmetro page deve ser maior ou igual a 1.")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "O parâmetro pageSize deve estar entre 1 e 100.")]
    public int PageSize { get; set; } = 10;
}

public class TodoResponse
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public TodoStatus Status { get; set; }

    public DateTime DataVencimento { get; set; }

    public DateTime DataCriacao { get; set; }

    public DateTime? DataAlteracao { get; set; }

    public static TodoResponse FromEntity(TodoItem item)
    {
        return new TodoResponse
        {
            Id = item.Id,
            Titulo = item.Titulo,
            Descricao = item.Descricao,
            Status = item.Status,
            DataVencimento = item.DataVencimento,
            DataCriacao = item.DataCriacao,
            DataAlteracao = item.DataAlteracao,
        };
    }
}