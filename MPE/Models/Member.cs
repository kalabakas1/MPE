using System;

namespace MPE.Models
{
    class Member : EntityAbstract
    {
        public int EntityId { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}