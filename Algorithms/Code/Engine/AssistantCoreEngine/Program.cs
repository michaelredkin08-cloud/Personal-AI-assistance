using Microsoft.Data.Sqlite;

string dbPath = @"C:\Users\Michael\Desktop\My Career\PERSONAL PROJECT\SECOND BRAIN ARCHITECTURE\Notes\db\assistant.db";
string connectionString = $"Data Source={dbPath}";

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


//Notes

//Testing