using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(GameVui.Server.Startup))]

namespace GameVui.Server
{
    public partial class Startup
    {
        
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            
            ConfigureAuth(app);
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new MyIdProvider());
            app.MapSignalR();
            
        }
    }
}
