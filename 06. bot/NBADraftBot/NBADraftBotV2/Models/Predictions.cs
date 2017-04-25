using System;
using System.ComponentModel.DataAnnotations;

namespace NBADraftBotV2.Models
{
    [Serializable]
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