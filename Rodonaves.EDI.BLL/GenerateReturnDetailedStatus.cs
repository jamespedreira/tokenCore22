using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;

namespace Rodonaves.EDI.BLL
{
    public class GenerateReturnDetailedStatus : BaseCrudBll<IGenerateReturnDetailedStatusDal, GenerateReturnDetailedStatusDTO, GenerateReturnDetailedStatusInfo>, IGenerateReturnDetailedStatus
    {
        #region Properties
        private readonly ILogDal _logDal;
        private readonly IOperationDal _operationDAl;
        private readonly IMessageBrokerManagement _messageBroker;
        private readonly IEDIDal _ediDal;

        #endregion
        #region Ctor
        public GenerateReturnDetailedStatus (IGenerateReturnDetailedStatusDal dal, IMapper mapper, ILogDal logDal, IOperationDal operationDal, IMessageBrokerManagement messageBroker, IEDIDal ediDal) : base (dal, mapper)
        {
            _logDal = logDal;
            _operationDAl = operationDal;
            _messageBroker = messageBroker;
            _ediDal = ediDal;
        }
        #endregion
        #region Methods

        public async Task<List<GenerateReturnDetailedStatusDTO>> GetAllAsync () =>
            _mapper.Map<List<GenerateReturnDetailedStatusInfo>, List<GenerateReturnDetailedStatusDTO>> (await _dal.GetAllAsync ());

        public async Task<List<GenerateReturnDetailedStatusDTO>> GetByParamsAsync (GenerateReturnDetailedStatusDTO dto, int page, int amountByPage)
        {
            var billOfLadingsByInvoice = (await _ediDal.GetBillOfLadingIdsByInvoiceAsync(dto.Key)).Distinct().ToList();

            var detailedStatus = new List<GenerateReturnDetailedStatusDTO>();

            if (page == 0 || page == 1)
            {
                var tasks = new List<System.Threading.Tasks.Task<List<GenerateReturnDetailedStatusInfo>>>();
                foreach (var invoice in billOfLadingsByInvoice)
                {
                    tasks.Add(_dal.GetByParamsAsync(_mapper.Map<GenerateReturnDetailedStatusInfo>(new GenerateReturnDetailedStatusDTO
                    {
                        Key = invoice.ToString()
                    }), 1, int.MaxValue));
                }
                await System.Threading.Tasks.Task.WhenAll(tasks.ToArray());

                foreach (var item in tasks)
                {
                    detailedStatus.AddRange(
                        _mapper.Map<List<GenerateReturnDetailedStatusInfo>, List<GenerateReturnDetailedStatusDTO>>(item.Result)
                    );
                }
            }

            detailedStatus.AddRange(_mapper.Map<List<GenerateReturnDetailedStatusInfo>, List<GenerateReturnDetailedStatusDTO>> (
                    await _dal.GetByParamsAsync (_mapper.Map<GenerateReturnDetailedStatusInfo> (dto), page, amountByPage)));
            
            CheckIsOutOfTime (detailedStatus);
            return detailedStatus;
        }

        public async Task<List<LogDTO>> GetLogByGenerateReturnIdAsync (long generatereturnId) =>
            _mapper.Map<List<LogInfo>, List<LogDTO>> (await _logDal.GetByGenerateReturnIdAsync (generatereturnId));

        public async Task<List<QueueDTO>> GetQueueStatus () => await _messageBroker.GetAllQueuesAsync ();

        public async Task<List<GenerateReturnDetailedStatusDTO>> GetResumeByProcessTypeAsync ()
        {
            var resume = _mapper.Map<List<GenerateReturnDetailedStatusInfo>, List<GenerateReturnDetailedStatusDTO>> (await _dal.GetResumeByProcessTypeAsync ());
            CheckIsOutOfTime (resume);
            return resume;
        }

        public async Task<List<OperationDetailedDTO>> GetTotalOperationByStatusAsync () =>
            _mapper.Map<List<OperationDetailedInfo>, List<OperationDetailedDTO>> (await _operationDAl.GetTotalByStatusAsync ());

        Task<int> IBaseCrud<GenerateReturnDetailedStatusDTO>.InsertAsync (GenerateReturnDetailedStatusDTO dto) =>
            throw new System.NotImplementedException ();

        private void CheckIsOutOfTime (List<GenerateReturnDetailedStatusDTO> process)
        {
            for (int i = 0; i < process.Count; i++)
            {
                if (process[i].ProgressStatus == ProgressStatusEnum.Unprocessed)
                    process[i].ProgressStatus = process[i].ProgressStatus;

                if (process[i].ProgressStatus == ProgressStatusEnum.Started )
                    process[i].ProgressStatus = process[i].TimeStampProcess.TotalHours > 24 ? ProgressStatusEnum.Late : process[i].ProgressStatus;
            }
        }
        #endregion
    }
}