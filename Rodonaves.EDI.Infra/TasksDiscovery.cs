using Microsoft.Extensions.DependencyInjection;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.TaskExecutor.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rodonaves.EDI.IoC;
using Microsoft.Extensions.Configuration;
using Rodonaves.Engine;

namespace Rodonaves.EDI.Infra
{
    public class TasksDiscovery : ITasksDiscovery
    {
        private IServiceCollection _services;
        private ServiceProvider _provider;

        public TasksDiscovery()
        {
            _services = new ServiceCollection();
            _services.RegisterServices();
        }

        public IServiceCollection GetServiceCollection()
        {
            return _services;
        }

        public Task<List<TaskInfo>> LoadAsync()
        {
            _provider = _services.BuildServiceProvider();
            Global.Configuration = _provider.GetService<IConfiguration>();
            var task = _provider.GetService<ITask>();
            return task.LoadTasksAsync();
        }
    }
}
