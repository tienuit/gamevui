using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameVui.Server.Models;

namespace GameVui.Server.ViewModels
{
    public class UserStatistics
    {
        public UserStatistics(string userId, bool getOpponentInfo)
        {
            var context = new ApplicationDbContext();
            var user = context.Users.Find(userId);
            Init(user, getOpponentInfo);
        }

        public UserStatistics(ApplicationUser user, bool getOpponentInfo)
        {
            Init(user, getOpponentInfo);
        }

        private void Init(ApplicationUser user, bool getOpponentInfo)
        {
            
            DisplayName = user.DisplayName;
            
            UserName = user.UserName;
            TotalMatch = user.HostMatches.Count + user.GuestMatches.Count;
            TotalWin = user.WinMatches.Count;
            TotalLose = user.HostMatches.Count(m => !string.IsNullOrEmpty(m.WinnerId) && m.WinnerId != user.Id)
                + user.GuestMatches.Count(m => !string.IsNullOrEmpty(m.WinnerId) && m.WinnerId != user.Id);
            Point = user.Point;
            TotalDraw = user.HostMatches.Count(m => string.IsNullOrEmpty(m.WinnerId))
                + user.GuestMatches.Count(m => string.IsNullOrEmpty(m.WinnerId));

            if (getOpponentInfo)
            {
                var userMatchList1 = user.HostMatches.Select(m => new UserMatch()
                {
                    Opponent = m.Player2,
                    MatchResult = string.IsNullOrEmpty(m.WinnerId) ? 0 : m.WinnerId == m.PlayerId1 ? 1 : 2
                });
                var userMatchList2 = user.GuestMatches.Select(m => new UserMatch()
                {
                    Opponent = m.Player1,
                    MatchResult = string.IsNullOrEmpty(m.WinnerId) ? 0 : m.WinnerId == m.PlayerId2 ? 1 : 2
                });
                OpponentInfoList = userMatchList1.Concat(userMatchList2).GroupBy(um => um.Opponent).Select(gr =>
                    new OpponentInfo
                    {
                        Opponent = gr.Key,
                        Win = gr.Count(m => m.MatchResult == 1),
                        Lose = gr.Count(m => m.MatchResult == 2),
                        Draw = gr.Count(m => m.MatchResult == 0),
                    }).ToList();
            }
        }
        public string DisplayName { get; set; }
        public int Point { get; set; }
        public string UserName { get; set; }
        public int TotalMatch { get; set; }
        public int TotalWin { get; set; }
        public int TotalLose { get; set; }
        public int TotalDraw { get; set; }
         public List<OpponentInfo> OpponentInfoList { get; set; }
        
    }
}