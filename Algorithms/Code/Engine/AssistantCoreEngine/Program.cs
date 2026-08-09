//Dependencies + Configuration
using Microsoft.Data.Sqlite;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

string notesPath = @"C:\Users\Michael\Desktop\My Career\PERSONAL PROJECT\SECOND BRAIN ARCHITECTURE\Notes"; // Hard code path for now 
string dbPath = @"C:\Users\Michael\Desktop\My Career\PERSONAL PROJECT\SECOND BRAIN ARCHITECTURE\Notes\db\assistant.db"; // Hard code path for now Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db", "assistant.db");
string connectionString = $"Data Source={dbPath}";
//END -- Dependencies + Configuration --




// --- Database Initialisation ---
using var connection = new SqliteConnection(connectionString);
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!); // Create db folder if it doesn't exist
Directory.CreateDirectory(Path.GetDirectoryName(notesPath)!); // Create Notes folder if it doesn't exist
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
    command.ExecuteNonQuery();

Console.WriteLine("Database ready.");
// END -- Database Initialisation --



// --- Indexer ---
var deserializer = new DeserializerBuilder()
    .WithNamingConvention(UnderscoredNamingConvention.Instance)
    .IgnoreUnmatchedProperties()
    .Build();

string[] files = Directory.GetFiles(notesPath, "*.md");

foreach (string file in files)
{
    string content = File.ReadAllText(file);

    // Split front-matter from body
    string[] parts = content.Split("---", 3, StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length < 2) continue;

    string yaml = parts[0].Trim();
    string body = parts[1].Trim();

    // Parse YAML front-matter
    var meta = deserializer.Deserialize<NoteFrontMatter>(yaml);

    // Extract title from first # heading
    string title = "";
    foreach (string line in body.Split('\n'))
    {
        if (line.StartsWith("# "))
        {
            title = line.Substring(2).Trim();
            break;
        }
    }

    // Insert into database
    string insertSql = @"
        INSERT OR REPLACE INTO Notes (FilePath, Title, Tags, Mode, HardToRemember, Important)
        VALUES (@FilePath, @Title, @Tags, @Mode, @HardToRemember, @Important);";

    using (var cmd = new SqliteCommand(insertSql, connection))
    {
        cmd.Parameters.AddWithValue("@FilePath", file);
        cmd.Parameters.AddWithValue("@Title", title);
        cmd.Parameters.AddWithValue("@Tags", string.Join(", ", meta.Tags ?? new List<string>()));
        cmd.Parameters.AddWithValue("@Mode", meta.Mode ?? "");
        cmd.Parameters.AddWithValue("@HardToRemember", meta.HardToRemember ? 1 : 0);
        cmd.Parameters.AddWithValue("@Important", meta.Important ? 1 : 0);
        cmd.ExecuteNonQuery();
    }

    Console.WriteLine($"Indexed: {title}");
}

Console.WriteLine("\nAll notes indexed successfully.");
// END -- Indexer --

















// --- Models ---
public class NoteFrontMatter
{
    public List<string>? Tags { get; set; }
    public string? Mode { get; set; }
    public bool HardToRemember { get; set; }
    public bool Important { get; set; }
}
// END -- Models --