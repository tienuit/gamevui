using System.Web;
using System.Web.Routing;

[assembly: PreApplicationStartMethod(typeof(GameVui.RegisterHubs), "Start")]

namespace GameVui
{
    public static class RegisterHubs
    {
        public static void Start()
        {       
        }
    }
}
