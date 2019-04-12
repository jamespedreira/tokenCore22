using Rodonaves.EDI.Service.Export.Interfaces;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Service.Export
{
    public partial class Service : ServiceBase
    {
        Thread thread;

        private readonly IManageQueue _manage;

        public Service(IManageQueue manage)
        {
            _manage = manage;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            thread = new Thread(async ()=> {
                await Execute();
            });

            thread.Start();
        }

        protected override void OnStop()
        {
            _manage.Dispose();
            thread.Abort();
        }

        private async Task Execute()
        {
            await _manage.Execute();
        }
    }
}
