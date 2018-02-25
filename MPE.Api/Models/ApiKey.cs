using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;

namespace MPE.Api.Models
{
    [TableName("Api_Key")]
    [PrimaryKey("KeyID")]
    internal class ApiKey : EntityAbstract
    {
        [Column("KeyID")]
        public override int Id { get; set; }
        [Column("Key")]
        public string Key { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Ignore]
        public List<ApiKeyMethod> Methods { get; set; }
        [Ignore]
        public List<ApiKeyField> Fields { get; set; }
    }
}
