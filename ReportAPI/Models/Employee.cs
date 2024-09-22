namespace KafkaConsumer.Models
{
    public record Message
    {
        public Guid Id { get; set; }
        public string value { get; set; } = string.Empty;
    }
}
