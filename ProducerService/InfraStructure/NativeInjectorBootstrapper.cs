using AutoMapper;
using Carter;
using FluentValidation;
using KafkaProducer.Api.Data.Interfaces;
using KafkaProducer.Api.Features.Message.Create;
using KafkaProducer.Api.InfraStructure.Context;
using KafkaProducer.Api.InfraStructure.Implimentation;
using KafkaProducer.Api.Shared.AutoMapper;
using KafkaProducer.Api.Shared.Behaviour;
using KafkaProducer.Api.Shared.Enums;
using KafkaProducer.Api.Shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace KafkaProducer.Api.InfraStructure
{
    public static class NativeInjectorBootstrapper
    {
        public static void RegisterDependencies(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();


            #region UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion


            #region Mediatr
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            services.AddValidatorsFromAssemblyContaining<CreateMessageValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            #endregion

            #region ApplicationSpecific
            services.AddDbContext<ApplicationDbContext>(op =>
            op.UseInMemoryDatabase("KenticoDb"));
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<IMapper, Mapper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddCarter();
            #endregion


            #region KafkaSpecific
            services.Configure<KafkaSettings>(configuration!.GetSection(KafkaSettings.Key));
            services.AddSingleton<IKafkaSettings>(provider => provider.GetRequiredService<IOptions<KafkaSettings>>().Value);
            services.AddSingleton<IKafkaSettings, KafkaSettings>();
            services.AddSingleton<IKafkaTopicManager, KafkaTopicManager>();
            services.AddSingleton<IPublisherManager, PublisherManager>();
            #endregion

            #region Message
            services.AddScoped<IRequestHandler<CreateMessageCommand, Result<Guid>>, CreateMessageCommandHandler>();
            #endregion


        }
        /// <summary>
        /// Make sure all topics from KafkaTopics has been created, Check if not already created, then create one.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static async Task EnsureTopicsExistAsync(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var topicManager = serviceProvider.GetRequiredService<IKafkaTopicManager>();

            foreach (KafkaTopics topic in Enum.GetValues(typeof(KafkaTopics)))
            {
                var topicName = topic.ToString();
                bool exists = await topicManager.TopicExistsAsync(topicName);
                if (!exists)
                {
                    string result = await topicManager.CreateTopicAsync(topicName);
                    Console.WriteLine(result);
                }
            }
        }
    }
}
