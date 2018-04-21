using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CosmicBox.Models {
    [Table("traces")]
    public class Trace {
        [Column("id")]
        public int Id { get; set; }
        [Column("timestamp"), JsonRequired]
        public DateTime Timestamp { get; set; }
        [Column("pressure"), JsonRequired]
        public float Pressure { get; set; }

        [Column("run_id"), JsonRequired]
        public int RunId { get; set; }
        [JsonIgnore]
        public Run Run { get; set; }
    }
}