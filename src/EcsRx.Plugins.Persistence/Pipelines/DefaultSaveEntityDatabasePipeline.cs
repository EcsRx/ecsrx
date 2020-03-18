using EcsRx.Collections.Database;
using EcsRx.Plugins.Persistence.Data;
using EcsRx.Plugins.Persistence.Transformers;
using LazyData.Serialization;
using Persistity.Endpoints;
using Persistity.Pipelines;

namespace EcsRx.Plugins.Persistence.Pipelines
{
    public class DefaultSaveEntityDatabasePipeline : SendDataPipeline, ISaveEntityDatabasePipeline
    {
        public DefaultSaveEntityDatabasePipeline(ISerializer serializer, ISendDataEndpoint sendToEndpoint, IEntityDatabaseTransformer transformer) : base(serializer, sendToEndpoint, null, new[]{transformer})
        {
        }

        public void SaveEntityDatabase(IEntityDatabase entityDatabase)
        { Execute(entityDatabase); }
    }
}