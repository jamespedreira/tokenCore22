using AutoMapper;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Model;
using RTEFramework.BLL.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class Customer : BaseCrudBll<ICustomerDal, CustomerDTO, CustomerInfo>, ICustomer
    {
        #region Ctor

        private readonly ICustomerDal _customerDal;
        private readonly IPersonDal _personDal;

        public Customer(ICustomerDal dal, IPersonDal personDal,  IMapper mapper) : base(dal, mapper)
        {
            _customerDal = dal;
            _personDal = personDal;
        }

        #endregion

        #region Methods

        public async Task<List<CustomerDTO>> GetAllAsync(long companyId)
        {
            var list = await _dal.GetAll(companyId);

            return _mapper.Map<List<CustomerInfo>, List<CustomerDTO>>(list);
        }

        public async Task<CustomerDTO> GetByIdAsync(int id)
        {
            var info = await _dal.GetByIdAsync(id);

            return _mapper.Map<CustomerInfo, CustomerDTO>(info);
        }

        public async Task<List<CustomerDTO>> GetByParams(CustomerDTO dto, int page, int amountByPage)
        {
            var list = await _dal.GetByParams(_mapper.Map<CustomerDTO, CustomerInfo>(dto), page, amountByPage);

            return _mapper.Map<List<CustomerInfo>, List<CustomerDTO>>(list);
        }

        public async Task<List<CustomerDTO>> GetByProcessType(long companyId, int processType, int page, int amountByPage)
        {
            var list = await _dal.GetByProcessType(companyId, processType, page, amountByPage);

            return _mapper.Map< List<CustomerInfo>, List<CustomerDTO>>(list);
        }

        public new async Task<int> InsertAsync(CustomerDTO dto)
        {
            using (var transaction = _dal.BeginTransaction())
            {

                dto.IdPerson = _personDal.ExistPersonId(dto.Person.TaxIdRegistration);

                if (dto.IdPerson == 0)
                {
                    PersonInfo person = new PersonInfo
                    {
                        Description = dto.Person.Description,
                        TaxIdRegistration = dto.Person.TaxIdRegistration,
                        Id = _personDal.GetNextId()                        
                    };

                    dto.IdPerson = person.Id;

                    var successPerson = await _personDal.InsertAsync(person);

                    if (!successPerson)
                        throw new BusinessException("Não foi possível inserir a Pessoa");
                }
                
                var success = await base.InsertAsync(dto);

                if (!success)
                    throw new BusinessException("Não foi possível inserir o cliente");

                _dal.FinallyTransaction(success, transaction);

                return dto.Id;
            }
        }

        public override async Task<bool> UpdateAsync(CustomerDTO dto)
        {
            using (var transaction = _dal.BeginTransaction())
            {
                var success = await base.UpdateAsync(dto);

                if (!success)
                    throw new BusinessException("Não foi possível editar o cliente");

                _dal.FinallyTransaction(success, transaction);

                return success;
            }
        }

        int ICustomer.ExistCustomerId(string taxIdRegistration, long companyId)
        {
            return _customerDal.ExistCustomerId(taxIdRegistration, companyId);
        }

        #endregion
    }
}
