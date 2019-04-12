using AutoMapper;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.EDI.Helpers.Interfaces;
using Rodonaves.EDI.Model;
using RTEFramework.BLL.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IConnection = Rodonaves.EDI.Helpers.Interfaces.IConnection;

namespace Rodonaves.EDI.BLL
{
    public class Provider : BaseCrudBll<IProviderDal, ProviderDTO, ProviderInfo>, IProvider
    {
        #region Ctor

        public Provider(IProviderDal dal, IMapper mapper) : base(dal, mapper) {}

        #endregion

        #region Methods

        public async Task<List<ProviderDTO>> GetAllAsync()
        {
            var list = await _dal.GetAll();

            return _mapper.Map<List<ProviderInfo>, List<ProviderDTO>>(list);
        }

        public async Task<ProviderDTO> GetByIdAsync(int id)
        {
            var obj = await _dal.GetByIdAsync(id);

            return _mapper.Map<ProviderInfo, ProviderDTO>(obj);
        }

        public new async Task<int> InsertAsync(ProviderDTO dto)
        {
            bool success = await base.InsertAsync(dto);
            
            if (!success)
                throw new BusinessException("Não foi possível inserir um novo provedor");

            return dto.Id;
        }

        public override async Task<bool> UpdateAsync(ProviderDTO dto)
        {
            bool success = await base.UpdateAsync(dto);

            if (!success)
                throw new BusinessException("Não foi possível editar o provedor");

            return success;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            
            bool success = false;
            success = await base.DeleteAsync(id);

            if (!success)
                throw new BusinessException("Não foi possível deletar o provedor");

            return success;
        }

        public async Task<List<ProviderDTO>> GetByParamsAsync(int id, int? providerType,string name, string descripton, string connectionString, int page, int amountByPage)
        {
            var list = await _dal.GetByParamsAsync(id, providerType,name, descripton, connectionString, page, amountByPage);

            return _mapper.Map<List<ProviderInfo>, List<ProviderDTO>>(list);
        }

        public Task<TestConnectionDTO> TestConnection(string connectionString, ProviderTypeEnum provider)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                ConnectionState _connectionState = new ConnectionState();
                try
                {
                    using (var connection = ConnectionFactory(provider, connectionString))
                    {
                        _connectionState = connection.TryConnection();
                    }
                }
                catch (Exception ex)
                {
                    _connectionState.IsConnected = false;
                    _connectionState.ErrorMessage = GetDeepestException(ex).Message;
                }

                return new TestConnectionDTO()
                {
                    Message = _connectionState.IsConnected ? "Conexão realizada com sucesso" : _connectionState.ErrorMessage,
                    Success = _connectionState.IsConnected
                };
            });
        }

        public Exception GetDeepestException(Exception ex)
        {
            return ex.InnerException != null ? GetDeepestException(ex.InnerException) : ex;
        }

        #endregion

        private IConnection ConnectionFactory(ProviderTypeEnum providerType, string connectionString)
        {
            Helpers.Interfaces.IConnection connection;
            switch (providerType)
            {
                case ProviderTypeEnum.PostgreSQL:
                    connection = new Helpers.PostgreSQL.ConnectionHelper(connectionString);
                    break;
                case ProviderTypeEnum.Oracle:
                    connection = new Helpers.Oracle.ConnectionHelper(connectionString);
                    break;
                default:
                    connection = new Helpers.Oracle.ConnectionHelper(connectionString);
                    break;
            }
            return connection;
        }

        
    }
}
