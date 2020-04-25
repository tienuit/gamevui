using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using GameVui.Server.CaroGameModels;
using GameVui.Server.TimSoGameModels;

namespace GameVui.Server.Models
{
    public class Match
    {
        [Key]
        [Column(Order=0)]
        public string PlayerId1 { get; set; }
        [Key]
        [Column(Order = 1)]
        public string PlayerId2 { get; set; }
        [Key]
        [Column(Order = 2)]
        public DateTime GameBegin { get; set; }
        public DateTime? GameEnd { get; set; }
        public int Player1Achievement { get; set; }
        public int Player2Achievement { get; set; }
        public string WinnerId { get; set; }
        public string Note { get; set; }
        public int TotalNumber { get; set; }
        public bool Finished { get; set; }

        [ForeignKey("PlayerId1")]
        public virtual ApplicationUser Player1 { get; set; }
        [ForeignKey("PlayerId2")]
        public virtual ApplicationUser Player2 { get; set; }
        [ForeignKey("WinnerId")]
        public virtual ApplicationUser Winner { get; set; }


        public static Match FromGame(TimSoGame game)
        {
            Match match = new Match();
            match.PlayerId1 = game.Player1.UserId;
            match.PlayerId2 = game.Player2.UserId;
            match.Player1Achievement = 0;
            match.Player2Achievement = 0;
            match.GameBegin = game.CreatedTime;
            return match;
        }

        public static Match FromGame(CaroGame game)
        {
            Match match = new Match();
            match.PlayerId1 = game.Player1.UserId;
            match.PlayerId2 = game.Player2.UserId;
            match.Player1Achievement = 0;
            match.Player2Achievement = 0;
            match.GameBegin = game.CreatedTime;
            return match;
        }

    }
}