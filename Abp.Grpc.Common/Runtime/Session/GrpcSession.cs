using Abp.Runtime.Session;
using MessagePack;

namespace Abp.Grpc.Common.Runtime.Session
{
    /// <summary>
    /// Grpc 接口会话状态
    /// </summary>
    [MessagePackObject(true)]
    public class GrpcSession
    {
        public static implicit operator GrpcSession(AbpSessionBase session)
        {
            return new GrpcSession(session.UserId, session.TenantId);
        }

        /// <summary>
        /// Grpc 接口会话状态
        /// </summary>
        public GrpcSession(long? userId, int? tenantId)
        {
            UserId = userId;
            TenantId = tenantId;
        }

        /// <summary>
        /// 用户 ID
        /// </summary>
        public long? UserId { get; }

        /// <summary>
        /// 租户 ID
        /// </summary>
        public int? TenantId { get; }
    }
}