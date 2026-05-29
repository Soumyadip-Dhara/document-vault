using documentvaultapi.RbbitMQ.Models;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace documentvaultapi.RbbitMQ.Extensions
{
    public static class HangfireServiceExtensions
    {
        // ===============================
        // 1. Register Hangfire Services
        // ===============================
        //public static IServiceCollection AddHangfireServices(
        //    this IServiceCollection services,
        //    IConfiguration configuration)
        //{
        //    services.AddHangfire(config => config
        //        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        //        .UseSimpleAssemblyNameTypeSerializer()
        //        .UseRecommendedSerializerSettings()
        //        .UsePostgreSqlStorage(
        //            configuration.GetConnectionString("DocumentVaultDB"),
        //            new PostgreSqlStorageOptions
        //            {
        //                SchemaName = "hangfire"
        //            }));

        //    services.AddHangfireServer();

        //    return services;
        //}

        // ===============================
        // 2. Configure Hangfire Middleware
        // ===============================
        public static IApplicationBuilder UseHangfireServices(
            this IApplicationBuilder app,
            IConfiguration configuration)
        {
            // Hangfire Dashboard
            app.UseHangfireDashboard("/engin-hang");

            var jobSettings = configuration
                .GetSection("HangfireJobs")
                .Get<HangfireJobSettings>();

            if (jobSettings == null || jobSettings.Jobs == null)
                return app;

            // Timezone safe handling
            TimeZoneInfo timeZone;
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(jobSettings.TimeZone);
            }
            catch
            {
                timeZone = TimeZoneInfo.Local;
            }

            // ===============================
            // 3. Dynamic Job Registration
            // ===============================
            //foreach (var job in jobSettings.Jobs)
            //{
            //    if (!job.Enabled) continue;

            //    switch (job.Method)
            //    {
            //        case "StartGenerateLot":
            //            RecurringJob.AddOrUpdate<RbiLotJob>(
            //                job.JobName,
            //                service => service.StartGenerateLot(null!),
            //                job.Cron,
            //                timeZone);
            //            break;

            //        case "FlushUserActivityLogs":
            //            RecurringJob.AddOrUpdate<UserActivityFlushJob>(
            //                job.JobName,
            //                service => service.StartFlushDailyLogs(null!),
            //                job.Cron,
            //                timeZone);
            //            break;

            //        case "StartGenerateLotFile":
            //            RecurringJob.AddOrUpdate<RbiLotJob>(
            //                job.JobName,
            //                service => service.StartGenerateLotFile(null!),
            //                job.Cron,
            //                timeZone);
            //            break;

            //        case "StartPushToRBI":
            //            RecurringJob.AddOrUpdate<RbiLotJob>(
            //                job.JobName,
            //                service => service.StartPushToRBI(null!),
            //                job.Cron,
            //                timeZone);
            //            break;

            //        case "StartRbiFileAcceptanceStatus":
            //            RecurringJob.AddOrUpdate<RbiLotJob>(
            //                job.JobName,
            //                service => service.StartAcknowledgementReceiveFromRBI(null!),
            //                job.Cron,
            //                timeZone);
            //            break;

            //        case "StartGetDebitNotificationFromRBI":
            //            RecurringJob.AddOrUpdate<RbiLotJob>(
            //                job.JobName,
            //                service => service.StartGetDebitOrReturnNotificationFromRBI("DN", null!),
            //                job.Cron,
            //                timeZone);
            //            break;

            //        case "StartGetReturnNotificationFromRBI":
            //            RecurringJob.AddOrUpdate<RbiLotJob>(
            //                job.JobName,
            //                service => service.StartGetDebitOrReturnNotificationFromRBI("RN", null!),
            //                job.Cron,
            //                timeZone);
            //            break;

            //        case "StartGetCreditNotificationFromRBI":
            //            RecurringJob.AddOrUpdate<RbiLotJob>(
            //                job.JobName,
            //                service => service.StartGetCreditNotificationFromRBI(null!),
            //                job.Cron,
            //                timeZone);
            //            break;

            //        case "StartGeneratePfmsLot":
            //            RecurringJob.AddOrUpdate<PfmsLotJob>(
            //                job.JobName,
            //                service => service.StartGeneratePfmsLot(null!),
            //                job.Cron,
            //                timeZone);
            //            break;

            //        case "StartGeneratePfmsLotFile":
            //            RecurringJob.AddOrUpdate<PfmsLotJob>(
            //                job.JobName,
            //                service => service.StartGeneratePfmsLotFile(null!),
            //                job.Cron,
            //                timeZone);
            //            break;

            //        case "StartPushToPfms":
            //            RecurringJob.AddOrUpdate<PfmsLotJob>(
            //                job.JobName,
            //                service => service.StartPushToPfms(null!),
            //                job.Cron,
            //                timeZone);
            //            break;

            //        case "StartPfmsFileAcceptanceStatus":
            //            RecurringJob.AddOrUpdate<PfmsLotJob>(
            //                job.JobName,
            //                service => service.StartPfmsFileAcceptanceStatus(null!),
            //                job.Cron,
            //                timeZone);
            //            break;

            //        case "StartFetchingPfmsFileDNStatus":
            //            RecurringJob.AddOrUpdate<PfmsLotJob>(
            //                job.JobName,
            //                service => service.StartFetchingPfmsFileDNStatus(null!),
            //                job.Cron,
            //                timeZone);
            //            break;

            //        default:
            //            throw new InvalidOperationException(
            //                $"Unknown method '{job.Method}' in Hangfire configuration.");
            //    }
            //}

            return app;
        }
    }
}


////using ctsapi.Jobs;
//using documentvaultapi.RbbitMQ.Models;
//using documentvaultapi.RbbitMQ.Models;
//using Hangfire;
//using Hangfire.PostgreSql;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;

//namespace documentvaultapi.RbbitMQ.Extensions;

//public static class HangfireServiceExtensions
//{
//    public static IServiceCollection AddHangfireServicess(this IServiceCollection services, IConfiguration configuration)
//    {
//        // Configure Hangfire
//        services.AddHangfire(config => config
//           .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
//           .UseSimpleAssemblyNameTypeSerializer()
//           .UseRecommendedSerializerSettings()
//           .UsePostgreSqlStorage(configuration.GetConnectionString("DBConnection")));

//        services.AddHangfireServer();

//        return services;
//    }

//    public static IApplicationBuilder UseHangfireServices(this IApplicationBuilder app, IConfiguration configuration)
//    {
//        // Configure Hangfire Dashboard
//        app.UseHangfireDashboard("/engin-hang");

//        var jobSettings = configuration.GetSection("HangfireJobs").Get<HangfireJobSettings>();
//        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(jobSettings.TimeZone);

//        //foreach (var job in jobSettings.Jobs)
//        //{
//        //    if (!job.Enabled) continue;
//        //    switch (job.Method)
//        //    {
//        //        case "StartGenerateLot":
//        //            RecurringJob.AddOrUpdate<RbiLotJob>(
//        //                job.JobName,
//        //                service => service.StartGenerateLot(null!),
//        //                job.Cron,
//        //                timeZone
//        //            );
//        //            break;
//        //        case "FlushUserActivityLogs":
//        //            RecurringJob.AddOrUpdate<UserActivityFlushJob>(
//        //                job.JobName,
//        //                service => service.StartFlushDailyLogs(null!),
//        //                job.Cron,
//        //                timeZone
//        //            );
//        //            break;
//        //        case "StartGenerateLotFile":
//        //            RecurringJob.AddOrUpdate<RbiLotJob>(
//        //                job.JobName,
//        //                service => service.StartGenerateLotFile(null!),
//        //                job.Cron,
//        //                timeZone
//        //            );
//        //            break;
//        //        case "StartPushToRBI":
//        //            RecurringJob.AddOrUpdate<RbiLotJob>(
//        //                job.JobName,
//        //                service => service.StartPushToRBI(null!),
//        //                job.Cron,
//        //                timeZone
//        //            );
//        //            break;
//        //        case "StartRbiFileAcceptanceStatus":
//        //            RecurringJob.AddOrUpdate<RbiLotJob>(
//        //                job.JobName,
//        //                service => service.StartAcknowledgementReceiveFromRBI(null!),
//        //                job.Cron,
//        //                timeZone
//        //            );
//        //            break;
//        //        case "StartGetDebitNotificationFromRBI":
//        //            RecurringJob.AddOrUpdate<RbiLotJob>(
//        //                job.JobName,
//        //                service => service.StartGetDebitOrReturnNotificationFromRBI("DN", null!),
//        //                job.Cron,
//        //                timeZone
//        //            );
//        //            break;
//        //        case "StartGetReturnNotificationFromRBI":
//        //            RecurringJob.AddOrUpdate<RbiLotJob>(
//        //                job.JobName,
//        //                service => service.StartGetDebitOrReturnNotificationFromRBI("RN", null!),
//        //                job.Cron,
//        //                timeZone
//        //            );
//        //            break;
//        //        case "StartGetCreditNotificationFromRBI":
//        //            RecurringJob.AddOrUpdate<RbiLotJob>(
//        //                job.JobName,
//        //                service => service.StartGetCreditNotificationFromRBI(null!),
//        //                job.Cron,
//        //                timeZone
//        //            );
//        //            break;
//        //        case "StartGeneratePfmsLot":
//        //            RecurringJob.AddOrUpdate<PfmsLotJob>(
//        //                job.JobName,
//        //                service => service.StartGeneratePfmsLot(null!),
//        //                job.Cron,
//        //                timeZone
//        //            );
//        //            break;
//        //        case "StartGeneratePfmsLotFile":
//        //            RecurringJob.AddOrUpdate<PfmsLotJob>(
//        //                job.JobName,
//        //                service => service.StartGeneratePfmsLotFile(null!),
//        //                job.Cron,
//        //                timeZone
//        //            );
//        //            break;
//        //        case "StartPushToPfms":
//        //            RecurringJob.AddOrUpdate<PfmsLotJob>(
//        //                job.JobName,
//        //                service => service.StartPushToPfms(null!),
//        //                job.Cron,
//        //                timeZone
//        //            );
//        //            break;
//        //        case "StartPfmsFileAcceptanceStatus":
//        //            RecurringJob.AddOrUpdate<PfmsLotJob>(
//        //                job.JobName,
//        //                service => service.StartPfmsFileAcceptanceStatus(null!),
//        //                job.Cron,
//        //                timeZone
//        //            );
//        //            break;
//        //        case "StartFetchingPfmsFileDNStatus":
//        //            RecurringJob.AddOrUpdate<PfmsLotJob>(
//        //                job.JobName,
//        //                service => service.StartFetchingPfmsFileDNStatus(null!),
//        //                job.Cron,
//        //                timeZone
//        //            );
//        //            break;
//        //        default:
//        //            throw new InvalidOperationException($"Unknown method '{job.Method}' in Hangfire job configuration.");
//        //    }
//        //}

//        // Schedule the CreditNotificationJob to run every 5 minutes
//        //RecurringJob.AddOrUpdate<RbiLotJob>(
//        //   "generate-lot-job",
//        //   service => service.StartGenerateLot(null!),
//        //   "0 16,22 * * *",
//        //   TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata")
//        //);
//        //RecurringJob.AddOrUpdate<RbiLotJob>(
//        //   "generate-lot-file-job",
//        //   service => service.StartGenerateLotFile(null!),
//        //   "5 16,22 * * *",
//        //   TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata")
//        //);
//        //RecurringJob.AddOrUpdate<RbiLotJob>(
//        //   "push-lot-file-job",
//        //   service => service.StartPushToRBI(null!),
//        //   "10 16,22 * * *",
//        //   TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata")
//        //);
//        return app;
//    }
//}