using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameVui.Server.Models;

namespace GameVui.Server.ViewModels
{
    public class UserMatch
    {
        public ApplicationUser Opponent { get; set; }
        public int MatchResult { get; set; } //0: draw, 1: win, 2: lose
    }
}