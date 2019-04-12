using Newtonsoft.Json;
using Rodonaves.EDI.Helpers.DTO;
using Rodonaves.EDI.Helpers.Interfaces;
using Rodonaves.Engine;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Rodonaves.EDI.Helpers
{
    public class ValidateClientAuthentication : IValidateClientAuthentication
    {
        TokenResult tokenResult = new TokenResult();

        public HttpClient CreateClient(string Route)
        {
            var client = new HttpClient 
            {
                BaseAddress = new Uri(Global.Configuration.GetSection("RouteServer").Value + Global.Configuration.GetSection(Route).Value)
            };

            client.DefaultRequestHeaders.Clear();

            if (tokenResult.Access_token != null)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Access_token);
            else
            {
                var UserServer = Global.Configuration.GetSection("UserServer").Value;
                var PassWordServer = Global.Configuration.GetSection("PasswordServer").Value;

                using (var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string> ("grant_type", "password"),
                    new KeyValuePair<string, string> ("username", UserServer),
                    new KeyValuePair<string, string> ("password", PassWordServer)
                }))
                {
                    //Gera o token para Response
                    var httpResponse = client.PostAsync(Global.Configuration.GetSection("RouteServer").Value + "/token", content).Result;
                    tokenResult = JsonConvert.DeserializeObject<TokenResult>(httpResponse.Content.ReadAsStringAsync().Result);
                    //Devolve o Token Para Atualização do Client
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Access_token);
                }
            }
            //
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //
            return client;
        }
    }

}
