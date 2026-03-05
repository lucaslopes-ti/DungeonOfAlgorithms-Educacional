using Microsoft.Xna.Framework;
using DungeonOfAlgorithms.Source.Entities;
using System;

namespace DungeonOfAlgorithms.Source.Core;

public class PatrolBehavior : IEnemyBehavior
{
    private float _moveTimer;
    private float _pauseTimer;
    private Vector2 _direction;
    private bool _isPaused;
    private float _moveDuration;
    private float _pauseDuration;
    private int _patternStep;
    private static readonly Random _random = new();

    private static readonly Vector2[] _directions = {
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(0, -1)
    };

    public PatrolBehavior()
    {
        _patternStep = _random.Next(0, _directions.Length);
        _direction = _directions[_patternStep];
        _moveDuration = 1.5f + (float)_random.NextDouble() * 1.5f;
        _pauseDuration = 0.5f + (float)_random.NextDouble() * 0.8f;
        _isPaused = false;
    }

    public void Update(Enemy enemy, Player player, GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_isPaused)
        {
            _pauseTimer += deltaTime;
            if (_pauseTimer >= _pauseDuration)
            {
                _isPaused = false;
                _moveTimer = 0f;
                _patternStep = (_patternStep + 1) % _directions.Length;
                _direction = _directions[_patternStep];
                _moveDuration = 1.5f + (float)_random.NextDouble() * 1.5f;
            }
            return;
        }

        _moveTimer += deltaTime;

        if (_moveTimer >= _moveDuration)
        {
            _isPaused = true;
            _pauseTimer = 0f;
            _pauseDuration = 0.5f + (float)_random.NextDouble() * 0.8f;
            return;
        }

        enemy.Position += _direction * enemy.Speed * deltaTime;
    }
}

public class ChaseBehavior : IEnemyBehavior
{
    private const float CHASE_RANGE = 120f;
    private const float LOSE_RANGE = 180f;
    private const float CHASE_SPEED_MULT = 0.85f;

    private bool _isChasing = false;
    private float _wanderTimer;
    private Vector2 _wanderDirection;
    private static readonly Random _random = new();

    public ChaseBehavior()
    {
        PickNewWanderDirection();
    }

    public void Update(Enemy enemy, Player player, GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float distToPlayer = Vector2.Distance(enemy.Position, player.Position);

        if (!_isChasing && distToPlayer < CHASE_RANGE)
            _isChasing = true;
        else if (_isChasing && distToPlayer > LOSE_RANGE)
            _isChasing = false;

        if (_isChasing)
        {
            Vector2 direction = player.Position - enemy.Position;
            if (direction != Vector2.Zero)
                direction.Normalize();

            float wobble = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3f + enemy.Id) * 0.15f;
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X);
            direction += perpendicular * wobble;

            if (direction != Vector2.Zero)
                direction.Normalize();

            enemy.Position += direction * (enemy.Speed * CHASE_SPEED_MULT) * deltaTime;
        }
        else
        {
            _wanderTimer += deltaTime;

            if (_wanderTimer >= 2f + (float)_random.NextDouble() * 1.5f)
            {
                PickNewWanderDirection();
                _wanderTimer = 0f;
            }

            enemy.Position += _wanderDirection * (enemy.Speed * 0.4f) * deltaTime;
        }
    }

    private void PickNewWanderDirection()
    {
        float angle = (float)(_random.NextDouble() * Math.PI * 2);
        _wanderDirection = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
    }
}

public class WanderBehavior : IEnemyBehavior
{
    private float _directionTimer;
    private float _changeInterval;
    private Vector2 _currentDirection;
    private float _targetAngle;
    private float _currentAngle;
    private static readonly Random _random = new();

    public WanderBehavior()
    {
        _currentAngle = (float)(_random.NextDouble() * Math.PI * 2);
        _targetAngle = _currentAngle;
        _currentDirection = new Vector2((float)Math.Cos(_currentAngle), (float)Math.Sin(_currentAngle));
        _changeInterval = 1f + (float)_random.NextDouble() * 2f;
    }

    public void Update(Enemy enemy, Player player, GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _directionTimer += deltaTime;

        if (_directionTimer >= _changeInterval)
        {
            _targetAngle = (float)(_random.NextDouble() * Math.PI * 2);
            _changeInterval = 1f + (float)_random.NextDouble() * 2f;
            _directionTimer = 0f;
        }

        float angleDiff = _targetAngle - _currentAngle;
        while (angleDiff > Math.PI) angleDiff -= (float)(Math.PI * 2);
        while (angleDiff < -Math.PI) angleDiff += (float)(Math.PI * 2);

        _currentAngle += angleDiff * 2f * deltaTime;
        _currentDirection = new Vector2((float)Math.Cos(_currentAngle), (float)Math.Sin(_currentAngle));

        enemy.Position += _currentDirection * (enemy.Speed * 0.6f) * deltaTime;
    }
}
