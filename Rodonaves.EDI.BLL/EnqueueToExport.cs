using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Enums;
using Rodonaves.Engine;
using Rodonaves.TaskExecutor.Infra;
using RTEFramework.BLL.Infra;
using System;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class EnqueueToExport : IEnqueueToExport
    {
        private readonly IAgreement _agreement;
        private readonly IGenerateReturn _generateReturn;
        private readonly ILayoutHeader _layoutHeader;
        private ILogger _logger;

        public EnqueueToExport(
            IAgreement agreement,
            IGenerateReturn generateReturn,
            ILayoutHeader layoutHeader
            )
        {
            _agreement = agreement;
            _generateReturn = generateReturn;
            _layoutHeader = layoutHeader;
        }

        public async System.Threading.Tasks.Task ExecuteAsync(ILogger logger, string arguments)
        {
            _logger = logger;
            if (int.TryParse(arguments, out int agreementId))
                await EnqueueAsync(agreementId);
        }

        public async Task<bool> EnqueueAsync(int agreementId)
        {
            await _logger.LogAsync(string.Empty, $"Iniciando o enfileiramento do acordo {agreementId}", string.Empty, LogType.Debug);

            var success = true;

            try
            {
                var agreement = await _agreement.GetByIdAsync(agreementId);

                if (agreement != null)
                {
                    foreach (var process in agreement.Processes)
                    {
                        string queueName = await GetQueueName(agreement, process);

                        var endingDate = DateTime.Now;
                        var startingDate = process.LastRun == DateTime.MinValue ? GetStartingDate(PeriodicityTypeEnum.Daily) : process.LastRun;

                        var generateReturn = new GenerateReturnDTO()
                        {
                            AgreementProcess = process,
                            Customer = agreement.Customer,
                            EndingDate = endingDate,
                            StartingDate = startingDate,
                            LayoutFileName = new LayoutFileNameDTO()
                            {
                                Id = process.LayoutFileNameId
                            },
                            LayoutHeader = new LayoutHeaderDTO()
                            {
                                Id = process.LayoutHeaderId
                            },
                            ProcessType = process.ProcessType,
                            ExecutionType = ExecutionTypeEnum.System
                        };

                        var id = await _generateReturn.InsertAsync(generateReturn, queueName);

                        await _logger.LogAsync(string.Empty, $"Acordo {agreementId} gerou o retorno {id} com sucesso", string.Empty, LogType.Debug);

                        if (!await _agreement.SetLastRunToProcess(process.Id, endingDate))
                            throw new BusinessException("Não foi possível atualizar a data de execução do processo");
                    }
                }
                else
                    await _logger.LogAsync(string.Empty, $"Acordo {agreementId} não encontrado", string.Empty, LogType.Error);
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(string.Empty, $"Erro gerado no processamento do Acordo {agreementId}", ex.Message, LogType.Error);
                success = false;
            }

            return success;
        }

        #region Date Utils

        private DateTime GetStartingDate(PeriodicityTypeEnum periodicity)
        {
            switch (periodicity)
            {
                case PeriodicityTypeEnum.Hourly:
                    return DateTime.Now.AddHours(-1);
                default:
                    return DateTime.Now.AddHours(-48);
            }
        }

        private DateTime NextRun(PeriodicityTypeEnum periodicity)
        {
            switch (periodicity)
            {
                case PeriodicityTypeEnum.Hourly:
                    return DateTime.Now.AddHours(1);
                default:
                    return DateTime.Now.AddDays(1);
            }
        }

        private async Task<string> GetQueueName(AgreementDTO agreement, AgreementProcessDTO process)
        {
            var layoutHeader = await _layoutHeader.GetByIdAsync(process.LayoutHeaderId);

            string tax_registration = agreement.Customer != null && agreement.Customer.Person != null ? agreement.Customer.Person.TaxIdRegistration : string.Empty,
                name = string.IsNullOrEmpty(tax_registration) ? "Enqueue" : tax_registration,
                description = layoutHeader.ProcessType.ToDescription();

            return $"EDI_{name}_{description}";
        }

        #endregion
    }
}