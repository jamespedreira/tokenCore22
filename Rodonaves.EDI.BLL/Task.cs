using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DAL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.EDI.Enums;
using Rodonaves.EDI.Model;
using RTEFramework.BLL.Infra;
using Tasks = System.Threading.Tasks;
using TaskExecutorInfra = Rodonaves.TaskExecutor.Infra;

namespace Rodonaves.EDI.BLL
{
    public class Task : BaseCrudBll<ITaskDal, TaskDTO, TaskInfo>, ITask
    {
        #region Ctor
        private ITriggerDal _configurationDal;
        private IDetailedDayDal _detailedDayDal;
        private IDetailedMonthDal _detailedMonthDal;
        private IActionDal _actionDal;
        private IActionObjectDal _actionObjectDal;
        private IExport _export;
        private IAgreement _agreement;
        private IGenerateReturn _generateReturn;
        private ILayoutHeader _layoutHeader;

        public Task (ITaskDal dal,
            IMapper mapper,
            ITriggerDal configurationDal,
            IDetailedMonthDal detailedMonthDal,
            IDetailedDayDal detailedDayhDal,
            IActionDal actionDal,
            IActionObjectDal actionObjectDal,
            IExport export,
            IAgreement agreement,
            IGenerateReturn generateReturn,
            ILayoutHeader layoutHeader
        ) : base (dal, mapper)
        {
            this._configurationDal = configurationDal;
            this._detailedMonthDal = detailedMonthDal;
            this._detailedDayDal = detailedDayhDal;
            this._actionDal = actionDal;
            this._actionObjectDal = actionObjectDal;
            this._export = export;
            this._agreement = agreement;
            this._generateReturn = generateReturn;
            this._layoutHeader = layoutHeader;
        }

        #endregion

        #region Methods

        public async Task<List<TaskDTO>> GetAllAsync ()
        {
            var list = await _dal.GetAll ();

            return _mapper.Map<List<TaskInfo>, List<TaskDTO>> (list);
        }

        public async Task<List<TaskExecutorInfra.TaskInfo>> LoadTasksAsync()
        {
            var all = await GetByParamsAsync(0, null, null, 1, int.MaxValue);
            var allInfo = all.Select(x => new TaskInfo { Id = x.Id }).ToList();
            await SetConfigurationDetailed(allInfo);
            var list = new List<TaskExecutorInfra.TaskInfo>();

            foreach (var item in all)
            {
                var taskInfo = new TaskExecutorInfra.TaskInfo()
                {
                    Description = item.Description,
                    Id = item.Id,
                    Name = item.Name,
                    Actions = new List<TaskExecutorInfra.ActionInfo>(),
                    Triggers = allInfo.Where(x => x.Id == item.Id).First().Triggers.Select(x => new TaskExecutorInfra.TriggerInfo
                    {
                        BeginDate = x.BeginDate,
                        BeginTime = x.BeginTime,
                        DaysOfMonth = x.DaysOfMonth,
                        Enable = x.Enable,
                        ExpireDate = x.ExpireDate,
                        ExpireTime = x.ExpireTime,
                        Frequence = (TaskExecutorInfra.FrequenceType)((int)x.Frequency),
                        Friday = x.Friday,
                        Id = x.Id,
                        Interval = x.Interval,
                        Monday = x.Monday,
                        Months = x.Months != null ? x.Months.Select(y => (TaskExecutorInfra.Month)((int)y)).ToList() : new List<TaskExecutorInfra.Month>(),
                        Saturday = x.Saturday,
                        Sunday = x.Sunday,
                        Thursday = x.Thursday,
                        Tuesday = x.Tuesday,
                        Wednesday = x.Wednesday
                    }).ToList()
                };
                foreach (var action in item.Actions)
                {
                    var taskActionInfo = new TaskExecutorInfra.ActionInfo
                    {
                        Description = action.Description,
                        Id = action.Id,
                        Objects = new List<TaskExecutorInfra.ActionObjectInfo>()
                    };
                    taskInfo.Actions.Add(taskActionInfo);

                    var actionInfo = new ActionInfo() { Id = action.Id };
                    await GetObjects(actionInfo);

                    foreach (var obj in actionInfo.Objects)
                    {
                        taskActionInfo.Objects.Add(new TaskExecutorInfra.ActionObjectInfo
                        {
                            Arguments = obj.Arguments,
                            Id = obj.Id,
                            Action = new EnqueueToExport(_agreement, _generateReturn, _layoutHeader)
                        });
                    }
                }

                list.Add(taskInfo);
            }
            return list;
        }

        public new async Task<int> InsertAsync (TaskDTO dto)
        {

            bool success = await base.InsertAsync (dto);

            if (!success)
                throw new BusinessException ("Não foi possível inserir uma nova tarefa");

            await InsertConfigurations (_mapper.Map<List<TriggerDTO>, List<TriggerInfo>> (dto.Triggers), dto.Id);
            await InsertActions (_mapper.Map<List<ActionDTO>, List<ActionInfo>> (dto.Actions), dto.Id);

            return dto.Id;

        }

        public override async Task<bool> UpdateAsync (TaskDTO dto)
        {
            bool success = await base.UpdateAsync (dto);

            if (!success)
                throw new BusinessException ("Não foi possível editar a tarefa");

            await DeleteConfigurations (dto.Id);
            await InsertConfigurations (_mapper.Map<List<TriggerDTO>, List<TriggerInfo>> (dto.Triggers), dto.Id);

            await DeleteActions (dto.Id);
            await InsertActions (_mapper.Map<List<ActionDTO>, List<ActionInfo>> (dto.Actions), dto.Id);

            return success;
        }

        public override async Task<bool> DeleteAsync (int id)
        {
            await DeleteConfigurations (id);
            await DeleteActions (id);

            bool success = false;
            success = await base.DeleteAsync (id);

            if (!success)
                throw new BusinessException ("Não foi possível deletar a tarefa");

            return success;
        }

        private async Task<bool> DeleteDetailAsync (TriggerInfo config) =>
            await _detailedMonthDal.DeleteByConfigurationId (config.Id) &&
            await _detailedDayDal.DeleteByConfigurationId (config.Id);

        public async Task<List<TaskDTO>> GetByParamsAsync (int id, string name, string descripton, int page, int amountByPage)
        {
            var tasks = await _dal.GetByParamsAsync (id, name, descripton, page, amountByPage);
            await SetConfigurationDetailed (tasks);

            foreach (var task in tasks)
            {
                await GetActions (task);
            }

            var TaskList = _mapper.Map<List<TaskInfo>, List<TaskDTO>> (tasks);

            TaskList[0].Triggers.ForEach ((x) =>
            {
                if (x.ExpireDate == DateTime.MinValue)
                    x.ExpireDate = null;
            });

            return TaskList;
        }
        private async Tasks.Task SetConfigurationDetailed (List<TaskInfo> list)
        {
            foreach (var item in list)
            {
                item.Triggers = await _configurationDal.GetByTaskIdAsync (item.Id);
                foreach (var config in item.Triggers)
                {
                    if (config.Frequency == FrequencyType.Monthly)
                    {
                        await SetMonthlyDetail (config);
                    }
                }
            }
        }

        private async Tasks.Task SetMonthlyDetail (TriggerInfo config)
        {
            var months = await _detailedMonthDal.GetMonthsByConfigurationIdAsync (config.Id);
            config.Months = months.Select (x => (Month) x).ToList ();

            var days = await _detailedDayDal.GetDaysByConfigurationIdAsync (config.Id);
            config.DaysOfMonth = days;
        }

        #region Actions
        private async Tasks.Task GetActions (TaskInfo task)
        {
            var actions = await _actionDal.GetByTaskIdAsync (task.Id);
            foreach (var action in actions)
            {
                await GetObjects (action);
            }

            task.Actions = actions;
        }

        private async Tasks.Task GetObjects (ActionInfo action) =>
            action.Objects = await _actionObjectDal.GetByActionIdAsync (action.Id);

        private async Tasks.Task InsertActions (List<ActionInfo> actions, int id)
        {
            foreach (var action in actions)
            {
                action.Id = _actionDal.GetNextId ();
                action.Task = new TaskInfo { Id = id };
                bool success = await _actionDal.InsertAsync (action);

                var actionObjects = action.Objects
                    .Select (obj =>
                        new ActionObjectInfo
                        {
                            ActionId = action.Id, ObjectId = obj.ObjectId, Arguments = obj.Arguments
                        }).ToList ();

                actionObjects.ForEach (async obj =>
                {
                    obj.Id = _actionObjectDal.GetNextId();
                    if (!await InsertActionObjetctAsync (obj))
                        throw new BusinessException ("Não foi possível inserir uma nova tarefa, ação inválida.");
                });

                if (!success)
                    throw new BusinessException ("Não foi possível inserir uma nova tarefa, ação inválida.");
            }
        }

        private async Tasks.Task DeleteActions (int taskId)
        {
            var actions = await _actionDal.GetByTaskIdAsync (taskId);
            foreach (var action in actions)
            {
                if (action.Id <= 0)
                    continue;

                bool success = await DeleteActionObjectsAsync (action.Id);
                success = await _actionDal.DeleteAsync (action);
            }
        }

        private async Task<bool> DeleteActionObjectsAsync (int actionId) => await _actionObjectDal.DeleteByActionAsync (actionId);
        private async Task<bool> InsertActionObjetctAsync (ActionObjectInfo actionObject) =>
            await _actionObjectDal.InsertAsync (actionObject);

        #endregion

        #region Configuration
        private async Tasks.Task DeleteConfigurations (int taskId)
        {
            var configurations = await _configurationDal.GetByTaskIdAsync (taskId);
            foreach (var config in configurations)
            {
                if (config.Id <= 0)
                    continue;

                if (config.Frequency == FrequencyType.Monthly)
                {
                    bool deletedDetail = await DeleteDetailAsync (config);
                    if (!deletedDetail)
                        throw new BusinessException ();
                }

                bool successInsertItem = await _configurationDal.DeleteAsync (config);
                if (!successInsertItem)
                    throw new BusinessException ();
            }
        }

        private async Tasks.Task InsertConfigurations (List<TriggerInfo> configuration, int taskId)
        {
            foreach (var config in configuration)
            {
                config.Task.Id = taskId;
                config.Id = _configurationDal.GetNextId ();
                bool successInsertItem = await _configurationDal.InsertAsync (config);
                if (!successInsertItem)
                    throw new BusinessException ("Não foi possível inserir ua nova tarefa");

                if (config.Frequency == FrequencyType.Monthly)
                {
                    foreach (var item in config.Months)
                    {
                        await _detailedMonthDal.InsertAsync (new DetailedMonthInfo
                        {
                            ConfigurationId = config.Id,
                                Month = (int) item
                        });
                    }

                    foreach (var item in config.DaysOfMonth)
                    {
                        await _detailedDayDal.InsertAsync (new DetailedDayInfo
                        {
                            ConfigurationId = config.Id,
                                Day = item
                        });
                    }
                }
            }
        }
        #endregion

        #endregion
    }
}