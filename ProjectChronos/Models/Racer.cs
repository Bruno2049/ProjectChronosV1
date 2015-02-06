using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace ProjectChronos.Models
{
    class Racer
    {
        public int id { get; set; }
        public int racerNo { get; set; }
        public string racerName { get; set; }
        public string epc1 { get; set; }
        public string epc2 { get; set; }

    }
}
