using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPE.Api.Models
{
    internal abstract class EntityAbstract
    {
        public abstract int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Deleted { get; set; }
    }
}
