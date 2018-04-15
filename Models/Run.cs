using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace CosmicBox.Models {
    [Table("runs")]
    public class Run {
        [Column("id")]
        public int Id { get; set; }
        [Column("start"), JsonRequired]
        public DateTime Start { get; set; }
        [Column("end"), JsonRequired]
        public DateTime End { get; set; }

        [Column("box_id"), JsonRequired]
        public int BoxId { get; set; }
        [JsonIgnore]
        public Box Box { get; set; }

        [JsonIgnore]
        public List<Trace> Traces { get; set; }
    }
}