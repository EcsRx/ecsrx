using System.Threading.Tasks;
using EcsRx.Collections.Database;
using EcsRx.Plugins.Persistence.Transformers;
using LazyData.Serialization;
using Persistity.Endpoints;

namespace EcsRx.Plugins.Persistence.Pipelines
{
    public class DefaultSaveEntityDatabasePipeline : ISaveEntityDatabasePipeline
    {
        public ISerializer Serializer { get; }
        public IToEntityDatabaseTransformer Transformer { get; }
        public ISendDataEndpoint Endpoint { get; }

        public DefaultSaveEntityDatabasePipeline(ISerializer serializer, IToEntityDatabaseTransformer transformer, ISendDataEndpoint endpoint)
        {
            Serializer = serializer;
            Transformer = transformer;
            Endpoint = endpoint;
        }

        public Task Execute(IEntityDatabase entityDatabase)
        { return Execute(entityDatabase, null); }

        public Task<object> Execute(object input = null, object state = null)
        {
            var transformedData = Transformer.Transform(input);
            var output = Serializer.Serialize(transformedData, true);
            return Endpoint.Send(output);
        }
    }
}