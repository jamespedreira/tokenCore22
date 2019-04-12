using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers;
using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.EDI.Model;
using Rodonaves.Engine;
using Rodonaves.QueueMessage.Interfaces;
using RTEFramework.BLL.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class GenerateReturn : BaseCrudBll<IGenerateReturnDal,GenerateReturnDTO, GenerateReturnInfo>, IGenerateReturn
    {
        #region Properties

        private readonly IGenerateReturnValueDal _generateReturnValueDal;
        private readonly IGenerateReturnStatusDal _generateReturnStatusDal;
        private readonly IGenerateReturnCommunicationChannelDal _generateReturnCommunicationChannelDal;
        private readonly IAgreementCommunicationChannelDal _agreementCommunicationChannelDal;
        private readonly ILayoutHeader _layoutHeader;
        private readonly ILayoutFileName _layoutFileName;
        private readonly ICustomer _customer;
        private readonly IProvider _provider;
        private readonly IRTEQueue _queue;

        const string _OBJECT_VALUE = "OBJECT_VALUE";

        #endregion

        #region Ctor

        public GenerateReturn(IGenerateReturnDal dal, 
            IMapper mapper,
            ILayoutHeader layoutHeader,
            ICustomer customer,
            IProvider provider,
            ILayoutFileName layoutFileName,
            IGenerateReturnValueDal generateReturnValueDal,
            IGenerateReturnStatusDal generateReturnStatusDal,
            IGenerateReturnCommunicationChannelDal generateReturnCommunicationChannelDal,
            IAgreementCommunicationChannelDal agreementCommunicationChannelDal,
            IRTEQueue queue) : base(dal, mapper)
        {
            _generateReturnCommunicationChannelDal = generateReturnCommunicationChannelDal;
            _generateReturnValueDal = generateReturnValueDal;
            _generateReturnStatusDal = generateReturnStatusDal;
            _layoutHeader = layoutHeader;
            _layoutFileName = layoutFileName;
            _provider = provider;
            _customer = customer;
            _agreementCommunicationChannelDal = agreementCommunicationChannelDal;
            _queue = queue;
        }

        #endregion

        #region Insert

        private async Task<int> InsertBaseAsync(GenerateReturnDTO dto)
        {
            using (var transaction = _dal.BeginTransaction())
            {
                try
                {
                    var info = _mapper.Map<GenerateReturnDTO, GenerateReturnInfo>(dto);

                    info.ProgressStatus = ProgressStatusEnum.Unprocessed;

                    info.Id = _dal.GetNextId();

                    if (!await _dal.InsertAsync(info, transaction))
                        throw new BusinessException("Não foi possível inserir a geração de retorno");

                    if (info.ExecutionType == ExecutionTypeEnum.System)
                    {
                        if (info.AgreementProcess.Id == 0)
                            throw new BusinessException("Id de processo de acordo não informado. Valor obrigatório para geração de arquivo automático");

                        var communicationChannels = await _agreementCommunicationChannelDal.GetByAgreementProcessAsync(info.AgreementProcess.Id);

                        info.CommunicationChannels = _mapper.Map<List<AgreementCommunicationChannelInfo>, List<CommunicationChannelInfo>>(communicationChannels);
                    }
                    else
                        info.AgreementProcess.Id = 0;

                    foreach (var item in info.CommunicationChannels)
                    {
                        item.GenerateReturnId = info.Id;
                        item.Id = _generateReturnCommunicationChannelDal.GetNextId();

                        if (!await _generateReturnCommunicationChannelDal.InsertAsync(item, transaction))
                            throw new BusinessException("Não foi possível inserir o canal de comunicação");
                    }

                    transaction.Commit();

                    if (!await UpdateProgressStatusAsync(info.Id, info.ProgressStatus))
                        throw new BusinessException("Não atualizar o status do processo");

                    return info.Id;
                }
                catch (BusinessException ex)
                {
                    transaction.Rollback();
                    throw new BusinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        #endregion

        async Task<int> IBaseCrud<GenerateReturnDTO>.InsertAsync(GenerateReturnDTO dto)
        {
            var id = await InsertBaseAsync(dto);

            string queueName = Global.Configuration.GetSection("QueuePrefix").Value + "GenerateReturn";
            await InsertToQueue(id, queueName);

            return id;
        }

        public async Task<int> InsertAsync(GenerateReturnDTO dto, string queueName)
        {
            var id = await InsertBaseAsync(dto);

            await InsertToQueue(id, queueName);

            return id;
        }

        public async Task<bool> UpdateProgressStatusAsync(int generateReturnId, ProgressStatusEnum progressStatus)
        {
            var info = await _dal.GetByIdAsync(generateReturnId);

            if(info == null)
                return false;

            info.ProgressStatus = progressStatus;

            if (await _dal.UpdateAsync(info))
            {
                var now = DateTime.Now;

                var statusInfo = new GenerateReturnSatusInfo()
                {
                    Id = _generateReturnStatusDal.GetNextId(),
                    ProgressStatus = progressStatus,
                    GenerateReturn = info,
                    Date = now,
                    Hour = new TimeSpan(now.Hour, now.Minute, now.Second)
                };

                return await _generateReturnStatusDal.InsertAsync(statusInfo);
            }

            return false;
        }

        private async System.Threading.Tasks.Task InsertToQueue(int generateReturnId, string queueName)
        {
            var newQueue = CreateQueue(queueName, out int maxPriority);
            newQueue.Enqueue(EncondQueueBody(generateReturnId.ToString()), maxPriority);
        }

        public IRTEQueue CreateQueue(string queueName, out int maxPriority)
        {
            var hostname = Global.Configuration.GetSection("RTEQueueEDIQueueHelper:hostname").Value;
            var username = Global.Configuration.GetSection("RTEQueueEDIQueueHelper:username").Value;
            var password = Global.Configuration.GetSection("RTEQueueEDIQueueHelper:password").Value;
            int.TryParse(Global.Configuration.GetSection("RTEQueueEDIQueueHelper:maxPriority").Value, out maxPriority);

            _queue.Enqueue(EncondQueueBody(queueName), maxPriority);

            return new EDIQueueHelper(hostname, queueName, username, password, maxPriority, Global.Configuration);
        }

        private byte[] EncondQueueBody(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        #region Get Methods


        public async Task<List<GenerateReturnDTO>> GetAllAsync()
        {
            var list = await  _dal.GetAllAsync();

            return _mapper.Map<List<GenerateReturnInfo>, List<GenerateReturnDTO>>(list);
        }

        public async Task<List<GenerateReturnDTO>> GetByParamsAsync(GenerateReturnDTO dto, int page, int amountByPage)
        {
            var info = _mapper.Map<GenerateReturnDTO,GenerateReturnInfo>(dto);

            var list = _mapper.Map<List<GenerateReturnInfo>, List<GenerateReturnDTO>>(await _dal.GetByParamsAsync(info, page, amountByPage));

            for (int i = 0; i < list.Count; i++)
                list[i] = await GetGenerateReturnDepencencies(list[i]);
            
            return list;
        }

        public async Task<GenerateReturnDTO> GetByIdAsync(int id)
        {
            var dto = _mapper.Map<GenerateReturnInfo, GenerateReturnDTO>(await _dal.GetByIdAsync(id));

            if(dto != null && dto.Id == id)
                dto = await GetGenerateReturnDepencencies(dto);

            return dto;
        }

        public async Task<GenerateReturnDTO> GetGenerateReturnDepencencies(GenerateReturnDTO generateReturn)
        {
            generateReturn.LayoutFileName = await _layoutFileName.GetByIdAsync(generateReturn.LayoutFileName.Id);

            generateReturn.LayoutHeader = await _layoutHeader.GetByIdAsync(generateReturn.LayoutHeader.Id);

            if (generateReturn.LayoutHeader == null || (generateReturn.LayoutHeader != null && generateReturn.LayoutHeader.Id == 0))
            {
                throw new BusinessException("Layout não encontrado");
            }

            generateReturn.LayoutHeader.Script.Provider = await _provider.GetByIdAsync(generateReturn.LayoutHeader.Script.ProviderId);

            if (generateReturn.LayoutHeader.Script.Provider == null || (generateReturn.LayoutHeader.Script.Provider != null && generateReturn.LayoutHeader.Script.Provider.Id == 0))
            {
                throw new BusinessException("Provedor não encontrado");
            }

            generateReturn.Customer = await _customer.GetByIdAsync(generateReturn.Customer.Id);

            if (generateReturn.Customer == null || (generateReturn.Customer != null && generateReturn.Customer.Id == 0))
            {
                throw new BusinessException("Cliente não encontrado");
            }

            return generateReturn;
        }

        public async Task<List<GenerateReturnDTO>> GetUnprocessedGenerateReturnAsync()
        {
            var list = _mapper.Map<List<GenerateReturnInfo>, List<GenerateReturnDTO>>(await _dal.GetUnprocessedGenerateReturnAsync());

            for (int i = 0; i < list.Count; i++)
                list[i] = await GetGenerateReturnDepencencies(list[i]);

            return list;
        }

        public async Task<List<GenerateReturnDTO>> GetUnprocessedGenerateReturnByCustomerAsync(int customerId)
        {
            var list = _mapper.Map<List<GenerateReturnInfo>, List<GenerateReturnDTO>>(await _dal.GetUnprocessedGenerateReturnByCustomerAsync(customerId));

            for (int i = 0; i < list.Count; i++)
                list[i] = await GetGenerateReturnDepencencies(list[i]);

            return list;
        }

        #endregion

        public async System.Threading.Tasks.Task InsertGenerateReturnValuesAsync(GenerateReturnDTO dto, QueryResult<object> resultValues)
        {
            using (var transaction = _dal.BeginTransaction())
            {
                try
                {
                    var resultAux = ProcessQueryResult(resultValues);

                    var values = await _generateReturnValueDal.GetByGenerateReturnIdAsync(dto.Id);

                    var list = resultAux.Rows.Select(x => (x as JObject).GetValue(_OBJECT_VALUE).ToString()).Where(x => !values.Select(y => y.Value).Contains(x));

                    foreach (var value in list)
                    {
                        var valueInfo = new GenerateReturnValueInfo()
                        {
                            GenerateReturnId = dto.Id,
                            Id = _generateReturnValueDal.GetNextId(),
                            Value = value
                        };

                        if (!await _generateReturnValueDal.InsertAsync(valueInfo))
                            throw new BusinessException("Erro ao inserir a valor de geração de retorno");
                    }
                }
                catch (BusinessException ex)
                {
                    transaction.Rollback();
                    throw new BusinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public QueryResult<object> ProcessQueryResult(QueryResult<object> result, string column = _OBJECT_VALUE)
        {
            var rows = result.Rows.ToArray();
            int count = rows.Count();

            var aux = new QueryResult<object>()
            {
                Columns = result.Columns,
                Rows = new List<object>()
            };

            for (int i = 0; i < count; i++)
            {
                var obj = (rows[i] as JObject);

                var val = obj.GetValue(column);

                if (val != null)
                {
                    var values = val.ToString();

                    foreach (var value in values.Split(',').ToList())
                    {
                        var json = new StringBuilder("{");
                        foreach (var item in result.Columns)
                        {
                            var property = item.Name == column ? value : obj.GetValue(item.Name).ToString();

                            json.AppendLine(CreateJsonProprerty(item.Name, property));
                        }

                        json.AppendLine("}");

                        aux.Rows.Add(JsonConvert.DeserializeObject<object>(json.ToString()));
                    }
                }
            }

            return aux;
        }

        private string CreateJsonProprerty(string name, string value)
        {
            return string.Format("'{0}': '{1}',", name, value);
        }

        public async System.Threading.Tasks.Task UpdateBillOfLadingQuantaty(int id)
        {
            await _dal.UpdateBillOfLadingQuantaty(id);
        }
    }
}
