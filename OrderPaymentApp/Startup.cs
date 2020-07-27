using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentApp.Model.Service;

namespace OrderPaymentApp
{
    public class Startup
    {
        /// <summary>
        /// Конфигурация приложения.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Конструктор конфигурации.
        /// </summary>
        /// <param name="configuration">Конфигурация приложения.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Конфигурация сервисов.
        /// </summary>
        /// <param name="services">Список сервисов.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IOrderService>(s => new OrderService(Configuration.GetConnectionString("OrdersDB"), new EmailNotificationService()));
            services.AddSingleton<INotificationService>(s => new EmailNotificationService());
        }

        /// <summary>
        /// Конфигурация приложения.
        /// </summary>
        /// <param name="app">Кофниг приложения</param>
        /// <param name="env">Конфиг нфраструктуры</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "DefaultApi",
                    template: "api/{controller}/{action}");

                routes.MapRoute(
                  name: "Default",
                  template: "{controller=Order}/{action=Index}/{id?}");
            }); 
        }
    }
}
