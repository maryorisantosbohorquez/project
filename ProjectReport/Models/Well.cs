using System;

namespace ProjectReport.Models
{
    public class Well
    {
        public int Id { get; set; }
        public string WellName { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public string Block { get; set; } = string.Empty;
        public WellStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime SpudDate { get; set; }
    }
}
