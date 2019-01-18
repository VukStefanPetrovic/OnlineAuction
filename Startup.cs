using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineAuctions.Startup))]
namespace OnlineAuctions
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
