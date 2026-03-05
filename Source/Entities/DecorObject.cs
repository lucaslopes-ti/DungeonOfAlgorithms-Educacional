using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonOfAlgorithms.Source.Entities;

public class DecorObject
{
    public Vector2 Position { get; private set; }
    public Rectangle SourceRect { get; private set; }

    private Texture2D _texture;

    public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, SourceRect.Width, SourceRect.Height);

    public DecorObject(Texture2D texture, Vector2 position, Rectangle sourceRect)
    {
        _texture = texture;
        Position = position;
        SourceRect = sourceRect;
    }

    public DecorObject(Texture2D texture, Vector2 position)
    {
        _texture = texture;
        Position = position;
        SourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, SourceRect, Color.White);
    }
}
