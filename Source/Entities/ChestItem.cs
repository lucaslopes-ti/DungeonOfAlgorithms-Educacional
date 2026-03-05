using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonOfAlgorithms.Source.Core;

namespace DungeonOfAlgorithms.Source.Entities;

public class ChestItem : Item
{
    private const int FRAME_W = 16;
    private const int FRAME_H = 24;

    public bool IsUnlocked { get; set; } = false;

    public override Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, FRAME_W, FRAME_H);

    public ChestItem(Texture2D texture, Vector2 position)
        : base(999, "Treasure Chest", texture, position)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;
        int frame = IsUnlocked ? 2 : 0;
        var src = new Rectangle(frame * FRAME_W, 0, FRAME_W, FRAME_H);
        spriteBatch.Draw(_texture, new Vector2(Position.X, Position.Y + _bobOffset), src, Color.White);
    }
}
