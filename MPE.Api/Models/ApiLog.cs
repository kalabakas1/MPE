using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NPoco;

namespace MPE.Api.Models
{
    [TableName("Api_Log")]
    [PrimaryKey("LogID")]
    internal class ApiLog : EntityAbstract
    {
        [Column("LogID")]
        public override int Id { get; set; }
        [Column("Ip")]
        public string Ip { get; set; }
        [Column("KeyID")]
        public int? KeyId { get; set; }
        [Column("Key")]
        public string Key { get; set; }
        [Column("RequestTimestamp")]
        public DateTime RequestTimestamp { get; set; }
        [Column("RequestUrl")]
        public string RequestUrl { get; set; }
        [Column("RequestContent")]
        public string RequestContent { get; set; }
        [Column("RequestMethod")]
        public string RequestMethod { get; set; }
        [Column("ResponseTimestamp")]
        public DateTime ResponseTimestamp { get; set; }
        [Column("ResponseStatusCode")]
        public HttpStatusCode ResponseStatusCode { get; set; }
        [Column("ResponseContent")]
        public string ResponseContent { get; set; }
    }
}
