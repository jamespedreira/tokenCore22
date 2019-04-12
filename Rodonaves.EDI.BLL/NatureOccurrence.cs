using Newtonsoft.Json;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class NatureOccurrence : INatureOccurrence
    {
        #region Ctor

        private readonly IValidateClientAuthentication _validateClientAuthentication;

        public NatureOccurrence(IValidateClientAuthentication validateClientAuthentication) 
        {
            _validateClientAuthentication = validateClientAuthentication;
        }

        #endregion

        #region Methods

        public async Task<List<NatureOccurrenceDTO>> GetNatureOccurrences()
        {
            using (var httpClient = _validateClientAuthentication.CreateClient("RouteNatureOccurrence"))
            {
                var response = await httpClient.GetAsync("");
                var list = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(list))
                {
                    var j = JsonConvert.DeserializeObject<List<NatureOccurrenceDTO>>(list);
                    return j;
                }
                else
                    return null;
            }
        }

        #endregion
    }
}
