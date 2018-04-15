using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmicBox.Models {
    [Table("boxes")]
    public class Box {
        [Column("id")]
        public int Id { get; set; }
        [Column("uuid")]
        public Guid Uuid { get; set; }

        public List<Run> Runs { get; set; }
        public List<Grant> Grants { get; set; }
    }
}