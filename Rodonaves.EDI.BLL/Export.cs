using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rodonaves.Core.Bll;
using Rodonaves.Core.Enums;
using Rodonaves.Core.Model;
using Rodonaves.EDI.BLL.Infra.Exceptions;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.BLL.Interfaces.Factories;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.EDI.Helpers.Interfaces;
using Rodonaves.EDI.Model;
using Rodonaves.TaskExecutor.Infra;
using RTEFramework.BLL.Infra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ParameterInfo = Rodonaves.Core.Model.ParameterInfo;

namespace Rodonaves.EDI.BLL
{
    #region Generate File Class

    public struct RowFile
    {
        public string FileName { get; set; }
        public string LineValue { get; set; }
        public string LineNumber { get; set; }
        public string LineWarning { get; set; }
        public string KeyValues { get; set; }
        public string ReferenceValues { get; set; }
    }

    #endregion

    public class Export : IExport
    {
        #region Properties

        private readonly IQueryResultFactory _queryResultFactory;
        private readonly IFtpClientFactory _ftpClientFactory;
        private readonly IGedFactory _gedFactory;
        private readonly IMapper _mapper;
        private readonly IGenerateReturn _generateReturn;
        private readonly ILayoutFileNameDal _layoutFileNameDal;
        private readonly IBandType _bandType;
        private ILogger _logger;

        private readonly GED _ged;

        public List<BandTypeDTO> BandTypes { get; set; } = new List<BandTypeDTO>();

        private readonly MailboxHeader _mailboxHeader;
        private readonly Parameter _parameter;
        private readonly SMTP _smtp;
        private readonly MailQueue _mail;

        const string _OBJECT_VALUE = "OBJECT_VALUE";
        const string _REFENCE_VALUE = "REFENCE_VALUE";

        private ParameterInfo parameterInfo;

        public ParameterInfo ParameterInfo
        {
            get
            {
                if (parameterInfo == null)
                    parameterInfo = _parameter.GetParameter("TIPBAN");

                return parameterInfo;
            }
            set { parameterInfo = value; }
        }

        #endregion

        #region Ctor

        public Export(
            IQueryResultFactory queryResultFactory,
            IFtpClientFactory ftpClientFactory,
            IGedFactory gedFactory,
            IMapper mapper,
            IGenerateReturn generateReturn,
            ILayoutFileNameDal layoutFileNameDal,
            ILogger logger,
            IBandType bandType,
            MailQueue mail,
            MailboxHeader mailboxHeader,
            Parameter parameter,
            GED ged,
            SMTP smtp)
        {
            _queryResultFactory = queryResultFactory;
            _ftpClientFactory = ftpClientFactory;
            _gedFactory = gedFactory;
            _mapper = mapper;
            _parameter = parameter;
            _generateReturn = generateReturn;
            _layoutFileNameDal = layoutFileNameDal;
            _mailboxHeader = mailboxHeader;
            _smtp = smtp;
            _mail = mail;
            _logger = logger;
            _bandType = bandType;
            _ged = ged;
        }

        #endregion

        #region Generate File

        public async System.Threading.Tasks.Task ExecuteAsync(ILogger logger, string arguments)
        {
            _logger = logger;
            await GenerateFileByReturnId(Convert.ToInt32(arguments));
        }

        public async Task<bool> GenerateFileByReturnId(int generateReturnId)
        {
            var generateReturn = await _generateReturn.GetByIdAsync(generateReturnId);

            if (generateReturn == null)
                return true;

            return await CreateFiles(generateReturn);
        }

        #region Write Methods

        async Task<bool> CreateFiles(GenerateReturnDTO generateReturn)
        {
            var success = true;

            try
            {
                await _generateReturn.UpdateProgressStatusAsync(generateReturn.Id, ProgressStatusEnum.Started);
                await _logger.LogAsync(generateReturn.Id.ToString(), $"Início do processamento do layout {generateReturn.LayoutHeader.Description}", string.Empty, LogType.Success);

                if (generateReturn.CommunicationChannels.Count > 0)
                {
                    var result = await GetBillOfLading(generateReturn);

                    result = PageQueryResult(result);

                    if (result.Rows.Count > 0)
                    {
                        await _generateReturn.InsertGenerateReturnValuesAsync(generateReturn, result);

                        await _generateReturn.UpdateBillOfLadingQuantaty(generateReturn.Id);

                        var info = _mapper.Map<GenerateReturnDTO, GenerateReturnInfo>(generateReturn);

                        await CreateFiles(info, result);
                    }
                }
                else
                    await _logger.LogAsync(generateReturn.Id.ToString(), "Nenhum canal de comunicação foi informado. A processamento será encerrado", string.Empty, LogType.Warning);

                await _generateReturn.UpdateProgressStatusAsync(generateReturn.Id, ProgressStatusEnum.Finished);
                await _logger.LogAsync(generateReturn.Id.ToString(), $"Término do processamento do layout {generateReturn.LayoutHeader.Description}", string.Empty, LogType.Success);

            }
            catch (BusinessException ex)
            {
                await _generateReturn.UpdateProgressStatusAsync(generateReturn.Id, ProgressStatusEnum.Error);
            }
            catch (Exception ex)
            {
                await _generateReturn.UpdateProgressStatusAsync(generateReturn.Id, ProgressStatusEnum.Error);
                await _logger.LogAsync(generateReturn.Id.ToString(), $"Ocorreu um erro não esperado no processameto do layout {generateReturn.LayoutHeader.Description}", ex.Message, LogType.Error);
                success = false;
            }

            return success;
        }

        async System.Threading.Tasks.Task CreateFiles(GenerateReturnInfo generateReturn, QueryResult<object> result)
        {
            var filePath = Environment.GetEnvironmentVariable("TEMP") + @"\";
            var lstRowFiles = await WriterToLayoutHeader(generateReturn, result);
            var lstFileNames = GetFileNames(lstRowFiles);

            if (lstFileNames.Count > 0)
            {
                var lstArchiveLogInfo = await WriteFiles(filePath, lstFileNames, lstRowFiles, generateReturn.Customer, generateReturn);

                await SendToCommunicationChannels(lstArchiveLogInfo, filePath, generateReturn.Customer, generateReturn.CommunicationChannels, generateReturn);
            }
            else
                await _logger.LogAsync(generateReturn.Id.ToString(), "Nenhum arquivo foi gerado", $"Por favor, verifique as colunas do arquivo {generateReturn.LayoutFileName.Description} vinculado ao layout {generateReturn.LayoutHeader.Description}", LogType.Warning);


        }

        #region Files

        private async Task<List<FileLogInfo>> WriteFiles(string filePath, List<string> lstFileNames, List<RowFile> lstRowFiles, CustomerInfo customer, GenerateReturnInfo generateReturn)
        {
            await _logger.LogAsync(generateReturn.Id.ToString(), $"Iniciando a gravação de arquivos", string.Empty, LogType.Success);

            var lstArchiveLogInfo = new List<FileLogInfo>();

            foreach (var fileName in lstFileNames.Where(x => !string.IsNullOrEmpty(x)))
            {
                var fileNameFull = filePath + fileName;

                lstArchiveLogInfo.Add(CreateFileLogInfo(customer, fileName));

                await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Iniciado a gravação do arquivo {fileName}"), $"Caminho alocado {fileNameFull}", LogType.Debug);

                try
                {
                    WrtieFile(fileNameFull, fileName, lstRowFiles);

                    await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Arquivo {fileName} gravado com sucesso"), $"Caminho alocado {fileNameFull}", LogType.Debug);
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Erro ao gravar o arquivo {fileName}"), ex.Message, LogType.Error);
                    throw ex;
                }
            }

            await _logger.LogAsync(generateReturn.Id.ToString(), $"Finalizado com sucesso a gravação de arquivos", string.Empty, LogType.Success);

            return lstArchiveLogInfo;
        }

        private void WrtieFile(string fileNameFull, string fileName, List<RowFile> lstRowFiles)
        {
            using (var streamWriter = new StreamWriter(fileNameFull))
            {
                var lstRowFilesFromFileName = lstRowFiles.Where(p => p.FileName.Equals(fileName)).ToList();

                foreach (var rowFile in lstRowFilesFromFileName)
                    streamWriter.WriteLine(rowFile.LineValue);
            }
        }

        #endregion

        private async Task<List<RowFile>> WriterToLayoutHeader(GenerateReturnInfo generateReturn, QueryResult<object> result)
        {
            var file = new List<RowFile>();
            List<string> keys = new List<string>();

            foreach (var dataRow in result.Rows)
            {
                //Reinicia bandas do layout
                generateReturn.LayoutHeader.LayoutBands.ToList().ForEach(p => p.Writed = false);

                //Registra evento do processo

                var dataRowCommand = (dataRow as JObject);
                var rowFile = new RowFile();
                rowFile.FileName = string.Empty;
                rowFile.KeyValues = GetRowValueToString(dataRowCommand, _OBJECT_VALUE);
                rowFile.ReferenceValues = GetRowValueToString(dataRowCommand, _REFENCE_VALUE);
                if (keys.Contains(rowFile.KeyValues))
                    continue;
                else
                    keys.Add(rowFile.KeyValues);

                await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Início do processamento {GetValueDescription(rowFile, true)}"), $"{GetValueDescription(rowFile, false)}", LogType.Success);

                //Grava nome do arquivo
                //WriterFileName(generateReturn, ref file, ref rowFile);
                int indexBandCurrent = 0;
                int indexBand = 0;

                try
                {
                    for (indexBandCurrent = 0, indexBand = 0; indexBandCurrent < generateReturn.LayoutHeader.LayoutBands.Count; indexBandCurrent++, indexBand++)
                    {
                        if (generateReturn.LayoutHeader.LayoutBands[indexBandCurrent].Writed)
                            continue;

                        (file, rowFile, dataRowCommand, indexBandCurrent) = await WriterToLayoutBands(file, rowFile, dataRowCommand, indexBandCurrent, indexBand, result.Columns, generateReturn);

                        if (indexBandCurrent == generateReturn.LayoutHeader.LayoutBands.Count && generateReturn.LayoutHeader.LayoutBands[indexBandCurrent].Break)
                            CreateNewRow(ref file, ref rowFile, true);

                        indexBand = indexBandCurrent;
                    }

                    await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Finalizado o processamento {GetValueDescription(rowFile, true)}"), $"{GetValueDescription(rowFile, false)}", LogType.Success);
                }
                catch (BusinessException ex)
                {
                    throw new BusinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    string layoutName = string.Empty;
                    if (generateReturn.LayoutHeader != null && generateReturn.LayoutHeader.LayoutBands != null &&
                                    generateReturn.LayoutHeader.LayoutBands.Count < indexBandCurrent && generateReturn.LayoutHeader.LayoutBands[indexBandCurrent].Script != null)
                    {
                        layoutName = Environment.NewLine + "Layout: " + generateReturn.LayoutHeader.LayoutBands[indexBandCurrent].Script.Description;
                    }

                    await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Erro no processamento {GetValueDescription(rowFile, true)}"), ex.Message, LogType.Error);
                    throw new Exception(ex.Message + layoutName);
                }
            }

            return file;
        }

        private async Task<(List<RowFile> file, RowFile rowFile, JObject dataRowCommand, int indexBandCurrent)> WriterToLayoutBands(List<RowFile> file, RowFile rowFile, JObject dataRowCommand, int indexBandCurrent, int indexBand, List<ColumnHeader> columns, GenerateReturnInfo generateReturn)
        {
            var currentLayoutBand = generateReturn.LayoutHeader.LayoutBands[indexBand];

            currentLayoutBand.Writed = true;

            var bandTypeDescription = await _bandType.GetBandTypeDescription(currentLayoutBand.BandTypeId, generateReturn.LayoutHeader.ProcessType);

            await _logger.LogAsync(generateReturn.Id.ToString(), $"Início do processamento da banda {bandTypeDescription}", string.Empty, LogType.Success);

            var commandBand = GetSqlCommand(columns, dataRowCommand, currentLayoutBand.Script.Script);

            var provider = generateReturn.LayoutHeader.Script.Provider;

            await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Executando o Script {currentLayoutBand.Script.Description} da banda {bandTypeDescription}"), $"Comando executado: {commandBand}", LogType.Debug);

            var dataReader = await GetDataReader(provider, commandBand);

            if (dataReader.Success)
            {

                await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Executado com sucesso o Script {currentLayoutBand.Script.Description} da banda {bandTypeDescription}"), $"Comando executado: {commandBand}", LogType.Debug);

                for (int i = 0; i < dataReader.Rows.Count; i++)
                {
                    var row = (dataReader.Rows[i] as JObject);

                    //Grava nome do arquivo
                    if (currentLayoutBand.BandTypeId.Equals(GetBandMain(generateReturn.LayoutHeader)))
                        (file, rowFile) = await WriterToLayoutColumns(file, rowFile, generateReturn.LayoutFileName.LayoutColumns.ToList(), row, commandBand, true, provider, generateReturn);

                    //Grava valores dos campos
                    (file, rowFile) = await WriterToLayoutColumns(file, rowFile, currentLayoutBand.LayoutColumns.ToList(), row, commandBand, false, provider, generateReturn);

                    if (indexBand + 1 >= generateReturn.LayoutHeader.LayoutBands.Count)
                        continue;

                    var key = currentLayoutBand.KeyBand;
                    var keyParent = generateReturn.LayoutHeader.LayoutBands[indexBand + 1].KeyParentBand;

                    if (key.Equals(keyParent))
                    {
                        indexBand++;
                        dataRowCommand = (dataReader.Rows[i] as JObject);

                        //Grava campos de bandas dependentes
                        if (i + 1 < dataReader.Rows.Count || (dataReader.Rows.Count > 1 && i + 1 >= dataReader.Rows.Count) || dataReader.Rows.Count == 1)
                        {
                            if (indexBand < generateReturn.LayoutHeader.LayoutBands.Count)
                                await WriterToLayoutBands(file, rowFile, dataRowCommand, indexBandCurrent, indexBand, dataReader.Columns, generateReturn);

                            indexBand--;
                        }
                    }
                }
            }
            else
            {
                await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Falha ao executar o Script {currentLayoutBand.Script.Description} da banda {bandTypeDescription}"), $"Erro: {dataReader.Message} \n\n Comando executado: {commandBand}", LogType.Error);
                throw new BusinessException(dataReader.Message);
            }

            await _logger.LogAsync(generateReturn.Id.ToString(), $"Finalizado com sucesso o processamento da banda {bandTypeDescription}", string.Empty, LogType.Success);

            return (file, rowFile, dataRowCommand, indexBandCurrent);
        }

        async Task<(List<RowFile> file, RowFile rowFile)> WriterToLayoutColumns(List<RowFile> file, RowFile rowFile, List<LayoutColumnInfo> lstLayoutColumnInfo, JObject dataReaderBand, string commandBand, bool isFileName, ProviderInfo provider, GenerateReturnInfo generateReturn)
        {
            foreach (var layoutColumnInfo in lstLayoutColumnInfo)
            {
                string columnValue = await GetColumnValue(commandBand, layoutColumnInfo, dataReaderBand, provider);

                if (layoutColumnInfo.ColumnType == ColumnType.FieldFromTo)
                {
                    var layoutDictionaryInfo = layoutColumnInfo.LayoutDictionaries.Find(p => p.TmsValue.Equals(columnValue));

                    if (layoutDictionaryInfo != null)
                        columnValue = layoutDictionaryInfo.ReferenceValue;
                    else
                    {
                        var messageError = $"Não foi cadastrado um valor \"De Para\" para o valor [{columnValue}]";
                        await _logger.LogAsync(generateReturn.Id.ToString(), $"Erro ao buscar o valor da coluna [{layoutColumnInfo.ElementValue}]", messageError, LogType.Warning);
                    }
                }

                if (!string.IsNullOrEmpty(rowFile.LineValue))
                {
                    var lenght = rowFile.LineValue.Length + (layoutColumnInfo.Begin - rowFile.LineValue.Length);
                    rowFile.LineValue = rowFile.LineValue.PadRight(lenght > 0 ? lenght - 1 : lenght, ' ');
                }

                var value = await GetValueFromColumn(layoutColumnInfo, columnValue, layoutColumnInfo.ElementValue, generateReturn.Id);
                value = value.Substring(0, layoutColumnInfo.Size);

                rowFile.LineValue += value;

                if (layoutColumnInfo.Register != null && layoutColumnInfo.ControlBreak)
                    CreateNewRow(ref file, ref rowFile, layoutColumnInfo.ControlBreak);
            }

            if (isFileName)
            {
                rowFile.FileName = rowFile.LineValue;
                rowFile.LineValue = string.Empty;

                if (string.IsNullOrEmpty(rowFile.FileName))
                {
                    var layoutFileName = lstLayoutColumnInfo.FirstOrDefault(x => x.LayoutFileNameId != 0);

                    if (layoutFileName != null)
                    {
                        var info = await _layoutFileNameDal.GetByIdAsync(layoutFileName.LayoutFileNameId);
                        await _logger.LogAsync(generateReturn.Id.ToString(), $"Nome de arquivo não informado. Registro ${info.Description}", string.Empty, LogType.Warning);
                    }
                }
            }

            return (file, rowFile);
        }

        private async Task<(List<RowFile> file, RowFile rowFile)> WriterFileName(GenerateReturnInfo generateReturn, List<RowFile> file, RowFile rowFile)
        {
            var script = generateReturn.LayoutHeader.Script;
            var provider = generateReturn.LayoutHeader.Script.Provider;
            var commandGeneral = script.Script + " AND ROWNUM = 1 ";

            SetParameters(_mapper.Map<CustomerInfo, CustomerDTO>(generateReturn.Customer), DateTime.Now, DateTime.Now, ref commandGeneral);

            var reader = await GetDataReader(provider, commandGeneral);

            var row = (reader.Rows[0] as JObject);

            (file, rowFile) = await WriterToLayoutColumns(file, rowFile, generateReturn.LayoutFileName.LayoutColumns, row, commandGeneral, true, provider, generateReturn);

            return (file, rowFile);
        }

        void CreateNewRow(ref List<RowFile> file, ref RowFile rowFile, bool bandBreak)
        {
            if (file != null && (!string.IsNullOrEmpty(rowFile.LineValue) || bandBreak))
                file.Add(rowFile);

            rowFile.LineValue = string.Empty;
            rowFile.LineWarning = string.Empty;
        }

        #endregion

        #region Erros and Warnings

        string GetError(LayoutColumnInfo layoutColumnInfo, string columnValue, string columnName)
        {
            string divisor = ", ";
            string msg = string.Empty;

            msg += "COLUNA";
            if (!string.IsNullOrEmpty(layoutColumnInfo.Register))
                msg += "Registro: " + layoutColumnInfo.Register.ToString();
            msg += divisor + "Sequência: " + layoutColumnInfo.Sequence.ToString();
            msg += divisor + "Início: " + layoutColumnInfo.Begin.ToString();
            msg += divisor + "Fim: " + layoutColumnInfo.End.ToString();
            msg += divisor + "Tamanho: " + layoutColumnInfo.Size.ToString();
            msg += divisor + "Valor: " + layoutColumnInfo.ElementValue;
            msg += divisor + "Visual: " + layoutColumnInfo.Visual;
            msg += divisor;

            if (columnValue != null)
            {
                msg += "SCRIPT";
                msg += divisor + "Coluna: " + columnName;
                msg += divisor + "Valor: " + columnValue;
                msg += divisor;
            }

            return msg;
        }

        void SetWarning(ref RowFile rowFile, ExportInfo columnValue, string messageError)
        {
            rowFile.LineWarning += !string.IsNullOrEmpty(rowFile.LineWarning) ? " / " + messageError : messageError;
        }

        #endregion

        #region File Utils

        FileLogInfo CreateFileLogInfo(CustomerInfo customerInfo, string fileName)
        {
            var fileLogInfo = new FileLogInfo();

            fileLogInfo.Customer.Id = customerInfo.Id;
            fileLogInfo.Date = DateTime.Now;
            fileLogInfo.Name = fileName;
            //fileLogInfo.Status = FileStatus.None;

            return fileLogInfo;
        }

        List<string> GetFileNames(List<RowFile> lstRowFile)
        {
            var lstFileName = new List<string>();
            for (int i = 0; i < lstRowFile.Count; i++)
            {
                if (lstFileName.Contains(lstRowFile[i].FileName))
                    continue;
                else
                    lstFileName.Add(lstRowFile[i].FileName);
            }

            return lstFileName;
        }

        public string FormatLogMessage(string value, int maxLength = 100)
        {
            return value?.Substring(0, Math.Min(value.Length, maxLength));
        }

        #endregion

        #region Column Value

        private string GetRowValueToString(JObject row, string column)
        {
            var value = row.GetValue(column);
            return value != null ? value.ToString() : string.Empty;
        }

        async Task<string> GetColumnCalculatedValue(string commandBand, LayoutColumnInfo layoutColumnInfo, ProviderInfo provider)
        {
            var columnValue = layoutColumnInfo.ElementValue;

            var dataReader = await GetDataReader(provider, columnValue);

            if (dataReader.Rows.Count > 0)
            {

                columnValue = columnValue.Replace(", ", ",");
                columnValue = columnValue.Replace(" ,", ",");

                var comands = columnValue.Split();

                var row = (dataReader.Rows[0] as JObject);

                for (int i = 0; i < comands.Length; i++)
                {
                    var value = row.GetValue(comands[i].Replace("'", string.Empty));

                    if (value != null)
                    {
                        columnValue = value.ToString();
                        break;
                    }
                }
            }

            return columnValue;
        }

        async Task<string> GetColumnValue(string commandBand, LayoutColumnInfo layoutColumnInfo, JObject lstExportInfo, ProviderInfo provider)
        {
            if (layoutColumnInfo.ColumnType == ColumnType.Calculated)
                return await GetColumnCalculatedValue(commandBand, layoutColumnInfo, provider);
            else
            {
                var value = GetRowValueToString(lstExportInfo, layoutColumnInfo.ElementValue);
                return value != null ? value.ToString() : string.Empty;
            }
        }

        private async Task<string> GetValueFromColumn(LayoutColumnInfo layoutColumnInfo, string columnValue, string columnName, int generateReturnId)
        {
            var attribute = _parameter.GetAttributesByParameter("FMTLAY", layoutColumnInfo.ColumnFormatType);

            var alignment = attribute.Find(p => p.Description.Equals("ALIGNMENT")).Value;
            var divide = attribute.Find(p => p.Description.Equals("DIVIDE")).Value;
            var mask = attribute.Find(p => p.Description.Equals("MASK")).Value;
            var multiply = attribute.Find(p => p.Description.Equals("MULTIPLY")).Value;
            var margin = attribute.Find(p => p.Description.Equals("MARGIN")).Value;
            var type = attribute.Find(p => p.Description.Equals("TYPE")).Value;

            var value = string.IsNullOrEmpty(columnValue) ? layoutColumnInfo.Visual : columnValue;

            value = await ConvertValue(value, type, mask, divide, multiply, layoutColumnInfo, columnName, generateReturnId);
            value = await AlignmValue(value, alignment, margin, layoutColumnInfo, columnName, generateReturnId);

            return value;
        }

        private async Task<string> AlignmValue(string value, string alignment, string margin, LayoutColumnInfo layoutColumnInfo, string columnName, int generateReturnId)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    margin = " ";

                if (!string.IsNullOrEmpty(alignment))
                {
                    if (string.IsNullOrEmpty(margin))
                        margin = " ";

                    switch (alignment)
                    {
                        case "L":
                            value = value.PadRight(layoutColumnInfo.Size, Convert.ToChar(margin));
                            break;

                        case "R":
                            value = value.PadLeft(layoutColumnInfo.Size, Convert.ToChar(margin));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(generateReturnId.ToString(), $"Não foi possível alinhar o valor {value} para a coluna {columnName}", ex.Message, LogType.Warning);
            }

            return value;
        }

        private async Task<string> ConvertValue(string value, string type, string mask, string divide, string multiply, LayoutColumnInfo layoutColumnInfo, string columnName, int generateReturnId)
        {
            string warning = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    switch (type)
                    {
                        case "DAT":
                            value = GetDateTime(value, mask, layoutColumnInfo, columnName);
                            break;

                        case "INT":
                            value = GetInteger(value, layoutColumnInfo, columnName);
                            break;

                        case "DEC":
                            value = GetDecimal(value, divide, multiply, layoutColumnInfo, columnName);
                            break;

                        case "TEX":
                            value = GetText(value, layoutColumnInfo, columnName);
                            break;
                    }
                }
                catch (WrongValueException ex)
                {
                    warning = ex.Message;
                }
            }
            else
                warning = $"Valor não informado para a Coluna: {columnName}";

            if (!string.IsNullOrEmpty(warning))
                await _logger.LogAsync(generateReturnId.ToString(), warning, string.Empty, LogType.Warning);

            return value;
        }

        #region Types

        string GetDateTime(string value, string mask, LayoutColumnInfo layoutColumnInfo, string columnName)
        {
            try
            {
                return Convert.ToDateTime(value).ToString(mask);
            }
            catch (Exception)
            {
                throw new WrongValueException("Esperado um valor do tipo: DATA.\n" + GetError(layoutColumnInfo, value, columnName));
            }
        }

        string GetInteger(string value, LayoutColumnInfo layoutColumnInfo, string columnName)
        {
            try
            {
                return long.Parse(value).ToString();
            }
            catch (Exception)
            {
                throw new WrongValueException("Esperado um valor do tipo: INTEIRO.\n" + GetError(layoutColumnInfo, value, columnName));
            }
        }

        string GetDecimal(string value, string divide, string multiply, LayoutColumnInfo layoutColumnInfo, string columnName)
        {
            try
            {
                value = Convert.ToDecimal(value).ToString();
                if (!string.IsNullOrEmpty(divide))
                    return (Convert.ToDecimal(value) / Convert.ToDecimal(divide)).ToString();
                if (!string.IsNullOrEmpty(multiply))
                    return (Convert.ToDecimal(value) * Convert.ToDecimal(multiply)).ToString();
                return value;
            }
            catch (Exception)
            {
                throw new WrongValueException("Esperado um valor do tipo: DECIMAL.\n" + GetError(layoutColumnInfo, value, columnName));
            }
        }

        string GetText(string value, LayoutColumnInfo layoutColumnInfo, string columnName)
        {
            try
            {
                return value.ToString();
            }
            catch (Exception)
            {
                throw new WrongValueException("Esperado um valor do tipo: STRING.\n" + GetError(layoutColumnInfo, value, columnName));
            }
        }

        #endregion

        #endregion

        #region SQL Command

        string GetSqlCommand(List<ColumnHeader> columns, JObject lstExportInfo, string command)
        {
            foreach (var column in columns)
            {
                var value = GetRowValueToString(lstExportInfo, column.Name);
                command = GetSqlCommand(column, value, command);
            }
            command = command.Replace("''", "'");
            return command;
        }

        string GetSqlCommand(ColumnHeader column, string value, string command)
        {
            var c = "'";
            if ((column.Name.Equals(_OBJECT_VALUE) || column.Name.Equals("IDOCOREN50")) && (value.Contains(",") || value.ToUpper().StartsWith("SELECT")))
                c = string.Empty;

            if (column.Name.Equals("DATINI") || column.Name.Equals("DATFIM"))
                return command.Replace($":{column.Name}", @"TO_DATE('" + value.Substring(0, 10) + @"', 'DD/MM/RRRR')");
            else
                return command.ToUpper().Replace($":{column.Name.ToUpper()}", c + value + c);
        }

        #endregion

        #region Communication Channels

        private async System.Threading.Tasks.Task SendToCommunicationChannels(List<FileLogInfo> lstArchiveLogInfo, string filePath, CustomerInfo customerInfo, List<CommunicationChannelInfo> communicationChannels, GenerateReturnInfo generateReturn)
        {
            int emailCount = generateReturn.CommunicationChannels.Where(x => x.SendingType == SendingTypeEnum.Email).Count();
            int ftpCount = generateReturn.CommunicationChannels.Where(x => x.SendingType == SendingTypeEnum.Ftp).Count();

            await _logger.LogAsync(generateReturn.Id.ToString(), $"Iniciando o envio de arquivos para os canais de comunicações", $"Quantidade E-mails: {emailCount}, servidores FTP/SFTP: {ftpCount}", LogType.Success);

            foreach (var archiveLogInfo in lstArchiveLogInfo)
            {
                await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Início do envio do arquivo {archiveLogInfo.Name}"), $"Nome do arquivo {archiveLogInfo.Name}", LogType.Debug);

                foreach (var communicationChannel in communicationChannels)
                    await SendToCommunicationChannel(communicationChannel, archiveLogInfo, filePath, generateReturn);

                await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Finalização de envio do arquivo {archiveLogInfo.Name}"), $"Nome do arquivo {archiveLogInfo.Name}", LogType.Debug);

                try
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Não foi possível excluir o arquivo {archiveLogInfo.Name}"), ex.Message, LogType.Debug);
                }
            }

            await _logger.LogAsync(generateReturn.Id.ToString(), $"Finalizado com sucesso o envio de arquivos", string.Empty, LogType.Success);
        }

        private async Task<bool> SendToCommunicationChannel(CommunicationChannelInfo communicationChannel, FileLogInfo archiveLogInfo, string filePath, GenerateReturnInfo generateReturn)
        {
            var success = true;

            switch (communicationChannel.SendingType)
            {
                case SendingTypeEnum.Ftp:

                    var ftp = communicationChannel.Ftp;
                    string senderFolder = GetSenderFolderDescription(ftp);

                    try
                    {
                        await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Enviando arquivo para o servidor FTP {ftp.Address}, diretório de destino {senderFolder}"), $"Nome do arquivo {archiveLogInfo.Name}", LogType.Success);

                        var ftpHelper = _ftpClientFactory.CreateNew(ftp.Address, ftp.SenderFolder, ftp.ReceiverFolder, ftp.User, ftp.Password, ftp.ActiveMode, ftp.EnableSSH);

                        ftpHelper.Update($"{filePath}/{archiveLogInfo.Name}");

                        await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Arquivo enviado com sucesso para o servidor FTP {ftp.Address}, diretório de destino {senderFolder}"), $"Nome do arquivo {archiveLogInfo.Name}", LogType.Success);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Erro ao enviar o arquivo para o servidor FTP {ftp.Address}, diretório de destino {senderFolder}"), ex.Message, LogType.Error);
                    }

                    break;
                case SendingTypeEnum.Email:

                    var email = communicationChannel.Email;
                    try
                    {
                        await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Enviando arquivo para o e-mail {email.Address}"), $"E-mail {email.Address}\n Nome do arquivo {archiveLogInfo.Name}", LogType.Success);

                        success = await SendToCommunicationChannelMail(email, filePath, archiveLogInfo.Name, generateReturn);

                        if(success)
                            await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Arquivo enviado com sucesso para o e-mail {email.Address}"), $"E-mail {email.Address}\n Nome do arquivo {archiveLogInfo.Name}", LogType.Success);
                        else
                            await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Falha no envio de arquivo para o {email.Address}"), $"Falha ao enviar o arquivo {archiveLogInfo.Name} para o {email.Address}", LogType.Success);

                    }
                    catch (Exception ex)
                    {
                        success = false;
                        await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Erro ao enviar o arquivo para o email {email.Address}"), ex.Message, LogType.Error);
                    }
                    break;
            }

            return success;
        }

        private string GetSenderFolderDescription(FtpInfo ftp)
        {
            return string.IsNullOrEmpty(ftp.SenderFolder) ? "/" : ftp.SenderFolder;
        }

        private byte[] GetFileContent(string filePath)
        {
            byte[] fileContent = null;

            using (var sourceStream = new StreamReader(filePath))
            {
                fileContent = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
            }

            return fileContent;
        }

        private async Task<bool> SendToCommunicationChannelMail(EmailInfo email, string filePath, string fileName, GenerateReturnInfo generateReturn)
        {
            var success = false;

            var header = new MailboxHeaderInfo();

            IncludeAttachment(header, fileName, Path.Combine(filePath, fileName));

            var sendingType = generateReturn.ExecutionType == ExecutionTypeEnum.Manual ? EmailSendTypeEnum.Manual : EmailSendTypeEnum.System;

            SetMailboxHeaderInformations(email.Title, header, fileName, sendingType);

            header.Recipients.Add(new MailboxRecipientInfo() { Recipient = email.Address });

            if (!_mailboxHeader.Save(header, null, enqueue: false))
                await _logger.LogAsync(generateReturn.Id.ToString(), FormatLogMessage($"Erro ao preparar o envio para o e-mail {email.Address}"), $"Erro ao preparar o envio do arquivo {fileName} para o e-mail {email.Address}", LogType.Error);
            else
                success = _mail.SendMail(header.Id);

            return success;
        }

        private void IncludeAttachment(MailboxHeaderInfo header, string fileName, string filePath)
        {
            var obj = new object[,]
            {
                { fileName, filePath, "EDI" }
            };

            var localFile = _ged.Move(obj);

            header.Attachments.Add(new MailboxAttachmentInfo(localFile));
        }

        void SetMailboxHeaderInformations(string summary, MailboxHeaderInfo header, string transportDocumentNumber, EmailSendTypeEnum sendingType)
        {
            if (string.IsNullOrEmpty(summary))
                summary = "EDI";

            header.Title = string.Format("{0} - {1}", summary, transportDocumentNumber);
            header.Message = transportDocumentNumber;

            header.Status = EmailStatusEnum.NotSent;
            header.SendType = sendingType;

            var parameterOriginEmail = Parameter.GetByParameter("ORIGEM EMAIL");
            if (parameterOriginEmail == null)
                throw new ArgumentNullException("Parâmetro ORIGEM EMAIL não cadastrado");

            var cteItem = parameterOriginEmail.Items["EDI"];
            if (cteItem == null)
                throw new ArgumentNullException("Parâmetro ORIGEM EMAIL - Item CTE não cadastrado");

            if (cteItem.Attributes == null || cteItem.Attributes.Count == 0)
                _parameter.GetParameterAttributeItems(parameterOriginEmail, cteItem);

            header.Origin = cteItem.Value;
            header.Priority = int.Parse(cteItem.Attributes["PRIORIDADE"].Value);

            var smtpInfo = GetSMTPByParameterCategory(cteItem.Attributes["CATEGORIA SMTP"]);
            header.SMTP = smtpInfo;
        }

        public SMTPInfo GetSMTPByParameterCategory(ParameterAttributeItemInfo attribute)
        {
            if (attribute.Value == null)
                return null;

            //Parâmetro deve estar separado por ponto e vírgula
            var lst = _smtp.GetByCategory(attribute.Value.ToString().ToUpper());
            if (lst == null || lst.Count == 0)
                return null;

            var random = new Random();
            var idPosition = random.Next(0, lst.Count - 1);

            return lst[idPosition];
        }

        #endregion

        #endregion

        #region Parameters

        public string GetBandMain(LayoutHeaderInfo layoutHeaderInfo)
        {

            for (int i = 0; i < layoutHeaderInfo.LayoutBands.Count; i++)
            {
                var parameterItemInfo = ParameterInfo.Items.ToList().Find(p => p.Value.Equals(layoutHeaderInfo.LayoutBands[i].BandTypeId));
                if (parameterItemInfo != null && !string.IsNullOrEmpty(parameterItemInfo.Attributes["MAIN"].Value) && parameterItemInfo.Attributes["MAIN"].Value.Equals("Y"))
                    return parameterItemInfo.Value.ToString();
            }

            return string.Empty;
        }

        #endregion

        #region Query Result

        public async Task<QueryResult<object>> GetBillOfLading(GenerateReturnDTO generateReturn)
        {
            string command = GetBillOfLadingCommand(generateReturn);

            var parameters = GetParameters(generateReturn.Customer, generateReturn.LayoutHeader.Script.Script);

            await _logger.LogAsync(generateReturn.Id.ToString(), $"Iniciando a busca de fretes para exportação", $"Comando: {command}", LogType.Success);

            var queryResult = _queryResultFactory.CreateNew(generateReturn.LayoutHeader.Script.Provider.ProviderType, generateReturn.LayoutHeader.Script.Provider.ConnectionString, command, parameters);

            QueryResult<object> result = await queryResult.GetQueryResult();

            if (!result.Success)
            {
                await _logger.LogAsync(generateReturn.Id.ToString(), $"Erro ao buscar de fretes vinculados ao layout {generateReturn.LayoutHeader.Description}", result.Message, LogType.Error);
                throw new BusinessException(result.Message);
            }
            else
            {
                if (result.Rows.Count > 0)
                    await _logger.LogAsync(generateReturn.Id.ToString(), $"Finalizado com sucesso a busca de fretes. Encontrado {GetTotalResult(result)} fretes", $"Comando: {command}", LogType.Success);
                else
                    await _logger.LogAsync(generateReturn.Id.ToString(), "Não foi encontrado fretes no período informado", string.Empty, LogType.Warning);
            }

            return result;
        }

        public int GetTotalResult(QueryResult<object> result)
        {
            int total = 0;

            foreach (var item in result.Rows)
            {
                var obj = (item as JObject);
                var value = GetRowValueToString(obj, _OBJECT_VALUE);

                if (!string.IsNullOrEmpty(value))
                    total += value.Split(',').Length;
            }

            return total;
        }

        public string GetBillOfLadingCommand(GenerateReturnDTO generateReturn)
        {
            string command = generateReturn.LayoutHeader.Script.Script;


            SetParameters(generateReturn.Customer, generateReturn.StartingDate, generateReturn.EndingDate, ref command);

            if (generateReturn.BillOfLadingId != 0)
                command = $"{command} AND CTR_IDENTI = {generateReturn.BillOfLadingId}";

            return command;
        }

        async Task<QueryResult<object>> GetDataReader(ProviderInfo provderInfo, string command)
        {
            var helper = _queryResultFactory.CreateNew(provderInfo.ProviderType, provderInfo.ConnectionString, command, new List<QueryParameter>());

            return await helper.GetQueryResult();
        }

        private QueryResult<object> PageQueryResult(QueryResult<object> result, int pageSize = 250)
        {
            var resultAux = new QueryResult<object>()
            {
                Columns = result.Columns,
                Rows = new List<object>()
            };

            var rows = result.Rows.ToArray();
            int count = rows.Count();

            for (int i = 0; i < count; i++)
            {
                var obj = (rows[i] as JObject);
                var val = obj.GetValue(_OBJECT_VALUE);

                if (val != null)
                {
                    var values = val.ToString();

                    var splitedObjectValues = values.Split(',');
                    var splitedCount = splitedObjectValues.Length / pageSize + 1;

                    for (var j = 0; j < splitedCount; j++)
                    {
                        var json = new StringBuilder("{");

                        var ctes = new StringBuilder();

                        for (var h = j * pageSize; h < (j * pageSize) + pageSize; h++)
                        {
                            if (h < splitedObjectValues.Length)
                            {
                                if (h == (j * pageSize))
                                    ctes.Append(splitedObjectValues[h].Trim());
                                else
                                    ctes.Append(", " + splitedObjectValues[h].Trim());
                            }
                        }

                        foreach (var item in result.Columns)
                        {
                            var property = item.Name == _OBJECT_VALUE ? ctes.ToString() : obj.GetValue(item.Name).ToString();

                            json.AppendLine(CreateJsonProprerty(item.Name, property));
                        }

                        json.AppendLine("}");

                        resultAux.Rows.Add(JsonConvert.DeserializeObject<object>(json.ToString()));
                    }
                }
            }

            return resultAux;
        }

        private string CreateJsonProprerty(string name, string value)
        {
            return string.Format("'{0}': '{1}',", name, value);
        }

        private string GetValueDescription(RowFile file, bool showPrefix)
        {
            string description = string.Empty;

            if (!string.IsNullOrEmpty(file.ReferenceValues))
                description = (showPrefix ? "da(s) " : string.Empty) + $"Nota(s): {file.ReferenceValues}";
            else if (!string.IsNullOrEmpty(file.KeyValues))
                description = (showPrefix ? "do(s) " : string.Empty) + $"CT-e(s): {file.KeyValues}";

            return description;
        }

        #region Parameters

        public List<QueryParameter> GetParameters(object obj, string script)
        {
            List<QueryParameter> lstParameters = new List<QueryParameter>();

            var lst = script.Split(':');

            if (lst.Length > 0)
            {
                for (int i = 1; i < lst.Length; i++)
                {
                    var parameter = new QueryParameter();
                    parameter.ParameterName = ":" + Regex.Replace(lst[i].Split(' ')[0], "[^a-zA-Z0-9_.]", "", RegexOptions.Compiled);

                    SetPropertyValue(obj, parameter);
                    lstParameters.Add(parameter);
                }
            }

            return lstParameters;
        }

        void SetPropertyValue(object obj, QueryParameter parameter)
        {
            var property = obj.GetType().GetProperties().FirstOrDefault(p => parameter.ParameterName == p.Name);

            if (property != null)
            {
                parameter.Value = property.GetValue(obj, null);
            }
        }

        void SetParameters(CustomerDTO customer, DateTime startingDate, DateTime endingDate, ref string command)
        {
            string initialDate = startingDate != null ? startingDate.ToString("dd/MM/yyyy") : string.Empty,
                lastDat = endingDate != null ? endingDate.ToString("dd/MM/yyyy") : string.Empty;

            command = command.Replace(":CNPJ", customer != null && !string.IsNullOrEmpty(customer.Person.TaxIdRegistration) ? "\'" + customer.Person.TaxIdRegistration + "\'" : string.Empty);
            command = command.Replace(":DATINI", @"TO_DATE('" + (!string.IsNullOrEmpty(initialDate) && initialDate.Length > 10 ? initialDate.Substring(0, 10) : initialDate) + @"', 'DD/MM/RRRR')");
            command = command.Replace(":DATFIM", lastDat != null ? @"TO_DATE('" + lastDat.Substring(0, 10) + @"', 'DD/MM/RRRR')" : "NULL");
        }

        #endregion

        #endregion
    }
}