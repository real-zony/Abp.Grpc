using Abp.Configuration.Startup;
using Abp.Grpc.Server.Configuration;
using System;

namespace Abp.Grpc.Server.Extensions
{
    /// <summary>
    /// Grpc 配置相关的扩展方法
    /// </summary>
    public static class GrpcServerConfigurationExtensions
    {
        /// <summary>
        /// 启用 Grpc 服务
        /// </summary>
        /// <param name="configs">Abp 配置模块</param>
        /// <param name="optionAction">配置操作</param>
        public static IGrpcServerConfiguration UseGrpcService(this IModuleConfigurations configs, Action<IGrpcConfiguration> optionAction)
        {
            var config = configs.AbpConfiguration.Get<IGrpcServerConfiguration>();
            optionAction(config);
            return config;
        }

        /// <summary>
        /// 启用 Consul 服务注册
        /// </summary>
        /// <param name="config">Grpc 配置项</param>
        /// <param name="optionAction">配置操作</param>
        public static void UseConsul(this IGrpcConfiguration config, Action<IConsulConfiguration> optionAction)
        {
            if (config is IConsulConfiguration consulConfig)
            {
                consulConfig.IsEnableConsul = true;
                optionAction(consulConfig);
            }
        }
    }
}