using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Genealoge.Startup))]
namespace Genealoge
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
