using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Rouse.PatentCalculator.Web.Startup))]
namespace Rouse.PatentCalculator.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
