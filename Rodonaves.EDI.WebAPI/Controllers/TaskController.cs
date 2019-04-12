using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.WebAPI.Requests;
using RTEFramework.BLL.Infra;
using RTEFramework.Web.Security.Infra;
using Swashbuckle.AspNetCore.Annotations;

namespace Rodonaves.EDI.WebAPI.Controllers
{
    /// <summary>API de Tarefas</summary>
    [RTEAuthorize]
    [ApiController]
    [Route ("api/tasks")]
    /// <summary></summary>
    public class TaskController : ControllerBase
    {
        const string _controllerName = "Tarefas";
        private readonly ITask _task;

        /// <summary>Construtor do ApiController de Tarefas</summary>
        public TaskController (ITask task) => this._task = task;

        /// <summary>Provedores</summary>
        /// <remarks>Retorna todos os provedores</remarks>
        /// <returns>Lista de Provedores</returns>
        [HttpGet]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> GetAll ()
        {
            try
            {
                var list = await _task.GetAllAsync ();

                return Ok (list);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        /// <summary>Provedores</summary>
        /// <remarks>Retorna o provedores por parâmetros</remarks>
        /// <returns>Provedor</returns>
        [HttpGet]
        [Route ("search")]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> GetByParams ([FromQuery] TaskRequest request)
        {
            try
            {
                var list = await _task.GetByParamsAsync (
                    request.id.GetValueOrDefault (),
                    request.name,
                    request.description,
                    request.page.GetValueOrDefault (),
                    request.amountByPage.GetValueOrDefault ()
                );

                return Ok (list);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        /// <summary>Cadastro de Tarefas</summary>
        /// <remarks>Insere uma nova tarefa</remarks>
        /// <param name="dto">Objeto que representa uma tarefa</param>
        /// <returns>Retorna se a inserção foi realizada com sucesso</returns>
        [HttpPost]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> Insert ([FromBody] TaskDTO dto)
        {
            try
            {
                int newId = 0;
                if (ModelState.IsValid)
                    newId = await _task.InsertAsync (dto);

                return Ok (newId);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        /// <summary>Atualização de Tarefas</summary>
        /// <remarks>Atualiza um tarefa cadastrada</remarks>
        /// <param name="dto">Objeto que representa uma tarefa</param>
        /// <returns>Retorna se a atualização foi realizada com sucesso</returns>
        [HttpPut]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> Update ([FromBody] TaskDTO dto)
        {
            try
            {
                bool success = false;
                if (ModelState.IsValid)
                    success = await _task.UpdateAsync (dto);

                return Ok (dto.Id);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        /// <summary>Remoção de Tarefa</summary>
        /// <remarks>Remove uma tarefa cadastrada</remarks>
        /// <param name="id">Identificador único da tarefa</param>
        /// <returns>Retorna se a atualização foi realizada com sucesso</returns>
        [HttpDelete]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> Delete ([FromQuery][Required] int id)
        {
            try
            {
                bool success = await _task.DeleteAsync (id);

                return Ok (success);
            }
            catch (BusinessException ex)
            {
                return BadRequest (ex.Message);
            }
        }

        [HttpGet("actions")]
        [SwaggerOperation (Tags = new [] { _controllerName })]
        public async Task<IActionResult> GetProcessesAsync ()
        {
            return Ok ( await Task.Run(() => EDIEnumsList.GetActionType ()));
        }

        [HttpGet("a")]
        [AllowAnonymous]
        public async Task<IActionResult> A()
        {
            var a = _task.LoadTasksAsync();
            return Ok();
        }
    }
}