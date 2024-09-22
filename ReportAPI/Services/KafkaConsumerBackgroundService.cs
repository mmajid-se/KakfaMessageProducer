using Confluent.Kafka;
using KafkaConsumer.Database;
using KafkaConsumer.Models;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;
using static KafkaConsumer.Shared.SharedEnum;

namespace KafkaConsumer.Services
{
    public class KafkaConsumerBackgroundService : BackgroundService
    {
        private ReportDbContext _reportDbContext;
        public KafkaConsumerBackgroundService(ReportDbContext reportDbContext)
        {
            _reportDbContext = reportDbContext;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = "localhost:9092",
                GroupId = "myReportConsumerGroup",
                ClientId = Guid.NewGuid().ToString(),
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            consumer.Subscribe(new[]
            {
            KafkaTopics.MessageTopic.ToString(),
            KafkaTopics.OrderTopic.ToString(),
            KafkaTopics.InventoryTopic.ToString()
            });

            var reportEntries = new List<object>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    ProcessMessage(consumeResult);
                    consumer.Commit(consumeResult);
                }
                catch (ConsumeException e)
                {
                    // Handle exceptions
                }
            }
        }
        private void ProcessMessage(ConsumeResult<Ignore, string> consumeResult)
        {
            var consumeTopic = consumeResult.Topic;

            switch (consumeTopic)
            {
                case var toptic when toptic == nameof(KafkaTopics.MessageTopic):
                    KafkaTopicsEmployeeTopic(consumeResult.Message);
                    break;
                case var toptic when toptic == KafkaTopics.OrderTopic.ToString():
                    KafkaTopicsOrderTopic(consumeResult.Message);
                    break;
                case var toptic when toptic == KafkaTopics.InventoryTopic.ToString():
                    KafkaTopicsInventoryTopic(consumeResult.Message);
                    break;
                default:
                    HandleUnknownTopic(consumeResult.Message);
                    break;
            }
        }

        private void KafkaTopicsEmployeeTopic(Message<Ignore, string> message)
        {
            try
            {
                // Deserialize the message value into an Employee object
                var employee = JsonSerializer.Deserialize<Message>(message.Value);

                if (employee == null)
                {
                    Console.WriteLine("Deserialization resulted in null object.");
                    return;
                }

                // Process the employee object
                Console.WriteLine($"Processing Employee: ID={employee.Id}, Name={employee.value}");
            }
            catch (JsonException ex)
            {
                // Handle JSON deserialization errors
                Console.WriteLine($"Error deserializing message: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle any other errors
                Console.WriteLine($"Unexpected error occurred: {ex.Message}");
            }

        }

        private void KafkaTopicsOrderTopic(Message<Ignore, string> message)
        {
            try
            {
                var order = JsonSerializer.Deserialize<Order>(message.Value);

                if (order == null)
                {
                    Console.WriteLine("Deserialization resulted in null object.");
                    return;
                }

                Console.WriteLine($"Processing Employee: ID={order.Id}, Name={order.ProdcutId}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing message: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred: {ex.Message}");
            }
        }

        private void KafkaTopicsInventoryTopic(Message<Ignore, string> message)
        {
            try
            {
                var inventory = JsonSerializer.Deserialize<Inventory>(message.Value);

                if (inventory == null)
                {
                    Console.WriteLine("Deserialization resulted in null object.");
                    return;
                }
                Console.WriteLine($"Processing Employee: ID={inventory.Id}, Name={inventory.Quantity}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing message: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred: {ex.Message}");
            }
        }

        private void HandleUnknownTopic(Message<Ignore, string> message)
        {
            try
            {
                var employee = JsonSerializer.Deserialize<Message>(message.Value);

                if (employee == null)
                {
                    Console.WriteLine("Deserialization resulted in null object.");
                    return;
                }

                Console.WriteLine($"Processing Employee: ID={employee.Id}, Name={employee.value}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing message: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred: {ex.Message}");
            }
        }

    }
}
