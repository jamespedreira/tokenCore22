using AutoMapper;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DAL.PostgreSQL;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Model;
using RTEFramework.BLL.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class Agreement : BaseCrudBll<IAgreementDal, AgreementDTO, AgreementInfo>, IAgreement
    {

        #region Ctor

        private readonly IAgreementDal _agreementDal;
        private readonly IAgreementProcessDal _agreementProcessDal;
        private readonly IAgreementOccurrenceDal _agreementOccurrenceDal;
        private readonly IAgreementCommunicationChannelDal _agreementCommunicationChannelDal;


        public Agreement(IAgreementDal dal, IAgreementProcessDal agreementProcessDal, IAgreementOccurrenceDal agreementOccurrenceDal, IAgreementCommunicationChannelDal agreementCommunicationChannelDal, IMapper mapper) : base(dal, mapper)
        {
            _agreementDal = dal;
            _agreementProcessDal = agreementProcessDal;
            _agreementOccurrenceDal = agreementOccurrenceDal;
            _agreementCommunicationChannelDal = agreementCommunicationChannelDal;
        }

        #endregion

        #region Methods

        public async Task<List<AgreementDTO>> GetAllAsync(long companyId)
        {
            var list = await _dal.GetAll(companyId);

            return _mapper.Map<List<AgreementInfo>, List<AgreementDTO>>(list);
        }

        public new async Task<int> InsertAsync(AgreementDTO dto)
        {
            using (var transaction = _dal.BeginTransaction())
            {
                //Convênio de Clientes
                var success = await base.InsertAsync(dto);

                if (!success)
                    throw new BusinessException("Não foi possível inserir o convênio do cliente.");

                //Processo
                foreach (var item in dto.Processes)
                {
                    item.Id = _agreementProcessDal.GetNextId(transaction);
                    item.AgreementId = dto.Id;

                    success = await _agreementProcessDal.InsertAsync(_mapper.Map<AgreementProcessDTO, AgreementProcessInfo>(item));

                    if (!success)
                        throw new BusinessException("Não foi possível inserir o processo.");

                    //Canal de comunicação
                    foreach (var c in item.CommunicationChannels)
                    {
                        c.Id = _agreementCommunicationChannelDal.GetNextId(transaction);
                        c.AgreementProcessId = item.Id;
                        success = await _agreementCommunicationChannelDal.InsertAsync(_mapper.Map<AgreementCommunicationChannelDTO, AgreementCommunicationChannelInfo>(c));

                        if (!success)
                            throw new BusinessException("Não foi possível inserir o canal de comunicação.");
                    }
                }

                //Ocorrência
                foreach (var item in dto.Occurrences)
                {
                    item.Id = _agreementOccurrenceDal.GetNextId(transaction);
                    item.AgreementId = dto.Id;
                  
                    success = await _agreementOccurrenceDal.InsertAsync(_mapper.Map<AgreementOccurrenceDTO, AgreementOccurrenceInfo>(item));

                    if (!success)
                        throw new BusinessException("Não foi possível inserir o ocorrência.");
                }

                _dal.FinallyTransaction(success, transaction);

                return dto.Id;
            }
        }

        public override async Task<bool> UpdateAsync(AgreementDTO dto)
        {
            using (var transaction = _dal.BeginTransaction())
            {
                //Convênio de Cliente
                var success = await base.UpdateAsync(dto);

                if (!success)
                    throw new BusinessException("Não foi possível editar o convênio do cliente.");

                // Processos
                foreach (var item in dto.Processes)
                {
                    if (item.Id > 0)
                    {
                        item.AgreementId = dto.Id;
                        success = await _agreementProcessDal.UpdateAsync(_mapper.Map<AgreementProcessDTO, AgreementProcessInfo>(item));

                        if (!success)
                            throw new BusinessException("Não foi possível editar o processo.");
                    }
                    else
                    {
                        item.Id = _agreementProcessDal.GetNextId(transaction);
                        item.AgreementId = dto.Id;
                        success = await _agreementProcessDal.InsertAsync(_mapper.Map<AgreementProcessDTO, AgreementProcessInfo>(item));

                        if (!success)
                            throw new BusinessException("Não foi possível editar o processo.");
                    }

                    //Canal de comunicação
                    foreach (var c in item.CommunicationChannels)
                    {
                        if (c.Id > 0)
                        {
                            c.AgreementProcessId = item.Id;                           
                            success = await _agreementCommunicationChannelDal.UpdateAsync(_mapper.Map<AgreementCommunicationChannelDTO, AgreementCommunicationChannelInfo>(c));

                            if (!success)
                                throw new BusinessException("Não foi possível editar o canal de comunicação.");
                        }
                        else
                        {
                            c.Id = _agreementCommunicationChannelDal.GetNextId(transaction);
                            c.AgreementProcessId = item.Id;
                            success = await _agreementCommunicationChannelDal.InsertAsync(_mapper.Map<AgreementCommunicationChannelDTO, AgreementCommunicationChannelInfo>(c));

                            if (!success)
                                throw new BusinessException("Não foi possível editar o canal de comunicação.");
                        }

                    }
                }

                //Ocorrência
                foreach (var item in dto.Occurrences)
                {
                    if (item.Id > 0)
                    {
                        item.AgreementId = dto.Id;
                        success = await _agreementOccurrenceDal.UpdateAsync(_mapper.Map<AgreementOccurrenceDTO, AgreementOccurrenceInfo>(item));

                        if (!success)
                            throw new BusinessException("Não foi possível editar a ocorrência.");
                    }
                    else
                    {
                        item.Id = _agreementOccurrenceDal.GetNextId(transaction);
                        item.AgreementId = dto.Id;
                        success = await _agreementOccurrenceDal.InsertAsync(_mapper.Map<AgreementOccurrenceDTO, AgreementOccurrenceInfo>(item));

                        if (!success)
                            throw new BusinessException("Não foi possível editar a ocorrência.");
                    }
                }

                _dal.FinallyTransaction(success, transaction);

                return success;
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            bool success = false;

            var entityProcess = await _agreementProcessDal.GetByAgreementAsync(id);

            foreach (var c in entityProcess)
                c.CommunicationChannels = await _agreementCommunicationChannelDal.GetByAgreementProcessAsync(c.Id);

            foreach (var item in entityProcess)
            {

                //Canal de Comunicação
                foreach (var c in item.CommunicationChannels)
                {
                    success = await _agreementCommunicationChannelDal.DeleteAsync(c);

                    if (!success)
                        throw new BusinessException("Não foi possível deletar o canal de comunicação.");
                }

                //Processo
                success = await _agreementProcessDal.DeleteAsync(item);

                if (!success)
                    throw new BusinessException("Não foi possível deletar o processo.");
            }

            var entityOccurrence = await _agreementOccurrenceDal.GetByAgreementAsync(id);
            foreach (var item in entityOccurrence)
            {
                //Ocorrência
                success = await _agreementOccurrenceDal.DeleteAsync(item);

                if (!success)
                    throw new BusinessException("Não foi possível deletar a ocorrência.");
            }

            //Convênio de Cliente
            success = await base.DeleteAsync(id);

            if (!success)
                throw new BusinessException("Não foi possível deletar o convênio do cliente.");

            return success;
        }

        public async  Task<bool> SetLastRunToProcess(int processId, DateTime lastRun)
        {
            var info = await _agreementProcessDal.GetByIdAsync(processId);

            if (info == null)
                throw new BusinessException($"Processo {processId} não encontrado");

            info.LastRun = lastRun;

            return await _agreementProcessDal.UpdateAsync(info);
        }

        public async Task<List<AgreementDTO>> GetByParamsAsync(AgreementDTO dto, bool? active, int page, int amountByPage)
        {
            var info = _mapper.Map<AgreementDTO, AgreementInfo>(dto);

            var list = await _dal.GetByParamsAsync(info,  active, page, amountByPage);

            foreach (var item in list)
            {
                item.Processes = await _agreementProcessDal.GetByAgreementAsync(item.Id);

                foreach (var c in item.Processes)
                    c.CommunicationChannels = await _agreementCommunicationChannelDal.GetByAgreementProcessAsync(c.Id);
            }               

            foreach (var item in list)
                item.Occurrences = await _agreementOccurrenceDal.GetByAgreementAsync(item.Id);

            return _mapper.Map<List<AgreementInfo>, List<AgreementDTO>>(list);
        }

        public async Task<List<AgreementDTO>> GetAgreementToExport()
        {
            var referenceDate = DateTime.Now;

            var list = await _agreementDal.GetAgreementToExport(referenceDate);

            foreach (var agreement in list)
                agreement.Processes = await _agreementProcessDal.GetAgreementProcessToExport(agreement.Id, referenceDate);

            return _mapper.Map<List<AgreementInfo>, List<AgreementDTO>>(list);
        }

        public async Task<AgreementDTO> GetByIdAsync(int id)
        {
            var agreement = await _agreementDal.GetByIdAsync(id);

            if(agreement != null)
                agreement.Processes = await _agreementProcessDal.GetByAgreementAsync(agreement.Id);

            return _mapper.Map<AgreementInfo, AgreementDTO>(agreement);
        }

        #endregion
    }
}
