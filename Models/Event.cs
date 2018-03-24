using System;

namespace CosmicBox.Models {
    public class Event {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double Pressure { get; set; }
    }
}