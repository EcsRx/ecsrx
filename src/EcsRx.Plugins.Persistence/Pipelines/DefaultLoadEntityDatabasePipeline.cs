using EcsRx.Plugins.Persistence.Transformers;
using LazyData.Serialization;
using Persistity.Endpoints;
using Persistity.Pipelines;

namespace EcsRx.Plugins.Persistence.Pipelines
{
    public class DefaultLoadEntityDatabasePipeline : ReceiveDataPipeline, ILoadEntityDatabasePipeline
    {
        public DefaultLoadEntityDatabasePipeline(IDeserializer deserializer, IReceiveDataEndpoint receiveFromEndpoint, IEntityDatabaseTransformer transformer) : base(deserializer, receiveFromEndpoint, null, new[] {transformer})
        {
        }
    }
}