using Rodonaves.EDI.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public interface IPersonDal : IBaseCrudDal<PersonInfo>
    {
        Task<List<PersonInfo>> GetAll(long companyId);
        int ExistPersonId(string taxIdRegistration);

        PersonInfo NewClassInfo(object reader);
    }
}
