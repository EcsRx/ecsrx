using System.Threading.Tasks;
using EcsRx.Collections.Database;
using Persistity.Pipelines;

namespace EcsRx.Plugins.Persistence.Pipelines
{
    public interface ISaveEntityDatabasePipeline : IFlowPipeline
    {
        Task Execute(IEntityDatabase database);
    }
}