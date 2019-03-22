using Autofac;
using Autofac.Integration.WebApi;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper.RedisCacheHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System.Data.Entity;
using System.Reflection;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API
{
    /// <summary>
    /// Ioc控制反转
    /// </summary>
    public static class Bootstrapper
    {
        /// <summary>
        /// 主方法
        /// </summary>
        /// <param name="configuration"></param>
        public static void Run(HttpConfiguration configuration)
        {
            SetAutofacWebAPIServices(configuration);
        }
        private static void SetAutofacWebAPIServices(HttpConfiguration configuration)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<WebConfigApplicationSettings>().As<IApplicationSettings>();

            #region DBContext
            builder.RegisterType<EntityContext>().As<DbContext>().InstancePerRequest();
            #endregion

            #region controllers

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            #endregion

            #region Service

            builder.RegisterType<CustomerCategoriesService>().As<ICustomerCategories>();

            var serviceAssembly = Assembly.Load("BiHuManBu.ExternalInterfaces.Services");

            builder.RegisterAssemblyTypes(serviceAssembly).Where(o => o.Name.Contains("Service")).AsImplementedInterfaces();

            #endregion

            #region reposities
            var repositoryAssembly = Assembly.Load("BiHuManBu.ExternalInterfaces.Repository");
            builder.RegisterAssemblyTypes(repositoryAssembly).Where(o => o.Name.Contains("Repository")).AsImplementedInterfaces();

            #endregion

            #region commmon

            builder.RegisterType<CacheHelper>().As<ICacheHelper>();
            builder.RegisterType<HashOperator>().As<IHashOperator>();
            #endregion

            var container = builder.Build();
            ApplicationSettingsFactory.InitializeApplicationSettingsFactory(container.Resolve<IApplicationSettings>());
            var resolver = new AutofacWebApiDependencyResolver(container);
            configuration.DependencyResolver = resolver;
        }
    }
}