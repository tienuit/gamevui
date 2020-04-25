using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameVui.Server.Models;
using GameVui.Server.ViewModels;

namespace GameVui.Server.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();
            var orderedUsers = context.Users.OrderByDescending(u => u.Point).Take(10).ToList().Select(u => new UserStatistics(u, false)).ToList();
            var best100 = context.Matches.Where(m => m.GameEnd.HasValue && m.TotalNumber == 100 && m.Finished).OrderBy(m =>DbFunctions.DiffSeconds(m.GameEnd.Value, m.GameBegin)).FirstOrDefault();
            var best50 = context.Matches.Where(m => m.GameEnd.HasValue && m.TotalNumber == 50 && m.Finished).OrderBy(m =>DbFunctions.DiffSeconds(m.GameEnd.Value, m.GameBegin)).FirstOrDefault();
            var chatClients = context.Users.Select(u => new ChatModels.ChatClient() { UserId = u.Id, UserName = u.UserName, DisplayName = u.DisplayName, Avatar = u.Avatar}).ToList();
            context.SaveChanges();
            HomeView homeView = new HomeView
            {
                OrderedUsers = orderedUsers,
                Best100 = best100,
                Best50 = best50,
                ChatClients = chatClients
            };
            return View(homeView);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [Authorize]
        public ActionResult TimSo()
        {
            return View();
        }
        [Authorize]
        public ActionResult Caro()
        {
            return View();
        }
        public ActionResult Ranking()
        {
            return View();
        }
    }
}