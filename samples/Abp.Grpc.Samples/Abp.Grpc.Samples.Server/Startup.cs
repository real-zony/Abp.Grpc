using System;
using Abp.AspNetCore;
using Abp.Grpc.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.Grpc.Samples.Server
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // 注入 MVC 组件
            services.AddMvc();
            // 注入 Abp 框架，并替换 DI 框架为 Castle
            return services.AddAbp<AbpGrpcSamplesServerModule>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // 启用 Abp 组件
            app.UseAbp();
            // 启用 MVC 组件
            app.UseMvc();
        }
    }
}