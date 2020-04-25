using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameVui.Server.Models;

namespace GameVui.Server.ViewModels
{
    public class OpponentInfo
    {
        public ApplicationUser Opponent { get; set; }
        public int Win { get; set; }
        public int Draw { get; set; }
        public int Lose { get; set; }

    }
}