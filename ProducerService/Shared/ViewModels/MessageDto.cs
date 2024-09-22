using Microsoft.AspNetCore.Mvc;

namespace KafkaProducer.Api.Shared.ViewModels
{
    public class MessageDto
    {
        [FromRoute]
        public Guid Id { get; set; }
        public string Value { get; set; }
    }
}
