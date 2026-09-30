using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace TokenAdministrationApi
{
    public static class Program
    {
#pragma warning disable ASPDEPR008
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
#pragma warning restore ASPDEPR008
    }
}
