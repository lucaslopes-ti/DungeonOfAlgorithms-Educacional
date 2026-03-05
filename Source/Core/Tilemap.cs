using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DungeonOfAlgorithms.Source.Core;

public class Tilemap
{
    private readonly Texture2D _tilesetTexture;
    public Texture2D Texture => _tilesetTexture;

    private readonly int[,] _mapData;
    private readonly int _tileWidth;
    private readonly int _tileHeight;
    private readonly int _tilesPerRow;

    private static readonly HashSet<int> SolidTiles = new()
    {
        96, 97, 98, 99, 100,
        77, 58, 39,
        20, 21, 22, 23, 24,
        43, 62, 81,
        26, 45,
        172, 173, 174
    };

    public static readonly int DoorEast = 99;
    public static readonly int DoorWest = 98;
    public static readonly int DoorNorth = 97;
    public static readonly int DoorSouth = 96;

    public int TileWidth => _tileWidth;
    public int TileHeight => _tileHeight;
    public int MapWidth => _mapData.GetLength(1) * _tileWidth;
    public int MapHeight => _mapData.GetLength(0) * _tileHeight;

    public Tilemap(Texture2D tileset, int[,] mapData, int tileWidth, int tileHeight)
    {
        _tilesetTexture = tileset;
        _mapData = mapData;
        _tileWidth = tileWidth;
        _tileHeight = tileHeight;
        _tilesPerRow = tileset.Width / tileWidth;
    }

    public int GetTileAt(int worldX, int worldY)
    {
        int tileX = worldX / _tileWidth;
        int tileY = worldY / _tileHeight;

        if (tileX < 0 || tileY < 0 || tileY >= _mapData.GetLength(0) || tileX >= _mapData.GetLength(1))
            return -1;

        return _mapData[tileY, tileX];
    }

    public bool IsSolid(int worldX, int worldY)
    {
        int tileX = worldX / _tileWidth;
        int tileY = worldY / _tileHeight;

        if (tileX < 0 || tileY < 0 || tileY >= _mapData.GetLength(0) || tileX >= _mapData.GetLength(1))
            return true;

        int tile = _mapData[tileY, tileX];
        if (tile < 0) return false;
        return SolidTiles.Contains(tile);
    }

    public bool IsColliding(Rectangle bounds)
    {
        int startCol = bounds.Left / _tileWidth;
        int endCol = (bounds.Right - 1) / _tileWidth;
        int startRow = bounds.Top / _tileHeight;
        int endRow = (bounds.Bottom - 1) / _tileHeight;

        for (int row = startRow; row <= endRow; row++)
        {
            for (int col = startCol; col <= endCol; col++)
            {
                if (IsSolid(col * _tileWidth, row * _tileHeight))
                    return true;
            }
        }
        return false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int rows = _mapData.GetLength(0);
        int cols = _mapData.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int tileIndex = _mapData[y, x];
                if (tileIndex < 0) continue;

                int tileX = (tileIndex % _tilesPerRow) * _tileWidth;
                int tileY = (tileIndex / _tilesPerRow) * _tileHeight;
                Rectangle sourceRect = new Rectangle(tileX, tileY, _tileWidth, _tileHeight);

                Vector2 position = new Vector2(x * _tileWidth, y * _tileHeight);
                spriteBatch.Draw(_tilesetTexture, position, sourceRect, Color.White);
            }
        }
    }
}
