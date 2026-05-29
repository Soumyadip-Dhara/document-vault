using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using documentvaultapi.BAL.Services;
using documentvaultapi.BAL.Services;
//using documentvaultapi.BAL.Services.MQueue.MasterQueue;
//using documentvaultapi.Common.Constants;
//using documentvaultapi.RabbitMQ;
//using documentvaultapi.Consumer.ConsumeAck;
//using documentvaultapi.Consumer.MasterConsumer;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.DTOs;
//using documentvaultapi.DTOs.Master;
//using documentvaultapi.DTOs.RabbitMQ;
//using documentvaultapi.DTOs.RabbitMQ.FromBilling.ActiveHoaMaster;
//using documentvaultapi.DTOs.RabbitMQ.FromBilling.Allotment;
//using documentvaultapi.DTOs.RabbitMQ.FromBilling.DDOAllotment;
//using documentvaultapi.DTOs.RabbitMQ.FromBilling.MotherSanction;
//using documentvaultapi.DTOs.RabbitMQ.FromBilling.NewBill;
//using documentvaultapi.DTOs.RabbitMQ.FromJit;
//using documentvaultapi.DTOs.RabbitMQ.FromJit.NewBill;
//using documentvaultapi.DTOs.RabbitMQ.FromMaster;
using documentvaultapi.RabbitMQ.Models;
using documentvaultapi.RbbitMQ;
//using documentvaultapi.RabbitMQ.Validators;
using FluentValidation;
using documentvaultapi.Common.Constants;
using documentvaultapi.RabbitMQ.Services;
using documentvaultapi.Consumer.ConsumeAck;
using documentvaultapi.RbbitMQ.Models.MQueue.FromUM;
using documentvaultapi.RbbitMQ.Services.FromUM;
using documentvaultapi.RbbitMQ.Consumers;
using documentvaultapi.RbbitMQ.Validators;
using documentvaultapi.RbbitMQ.Models.MQueue;
using documentvaultapi.RbbitMQ.Services;

namespace documentvaultapi.Extensions
{
    public static class RabbitMqRegiserExtensions
    {
        public static IServiceCollection AddRabbitMQ(
         this IServiceCollection services,
         IConfiguration configuration)
        {
            //var rabbitMQConfig = configuration.GetSection("RabbitMQConnection").Get<RabbitMQConfigurationModel>();
            //if (rabbitMQConfig == null)
            //{
            //    throw new InvalidOperationException("RabbitMQ configuration is missing");
            //}

            //services.AddSingleton(rabbitMQConfig);
            //services.AddSingleton<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>();

            //return services;


            // Try new multi-host format first: "RabbitMQConnections"
            var hosts = configuration.GetSection("RabbitMQConnections")
                .Get<Dictionary<string, RabbitMQConfigurationModel>>();

            // Fallback to old single-host format: "RabbitMQConnection"
            if (hosts == null || hosts.Count == 0)
            {
                var singleConfig = configuration.GetSection("RabbitMQConnection")
                    .Get<RabbitMQConfigurationModel>();

                if (singleConfig == null)
                {
                    throw new InvalidOperationException(
                        "RabbitMQ configuration is missing. Ensure 'RabbitMQConnections' or 'RabbitMQConnection' section exists in appsettings.");
                }

                hosts = new Dictionary<string, RabbitMQConfigurationModel>
                {
                    { "Default", singleConfig }
                };
            }

            var multiConfig = new RabbitMQMultiHostConfiguration { Hosts = hosts };

            services.AddSingleton(multiConfig);

            // Register default single config for backward compatibility (used by RabbitMqService)
            //if (hosts.TryGetValue("Default", out var defaultConfig))
            //{

            //    services.AddSingleton(defaultConfig);
            //}


            services.AddSingleton<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>();

            return services;

        }

        public static IServiceCollection AddMessageProcessing(
            this IServiceCollection services)
        {
            // ApplicationMap Processing
            services.AddScoped<IValidator<ConsumeApplicationMapDTO>, ApplicationMapValidator>();
            services.AddScoped<IMessageProcessor<ConsumeApplicationMapDTO>, ApplicationMapQueueService>();
            services.AddHostedService<ApplicationMapConsumer>();

            //// Document Upload Processing
            services.AddScoped<IValidator<DocumentUploadMessageDTO>, DocumentUploadMessageValidator>();
            services.AddScoped<IMessageProcessor<DocumentUploadMessageDTO>, DocumentUploadQueueService>();
            services.AddHostedService<DocumentUploadConsumer>();

            
            // ================= ADD ACK CONSUMERS HERE ==================

            //Register ACK Validator
            services.AddSingleton<IValidator<AckPayloadModel>, MQueueAckValidator>();

            var ackQueues = new[]
        {
                //MessageQueueConstants.WBJIT_CTS_BILLING_BILL_STATUS_ACK,
                //MessageQueueConstants.WBJIT_CTS_BILLING_CHALLAN_ACK,
                //MessageQueueConstants.WBJIT_CTS_BILLING_DDO_ALLOTMENT_ACTUAL_AMOUNT_ACK,
                //MessageQueueConstants.WBJIT_CTS_BILLING_FAILED_BENEFICIARY_ACK,
                //MessageQueueConstants.WBJIT_CTS_BILLING_OBJECTED_BILL_ACK,
                //MessageQueueConstants.WBJIT_CTS_BILLING_PFMS_FAILED_ACK,
                //MessageQueueConstants.WBJIT_CTS_BILLING_PFMS_FILE_STATUS_DETAILS_ACK,
                //MessageQueueConstants.WBJIT_CTS_BILLING_SUCCESS_BENEFICIARY_ACK,
                //MessageQueueConstants.WBJIT_CTS_BILLING_TOKEN_ACK,
                //MessageQueueConstants.WBJIT_CTS_BILLING_VOUCHER_ACK,
                MessageQueueConstants.UM_APPLICATION_MAP_ACK,
                MessageQueueConstants.DOCUMENT_UPLOAD_ACK
        };
            foreach (var queue in ackQueues)
            {
                services.AddSingleton<IHostedService>(sp =>
                    new RabbitMQAckConsumerHostedService(
                        sp.GetRequiredService<ILogger<RabbitMQAckConsumer>>(),
                        sp.GetRequiredService<IRabbitMQConnectionFactory>(),
                        sp.GetRequiredService<IServiceScopeFactory>(),
                        sp.GetRequiredService<IValidator<AckPayloadModel>>(),
                        sp.GetRequiredService<IConfiguration>(),
                        queue
                    ));
            }
            // ============================================================


            return services;
        }
    }
}