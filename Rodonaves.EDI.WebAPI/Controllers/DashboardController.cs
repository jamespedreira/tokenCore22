using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.WebAPI.Reponses;
using Rodonaves.EDI.WebAPI.Requests;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        #region Properties

        const string _controllerName = "Dashboard";

        private readonly IGenerateReturnDetailedStatus _detailedStatus;
        private IMemoryCache _memoryCache;
        #endregion

        #region contructor

        public DashboardController(IGenerateReturnDetailedStatus detailedStatus, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _detailedStatus = detailedStatus;
        }

        #endregion

        #region Methods

        /// <summary>Retorna Processos</summary>
        /// <remarks>Retorna os tipos de processos que se pode selecionar</remarks>
        /// <returns>Tipos de processos</returns>
        [HttpGet("processType")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public IActionResult GetProcesses() =>
            Ok(EDIEnumsList.GetProcessType());

        /// <summary>Retorna lista dos processos EDI</summary>
        /// <remarks>Retorna a lista de objetos que representam os processo em andamento e ja concluidos</remarks>
        /// <returns>Lista de processos EDI</returns>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetAsync([FromQuery] DetailedStatusRequest request)
        {
            ProcessTypeEnum processType = ProcessTypeEnum.None;
            if (string.IsNullOrEmpty(request.process) == false)
            {
                if (EDIEnumsList.GetProcessType().Find(p => p.Description.Contains(request.process)) != null)
                    processType = EDIEnumsList.GetProcessType().Find(p => p.Description.Contains(request.process)).Enum;
            }

            var res = await _detailedStatus.GetByParamsAsync(
                new DTO.GenerateReturnDetailedStatusDTO
                {
                    Customer = request.customer,
                    ProcessType = processType,
                    Key = request.key
                },
                request.page.GetValueOrDefault(),
                request.amountByPage.GetValueOrDefault()
            );
            return Ok(res);
        }

        /// <summary>Retorna detalhes do processamento</summary>
        /// <remarks>Retorna detalhes do processamento da geração do retorno</remarks>
        /// <returns>Objeto com as informações do detalhes do processamento</returns>
        [HttpGet("logs")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetLogsAsync([FromQuery] long generateReturnId) =>
            Ok(await _detailedStatus.GetLogByGenerateReturnIdAsync(generateReturnId));

        /// <summary>Retorna Processos resumidos para o gráfico</summary>
        /// <remarks>Retorna os processos agrupados por tipo e por status de progresso</remarks>
        /// <returns>Objeto contendo Total, Tipo e Progresso</returns>
        [HttpGet("resume")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetResumeAsync() =>
            Ok(
                (await _detailedStatus.GetResumeByProcessTypeAsync())
                .Select(
                    result => new GenerateReturnResumeStatusResponse
                    {
                        ProcessType = (int)result.ProcessType,
                        ProgressStatus = (int)result.ProgressStatus,
                        TotalItems = result.TotalItems
                    }
                ).ToList()
            );

        /// <summary>Retorna número de fretes por status</summary>
        /// <remarks>Retorna número de fretes por statuso</remarks>
        /// <returns>Objeto contendo Total, Tipo e status</returns>
        [HttpGet("operationresume")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetTotalOperationAsync()
        {
            var cacheKey = "operationList";
            if (_memoryCache.TryGetValue(cacheKey, out List<DTO.OperationDetailedDTO> result))
            {
                return Ok(result);
            }

            result = await _detailedStatus.GetTotalOperationByStatusAsync();
            var options = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(10)
            };

            _memoryCache.Set(cacheKey, result, options);
            return Ok(result);
        }


        /// <summary>Retorna status atual da fila do EDI</summary>
        /// <remarks>Retorna status atual da fila do EDI</remarks>
        /// <returns>Objeto contendo Total de item em processamento, não processados e concluídos</returns>
        [HttpGet("queues")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetAllQueues()
        {
            var res = await _detailedStatus.GetQueueStatus();
            var status = new QueueStatusResponse
            {
                Finished = res.Select(x => int.Parse(x.MessagesReady)).ToArray().Sum(),
                Started = res.Select(x => int.Parse(x.Unacknowledged)).ToArray().Sum()
            };

            status.Unprocessed =
                res.Select(x => int.Parse(x.Messages)).ToArray().Sum() - (status.Finished + status.Started);

            return Ok(status);
        }

        [HttpGet("queuesDetailed")]
        [SwaggerOperation(Tags = new[] { _controllerName })]
        public async Task<IActionResult> GetAllQueuesByCustomer() => Ok(await _detailedStatus.GetQueueStatus());

        #endregion

    }
}