using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameVui.Server.Models;

namespace GameVui.Server.ViewModels
{
    public class HomeView
    {
        public List<UserStatistics> OrderedUsers { get; set; }
        public Match Best100 { get; set; }
        public Match Best50 { get; set; }
        public List<ChatModels.ChatClient> ChatClients { get; set; }

    }
}