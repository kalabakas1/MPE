using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;

namespace MPE.Api.Models
{
    [TableName("Api_KeyField")]
    [PrimaryKey("KeyFieldID")]
    internal class ApiKeyField : EntityAbstract
    {
        [Column("KeyFieldID")]
        public override int Id { get; set; }
        [Column("TypePath")]
        public string TypeAlias { get; set; }
        [Column("FieldName")]
        public string FieldName { get; set; }
    }
}
