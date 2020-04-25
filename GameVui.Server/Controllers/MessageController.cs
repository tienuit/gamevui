using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using GameVui.Server.Models;
using System.Linq;
using System.Collections.Generic;
namespace GameVui.Server.Controllers
{
    
    public class MessageController : Controller
    {
        [Authorize]
        // GET: Message
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public PartialViewResult Get10Last(string userId)
        {
            var context = new ApplicationDbContext();
            var myId = User.Identity.GetUserId();
            var messages = context.Messages.Where(m => (m.SenderId == myId && m.ReceiverId == userId) || (m.SenderId == userId && m.ReceiverId == myId)).OrderByDescending(m => m.CreatedTime).Take(10).ToList();
            messages.Reverse();
            ViewBag.MyId = myId;
            return PartialView("_Last10", messages);
        }
        public PartialViewResult Get20GroupLast()
        {
            var context = new ApplicationDbContext();
            var messages = context.GroupMessages.OrderByDescending(m => m.CreatedTime).Take(20).ToList();
            messages.Reverse();
            return PartialView("_Last20Group", messages);
        }
    }
}