using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace NBADraftBot.Models
{

    public class Predictions
    {
        [Key]
        public int Id { get; set; }
        public string Player { get; set; }
        public string Position { get; set; }
        public int? Age { get; set; }
        public string Team { get; set; }
        public int? Season { get; set; }
        public double? PER { get; set; }
        public int? Drafted { get; set; }
    }
}