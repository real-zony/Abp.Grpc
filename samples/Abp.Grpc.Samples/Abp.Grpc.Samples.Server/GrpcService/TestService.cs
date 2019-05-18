using System;
using Abp.Dependency;
using Abp.Grpc.Common.Runtime.Session;
using Abp.Runtime.Session;
using MagicOnion;
using MagicOnion.Server;

namespace Abp.Grpc.Samples.Server.GrpcService
{
    public class TestService : ServiceBase<ITestService>,ITestService
    {
        private readonly IAbpSession _tmpAbpSession;

        public TestService(IAbpSession tmpAbpSession)
        {
            _tmpAbpSession = tmpAbpSession;
        }

        public UnaryResult<int> Sum(int x, int y)
        {
            return new UnaryResult<int>(x + y);
        }

        public UnaryResult<string> PrintCurrentUserId(GrpcSession session)
        {
            Console.WriteLine($"接收客户端传递 Session 值之前，服务端的用户 Id 值: {_tmpAbpSession.UserId}");
            string resultUserIdStr;

            using (_tmpAbpSession.Use(session.TenantId, session.UserId))
            {
                resultUserIdStr = (_tmpAbpSession.UserId ?? 0).ToString();
                Console.WriteLine($"临时变更的 AbpSession 值: {_tmpAbpSession.UserId}");
            }
            
            Console.WriteLine($"退出 using 语句块时，当前用户的 Id 值: {_tmpAbpSession.UserId}");
            
            return new UnaryResult<string>(resultUserIdStr);
        }
    }
}