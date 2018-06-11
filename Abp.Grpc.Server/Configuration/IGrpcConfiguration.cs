namespace Abp.Grpc.Server.Configuration
{
    /// <summary>
    /// Grpc 配置
    /// </summary>
    public interface IGrpcConfiguration
    {
        /// <summary>
        /// Grpc 服务端绑定的监听地址
        /// </summary>
        string GrpcBindAddress { get; set; }

        /// <summary>
        /// Grpc 服务端绑定的监听端口
        /// </summary>
        int GrpcBindPort { get; set; }
    }
}
