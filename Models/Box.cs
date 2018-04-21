using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

        public bool IsOwner(string sub) =>
            Grants.Where(g => g.Sub == sub).Any(g => g.Type == Grant.Types.Owner);

        public bool HasWriteAccess(string sub) =>
            Grants.Where(g => g.Sub == sub).Any(g => g.CanWrite());
    }
}