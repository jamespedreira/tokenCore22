using AutoMapper;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Model;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class Log : BaseCrudBll<ILogDal, LogDTO, LogInfo>, ILog
    {
        private readonly ILogDal _logDal;

        #region ctor

        public Log(ILogDal logDal, IMapper mapper) : base(logDal, mapper)
        {
            _logDal = logDal;
        }

        #endregion
        
        public new async Task<int> InsertAsync(LogDTO dto)
        {
            using (var transaction = _dal.BeginTransaction())
            {
                var success = await base.InsertAsync(dto);

                _dal.FinallyTransaction(success, transaction);

                return dto.Id;
            }
        }
    }
}
