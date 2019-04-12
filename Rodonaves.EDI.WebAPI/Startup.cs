using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rodonaves.Engine;
using RTEFramework.DAL.PostgreSQL.Infra;
using RTEFramework.Web.Security.Infra;
using RTEFramework.DAL.PostgreSQL.Repositories;
using Rodonaves.EDI.IoC;
using AutoMapper;
using Rodonaves.EDI.BLL.Infra.AutoMapperProfille;
using Rodonaves.QueueMessage.Interfaces;
using Rodonaves.EDI.Helpers;

namespace Rodonaves.EDI.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AccessTokenExpireHours = int.Parse(Configuration.GetSection("AccessTokenExpireHours").Value);
            Global.Configuration = configuration;
        }
        private int AccessTokenExpireHours;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRTEApi();
            services.AddMemoryCache();
            services.AddTransient<IRTEQueue, EDIQueueHelper>();
            services.RegisterServices();

            services.AddRTEAuth<CurrentUser>(typeof(PostgreSQLDatabaseParameter), typeof(LoginRepository));
            services.AddRTESwagger("API EDI", "v1", "Rodonaves.EDI.WebAPI.xml");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRTEAuth(AccessTokenExpireHours);
            app.UseRTEApi();
            //app.AddRTESwagger("API PDR", "v1");
        }
    }
}
