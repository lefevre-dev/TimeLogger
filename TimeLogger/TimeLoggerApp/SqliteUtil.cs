using System.Data.Entity;
using System;
using System.Data.SQLite;
using System.Collections.Generic;
using TimeLoggerApp.Models;
using System.IO;

namespace TimeLoggerApp
{
    public static class SqliteUtil
    {
        public static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            // appdata
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string program_data = Path.Combine(appdata, "TimeLogger");
            if (!Directory.Exists(program_data))
            {
                Directory.CreateDirectory(program_data);
            }
            sqlite_conn = new SQLiteConnection($"Data Source={program_data}/database.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
                CreateDb(sqlite_conn);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error");
                Console.WriteLine(ex.Message);
            }
            return sqlite_conn;
        }

        public static void CreateDb(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS Data(Date VARCHAR(20), Log TEXT)";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        public static void InsertData(SQLiteConnection conn, string dateTime, string log)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO Data (Date, Log) VALUES('{dateTime}', '{log}'); ";
            sqlite_cmd.ExecuteNonQuery();
        }

        public static List<DataLog> SelectData(SQLiteConnection conn)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT Date, Log FROM Data";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            List<DataLog> table = new List<DataLog>();
            while (sqlite_datareader.Read())
            {
                string date_reader = sqlite_datareader.GetString(0);
                string log_reader = sqlite_datareader.GetString(1);
                table.Add(new DataLog
                {
                    Date = date_reader,
                    Log = log_reader
                });
            }
            conn.Close();
            return table;
        }
    }

}
