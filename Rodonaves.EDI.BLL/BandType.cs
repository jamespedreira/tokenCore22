using Rodonaves.Core.Bll;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class BandType : IBandType
    {
        private readonly Parameter _parameter;

        public List<BandTypeDTO> BandTypes { get; set; } = new List<BandTypeDTO>();

        public BandType(Parameter parameter)
        {
            _parameter = parameter;
        }

        public async Task<List<BandTypeDTO>> GetBandTypes(string processType)
        {
            var parameterInfo = _parameter.GetParameterByAttributeCondition("TIPBAN", new string[] { "PROCESS" }, new string[] { processType });

            return parameterInfo.Items.Select(x => new BandTypeDTO()
            {
                Value = x.Value.ToString(),
                Description = x.Description.ToString()
            }).ToList();
        }

        public async Task<string> GetBandTypeDescription(string value, ProcessTypeEnum processType)
        {
            var bandType = GetBandTypeByValue(value);

            if (bandType == null)
            {
                string process = ((int)processType).ToString();

                var bands = await GetBandTypes(process);

                BandTypes.AddRange(bands);

                bandType = GetBandTypeByValue(value);
            }

            return bandType != null ? bandType.Description : string.Empty;
        }

        private BandTypeDTO GetBandTypeByValue(string value)
        {
            return BandTypes.FirstOrDefault(x => x.Value == value);
        }
    }
}
