using Autofac;
using Autofac.Integration.WebApi;
using System.Reflection;
using System.Web;
using System.Web.Http;
using XiaoTasiBackend.Repository;
using XiaoTasiBackend.Repository.Impl;
using XiaoTasiBackend.Service;
using XiaoTasiBackend.Service.Impl;

namespace XiaoTasiBackend.App_Start
{
    public class AutofacConfig
    {
        public static IContainer Container;

        public static void Initialize(HttpConfiguration config)
        {
            Initialize(config, RegisterServices(new ContainerBuilder()));
        }

        public static void Initialize(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            //取得目前正在執行之程式碼的組件
            Assembly assembly = Assembly.GetExecutingAssembly();

            //註冊 API controller
            builder.RegisterApiControllers(assembly);

            //註冊 HttpContext
            builder.Register(c => new HttpContextWrapper(HttpContext.Current))
                .As<HttpContextBase>()
                .InstancePerRequest();

            // 註冊Service
            builder.RegisterType<TransportationServiceImpl>()
               .As<TransportationService>().InstancePerRequest();

            builder.RegisterType<ReservationServiceImpl>()
               .As<ReservationService>().InstancePerRequest();

            builder.RegisterType<SeatServiceImpl>()
               .As<SeatService>().InstancePerRequest();

            builder.RegisterType<TransportationRepoImpl>()
               .As<TransportationRepo>().InstancePerRequest();

            builder.RegisterType<MemberReservationRepoImpl>()
               .As<MemberReservationRepo>().InstancePerRequest();

            builder.RegisterType<ReservationRepoImpl>()
               .As<ReservationRepo>().InstancePerRequest();

            builder.RegisterType<SeatRepoImpl>()
               .As<SeatRepo>().InstancePerRequest();

            builder.RegisterType<SeatTravelMatchRepoImpl>()
               .As<SeatTravelMatchRepo>().InstancePerRequest();

            builder.RegisterType<TravelStepRepoImpl>()
               .As<TravelStepRepo>().InstancePerRequest();

            //建立 Container
            Container = builder.Build();

            return Container;
        }
    }
}
