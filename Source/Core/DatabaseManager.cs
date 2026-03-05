using System;
using Microsoft.Data.Sqlite;
using System.IO;
using Microsoft.Xna.Framework;

namespace DungeonOfAlgorithms.Source.Core;

public class DatabaseManager
{
    private static DatabaseManager _instance;
    public static DatabaseManager Instance => _instance ??= new DatabaseManager();

    private string _connectionString;

    private DatabaseManager()
    {
        try
        {
            string baseDir = Path.GetDirectoryName(Environment.ProcessPath) ?? AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(baseDir, "game.db");

            try
            {
                using var test = File.Create(Path.Combine(baseDir, ".write_test"), 1, FileOptions.DeleteOnClose);
            }
            catch
            {
                string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DungeonOfAlgorithms");
                Directory.CreateDirectory(appData);
                dbPath = Path.Combine(appData, "game.db");
            }

            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Database init error: " + ex.Message);
            _connectionString = null;
        }
    }

    private void InitializeDatabase()
    {
        using var con = new SqliteConnection(_connectionString);
        con.Open();

        string sql = @"
            CREATE TABLE IF NOT EXISTS SaveSlots (
                Id INTEGER PRIMARY KEY,
                Level INTEGER,
                PlayerX REAL,
                PlayerY REAL,
                Score INTEGER
            )";

        using var cmd = new SqliteCommand(sql, con);
        cmd.ExecuteNonQuery();
    }

    public void SaveGame(int level, Vector2 position, int score)
    {
        if (_connectionString == null) return;

        try
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            string sql = "INSERT OR REPLACE INTO SaveSlots (Id, Level, PlayerX, PlayerY, Score) VALUES (1, @Level, @X, @Y, @Score)";

            using var cmd = new SqliteCommand(sql, con);
            cmd.Parameters.AddWithValue("@Level", level);
            cmd.Parameters.AddWithValue("@X", position.X);
            cmd.Parameters.AddWithValue("@Y", position.Y);
            cmd.Parameters.AddWithValue("@Score", score);

            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Save error: " + ex.Message);
        }
    }

    public (int Level, Vector2 Position, int Score)? LoadGame()
    {
        if (_connectionString == null) return null;

        try
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            string sql = "SELECT Level, PlayerX, PlayerY, Score FROM SaveSlots WHERE Id = 1";
            using var cmd = new SqliteCommand(sql, con);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int level = reader.GetInt32(0);
                float x = reader.GetFloat(1);
                float y = reader.GetFloat(2);
                int score = reader.GetInt32(3);

                return (level, new Vector2(x, y), score);
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Load error: " + ex.Message);
            return null;
        }
    }
}
