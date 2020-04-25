using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameVui.Server.Models;

namespace GameVui.Server.ChatModels
{
    public class ChatClient
    {
        public string DisplayName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public List<string> ConnectionIds { get; set; }
        public ChatClient()
        { }
        public ChatClient(ApplicationUser user)
        {
            DisplayName = user.DisplayName;
            UserId = user.Id;
        }
    }
}