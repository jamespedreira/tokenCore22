using System.Collections.Generic;
using System.Linq;
using Rodonaves.Core.Bll;
using Rodonaves.EDI.DTO;

namespace Rodonaves.EDI.Helpers
{
    public static class MainBandHelper
    {
        public static string GetBandMain (List<LayoutBandDTO> bandDTOs, Parameter parameter)
        {

            var parameterInfo = parameter.GetParameter ("TIPBAN");

            for (int i = 0; i < bandDTOs.Count; i++)
            {
                var parameterValue = parameterInfo.Items
                    .Select (x => x.Value).ToList ()
                    .Find (p => p.ToString().Equals(bandDTOs[i].BandTypeId));
                var parameterItemInfo = parameterInfo.Items.Where (x => x.Value == parameterValue).FirstOrDefault ();
                if (parameterItemInfo != null && !string.IsNullOrEmpty (parameterItemInfo.Attributes["MAIN"].Value) && parameterItemInfo.Attributes["MAIN"].Value.Equals ("Y"))
                    return parameterValue.ToString ();

            }

            return string.Empty;
        }
    }
}