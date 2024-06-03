using System.Data.SqlClient;

namespace Race.Models 
{
    public class Connexion 
    {
        public SqlConnection connection;
        public Connexion()
        {
            string connectionString = "Server=(Localdb)\\MSSQLLocalDB;Database=race;Trusted_Connection=True;";
            connection = new SqlConnection(connectionString);
        }
        public SqlConnection connexion 
        {
            get; set;
        }

        public static List<string> allTables(Connexion connexion)
        {
            List<string> val = new();
            var cmd = new SqlCommand("SELECT name FROM sysobjects WHERE xtype='U'", connexion.connection);
            var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                val.Add(reader.GetString(0));
                Console.WriteLine(reader.GetString(0));
            }

            reader.Close();
            return val;
        }
        public static void ResetDatabase(Connexion connexion)
        {
            try
            {
                DisableAllForeignKeys(connexion);

                List<string> tables = Connexion.allTables(connexion);

                // Delete all data from all tables
                foreach (var item in tables)
                {
                    if (item != "category")
                    {
                        Console.WriteLine($"Deleting data from table: {item}");
                        var deleteCmd = new SqlCommand($"DELETE FROM {item}", connexion.connection);
                        deleteCmd.ExecuteNonQuery();

                        var resetCmd = new SqlCommand($"DBCC CHECKIDENT ('{item}', RESEED, 0)", connexion.connection);
                        resetCmd.ExecuteNonQuery();
                        EnableForeignKeyConstraintsForTable(connexion, item);
                    }
                }

                // Insert the default user into the uuser table
                Console.WriteLine("Inserting default admin user into uuser table");
                var insertAdminCmd = new SqlCommand("INSERT INTO uuser (name, uuser, passWord, admin) VALUES ('Admin', 'admin', 'admin', 1)", connexion.connection);
                insertAdminCmd.ExecuteNonQuery();

                // Truncate all tables
                foreach (var item in tables)
                {
                    if (item != "category")
                    {
                        Console.WriteLine($"Truncating table: {item}");
                        var truncateCmd = new SqlCommand($"TRUNCATE TABLE {item}", connexion.connection);
                        truncateCmd.ExecuteNonQuery();

                        var resetCmd = new SqlCommand($"DBCC CHECKIDENT ('{item}', RESEED, 0)", connexion.connection);
                        resetCmd.ExecuteNonQuery();
                        EnableForeignKeyConstraintsForTable(connexion, item);
                    }
                }
                
                EnableAllForeignKeys(connexion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }


        private static void DisableAllForeignKeys(Connexion connexion)
        {
            var disableFkCmd = new SqlCommand("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'", connexion.connection);
            disableFkCmd.ExecuteNonQuery();
        }

        private static void EnableAllForeignKeys(Connexion connexion)
        {
            var enableFkCmd = new SqlCommand("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'", connexion.connection);
            enableFkCmd.ExecuteNonQuery();
        }

        private static void EnableForeignKeyConstraintsForTable(Connexion connexion, string tableName)
        {
            var enableFkCmd = new SqlCommand($"ALTER TABLE {tableName} WITH CHECK CHECK CONSTRAINT ALL", connexion.connection);
            enableFkCmd.ExecuteNonQuery();
        }
    }
}
