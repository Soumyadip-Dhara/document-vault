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
            var rabbitMQConfig = configuration.GetSection("RabbitMQConnection").Get<RabbitMQConfigurationModel>();
            if (rabbitMQConfig == null)
            {
                throw new InvalidOperationException("RabbitMQ configuration is missing");
            }

            services.AddSingleton(rabbitMQConfig);
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
            //services.AddScoped<IValidator<DocumentUploadMessageDTO>, DocumentUploadMessageValidator>();
            //services.AddScoped<IMessageProcessor<DocumentUploadMessageDTO>, DocumentUploadQueueService>();
            //services.AddHostedService<DocumentUploadConsumer>();

            //services.AddScoped<IValidator<OrderMessage>, OrderMessageValidator>();
            //services.AddScoped<IMessageProcessor<OrderMessage>, OrderMessageProcessor>();
            //services.AddHostedService<OrderConsumerService>();

            //// For JIT-Billing Bill Receive
            //services.AddScoped<IValidator<eJitBillDetail>, eBillDetailValidator>();
            //services.AddScoped<IMessageProcessor<eJitBillDetail>, JitBillQueueService>();
            //services.AddHostedService<JitBillConsumer>();

            //// For e-Billing Bill Receive
            ////services.AddScoped<IValidator<eBillDetail>, NormalBillDetailValidator>();
            ////services.AddScoped<IMessageProcessor<eBillDetail>, EBillingQueueService>();
            ////services.AddHostedService<NormalBillConsumer>();

            //services.AddScoped<IValidator<ActiveHoaMasterDTOs>, ActiveHoaMasterValidator>();
            //services.AddScoped<IMessageProcessor<ActiveHoaMasterDTOs>, ActiveHoaMasterQueueService>();
            ////services.AddHostedService<ActiveHoaMasterConsumer>();
            //services.AddHostedService<JitActiveHoaMasterConsumer>();

            //services.AddScoped<IValidator<JitSchemeConfigDTOs>, SchemeConfigValidator>();
            //services.AddScoped<IMessageProcessor<JitSchemeConfigDTOs>, SchemeConfigQueueService>();
            //services.AddHostedService<SchemeConfigConsumer>();

            //// For e-Billing
            ////services.AddScoped<IValidator<List<DDOAllotmentDTOs>>, DDOAllotmentMasterValidator>();
            ////services.AddScoped<IMessageProcessor<List<DDOAllotmentDTOs>>, DDOAllotmentMasterQueueService>();
            ////services.AddHostedService<DDOAllotmentMasterConsumer>();

            //// For JIT-Billing
            //services.AddScoped<IValidator<List<JitDDOAllotmentDTOs>>, JitDDOAllotmentMasterValidator>();
            //services.AddScoped<IMessageProcessor<List<JitDDOAllotmentDTOs>>, JitDDOAllotmentMasterQueueService>();
            //services.AddHostedService<JitDDOAllotmentMasterConsumer>();

            //// For JIT-Billing
            //services.AddScoped<IValidator<List<DDOAllotmentWithdrawalDTOs>>, JitDDOAllotmentWithdrawalValidator>();
            //services.AddScoped<IMessageProcessor<List<DDOAllotmentWithdrawalDTOs>>, JitDDOAllotmentWithdrawalQueueService>();
            //services.AddHostedService<JitDDOAllotmentWithdrawalConsumer>();
            ////For Master
            //services.AddScoped<IValidator<MasterToCtsFinancialYearDTOs>, MasterToCtsFinancialYearValidator>();
            //services.AddScoped<IMessageProcessor<MasterToCtsFinancialYearDTOs>, MasterToCtsFinancialYearQueueService>();
            //services.AddHostedService<MasterToCtsFinancialYearConsumer>();

            //services.AddScoped<IValidator<MasterToCtsRbiIfscStockDTOs>, MasterToCtsRbiIfscStockValidator>();
            //services.AddScoped<IMessageProcessor<MasterToCtsRbiIfscStockDTOs>, MasterToCtsRbiIfscStockQueueService>();
            //services.AddHostedService<MasterToCtsRbiIfscStockConsumer>();

            //services.AddScoped<IValidator<MasterToCtsDdoDTOs>, MasterToCtsDdoValidator>();
            //services.AddScoped<IMessageProcessor<MasterToCtsDdoDTOs>, MasterToCtsDdoQueueService>();
            //services.AddHostedService<MasterToCtsDdoConsumer>();

            //services.AddScoped<IValidator<MasterToCtsDepartmentDTOs>, MasterToCtsDepartmentValidator>();
            //services.AddScoped<IMessageProcessor<MasterToCtsDepartmentDTOs>, MasterToCtsDepartmentQueueService>();
            //services.AddHostedService<MasterToCtsDepartmentConsumer>();

            //services.AddScoped<IValidator<MasterToCtsMajorHeadDTOs>, MasterToCtsMajorHeadValidator>();
            //services.AddScoped<IMessageProcessor<MasterToCtsMajorHeadDTOs>, MasterToCtsMajorHeadQueueService>();
            //services.AddHostedService<MasterToCtsMajorHeadConsumer>();

            //services.AddScoped<IValidator<MasterToCtsMinorHeadDTOs>, MasterToCtsMinorHeadValidator>();
            //services.AddScoped<IMessageProcessor<MasterToCtsMinorHeadDTOs>, MasterToCtsMinorHeadQueueService>();
            //services.AddHostedService<MasterToCtsMinorHeadConsumer>();

            //services.AddScoped<IValidator<MasterToCtsDetailHeadDTOs>, MasterToCtsDetailHeadValidator>();
            //services.AddScoped<IMessageProcessor<MasterToCtsDetailHeadDTOs>, MasterToCtsDetailHeadQueueService>();
            //services.AddHostedService<MasterToCtsDetailHeadConsumer>();

            //services.AddScoped<IValidator<MasterToCtsSubDetailHeadDTOs>, MasterToCtsSubDetailHeadValidator>();
            //services.AddScoped<IMessageProcessor<MasterToCtsSubDetailHeadDTOs>, MasterToCtsSubDetailHeadQueueService>();
            //services.AddHostedService<MasterToCtsSubDetailHeadConsumer>();

            //services.AddScoped<IValidator<MasterToCtsTreasuryDTOs>, MasterToCtsTreasuryValidator>();
            //services.AddScoped<IMessageProcessor<MasterToCtsTreasuryDTOs>, MasterToCtsTreasuryQueueService>();
            //services.AddHostedService<MasterToCtsTreasuryConsumer>();

            //services.AddScoped<IValidator<MasterToCtsSchemeHeadDTOs>, MasterToCtsSchemeHeadValidator>();
            //services.AddScoped<IMessageProcessor<MasterToCtsSchemeHeadDTOs>, MasterToCtsSchemeHeadQueueService>();
            //services.AddHostedService<MasterToCtsSchemeHeadConsumer>();

            //services.AddScoped<IValidator<MasterToCtsSubMajorHeadDTOs>, MasterToCtsSubMajorHeadValidator>();
            //services.AddScoped<IMessageProcessor<MasterToCtsSubMajorHeadDTOs>, MasterToCtsSubMajorHeadQueueService>();
            //services.AddHostedService<MasterToCtsSubMajorHeadConsumer>();

            //services.AddScoped<IValidator<MasterToCtsSchemeTypeDTOs>, MasterToCtsSchemeTypeValidator>();
            //services.AddScoped<IMessageProcessor<MasterToCtsSchemeTypeDTOs>, MasterToCtsSchemeTypeQueueService>();
            //services.AddHostedService<MasterToCtsSchemeTypeConsumer>();

            //services.AddScoped<IValidator<JitToCtsRbiIfscStockDTOs>, JitToCtsRbiIfscStockValidator>();
            //services.AddScoped<IMessageProcessor<JitToCtsRbiIfscStockDTOs>, JitToCtsRbiIfscStockQueueService>();
            //services.AddHostedService<JitToCtsRbiIfscStockConsumer>();
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
                MessageQueueConstants.UM_APPLICATION_MAP_ACK
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