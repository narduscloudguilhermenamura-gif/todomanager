using Microsoft.AspNetCore.Mvc;
using Petrobras.TodoManager.Models;

namespace Petrobras.TodoManager.API.Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v1/todo")]
[Produces("application/json")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _service;

    public TodoController(ITodoService service)
    {
        _service = service;
    }

    /// <summary>
    /// Cria uma nova tarefa de acordo com as regras de negócio da Petrobras.
    /// </summary>
    /// <param name="request">Dados da tarefa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    [HttpPost]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TodoResponse>> Create([FromBody] CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(request, cancellationToken);
        var response = TodoResponse.FromEntity(created);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Lista tarefas com filtros opcionais por status e vencimento, com paginação.
    /// </summary>
    /// <param name="status">Status da tarefa.</param>
    /// <param name="vencimento">Data de vencimento para filtro exato (yyyy-MM-dd).</param>
    /// <param name="page">Página atual (mínimo 1).</param>
    /// <param name="pageSize">Quantidade por página (1 a 100).</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TodoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyCollection<TodoResponse>>> List(
        [FromQuery] ListTodoRequest request,
        CancellationToken cancellationToken = default)
    {
        var items = await _service.ListAsync(request.Status, request.Vencimento, request.Page, request.PageSize, cancellationToken);
        return Ok(items.Select(TodoResponse.FromEntity).ToList());
    }

    /// <summary>
    /// Obtém uma tarefa pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da tarefa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TodoResponse>> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        return Ok(TodoResponse.FromEntity(item));
    }

    /// <summary>
    /// Atualiza uma tarefa existente (atualização parcial dos campos informados).
    /// </summary>
    /// <param name="id">Identificador da tarefa.</param>
    /// <param name="request">Campos a serem atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TodoResponse>> Update([FromRoute] int id, [FromBody] UpdateTodoRequest request, CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateAsync(id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(TodoResponse.FromEntity(updated));
    }

    /// <summary>
    /// Remove uma tarefa pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da tarefa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var removed = await _service.DeleteAsync(id, cancellationToken);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}