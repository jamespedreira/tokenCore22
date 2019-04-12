using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class GenerateReturnStatusDal : BaseCrudDal<GenerateReturnSatusInfo>, IGenerateReturnStatusDal
    {
        #region Mapper

        readonly FieldsClass<GenerateReturnSatusInfo> fields = new FieldsClass<GenerateReturnSatusInfo>();

        protected override PostgreSQLMapper<GenerateReturnSatusInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(GenerateReturnSatusInfo).Name;

            mapper.TableName = "EDI_GTNSTS";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "GTS_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.ProgressStatus), "GTS_PRGSTU", false, 0, ORMapper.FieldFlags.InsertField , true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.GenerateReturn.Id), "GTS_GTN_IDENTI", false, 0, ORMapper.FieldFlags.InsertField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Date), "GTS_DATA", false, null, ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Hour), "GTS_HORA", false, null, ORMapper.FieldFlags.InsertField, false);

            mapper.OnGetParameterValue += Mapper_OnGetParameterValue;
            mapper.OnSetInstanceValue += Mapper_OnSetInstanceValue;

            return mapper;
        }

        void Mapper_OnGetParameterValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProgressStatus))
                newValue = (int)((ProgressStatusEnum)value);
        }
        void Mapper_OnSetInstanceValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProgressStatus))
                newValue = (ProgressStatusEnum)(int)value;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_EDI_GTNSTS";
        }

        public override Task<GenerateReturnSatusInfo> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
