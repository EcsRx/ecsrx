using System.Threading.Tasks;
using EcsRx.Collections.Database;
using Persistity.Pipelines;

namespace EcsRx.Plugins.Persistence.Pipelines
{
    public interface ILoadEntityDatabasePipeline : IFlowPipeline
    {
        Task<IEntityDatabase> Execute();
    }
}