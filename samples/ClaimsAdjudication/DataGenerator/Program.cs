using System;
using System.Data.SQLite;
using System.IO;

namespace NRules.Samples.DataGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string databaseFile = Path.GetFullPath(@"..\..\..\Data\ClaimsExpert.sqlite");
            if (args.Length == 1)
            {
                databaseFile = args[0];
            }

            if (File.Exists(databaseFile))
            {
                Console.WriteLine("Database file already exists. File={0}", databaseFile);
                Console.WriteLine("Delete the database file if you want it to get regenerated.");
                return;
            }

            EnsureDirectoryExists(databaseFile);

            Console.WriteLine("Creating database. File={0}", databaseFile);
            SQLiteConnection.CreateFile(databaseFile);

            string connectionString = string.Format("Data Source={0};Version=3;", databaseFile);
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                connection.Trace += DatabaseTrace;

                var scripts = new[] { @"Scripts\Schema.sql", @"Scripts\Data.sql" };
                foreach (var script in scripts)
                {
                    var scriptFile = Path.GetFullPath(script);
                    Console.WriteLine("Executing script. File={0}", scriptFile);
                    string sql = File.ReadAllText(script);
                    var command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void EnsureDirectoryExists(string databaseFile)
        {
            var path = Path.GetDirectoryName(databaseFile);
            if (path != null && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static void DatabaseTrace(object sender, TraceEventArgs e)
        {
            Console.WriteLine(e.Statement);
            Console.WriteLine();
        }
    }
}