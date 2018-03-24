using System;
using System.ComponentModel.DataAnnotations;

namespace CosmicBox.Models {
    public class Event {
        [Key]
        public long Id { get; set; }


        [Required]
        public DateTime? Timestamp { get; set; }

        [Required]
        public double? Pressure { get; set; }
    }
}