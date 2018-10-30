using Abp.Grpc.Common.Runtime.Session;
using MagicOnion;

namespace Abp.Grpc.Samples.Client.GrpcService
{
    public interface ITestService : IService<ITestService>
    {
        /// <summary>
        /// 计算两数之和
        /// </summary>
        /// <param name="x">操作数1</param>
        /// <param name="y">操作数2</param>
        /// <returns>两个操作数相加之和</returns>
        UnaryResult<int> Sum(int x, int y);

        /// <summary>
        /// 打印调用者传递的用户 ID 数据，并返回其转换为字符串的结果
        /// </summary>
        /// <param name="session">AbpSession 的值</param>
        /// <returns>用户 ID</returns>
        UnaryResult<string> PrintCurrentUserId(GrpcSession session);
    }
}