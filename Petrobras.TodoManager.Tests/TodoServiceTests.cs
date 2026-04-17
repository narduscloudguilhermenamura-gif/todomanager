using Moq;
using Petrobras.TodoManager.BLL;
using Petrobras.TodoManager.Models;

namespace Petrobras.TodoManager.Tests;

public class TodoServiceTests
{
    private readonly Mock<ITodoRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Fact]
    public async Task CreateAsync_ComDataRetroativa_DeveLancarExcecao()
    {
        var sut = new TodoService(_repository.Object, _unitOfWork.Object);
        var request = new CreateTodoRequest
        {
            Titulo = "Teste",
            DataVencimento = DateTime.UtcNow.Date.AddDays(-1),
        };

        var act = async () => await sut.CreateAsync(request, CancellationToken.None);

        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("Regra Petrobras: Vencimento não pode ser retroativo.", exception.Message);
        _repository.Verify(r => r.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_Valido_DeveExecutarFluxoTransacional()
    {
        var sut = new TodoService(_repository.Object, _unitOfWork.Object);
        var request = new CreateTodoRequest
        {
            Titulo = "Tarefa Petro",
            DataVencimento = DateTime.UtcNow.Date.AddDays(3),
        };

        await sut.CreateAsync(request, CancellationToken.None);

        _unitOfWork.Verify(u => u.BeginAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_QuandoRepositorioFalha_DeveExecutarRollback()
    {
        var sut = new TodoService(_repository.Object, _unitOfWork.Object);
        var entity = new TodoItem
        {
            Titulo = "Tarefa",
            DataVencimento = DateTime.UtcNow.Date.AddDays(1),
        };

        _repository.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _repository.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("erro"));

        var request = new UpdateTodoRequest { Status = TodoStatus.EmAndamento };

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.UpdateAsync(10, request, CancellationToken.None));

        _unitOfWork.Verify(u => u.BeginAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
