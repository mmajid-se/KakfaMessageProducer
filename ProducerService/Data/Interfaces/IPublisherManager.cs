using KafkaProducer.Api.Shared.Enums;
using System.Threading;

namespace KafkaProducer.Api.Data.Interfaces
{
    public interface IPublisherManager
    {
        Task ProduceMessageAsync(KafkaTopics topic, string key, string value, CancellationToken cancellationToken = default);
        Task ProduceMessageAsync(string topic, string key, string value, int partition, CancellationToken cancellationToken = default);

    }
}
