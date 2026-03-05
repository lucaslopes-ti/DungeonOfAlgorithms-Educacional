using System.Collections.Generic;

namespace DungeonOfAlgorithms.Source.Core;

public class DungeonManager
{
    private static DungeonManager _instance;
    public static DungeonManager Instance => _instance ??= new DungeonManager();

    public Dictionary<int, Room> Rooms { get; private set; } = new();
    public Room CurrentRoom { get; private set; }

    private DungeonManager() { }

    public int TotalCoins
    {
        get
        {
            int total = 0;
            foreach (var room in Rooms.Values)
                foreach (var item in room.Items)
                    if (item is not DungeonOfAlgorithms.Source.Entities.ChestItem)
                        total++;
            return total;
        }
    }

    public int CollectedCoins
    {
        get
        {
            int collected = 0;
            foreach (var room in Rooms.Values)
                foreach (var item in room.Items)
                    if (item is not DungeonOfAlgorithms.Source.Entities.ChestItem && !item.IsActive)
                        collected++;
            return collected;
        }
    }

    public bool AllCoinsCollected => CollectedCoins >= TotalCoins && TotalCoins > 0;

    public void AddRoom(Room room)
    {
        if (!Rooms.ContainsKey(room.Id))
            Rooms.Add(room.Id, room);
    }

    public void CheckTransition(int playerX, int playerY, int mapWidthPixel, int mapHeightPixel)
    {
        string direction = null;

        if (playerX > mapWidthPixel) direction = "East";
        else if (playerX < 0) direction = "West";
        else if (playerY > mapHeightPixel) direction = "South";
        else if (playerY < 0) direction = "North";

        if (direction != null && CurrentRoom.Connections.ContainsKey(direction))
            ChangeRoom(CurrentRoom.Connections[direction]);
    }

    public void ChangeRoom(int roomId)
    {
        if (Rooms.ContainsKey(roomId))
            CurrentRoom = Rooms[roomId];
    }

    public int[,] LoadMapFromCSV(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
        {
            string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string altPath = System.IO.Path.Combine(baseDir, filePath);
            if (System.IO.File.Exists(altPath))
                filePath = altPath;
            else
            {
                string procDir = System.IO.Path.GetDirectoryName(System.Environment.ProcessPath) ?? baseDir;
                string procPath = System.IO.Path.Combine(procDir, filePath);
                if (System.IO.File.Exists(procPath))
                    filePath = procPath;
            }
        }

        string[] lines = System.IO.File.ReadAllLines(filePath);
        int height = lines.Length;
        int width = lines[0].Split(',').Length;
        int[,] mapData = new int[height, width];

        for (int y = 0; y < height; y++)
        {
            string[] values = lines[y].Split(',');
            for (int x = 0; x < width; x++)
            {
                if (int.TryParse(values[x].Trim(), out int tileIndex))
                    mapData[y, x] = tileIndex;
            }
        }

        return mapData;
    }
}
