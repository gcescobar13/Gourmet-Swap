using GSwap.Services;
using QnABot.Models;
using QnABot.Models.MealPositions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Unity;
using Unity.AspNet.WebApi;

namespace QnABot.App_Start
{
    public static class UnityConfig
    {

        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();


            //this should be per request

            container.RegisterType<IGetMeals, GetMeals>();
            container.RegisterType<ISiteConfigService, SiteConfigService>();
            container.RegisterType<IExternalService, ExternalService>();


            //9:01 pm using System.Web.Http;
            //9:02 pm using Unity.AspNet.WebApi;
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

            

        }

    }
}