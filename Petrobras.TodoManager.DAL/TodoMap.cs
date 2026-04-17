using FluentNHibernate.Mapping;
using Petrobras.TodoManager.Models;

namespace Petrobras.TodoManager.DAL;

public class TodoMap : ClassMap<TodoItem>
{
    public TodoMap()
    {
        Table("TB_PETRO_TODO");
        Id(x => x.Id).Column("CD_TAREFA").GeneratedBy.Sequence("SEQ_PETRO_TODO");
        Map(x => x.Titulo).Column("DS_TITULO").Length(100).Not.Nullable();
        Map(x => x.Descricao).Column("DS_DESCRICAO").Length(500);
        Map(x => x.Status).Column("ST_STATUS").CustomType<TodoStatus>().Not.Nullable();
        Map(x => x.DataVencimento).Column("DT_VENCIMENTO").Not.Nullable();
        Map(x => x.DataCriacao).Column("DT_CRIACAO").Not.Nullable().ReadOnly();
        Map(x => x.DataAlteracao).Column("DT_ALTERACAO");
    }
}
