using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerLizardFile.Bdd
{
    public class Navicat
    {
        private static MySqlConnection Create()
        {
            var connection = new MySqlConnection("Server=127.0.0.1;Database=lizardfile;Uid=root;Pwd=;SSL Mode=None");
            return connection;
        }
        public static List<T> Query<T>(string sql, object parameters = null)
        {
            using (var connection = Create())
            {
                connection.Open();
                return connection.Query<T>(sql, parameters).ToList();
            }
        }
    }
}
