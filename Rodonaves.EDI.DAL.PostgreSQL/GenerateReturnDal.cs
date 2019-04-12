using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DAL.PostgreSQL.Util;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class GenerateReturnDal : BaseCrudDal<GenerateReturnInfo>, IGenerateReturnDal
    {
        private readonly IGenerateReturnCommunicationChannelDal _fileReturnCommunicationChannelDal;

        public GenerateReturnDal(IGenerateReturnCommunicationChannelDal fileReturnCommunicationChannelDal){
            _fileReturnCommunicationChannelDal = fileReturnCommunicationChannelDal;
        }

        #region Sql

        const string _sqlGetAll = @"SELECT *
                                     FROM EDI_GRARTN ";

        const string _sqlGetById = _sqlGetAll + @" WHERE GTN_IDENTI = :GTN_IDENTI";

        const string _sqlGetByProgress = @"SELECT *
                                             FROM EDI_GRARTN 
                                            WHERE GTN_PRGSTU = :GTN_PRGSTU";

        const string _comandUpdateBillOfLadingQuantaty = @"UPDATE EDI_GRARTN RET
                                                            SET  GTN_QNTVLR = (SELECT COUNT(*) FROM EDI_GTNVLR WHERE GTV_GTN_IDENTI = RET.GTN_IDENTI)
                                                           WHERE  RET.GTN_IDENTI = :GTN_IDENTI";

        #endregion

        #region Mapper

        readonly FieldsClass<GenerateReturnInfo> fields = new FieldsClass<GenerateReturnInfo>();

        protected override PostgreSQLMapper<GenerateReturnInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(GenerateReturnInfo).Name;

            mapper.TableName = "EDI_GRARTN";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "GTN_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.LayoutHeader.Id), "GTN_CBL_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.LayoutFileName.Id), "GTN_NAR_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Customer.Id), "GTN_CLI_IDENTI", false, false, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.AgreementProcess.Id), "GTN_CLP_IDENTI", true, 0, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.BillOfLadingId), "GTN_FLTCTR", true, 0, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.BillOfLadingQuantity), "GTN_QNTVLR", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.ProgressStatus), "GTN_PRGSTU", false, 0, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.ProcessType), "GTN_EDIPRC", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.StartingDate), "GTN_DATAINI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.EndingDate), "GTN_DATAFIN", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.ExecutionType), "GTN_TIPEXE", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);

            mapper.OnGetParameterValue += Mapper_OnGetParameterValue;
            mapper.OnSetInstanceValue += Mapper_OnSetInstanceValue;

            return mapper;
        }

        void Mapper_OnGetParameterValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProcessType))
                newValue = (int)((ProcessTypeEnum)value);

            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ExecutionType))
                newValue = (int)((ExecutionTypeEnum)value);

            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProgressStatus))
                newValue = (int)((ProgressStatusEnum)value);
        }
        void Mapper_OnSetInstanceValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProcessType))
                newValue = (ProcessTypeEnum)(int)value;

            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ExecutionType))
                newValue = (ExecutionTypeEnum)(int)value;

            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.ProgressStatus))
                newValue = (ProgressStatusEnum)(int)value;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_EDI_GRARTN";
        }

        #endregion

        #region Class Info

        public async Task<GenerateReturnInfo> NewClassInfo(object reader)
        {
            var info = base.PostgreSQLMapperBaseNew.NewClassInfo<GenerateReturnInfo>(reader as NpgsqlDataReader);

            if(info != null && info.Id > 0)
                info.CommunicationChannels = await _fileReturnCommunicationChannelDal.GetByFileReturnAsync(info.Id);

            return info;
        }

        #endregion

        public async Task<List<GenerateReturnInfo>> GetAllAsync()
        {
            var list = new List<GenerateReturnInfo>();

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetAll))
                while (rdr.Read())
                    list.Add(await NewClassInfo(rdr));

            return list;
        }

        public override async Task<GenerateReturnInfo> GetByIdAsync(int id)
        {
            GenerateReturnInfo info = null;

            var param = new List<NpgsqlParameter>()
            {
                PostgreSQLHelper.CreateParameter(":GTN_IDENTI", NpgsqlDbType.Integer, id)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                while (rdr.Read())
                    info = await NewClassInfo(rdr);

            return info;
        }

        public async Task<List<GenerateReturnInfo>> GetByParamsAsync(GenerateReturnInfo info, int page, int amountByPage)
        {
            var list = new List<GenerateReturnInfo>();

            var pagination = new OraPaginationInfo
            {
                AmountByPage = amountByPage,
                Page = page,
                FilterParameter = new StringBuilder()
            };

            string sqlCommand = _sqlGetAll,
                   where = " ";

            var parameters = new List<NpgsqlParameter>();

            #region Parameters

            if (info.Id != 0)
            {
                DalUtils.SetFilterOnQuery("GTN_IDENTI", ":GTN_IDENTI", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTN_IDENTI", NpgsqlDbType.Integer, info.Id));
            }

            if (info.LayoutHeader.Id != 0)
            {
                DalUtils.SetFilterOnQuery("GTN_CBL_IDENTI", ":GTN_CBL_IDENTI", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTN_CBL_IDENTI", NpgsqlDbType.Integer, info.LayoutHeader.Id));
            }

            if (info.LayoutFileName.Id != 0)
            {
                DalUtils.SetFilterOnQuery("GTN_NAR_IDENTI", ":GTN_NAR_IDENTI", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTN_NAR_IDENTI", NpgsqlDbType.Integer, info.LayoutFileName.Id));
            }

            if (info.Customer.Id != 0)
            {
                DalUtils.SetFilterOnQuery("GTN_CLI_IDENTI", ":GTN_CLI_IDENTI", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTN_CLI_IDENTI", NpgsqlDbType.Integer, info.Customer.Id));
            }

            if (info.StartingDate != null)
            {
                DalUtils.SetFilterOnQuery("GTN_DATAINI", ":GTN_DATAINI", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTN_DATAINI", NpgsqlDbType.Date, info.StartingDate));
            }

            if (info.EndingDate != null)
            {
                DalUtils.SetFilterOnQuery("GTN_DATAFIN", ":GTN_DATAFIN", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTN_DATAFIN", NpgsqlDbType.Date, info.EndingDate));
            }

            if (info.BillOfLadingId != 0)
            {
                DalUtils.SetFilterOnQuery("GTN_FLTCTR", ":GTN_FLTCTR", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTN_FLTCTR", NpgsqlDbType.Integer, info.BillOfLadingId));
            }

            #endregion

            foreach (NpgsqlParameter item in parameters)
                pagination.FilterParameter.Append(item.Value);

            sqlCommand += where;

            int totalPages = PostgreSQLHelper.GetTotalPages(GetConnectionString(), parameters.ToArray(), sqlCommand, amountByPage);
            using (var rdr = PostgreSQLHelper.ExecuteReaderPagination(GetConnectionString(), CommandType.Text, sqlCommand, pagination, parameters.ToArray()))
                while (rdr.Read())
                {
                    var data = await NewClassInfo(rdr);
                    data.AmountPages = totalPages;

                    list.Add(data);
                }


            return list;
        }

        public async Task<List<GenerateReturnInfo>> GetUnprocessedGenerateReturnAsync()
        {
            var list = new List<GenerateReturnInfo>();

            var parameters = new List<NpgsqlParameter>() {
                PostgreSQLHelper.CreateParameter(":GTN_PRGSTU", NpgsqlDbType.Integer, (int)ProgressStatusEnum.Unprocessed)
            };

            string command = _sqlGetByProgress;

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, command, parameters.ToArray()))
                while (rdr.Read())
                    list.Add(await NewClassInfo(rdr));

            return list;
        }

        public async Task<List<GenerateReturnInfo>> GetUnprocessedGenerateReturnByCustomerAsync(int customerId)
        {
            var list = new List<GenerateReturnInfo>();

            var parameters = new List<NpgsqlParameter>() {
                PostgreSQLHelper.CreateParameter(":GTN_PRGSTU", NpgsqlDbType.Integer, (int)ProgressStatusEnum.Unprocessed)
            };

            string command = _sqlGetByProgress;

            if (customerId != 0)
            {
                DalUtils.SetFilterOnQuery("GTN_CLI_IDENTI", ":GTN_CLI_IDENTI", ref command);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTN_CLI_IDENTI", NpgsqlDbType.Integer, customerId));
            }

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, command, parameters.ToArray()))
                while (rdr.Read())
                    list.Add(await NewClassInfo(rdr));

            return list;
        }

        public Task UpdateBillOfLadingQuantaty(int id)
        {
            return Task.Run(() =>
            {
                var parameters = new List<NpgsqlParameter>() {
                    PostgreSQLHelper.CreateParameter(":GTN_IDENTI", NpgsqlDbType.Integer, id)
                };

                PostgreSQLHelper.ExecuteNonQuery(GetConnectionString(), CommandType.Text, _comandUpdateBillOfLadingQuantaty, parameters.ToArray());
            });
        }
    }
}
