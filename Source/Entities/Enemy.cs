using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using DungeonOfAlgorithms.Source.Core;

namespace DungeonOfAlgorithms.Source.Entities;

public class Enemy : IGameEntity
{
    public int Id { get; private set; }
    public Vector2 Position { get; set; }
    public float Speed { get; set; } = 50f;
    public int Damage { get; set; } = 10;

    private Dictionary<string, Texture2D> _textures;
    private string _currentAnimation;
    private int _currentFrame;
    private double _timer;

    private const double FRAME_TIME = 0.15;
    private const int FRAME_WIDTH = 32;
    private const int FRAME_HEIGHT = 32;

    private SpriteEffects _flipEffect = SpriteEffects.None;

    private enum FaceDirection { Down, Up, Left, Right }
    private FaceDirection _facing = FaceDirection.Down;
    private bool _isMoving = false;

    private IEnemyBehavior _behavior;

    public Rectangle Bounds => new Rectangle((int)Position.X + 4, (int)Position.Y + 4, 24, 24);

    public Enemy(int id, Dictionary<string, Texture2D> textures, Vector2 startPosition, IEnemyBehavior behavior)
    {
        Id = id;
        _textures = textures;
        Position = startPosition;
        _behavior = behavior;
        _currentAnimation = "Down_Idle";
    }

    public void ChangeBehavior(IEnemyBehavior newBehavior)
    {
        _behavior = newBehavior;
    }

    public void Update(GameTime gameTime)
    {
    }

    public void Update(GameTime gameTime, Player player, Tilemap tilemap = null)
    {
        Vector2 prevPos = Position;

        _behavior.Update(this, player, gameTime);

        if (tilemap != null && tilemap.IsColliding(Bounds))
            Position = prevPos;

        Vector2 delta = Position - prevPos;
        _isMoving = delta.LengthSquared() > 0.0001f;

        if (_isMoving)
        {
            if (Math.Abs(delta.X) > Math.Abs(delta.Y))
                _facing = delta.X > 0 ? FaceDirection.Right : FaceDirection.Left;
            else
                _facing = delta.Y > 0 ? FaceDirection.Down : FaceDirection.Up;
        }

        UpdateAnimation(gameTime);
    }

    private void UpdateAnimation(GameTime gameTime)
    {
        _flipEffect = SpriteEffects.None;
        string suffix = _isMoving ? "" : "_Idle";

        switch (_facing)
        {
            case FaceDirection.Down:
                _currentAnimation = _textures.ContainsKey("Down" + suffix) ? "Down" + suffix : "Down";
                break;
            case FaceDirection.Up:
                _currentAnimation = _textures.ContainsKey("Up" + suffix) ? "Up" + suffix : "Up";
                break;
            case FaceDirection.Right:
                _currentAnimation = "Side" + suffix;
                _flipEffect = SpriteEffects.FlipHorizontally;
                break;
            case FaceDirection.Left:
                _currentAnimation = "Side" + suffix;
                _flipEffect = SpriteEffects.None;
                break;
        }

        _timer += gameTime.ElapsedGameTime.TotalSeconds;
        if (_timer >= FRAME_TIME)
        {
            _timer = 0;
            _currentFrame++;
            if (_currentFrame >= 6)
                _currentFrame = 0;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        string animToPlay = _currentAnimation;
        if (!_textures.ContainsKey(animToPlay)) animToPlay = "Down_Idle";
        if (!_textures.ContainsKey(animToPlay)) animToPlay = "Down";

        if (_textures.ContainsKey(animToPlay))
        {
            Texture2D texture = _textures[animToPlay];
            Rectangle sourceRect = new Rectangle(_currentFrame * FRAME_WIDTH, 0, FRAME_WIDTH, FRAME_HEIGHT);
            spriteBatch.Draw(texture, Position, sourceRect, Color.White, 0f, Vector2.Zero, 1f, _flipEffect, 0f);
        }
    }
}
