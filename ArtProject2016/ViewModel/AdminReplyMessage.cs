using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtProject2016.Models;

namespace ArtProject2016.ViewModel
{
    public class AdminReplyMessage
    {
        public virtual Message Message { get; set; }
        public virtual MessageReply MessageReply { get; set; }
    }
}