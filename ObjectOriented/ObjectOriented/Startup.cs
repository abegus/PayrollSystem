using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ObjectOriented.Startup))]
namespace ObjectOriented
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
