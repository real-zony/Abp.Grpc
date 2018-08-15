namespace Abp.Grpc.Client.Configuration
{
    public class GrpcDebugConfiguration
    {
        /// <summary>
        /// 调试模式时，指定的远程 Grpc 提供服务器 IP
        /// </summary>
        public string DebugGrpcServerIp { get; set; }

        /// <summary>
        /// 调试模式时，指定的远程 Grpc 提供服务器端口
        /// </summary>
        public int DebugGrpcServerPort { get; set; }
    }
}