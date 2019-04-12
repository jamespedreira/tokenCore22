using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DAL.PostgreSQL.Util;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;
using Rodonaves.Engine.BaseObjects;
using Rodonaves.Engine.DbHelper;

namespace Rodonaves.EDI.DAL.PostgreSQL
{
    public class GenerateReturnDetailedStatusDal : BaseCrudDal<GenerateReturnDetailedStatusInfo>, IGenerateReturnDetailedStatusDal
    {
        #region Views

        const string _sqlMaxStatus = @"(SELECT GTS_PRGSTU STATUS,
					                           GTS_GTN_IDENTI
				                        FROM EDI_GTNSTS A
			                            INNER JOIN (SELECT MAX(GTS_IDENTI) GTS_IDENTI 
							                         FROM EDI_GTNSTS 
							                        GROUP BY GTS_GTN_IDENTI) B ON B.GTS_IDENTI = A.GTS_IDENTI)";
        const string _viewQuery = @"
                            SELECT 
	                            PES.PES_DESCRI, 
	                            LAY.CBL_EDIPRC,
	                            GTN_QNTVLR AS FRETES,
	                            STATUSINI.GTS_HORA GTS_HORAINI,
	                            STATUSFIM.GTS_HORA GTS_HORAFIM,
	                            STATUSINI.GTS_DATA,
	                            RET.GTN_IDENTI,
	                            STATUS
                            FROM 
	                            EDI_GRARTN RET
                            LEFT JOIN  EDI_GTNSTS STATUSINI     ON (STATUSINI.GTS_GTN_IDENTI = RET.GTN_IDENTI AND STATUSINI.GTS_PRGSTU = 1)
                            LEFT JOIN  EDI_GTNSTS STATUSFIM     ON (STATUSFIM.GTS_GTN_IDENTI = RET.GTN_IDENTI AND (STATUSFIM.GTS_PRGSTU = 2 OR STATUSFIM.GTS_PRGSTU = 3))
                            LEFT JOIN  
	                             " + _sqlMaxStatus + @" STATUS  ON STATUS.GTS_GTN_IDENTI = RET.GTN_IDENTI
                            INNER JOIN 
	                            EDI_CABLAY LAY 		 		    ON LAY.CBL_IDENTI = RET.GTN_CBL_IDENTI
                            INNER JOIN 
	                            EDI_CLIENT CLI  	 		    ON CLI.CLI_IDENTI = RET.GTN_CLI_IDENTI
                            INNER JOIN 
	                            EDI_PESSOA PES 		 		    ON PES.PES_IDENTI = CLI.CLI_PES_IDENTI
                            ";
        #endregion

        #region SQL
        const string _sqlGetAll = _viewQuery;

        const string _sqlGetResumeByProcessType = @"SELECT 
                                                            COUNT(GTS_GTN_IDENTI) TOTAL,
                                                            GTN_EDIPRC CBL_EDIPRC,
                                                            CASE 
                                                                WHEN GTS_PRGSTU = 0
                                                                THEN 10      
                                                                WHEN GTS_PRGSTU = 1
                                                                THEN 
                                                                    CASE 
                                                                        WHEN (NOW() - (GTS_DATA::DATE + GTS_HORA::INTERVAL)) > '24:00:00.000' 
                                                                        THEN 10
                                                                        ELSE GTS_PRGSTU
                                                                        END
                                                                ELSE GTS_PRGSTU	
                                                                END STATUS
                                                        FROM  EDI_GTNSTS S
                                                        INNER JOIN (SELECT MAX(GTS_IDENTI) GTS_IDENTI 
                                                                    FROM EDI_GTNSTS C
                                                                    GROUP BY GTS_GTN_IDENTI) C ON C.GTS_IDENTI = S.GTS_IDENTI
                                                        INNER JOIN EDI_GRARTN RET ON GTS_GTN_IDENTI = RET.GTN_IDENTI
                                                    GROUP BY 
                                                    GTN_EDIPRC,
                                                    STATUS";

        const string groupBy = @" 
                            GROUP BY
	                            PES_IDENTI, 
	                            CBL_EDIPRC, 
	                            STATUSINI.GTS_HORA,
	                            STATUSFIM.GTS_HORA,
	                            STATUSINI.GTS_DATA,
	                            GTN_IDENTI,
	                            STATUS";
        const string orderBy = @" ORDER BY RET.GTN_IDENTI DESC";
        #endregion
        #region Mapper

        readonly FieldsClass<GenerateReturnDetailedStatusInfo> fields = new FieldsClass<GenerateReturnDetailedStatusInfo> ();

        protected override PostgreSQLMapper<GenerateReturnDetailedStatusInfo> CreateMapperNew ()
        {
            var mapper = base.CreateMapperNew ();

            if (!base.DoMapperBase)
                return mapper;

            var className = typeof (ActionInfo).Name;

            mapper.TableName = "EDI_ACAO";

            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.Customer), "PES_DESCRI", false, null, ORMapper.FieldFlags.NonVersionField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.ProcessType), "CBL_EDIPRC", false, null, ORMapper.FieldFlags.NonVersionField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.TotalFreights), "FRETES", false, null, ORMapper.FieldFlags.NonVersionField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.StartingTime), "GTS_HORAINI", false, null, ORMapper.FieldFlags.NonVersionField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.EndingTime), "GTS_HORAFIM", false, null, ORMapper.FieldFlags.NonVersionField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.Date), "GTS_DATA", false, null, ORMapper.FieldFlags.NonVersionField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.GenerateReturnId), "GTN_IDENTI", false, null, ORMapper.FieldFlags.NonVersionField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.ProgressStatus), "STATUS", false, null, ORMapper.FieldFlags.NonVersionField, false);
            mapper.AddLineMapper (className, classFields.GetFieldNameFull (p => p.TotalItems), "TOTAL", false, null, ORMapper.FieldFlags.NonVersionField, false);

            mapper.OnGetParameterValue += Mapper_OnGetParameterValue;
            mapper.OnSetInstanceValue += Mapper_OnSetInstanceValue;

            return mapper;
        }
        void Mapper_OnGetParameterValue (ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull (p => p.ProcessType))
                newValue = (int) ((ProcessTypeEnum) value);

            if (orLineMapper.PropertyName == fields.GetFieldNameFull (p => p.ProgressStatus))
                newValue = (int) ((ProgressStatusEnum) value);
        }
        void Mapper_OnSetInstanceValue (ORLineMapper orLineMapper, object value, ref object newValue)
        {
            if (orLineMapper.PropertyName == fields.GetFieldNameFull (p => p.ProcessType))
                newValue = (ProcessTypeEnum) (int) value;

            if (orLineMapper.PropertyName == fields.GetFieldNameFull (p => p.ProgressStatus))
                newValue = (ProgressStatusEnum) (int) value;
        }

        protected override string GetSequenceName ()
        {
            throw new System.NotImplementedException ();
        }
        #endregion

        #region Methods
        public Task<List<GenerateReturnDetailedStatusInfo>> GetAllAsync () => Task.Run (() =>
        {
            var list = new List<GenerateReturnDetailedStatusInfo> ();

            using (var rdr = PostgreSQLHelper.ExecuteReader (GetConnectionString (), CommandType.Text, _sqlGetAll))
            while (rdr.Read ())
                list.Add (base.PostgreSQLMapperBaseNew.NewClassInfo<GenerateReturnDetailedStatusInfo> (rdr));

            return list;
        });
        public Task<List<GenerateReturnDetailedStatusInfo>> GetResumeByProcessTypeAsync () => Task.Run (() =>
        {
            var list = new List<GenerateReturnDetailedStatusInfo> ();

            using (var rdr = PostgreSQLHelper.ExecuteReader (GetConnectionString (), CommandType.Text, _sqlGetResumeByProcessType))
            while (rdr.Read ())
                list.Add (base.PostgreSQLMapperBaseNew.NewClassInfo<GenerateReturnDetailedStatusInfo> (rdr));

            return list;
        });

        public override Task<GenerateReturnDetailedStatusInfo> GetByIdAsync (int id)
        {
            throw new System.NotImplementedException ();
        }

        public Task<List<GenerateReturnDetailedStatusInfo>> GetByParamsAsync (GenerateReturnDetailedStatusInfo info, int page, int amountByPage) => Task.Run (() =>
        {
            var list = new List<GenerateReturnDetailedStatusInfo> ();

            var pagination = new OraPaginationInfo
            {
                AmountByPage = amountByPage,
                Page = page,
                FilterParameter = new StringBuilder ()
            };

            string sqlCommand = _sqlGetAll,
                where = string.Empty;

            var parameters = new List<NpgsqlParameter> ();

            #region Parameters

            if (info.ProcessType != ProcessTypeEnum.None)
            {
                DalUtils.SetFilterOnQuery ("CBL_EDIPRC", ":CBL_EDIPRC", ref where);
                parameters.Add (PostgreSQLHelper.CreateParameter (":CBL_EDIPRC", NpgsqlDbType.Varchar, (int) info.ProcessType));
            }

            if (string.IsNullOrEmpty (info.Customer) == false)
            {
                DalUtils.SetCustomFilterOnQuery ("LOWER(PES_DESCRI) LIKE :PES_DESCRI", ref where);
                parameters.Add (PostgreSQLHelper.CreateParameter (":PES_DESCRI", NpgsqlDbType.Varchar, "%" + info.Customer.ToLower () + "%"));
            }

            if (string.IsNullOrEmpty (info.Key) == false)
            {
                DalUtils.SetCustomFilterOnQuery ("GTN_IDENTI IN (SELECT distinct GTV_GTN_IDENTI FROM EDI_GTNVLR WHERE GTV_VALOR = :GTV_VALOR)", ref where);
                parameters.Add (PostgreSQLHelper.CreateParameter (":GTV_VALOR", NpgsqlDbType.Varchar, info.Key));
            }

            #endregion

            foreach (NpgsqlParameter item in parameters)
                pagination.FilterParameter.Append (item.Value);

            sqlCommand += " " + where.Replace ("AND", "OR") + groupBy + orderBy;

            int totalItems = 0;
            try
            {
                using (var rdr = PostgreSQLHelper.ExecuteReader (GetConnectionString (), CommandType.Text, $"select count(*) as total from ({ sqlCommand }) a", parameters.ToArray ()))
                while (rdr.Read ())
                    totalItems = int.Parse (rdr["total"].ToString ());
            }
            catch (System.Exception)
            {

            }

            int totalPages = PostgreSQLHelper.GetTotalPages (GetConnectionString (), parameters.ToArray (), sqlCommand, amountByPage);
            using (var rdr = PostgreSQLHelper.ExecuteReaderPagination (GetConnectionString (), CommandType.Text, sqlCommand, pagination, parameters.ToArray ()))
            while (rdr.Read ())
            {
                var data = base.PostgreSQLMapperBaseNew.NewClassInfo<GenerateReturnDetailedStatusInfo> (rdr);
                data.TotalItems = totalItems;
                data.AmountPages = totalPages;

                list.Add (data);
            }

            return list;

        });
        #endregion
    }
}