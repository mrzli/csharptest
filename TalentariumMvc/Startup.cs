using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TalentariumMvc.Startup))]
namespace TalentariumMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
