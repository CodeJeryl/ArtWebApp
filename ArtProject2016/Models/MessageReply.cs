using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ArtProject2016.Models
{
    public class MessageReply
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string BodyReply { get; set; }
        public DateTime DateTime { get; set; }
       
        public int AdminAccountId { get; set; }
        [ForeignKey("AdminAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        public int MessageId { get; set; }
        public virtual Message Message { get; set; } 

    }
}