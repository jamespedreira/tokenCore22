using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;
using Rodonaves.TaskExecutor.Infra;
using System;

namespace Rodonaves.EDI.BLL
{
    public class AgreementExecutor : IAgreementExecutor
    {
        #region Properties

        private readonly IExport _export;
        private readonly IAgreement _agreement;
        private readonly IGenerateReturn _generateReturn;

        #endregion

        #region Ctor

        public AgreementExecutor(IExport export,
            IAgreement agreement,
            IGenerateReturn generateReturn)
        {
            _export = export;
            _agreement = agreement;
            _generateReturn = generateReturn;
        }

        #endregion

        public async System.Threading.Tasks.Task ExecuteAsync(ILogger logger, string arguments)
        {
            if(int.TryParse(arguments, out int id)) {

                var agreement = await _agreement.GetByIdAsync(id);

                if(agreement != null)
                {
                    //busca dados para gerar retorno

                    foreach (var process in agreement.Processes)
                    {
                        var generateReturn = new GenerateReturnDTO()
                        {
                            AgreementProcess = process,
                            Customer = agreement.Customer,
                            EndingDate = DateTime.Now,
                            StartingDate = process.LastRun,
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

                        int generateReturnId = await _generateReturn.InsertAsync(generateReturn);

                        //if(generateReturnId != 0) { 
                        //    await _export.ExecuteAsync(logger, generateReturnId.ToString());
                        //}
                    }
                }
            }
        }
    }
}
