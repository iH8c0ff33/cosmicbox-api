using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmicBox.Models {
    [Table("runs")]
    public class Run {
        [Column("id")]
        public int Id { get; set; }
        [Column("start")]
        public DateTime Start { get; set; }
        [Column("end")]
        public DateTime End { get; set; }

        [Column("box_id")]
        public int BoxId { get; set; }
        public Box Box { get; set; }

        public List<Trace> Traces { get; set; }
    }
}