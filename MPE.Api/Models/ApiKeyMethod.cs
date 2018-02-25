using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;

namespace MPE.Api.Models
{
    [TableName("Api_KeyMethod")]
    [PrimaryKey("KeyMethodID")]
    internal class ApiKeyMethod : EntityAbstract
    {
        [Column("KeyMethodID")]
        public override int Id { get; set; }
        [Column("KeyID")]
        public int KeyId { get; set; }
        [Column("Method")]
        public string Method { get; set; }
    }
}
