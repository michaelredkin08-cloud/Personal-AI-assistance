//Dependencies + Configuration
using Microsoft.Data.Sqlite;


string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db", "assistant.db"); // Because it is public , better not show path 
string connectionString = $"Data Source={dbPath}";

//END -- Dependencies + Configuration --





// --- Database Initialisation ---
using (var connection = new SqliteConnection(connectionString))
{
    connection.Open();

    string createTableSql = @"
        CREATE TABLE IF NOT EXISTS Notes (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            FilePath TEXT NOT NULL,
            Title TEXT,
            Tags TEXT,
            Mode TEXT,
            HardToRemember INTEGER DEFAULT 0,
            Important INTEGER DEFAULT 0
        );";
    
    using (var command = new SqliteCommand(createTableSql, connection))
    {
        command.ExecuteNonQuery();
    }

    Console.WriteLine("Database ready at: " + dbPath);
}
// END -- Database Initialisation --







//Notes

//Testing