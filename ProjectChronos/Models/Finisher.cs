using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChronos.Models
{
    class Finisher
    {
        public int id { get; set; }
        public int racerNo { get; set; }
        public string epc { get; set; }
        public string time { get; set; }

        public string racerName { get; set; }
    }
}
