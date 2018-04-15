using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmicBox.Models {
    [Table("traces")]
    public class Trace {
        [Column("id")]
        public int Id { get; set; }
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
        [Column("pressure")]
        public float Pressure { get; set; }

        [Column("run_id")]
        public int RunId { get; set; }
        public Run Run { get; set; }
    }
}