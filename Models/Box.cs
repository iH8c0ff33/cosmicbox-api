using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CosmicBox.Models {
    [Table("boxes")]
    public class Box {
        [Column("id")]
        public int Id { get; set; }

        [JsonIgnore]
        public List<Run> Runs { get; set; }
        [JsonIgnore]
        public List<Grant> Grants { get; set; }
    }
}