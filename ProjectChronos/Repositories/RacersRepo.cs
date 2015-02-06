using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProjectChronos.Models;
using MySql.Data.MySqlClient;

namespace ProjectChronos.Repositories
{
    class RacersRepo : AbstractModel<Racer>
    {
        public override List<Racer> all()
        {
            List<Racer> racers = new List<Racer>();

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionstring))
                {
                    con.Open();
                    string sql = "SELECT * FROM racers";
                    MySqlCommand sqlCmd = new MySqlCommand(sql, con);
                    MySqlDataReader reader = sqlCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Racer racer = new Racer();
                        racer.id = Int32.Parse(reader["id"].ToString());
                        racer.racerNo = Int32.Parse(reader["racerNo"].ToString());
                        racer.racerName = reader["racerName"].ToString();
                        racer.epc1 = reader["epc1"].ToString();
                        racer.epc2 = reader["epc2"].ToString();
                        racers.Add(racer);
                    }
                }
            }
            catch (Exception ex)
            {
                return racers;
            }

            return racers;
        }

        public override int create()
        {
            return 0;
        }

        public override int update(int id)
        {
            return 0;
        }

        public override int delete(int id)
        {
            return 0;
        }
    }
}
