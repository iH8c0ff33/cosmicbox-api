using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmicBox.Models {
    [Table("grants")]
    public class Grant {
        public static class Types {
            public static string Owner = "owner";
            public static string Write = "write";
            public static string Read = "read";
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("type"), Required]
        public string Type { get; set; }
        [Column("sub"), Required]
        public string Sub { get; set; }

        [Column("box_id")]
        public int BoxId { get; set; }
        public Box Box { get; set; }
    }
}