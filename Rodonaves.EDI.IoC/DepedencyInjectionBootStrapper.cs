using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rodonaves.Core.Bll;
using Rodonaves.Core.Bll.Queues;
using Rodonaves.Core.Interfaces;
using Rodonaves.EDI.BLL;
using Rodonaves.EDI.BLL.Infra.AutoMapperProfille;
using Rodonaves.EDI.BLL.Infra.Factories;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.BLL.Interfaces.Factories;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DAL.Oracle;
using Rodonaves.EDI.DAL.PostgreSQL;
using Rodonaves.EDI.Helpers;
using Rodonaves.EDI.Helpers.Interfaces;
using Rodonaves.EDI.Infra.Factories;
using Rodonaves.QueueMessage;
using Rodonaves.QueueMessage.Interfaces;
using Rodonaves.TaskExecutor.Infra;
using IConnection = Rodonaves.EDI.BLL.Interfaces.IConnection;

namespace Rodonaves.EDI.IoC
{
    public static class DepedencyInjectionBootStrapper
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            //Dalls
            services.AddTransient<IProviderDal, ProviderDal>();
            services.AddTransient<IScriptDal, ScriptDal>();
            services.AddTransient<ITaskDal, TaskDal>();
            services.AddTransient<ITriggerDal, TriggerDal>();
            services.AddTransient<IDetailedDayDal, DetailedDayDal>();
            services.AddTransient<IDetailedMonthDal, DetailedMonthDal>();
            services.AddTransient<ILayoutGroupDal, LayoutGroupDal>();
            services.AddTransient<ILayoutHeaderDal, LayoutHeaderDal>();
            services.AddTransient<ILayoutBandDal, LayoutBandDal>();
            services.AddTransient<ILayoutColumnDal, LayoutColumnDal>();
            services.AddTransient<IActionDal, ActionDal>();
            services.AddTransient<IActionObjectDal, ActionObjectDal>();
            services.AddTransient<IPersonDal, PersonDal>();
            services.AddTransient<ICustomerDal, CustomerDal>();
            services.AddTransient<IAgreementDal, AgreementDal>();
            services.AddTransient<IAgreementProcessDal, AgreementProcessDal>();
            services.AddTransient<IAgreementOccurrenceDal, AgreementOccurrenceDal>();
            services.AddTransient<IAgreementCommunicationChannelDal, AgreementCommunicationChannelDal>();
            services.AddTransient<ILayoutFileNameDal, LayoutFileNameDal>();
            services.AddTransient<IGenerateReturnCommunicationChannelDal, GenerateReturnCommunicationChannelDal>();
            services.AddTransient<IGenerateReturnDal, GenerateReturnDal>();
            services.AddTransient<IGenerateReturnValueDal, GenerateReturnValueDal>();
            services.AddTransient<ILayoutDictionaryDal, LayoutDictionaryDal>();
            services.AddTransient<ILogDal, LogDal>();
            services.AddTransient<IExportDal, ExportDal>();
            services.AddTransient<IGenerateReturnDetailedStatusDal, GenerateReturnDetailedStatusDal>();
            services.AddTransient<IGenerateReturnStatusDal, GenerateReturnStatusDal>();
            services.AddTransient<IOperationDal, OperationDal>();
            services.AddTransient<IEDIDal, EDIDal>();

            // Blls
            services.AddTransient<IProvider, Provider>();
            services.AddTransient<IScript, Script>();
            services.AddTransient<ILayoutGroup, LayoutGroup>();
            services.AddTransient<ILayoutHeader, LayoutHeader>();
            services.AddTransient<IPerson, Person>();
            services.AddTransient<ICustomer, Customer>();
            services.AddTransient<IAgreement, Agreement>();
            services.AddTransient<ITask, Rodonaves.EDI.BLL.Task>();
            services.AddTransient<ILayoutFileName, LayoutFileName>();
            services.AddTransient<ILayoutColumn, LayoutColumn>();
            services.AddTransient<IGenerateReturn, GenerateReturn>();
            services.AddTransient<INatureOccurrence, NatureOccurrence>();
            services.AddTransient<IConnection, Connection>();
            services.AddTransient<IExport, Export>();
            services.AddTransient<IEnqueueToExport, EnqueueToExport>();
            services.AddTransient<ILogger, DatabaseLogger>();
            services.AddTransient<ILog, Log>();
            services.AddTransient<IAgreementExecutor, AgreementExecutor>();
            services.AddTransient<IGenerateReturnDetailedStatus, GenerateReturnDetailedStatus>();
            services.AddTransient<IMessageBrokerManagement, MessageBroker>();

            services.AddTransient<Parameter>();
            services.AddTransient<SMTP>();

            services.AddTransient<IBandType, BandType>();

            // Factories
            services.AddTransient<IColumnHeaderFactory, ColumnHeaderFactory>();
            services.AddTransient<IConnectionFactory, ConnectionFactory>();
            services.AddTransient<IExplainPlanFactory, ExplainPlanFactory>();
            services.AddTransient<IQueryResultFactory, QueryResultFactory>();
            services.AddTransient<IFtpClientFactory, FtpClientFactory>();
            services.AddTransient<IGedFactory, GedFactory>();

            //Helpers
            services.AddSingleton<IValidateClientAuthentication, ValidateClientAuthentication>();

            // Rabbit MQ
            services.AddTransient<IMailQueue, MailQueueRMQ>();
            services.AddTransient<Mail>();
            services.AddTransient<MailQueue>();
            services.AddTransient<MailboxHeader>();
            services.AddTransient<IRTEConsumer, RTEConsumer>();
            services.AddTransient<IRTEQueue, EDIQueueHelper>();

            services.AddTransient<GED>();

            services.AddAutoMapper(x => x.AddProfile(new MappingModel()));
            return services;
        }
    }
}
