using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.PostgreSQL;
using Rodonaves.EDI.DAL.PostgreSQL.Util;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.Interfaces
{
    public class AgreementCommunicationChannelDal : BaseCrudDal<AgreementCommunicationChannelInfo>, IAgreementCommunicationChannelDal
    {

        #region Ctor


        #endregion

        #region SQL

        const string _sqlGetAll = @"SELECT *
                                     FROM EDI_CLPCCO";

        const string _sqlGetById = _sqlGetAll + @"WHERE CLC_IDENTI = :CLC_IDENTI";

        const string _sqlGetByAgreementProcess = @"SELECT * 
                                                    FROM EDI_CLPCCO
                                                   WHERE CLC_CLP_IDENTI = :CLC_CLP_IDENTI";

        #endregion

        #region Mapper

        readonly FieldsClass<AgreementCommunicationChannelInfo> fields = new FieldsClass<AgreementCommunicationChannelInfo>();

        protected override PostgreSQLMapper<AgreementCommunicationChannelInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(AgreementCommunicationChannelInfo).Name;

            mapper.TableName = "EDI_CLPCCO";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "CLC_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.AgreementProcessId), "CLC_CLP_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.SendingType), "CLC_MODENV", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);

            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Email.Address), "CLC_E_MAIL", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Email.Name), "CLC_NOME", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Email.Title), "CLC_EMATIT", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Email.CellPhone), "CLC_TELCEL", true, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Email.ResidentialPhone), "CLC_TELCOM", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.ActiveMode), "CLC_MODATI", false, false, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.Address), "CLC_FTPEND", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.EnableSSH), "CLC_SSL", false, false, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.Password), "CLC_FTPSEN", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.ReceiverFolder), "CLC_FTPRET", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.SenderFolder), "CLC_FTPENV", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.User), "CLC_FTPUSU", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);


            mapper.OnGetParameterValue += Mapper_OnGetParameterValue;
            mapper.OnSetInstanceValue += Mapper_OnSetInstanceValue;

            return mapper;
        }

        void Mapper_OnGetParameterValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.Ftp.ActiveMode))
                newValue = (bool)value == true ? "S" : "N";

            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.Ftp.EnableSSH))
                newValue = (bool)value == true ? "S" : "N";

            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.SendingType))
                newValue = EDIEnumsList.GetSendingType().FirstOrDefault(x => x.Enum == (SendingTypeEnum)value).Value;

        }

        void Mapper_OnSetInstanceValue(ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.Ftp.ActiveMode))
                newValue = value.ToString() == "S" ? true : false;

            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.Ftp.EnableSSH))
                newValue = value.ToString() == "S" ? true : false;

            if (orLineMapper.PropertyName == fields.GetFieldNameFull(p => p.SendingType))
                newValue = EDIEnumsList.GetSendingType().FirstOrDefault(x => x.Value == value.ToString()).Enum;
        }

        #endregion

        #region PostgreSQLDal Methods

        protected override string GetSequenceName()
        {
            return "SEQ_EDI_CLPCCO";
        }

        #endregion

        #region Class Info

        public AgreementCommunicationChannelInfo NewClassInfo(object reader)
        {
            return base.PostgreSQLMapperBaseNew.NewClassInfo<AgreementCommunicationChannelInfo>(reader as NpgsqlDataReader);
        }

        #endregion

        #region Methods

        public async Task<List<AgreementCommunicationChannelInfo>> GetAllAsync()
        {
            var list = new List<AgreementCommunicationChannelInfo>();

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetAll))
                while (rdr.Read())
                    list.Add(NewClassInfo(rdr));

            return list;
        }

        public override async Task<AgreementCommunicationChannelInfo> GetByIdAsync(int id)
        {
            AgreementCommunicationChannelInfo info = null;

            var param = new List<NpgsqlParameter>()
            {
                PostgreSQLHelper.CreateParameter(":CLC_IDENTI", NpgsqlDbType.Integer, id)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                while (rdr.Read())
                    info = NewClassInfo(rdr);

            return info;
        }

        public async Task<List<AgreementCommunicationChannelInfo>> GetByParamsAsync(AgreementCommunicationChannelInfo info, int page, int amountByPage)
        {
            var list = new List<AgreementCommunicationChannelInfo>();

            var pagination = new OraPaginationInfo
            {
                AmountByPage = amountByPage,
                Page = page,
                FilterParameter = new StringBuilder()
            };

            string sqlCommand = _sqlGetAll,
                   where = " ";

            var parameters = new List<NpgsqlParameter>();


            if (info.Id != 0)
            {
                DalUtils.SetFilterOnQuery("CLC_IDENTI", ":CLC_IDENTI", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":CLC_IDENTI", NpgsqlDbType.Integer, info.Id));
            }

            if (info.SendingType != 0)
            {
                DalUtils.SetFilterOnQuery("CLC_MODENV", ":CLC_MODENV", ref where);
                string value = EDIEnumsList.GetSendingType().FirstOrDefault(x => x.Enum == info.SendingType).Value;
                parameters.Add(PostgreSQLHelper.CreateParameter(":CLC_MODENV", NpgsqlDbType.Varchar, value));
            }

            #region Email

            if (!string.IsNullOrEmpty(info.Email.Address))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(CLC_E_MAIL) LIKE :CLC_E_MAIL", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":CLI_IDENTI", NpgsqlDbType.Varchar, "%" + info.Email.Address.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(info.Email.Name))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(CLC_NOME) LIKE :CLC_NOME", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":CLC_NOME", NpgsqlDbType.Varchar, "%" + info.Email.Name.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(info.Email.Title))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(CLC_EMATIT) LIKE :CLC_EMATIT", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":CLC_EMATIT", NpgsqlDbType.Varchar, "%" + info.Email.Title.ToLower() + "%"));
            }

            #endregion

            #region Ftp

            if (!string.IsNullOrEmpty(info.Ftp.Address))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(CLC_FTPEND) LIKE :CLC_FTPEND", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":CLC_FTPEND", NpgsqlDbType.Varchar, "%" + info.Ftp.Address.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(info.Ftp.Password))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(CLC_FTPSEN) LIKE :CLC_FTPSEN", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":CLC_FTPSEN", NpgsqlDbType.Varchar, "%" + info.Ftp.Password.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(info.Ftp.ReceiverFolder))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(CLC_FTPENV) LIKE :CLC_FTPENV", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":CLC_FTPENV", NpgsqlDbType.Varchar, "%" + info.Ftp.ReceiverFolder.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(info.Ftp.ReceiverFolder))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(CLC_FTPRET) LIKE :CLC_FTPRET", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":CLC_FTPRET", NpgsqlDbType.Varchar, "%" + info.Ftp.ReceiverFolder.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(info.Ftp.User))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(CLC_FTPUSU) LIKE :CLC_FTPUSU", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":CLC_FTPUSU", NpgsqlDbType.Varchar, "%" + info.Ftp.User.ToLower() + "%"));
            }
            //

            #endregion

            foreach (NpgsqlParameter item in parameters)
                pagination.FilterParameter.Append(item.Value);

            sqlCommand += where;

            int totalPages = PostgreSQLHelper.GetTotalPages(GetConnectionString(), parameters.ToArray(), sqlCommand, amountByPage);
            using (var rdr = PostgreSQLHelper.ExecuteReaderPagination(GetConnectionString(), CommandType.Text, sqlCommand, pagination, parameters.ToArray()))
                while (rdr.Read())
                {
                    var data = NewClassInfo(rdr);
                    data.AmountPages = totalPages;

                    list.Add(data);
                }


            return list;
        }

        public Task<List<AgreementCommunicationChannelInfo>> GetByAgreementProcessAsync(int agreementProcessId)
        {
            return Task.Run(() =>
            {
                List<AgreementCommunicationChannelInfo> list = new List<AgreementCommunicationChannelInfo>();

                var param = new List<NpgsqlParameter>()
                {
                    PostgreSQLHelper.CreateParameter(":CLC_CLP_IDENTI", NpgsqlDbType.Integer, agreementProcessId)
                };

                using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetByAgreementProcess, param.ToArray()))
                    while (rdr.Read())
                        list.Add(NewClassInfo(rdr));
                return list;
            });
        }

        #endregion

    }
}
