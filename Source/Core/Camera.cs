using Microsoft.Xna.Framework;

namespace DungeonOfAlgorithms.Source.Core;

public class Camera
{
    public Matrix Transform { get; private set; }
    public float Zoom { get; set; } = 3.0f;

    public void Follow(Vector2 target, int screenWidth, int screenHeight)
    {
        var position = Matrix.CreateTranslation(-target.X, -target.Y, 0);
        var offset = Matrix.CreateTranslation(screenWidth / 2, screenHeight / 2, 0);
        var scale = Matrix.CreateScale(Zoom, Zoom, 1);

        Transform = position * scale * offset;
    }
}
