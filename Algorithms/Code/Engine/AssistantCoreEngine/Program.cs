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
    .WithNamingConvention(UnderscoredNamingConvention.Instance)   // Converts HardToRemember from C# into hard_to_remember for YamlDotNet
    .IgnoreUnmatchedProperties()
    .Build();

string[] files = Directory.GetFiles(notesPath, "*.md");

foreach (string file in files)
{
    string content = File.ReadAllText(file);

    // Split front-matter from body
    string[] parts = content.Split("---", 3, StringSplitOptions.RemoveEmptyEntries);
    /*
                    ---
                    tags: [python]
                    mode: coding
                    ---
                    # Title
                    Body text



                    Piece 0 → ""                                (empty — nothing before the first ---)
                    Piece 1 → "tags: [python] mode: coding"     (the YAML)
                    Piece 2 → "# FastAPI... Remember to run"    (the note body)
    */
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



// --- Search --- 
while (true)
{
    Console.WriteLine("\nType a keyword to search (or 'exit' to quit):");
    string query = Console.ReadLine() ?? "";

    if (query.ToLower() == "exit") break;
    if (string.IsNullOrWhiteSpace(query)) continue;

    string searchSql = @"
        SELECT Title, Tags, Mode, FilePath, HardToRemember, Important
        FROM Notes
        WHERE Title LIKE @query
           OR Tags LIKE @query
           OR Mode LIKE @query
        ORDER BY Important DESC, HardToRemember DESC";

    using (var cmd = new SqliteCommand(searchSql, connection))
    {
        cmd.Parameters.AddWithValue("@query", $"%{query}%");

        using (var reader = cmd.ExecuteReader())
        {
            bool found = false;

            while (reader.Read())
            {
                found = true;
                Console.WriteLine("\n--- Result ---");
                Console.WriteLine($"Title    : {reader["Title"]}");
                Console.WriteLine($"Tags     : {reader["Tags"]}");
                Console.WriteLine($"Mode     : {reader["Mode"]}");
                Console.WriteLine($"File     : {reader["FilePath"]}");
                Console.WriteLine($"Important: {(Convert.ToInt32(reader["Important"]) == 1 ? "Yes" : "No")}");
                Console.WriteLine($"Hard to remember: {(Convert.ToInt32(reader["HardToRemember"]) == 1 ? "Yes" : "No")}");
            }

            if (!found)
                Console.WriteLine("No notes found for that keyword.");
        }
    }
}
// END -- Search --













// --- Models ---
public class NoteFrontMatter
{
    public List<string>? Tags { get; set; }
    public string? Mode { get; set; }
    public bool HardToRemember { get; set; }
    public bool Important { get; set; }
}
// END -- Models --