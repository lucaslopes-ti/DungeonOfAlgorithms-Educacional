using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonOfAlgorithms.Source.Core;

public class Item : IGameEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public Vector2 Position { get; private set; }
    public bool IsActive { get; set; } = true;
    public int Value { get; private set; } = 10;

    protected Texture2D _texture;
    private float _bobTimer = 0f;
    protected float _bobOffset = 0f;
    private float _bobSpeed;

    public virtual Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

    public Item(int id, string name, Texture2D texture, Vector2 position)
    {
        Id = id;
        Name = name;
        _texture = texture;
        Position = position;
        _bobSpeed = 3f + (position.X * 0.1f % 2f);
    }

    public void Update(GameTime gameTime)
    {
        if (!IsActive) return;
        _bobTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        _bobOffset = (float)System.Math.Sin(_bobTimer * _bobSpeed) * 2f;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (IsActive)
            spriteBatch.Draw(_texture, new Vector2(Position.X, Position.Y + _bobOffset), Color.White);
    }
}
