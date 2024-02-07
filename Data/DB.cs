using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Mysqlx.Cursor;

namespace InventorySystem.Data
{
    internal class DB
    {
        public MySqlConnection connection = new MySqlConnection($"server=127.0.0.1; database=inventory_system; Uid=root; password=;");
        
        public void Open()
        {
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        public MySqlCommand PrepareCommand(string cmd)
        {
            return new MySqlCommand(cmd, connection);
        }
    }
}
