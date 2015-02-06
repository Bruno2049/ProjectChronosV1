using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProjectChronos.Models;
using MySql.Data.MySqlClient;

namespace ProjectChronos.Repositories
{
    class FinishersRepo : AbstractModel<Finisher>
    {
        public override List<Finisher> all()
        {
            List<Finisher> finishers = new List<Finisher>();

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionstring))
                {
                    con.Open();
                    string sql = "SELECT finishers.id, finishers.racerNo, finishers.epc, MIN(finishers.time) as time, racers.racerName FROM finishers INNER JOIN racers ON finishers.racerNo = racers.racerNo GROUP BY finishers.racerNo";
                    MySqlCommand sqlCmd = new MySqlCommand(sql, con);
                    MySqlDataReader reader = sqlCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Finisher finisher = new Finisher();
                        finisher.id = Int32.Parse(reader["id"].ToString());
                        finisher.racerNo = Int32.Parse(reader["racerNo"].ToString());
                        finisher.racerName = reader["racerName"].ToString();
                        finisher.epc = reader["epc"].ToString();
                        finisher.time = reader["time"].ToString();

                        finishers.Add(finisher);
                    }
                }
            }
            catch (Exception ex)
            {
                return finishers;
            }

            return finishers;
        }

        public override int create(Finisher finisher)
        {
            int result = 0;

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionstring))
                {
                    con.Open();
                    string sql = "INSERT INTO finishers(racerNo, epc, time) VALUES(@racerNo, @epc, @time)";
                    MySqlCommand sqlCmd = new MySqlCommand(sql, con);

                    sqlCmd.Parameters.AddWithValue("racerNo", finisher.racerNo);
                    sqlCmd.Parameters.AddWithValue("epc", finisher.epc);
                    sqlCmd.Parameters.AddWithValue("time", finisher.time);

                    sqlCmd.ExecuteNonQuery();
                    result = 1;
                }
            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }

        public override int update(int id)
        {
            return 0;
        }

        public override int delete(int id)
        {
            int result = 0;

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionstring))
                {
                    con.Open();
                    string sql = "DELETE FROM finishers WHERE id = @id";
                    MySqlCommand sqlCmd = new MySqlCommand(sql, con);

                    sqlCmd.Parameters.AddWithValue("id", id);

                    sqlCmd.ExecuteNonQuery();
                    result = 1;
                }
            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }
    }
}
