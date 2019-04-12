using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DAL.PostgreSQL.Util;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class GenerateReturnCommunicationChannelDal : BaseCrudDal<CommunicationChannelInfo>, IGenerateReturnCommunicationChannelDal
    {
        #region Sql

        const string _sqlGetAll = @"SELECT *
                                     FROM EDI_GTNCCO";

        const string _sqlGetById = _sqlGetAll + @"WHERE GTC_IDENTI = :GTC_IDENTI";

        #endregion

        #region Mapper

        readonly FieldsClass<CommunicationChannelInfo> fields = new FieldsClass<CommunicationChannelInfo>();

        protected override PostgreSQLMapper<CommunicationChannelInfo> CreateMapperNew()
        {
            var mapper = base.CreateMapperNew();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof(CommunicationChannelInfo).Name;

            mapper.TableName = "EDI_GTNCCO";
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Id), "GTC_IDENTI", false, null, ORMapper.FieldFlags.KeyField | ORMapper.FieldFlags.InsertField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.GenerateReturnId), "GTC_GTN_IDENTI", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.SendingType), "GTC_MODENV", false, null, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Email.Address), "GTC_E_MAIL", true, string.Empty, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Email.Name), "GTC_NOME", true, string.Empty, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Email.Title), "GTC_EMATIT", true, string.Empty, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);

            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.ActiveMode), "GTC_MODATI", true, string.Empty, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.Address), "GTC_FTPEND", true, string.Empty, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.EnableSSH), "GTC_SSL", true, string.Empty, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, true);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.Password), "GTC_FTPSEN", true, string.Empty, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.ReceiverFolder), "GTC_FTPRET", true, string.Empty, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.SenderFolder), "GTC_FTPENV", true, string.Empty, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);
            mapper.AddLineMapper(className, fields.GetFieldNameFull(p => p.Ftp.User), "GTC_FTPUSU", true, string.Empty, ORMapper.FieldFlags.InsertField | ORMapper.FieldFlags.UpdateField, false);

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
            return "SEQ_EDI_GTNCCO";
        }

        #endregion

        #region Class Info

        public CommunicationChannelInfo NewClassInfo(object reader)
        {
            return base.PostgreSQLMapperBaseNew.NewClassInfo<CommunicationChannelInfo>(reader as NpgsqlDataReader);
        }

        #endregion

        public async Task<List<CommunicationChannelInfo>> GetAllAsync()
        {
            var list = new List<CommunicationChannelInfo>();

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetAll))
                while (rdr.Read())
                    list.Add(NewClassInfo(rdr));

            return list;
        }

        public override async Task<CommunicationChannelInfo> GetByIdAsync(int id)
        {
            CommunicationChannelInfo info = null;

            var param = new List<NpgsqlParameter>()
            {
                PostgreSQLHelper.CreateParameter(":GTC_IDENTI", NpgsqlDbType.Integer, id)
            };

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, _sqlGetById, param.ToArray()))
                while (rdr.Read())
                    info = NewClassInfo(rdr);

            return info;
        }

        public async Task<List<CommunicationChannelInfo>> GetByParamsAsync(CommunicationChannelInfo info, int page, int amountByPage)
        {
            var list = new List<CommunicationChannelInfo>();

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
                DalUtils.SetFilterOnQuery("GTC_IDENTI", ":GTC_IDENTI", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTC_IDENTI", NpgsqlDbType.Integer, info.Id));
            }

            if (info.GenerateReturnId != 0)
            {
                DalUtils.SetFilterOnQuery("GTC_GTN_IDENTI", ":GTC_GTN_IDENTI", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTC_GTN_IDENTI", NpgsqlDbType.Integer, info.GenerateReturnId));
            }

            if (info.SendingType != 0)
            {
                DalUtils.SetFilterOnQuery("GTC_MODENV", ":GTC_MODENV", ref where);
                string value = EDIEnumsList.GetSendingType().FirstOrDefault(x => x.Enum == info.SendingType).Value;
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTC_MODENV", NpgsqlDbType.Varchar, value));
            }

            #region Email

            if (!string.IsNullOrEmpty(info.Email.Address))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(GTC_E_MAIL) LIKE :GTC_E_MAIL", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":CLI_IDENTI", NpgsqlDbType.Varchar, "%" + info.Email.Address.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(info.Email.Name))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(GTC_NOME) LIKE :GTC_NOME", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTC_NOME", NpgsqlDbType.Varchar, "%" + info.Email.Name.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(info.Email.Title))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(GTC_EMATIT) LIKE :GTC_EMATIT", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTC_EMATIT", NpgsqlDbType.Varchar, "%" + info.Email.Title.ToLower() + "%"));
            }

            #endregion

            #region Ftp

            if (!string.IsNullOrEmpty(info.Ftp.Address))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(GTC_FTPEND) LIKE :GTC_FTPEND", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTC_FTPEND", NpgsqlDbType.Varchar, "%" + info.Ftp.Address.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(info.Ftp.Password))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(GTC_FTPSEN) LIKE :GTC_FTPSEN", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTC_FTPSEN", NpgsqlDbType.Varchar, "%" + info.Ftp.Password.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(info.Ftp.ReceiverFolder))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(GTC_FTPENV) LIKE :GTC_FTPENV", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTC_FTPENV", NpgsqlDbType.Varchar, "%" + info.Ftp.ReceiverFolder.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(info.Ftp.ReceiverFolder))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(GTC_FTPRET) LIKE :GTC_FTPRET", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTC_FTPRET", NpgsqlDbType.Varchar, "%" + info.Ftp.ReceiverFolder.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(info.Ftp.User))
            {
                DalUtils.SetCustomFilterOnQuery("LOWER(GTC_FTPUSU) LIKE :GTC_FTPUSU", ref where);
                parameters.Add(PostgreSQLHelper.CreateParameter(":GTC_FTPUSU", NpgsqlDbType.Varchar, "%" + info.Ftp.User.ToLower() + "%"));
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

        public async Task<List<CommunicationChannelInfo>> GetByFileReturnAsync(int FileReturnId)
        {
            var list = new List<CommunicationChannelInfo>();

            string sqlCommand = _sqlGetAll,
                   where = "";

            DalUtils.SetFilterOnQuery("GTC_GTN_IDENTI", ":GTC_GTN_IDENTI", ref where);

            var parameters = new List<NpgsqlParameter>() {
                PostgreSQLHelper.CreateParameter(":GTC_GTN_IDENTI", NpgsqlDbType.Integer, FileReturnId)
            };

            sqlCommand += where;

            using (var rdr = PostgreSQLHelper.ExecuteReader(GetConnectionString(), CommandType.Text, sqlCommand, parameters.ToArray()))
                while (rdr.Read())
                    list.Add(NewClassInfo(rdr));

            return list;
        }
    }
}
