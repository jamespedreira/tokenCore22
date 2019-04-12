using Microsoft.Extensions.DependencyInjection;
using Rodonaves.EDI.IoC;
using System.ServiceProcess;
using Microsoft.Extensions.Configuration;
using System.IO;
using Rodonaves.Engine;
using Rodonaves.EDI.Infra;
using Rodonaves.EDI.Service.Export.Interfaces;
using System;
using System.Threading;
using Rodonaves.EDI.BLL.Interfaces;
using System.Text;
using Rodonaves.TaskExecutor.Infra;

namespace Rodonaves.EDI.Service.Export
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var services = new ServiceCollection();
            services.RegisterServices();

            services.AddSingleton<IConfiguration>(prov =>
             {
                 var builder = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: true, reloadOnChange: true);

                 return builder.Build();
             });

            services.AddTransient<IManageQueue, MonitorProcessQueue>();
            services.AddTransient<Service>();

            var provider = services.BuildServiceProvider();

            Global.Configuration = provider.GetService<IConfiguration>();

            //ServiceBase.Run(provider.GetService<Service>());
            //provider.GetService<IEnqueueToExport>().ExecuteAsync(provider.GetService<ILogger>(), "49").Wait();
            var a = provider.GetService<IManageQueue>().ExecuteProcess(null, new RabbitMQ.Client.Events.BasicDeliverEventArgs
            {
                Body = Encoding.UTF8.GetBytes("EDI_01008713011107_OCOREN")
            }).Result;
        }
    }
}
