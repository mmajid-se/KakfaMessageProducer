using AutoMapper;
using KafkaProducer.Api.Data.Models;
using KafkaProducer.Api.Features.Message.Create;

namespace KafkaProducer.Api.Shared.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateMessageCommand, Message>();
        }
    }
}
