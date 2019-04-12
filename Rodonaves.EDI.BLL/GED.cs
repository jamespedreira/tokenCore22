using Rodonaves.Core.Bll;
using Rodonaves.EDI.BLL.Interfaces.Factories;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.Interfaces;
using System;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class GED
    {
        private readonly Parameter _parameter;
        private readonly IGedFactory _gedFactory;

        public GED(Parameter parameter, IGedFactory gedFactory)
        {
            _parameter = parameter;
            _gedFactory = gedFactory;
        }

        public string Dowload(object[,] parms)
        {
            IGed helper = null;
            var newParms = GetParms(parms, ref helper);

            var success = false;
            var key = string.Empty;

            success = helper.Authenticate(newParms);
            if (success)
                key = helper.Dowload(newParms).Result;
            return key;
        }

        public string Move(object[,] parms)
        {
            IGed helper = null;
            var newParms = GetParms(parms, ref helper);

            var success = false;
            var key = string.Empty;

            success = helper.Authenticate(newParms);
            if (success)
                key = helper.Upload(newParms);
            return key;
        }

        private object[,] GetParms(object[,] parms, ref IGed gedHelper)
        {
            var mailParm = _parameter.GetParameter("ORIGEM EMAIL");

            var mailOrigin = parms.Length.Equals(3) ? parms[0, 2].ToString() : "OUT";
            var mailProvider = mailParm.GetValueAttribute(mailOrigin, "PROVIDER", true);
            var directory = mailParm.GetValueAttribute(mailOrigin, "DIRETORIO", true);
            var expires = mailParm.GetValueAttribute(mailOrigin, "EXPIRA", true);

            var gedParameter = _parameter.GetParameter("GED");
            var providerStr = ((short)Enum.Parse(typeof(ProviderEnum), mailProvider)).ToString();
            var factory = gedParameter.GetValueAttribute(providerStr, "FACTORY", true);
            var url = gedParameter.GetValueAttribute(providerStr, "URL", true);
            var user = gedParameter.GetValueAttribute(providerStr, "USER", true);
            var password = gedParameter.GetValueAttribute(providerStr, "PASSWORD", true);

            var newParms = new object[5, parms.Length];
            for (int i = 0; i < parms.Length; i++)
            {
                newParms[0, i] = parms[0, i];
            }

            newParms[1, 0] = user;
            newParms[2, 0] = password;
            newParms[3, 0] = url;
            newParms[3, 1] = directory;
            newParms[4, 0] = expires;

            var provider = ((ProviderEnum)Enum.Parse(typeof(ProviderEnum), factory));

            gedHelper = _gedFactory.CreateNew(provider);

            return newParms;
        }
    }
}
