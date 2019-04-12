using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Rodonaves.EDI.BLL.Interfaces;
using Rodonaves.EDI.DTO;
using Rodonaves.Engine;
using Tasks = System.Threading.Tasks;

namespace Rodonaves.EDI.BLL
{
    public class MessageBroker : IMessageBrokerManagement
    {
        #region Properties
        private string HostUrl;
        #endregion

        #region Methods
        public Tasks.Task<List<QueueDTO>> GetAllQueuesAsync () => Tasks.Task.Run (async () =>
        {
            using (var httpClient = new HttpClient ())
            {
                SetHostUrl ();
                SetBasicAuth (httpClient);
                var response = AllQueuesRequest(httpClient);

                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<QueueDTO>> (await response.Result.ReadAsStringAsync ());
                return obj.Where (x => x.Name.Contains ("EDI")).ToList ();
            }
        });

        private void SetBasicAuth (HttpClient httpClient) =>
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Basic", GetCredentialsParameters ());

        private string GetCredentialsParameters ()
        {
            string
            username = GetUserNAme (),
                password = GetPassword ();

            return Convert.ToBase64String (Encoding.ASCII.GetBytes ($"{username}:{password}"));
        }

        private void SetHostUrl () => this.HostUrl = Global.Configuration.GetSection ("RTEMessageBrokerManagement:hostname").Value;
        private string GetPassword () => Global.Configuration.GetSection ("RTEMessageBrokerManagement:username").Value;
        private string GetUserNAme () => Global.Configuration.GetSection ("RTEMessageBrokerManagement:password").Value;
        private async Tasks.Task<HttpContent> AllQueuesRequest (HttpClient httpClient)
        {
            var response = await httpClient.GetAsync ($"{HostUrl}:15672/api/queues");;
            return response.Content;
        }
        #endregion

    }

}