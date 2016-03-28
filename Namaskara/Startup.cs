using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Namaskara.Startup))]
namespace Namaskara
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
