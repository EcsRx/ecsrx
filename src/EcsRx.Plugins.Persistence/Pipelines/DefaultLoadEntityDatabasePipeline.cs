using System.Threading.Tasks;
using EcsRx.Collections.Database;
using EcsRx.Plugins.Persistence.Transformers;
using LazyData.Serialization;
using Persistity.Endpoints;

namespace EcsRx.Plugins.Persistence.Pipelines
{
    public class DefaultLoadEntityDatabasePipeline : ILoadEntityDatabasePipeline
    {
        public IDeserializer Deserializer { get; }
        public IFromEntityDatabaseTransformer Transformer { get; }
        public IReceiveDataEndpoint Endpoint { get; }

        public DefaultLoadEntityDatabasePipeline(IDeserializer deserializer, IFromEntityDatabaseTransformer transformer, IReceiveDataEndpoint endpoint)
        {
            Deserializer = deserializer;
            Transformer = transformer;
            Endpoint = endpoint;
        }

        public async Task<IEntityDatabase> Execute()
        { return (IEntityDatabase) await Execute(null); }

        public async Task<object> Execute(object input = null, object state = null)
        {
            var endpointData = await Endpoint.Receive();
            var deserializedData = Deserializer.Deserialize(endpointData);
            var transformedData = Transformer.Transform(deserializedData);
            return transformedData;
        }
    }
}