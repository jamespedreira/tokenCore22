using AutoMapper;
using Newtonsoft.Json;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Helpers.Interfaces;
using Rodonaves.EDI.Model;
using RTEFramework.BLL.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class Person : BaseCrudBll<IPersonDal, PersonDTO, PersonInfo>, IPerson
    {
        #region Ctor

        private readonly IPersonDal _personDal;
        private readonly IValidateClientAuthentication _validateClientAuthentication;

        public Person(IPersonDal persondal, IValidateClientAuthentication validateClientAuthentication, IMapper mapper) : base(persondal, mapper)
        {
            _personDal = persondal;
            _validateClientAuthentication = validateClientAuthentication;
        }

        #endregion

        #region Methods

        public async Task<List<PersonDTO>> GetAllAsync(long companyId)
        {
            var list = await _personDal.GetAll(companyId);

            return _mapper.Map<List<PersonInfo>, List<PersonDTO>>(list);
        }

        public new async Task<int> InsertAsync(PersonDTO dto)
        {
            using (var transaction = _dal.BeginTransaction())
            {
                var success = await base.InsertAsync(dto);

                if (!success)
                    throw new BusinessException("Não foi possível inserir a Pessoa");

                _dal.FinallyTransaction(success, transaction);

                return dto.Id;
            }
        }

        public override async Task<bool> UpdateAsync(PersonDTO dto)
        {
            using (var transaction = _dal.BeginTransaction())
            {
                var success = await base.UpdateAsync(dto);

                if (!success)
                    throw new BusinessException("Não foi possível editar a Pessoa");

                _dal.FinallyTransaction(success, transaction);

                return success;
            }
        }

        public async Task<List<PersonDTO>> GetPersonExternal(int? page, int? amountByPage, string description, string taxIdRegistration)
        {

            using (var httpClient = _validateClientAuthentication.CreateClient("RoutePerson"))
            {
                var Url = @"?page=" + page + "&amountByPage=" + amountByPage + "&description=" + (description == null ? description : description.ToUpper()) + "&taxIdRegistration=" + taxIdRegistration;
                var response = await httpClient.GetAsync(Url);

                //Retorna a Mensagem da Outra API
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new BusinessException(message);
                }

                var list = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(list))
                {
                    var j = JsonConvert.DeserializeObject<List<PersonDTO>>(list);
                    return j;
                }
                else
                 throw new BusinessException("Erro ao tentar retornar dados da API de pessoas.");
            }
        }

        #endregion
    }
}
