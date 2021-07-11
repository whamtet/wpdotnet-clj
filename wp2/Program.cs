using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using clojure.lang;

namespace PeachPied.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(typeof(Program).Assembly.Location));

            //
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:5004/")
                .Build();

            IFn load = clojure.clr.api.Clojure.var("clojure.core", "load-string");
            load.invoke("(clojure.core.server/start-server {:port 5555 :name \"repl\" :accept 'clojure.core.server/repl})" +
                        "(println \"serving!\")");

            host.Run();
        }
    }

    class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddResponseCompression();
            services.AddWordPress(options =>
            {
                // options.SiteUrl =
                // options.HomeUrl = "http://localhost:5004";

                // options.PluginContainer.Add(new DashboardPlugin());
            });
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env, IConfiguration configuration)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWordPress();

            app.UseDefaultFiles();
        }
    }
}