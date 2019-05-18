using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Abp.Grpc.Samples.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(op => op.ListenAnyIP(5000))
                .UseStartup<Startup>();
    }
}