using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DungeonOfAlgorithms.Source.Core;

public class InputManager
{
    private static InputManager _instance;
    public static InputManager Instance => _instance ??= new InputManager();

    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;

    private InputManager()
    {
        _currentKeyboardState = Keyboard.GetState();
        _previousKeyboardState = _currentKeyboardState;
    }

    public void Update()
    {
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();
    }

    public Vector2 GetMovementDirection()
    {
        Vector2 direction = Vector2.Zero;

        if (_currentKeyboardState.IsKeyDown(Keys.W) || _currentKeyboardState.IsKeyDown(Keys.Up))
            direction.Y -= 1;
        if (_currentKeyboardState.IsKeyDown(Keys.S) || _currentKeyboardState.IsKeyDown(Keys.Down))
            direction.Y += 1;
        if (_currentKeyboardState.IsKeyDown(Keys.A) || _currentKeyboardState.IsKeyDown(Keys.Left))
            direction.X -= 1;
        if (_currentKeyboardState.IsKeyDown(Keys.D) || _currentKeyboardState.IsKeyDown(Keys.Right))
            direction.X += 1;

        if (direction != Vector2.Zero)
            direction.Normalize();

        return direction;
    }

    public bool IsKeyPressed(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
    }

    public bool IsKeyDown(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key);
    }

    public bool IsActionPressed()
    {
        return IsKeyPressed(Keys.Space);
    }
}
