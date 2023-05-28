using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using XiaoTasiBackend.Controllers;

namespace XiaoTasiBackend
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            // 第一步，建立ContainerBuilder
            var builder = new ContainerBuilder();

            // 註冊所有的Controller作為Service
            builder.RegisterControllers(typeof(HomeController).Assembly);

            // 開始 第二步，註冊service

            // 第三步，註冊都完成了，建立自己的container
            var container = builder.Build();

            // 用DI Container作為建立Controller時候的DI Resolver。
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //AutofacConfig.Initialize(GlobalConfiguration.Configuration);
        }
    }
}
