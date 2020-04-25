using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameVui.Server.Models
{
    public class Message
    {
        [Key, Column(Order=0)]
        public string SenderId { get; set; }
        [Key, Column(Order = 1)]
        public string ReceiverId { get; set; }
        [Key, Column(Order = 2)]
        public DateTime CreatedTime { get; set; }
        public string MessageContent { get; set; }
        [ForeignKey("SenderId")]
        public virtual ApplicationUser Sender {get;set;}
        [ForeignKey("ReceiverId")]
        public virtual ApplicationUser Receiver { get; set; }

    }

    public class GroupMessage
    {
        [Key, Column(Order = 0)]
        public string SenderId { get; set; }
        [Key, Column(Order = 1)]
        public DateTime CreatedTime { get; set; }
        public string MessageContent { get; set; }
        [ForeignKey("SenderId")]
        public virtual ApplicationUser Sender { get; set; }

    }
}