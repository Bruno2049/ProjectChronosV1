using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChronos
{
    abstract class AbstractModel<T>
    {
        protected string connectionstring = string.Format("Server={0};Database={1};Uid={2};Pwd={3};", Properties.Settings.Default.host, Properties.Settings.Default.database, Properties.Settings.Default.username, Properties.Settings.Default.password);

        abstract public List<T> all();
        abstract public int create();
        abstract public int update(int id);
        abstract public int delete(int id);
    }
}
