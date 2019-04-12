using Rodonaves.EDI.BLL.Interfaces.Factories;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.BLL.Infra.Factories
{
    public class GedFactory : IGedFactory
    {
        public IGed CreateNew(ProviderEnum provider)
        {
            switch (provider)
            {
                case ProviderEnum.Path:
                    return new Helpers.GED.Path();
                case ProviderEnum.S3:
                default:
                    return new Helpers.GED.S3();
            }
        }
    }
}
