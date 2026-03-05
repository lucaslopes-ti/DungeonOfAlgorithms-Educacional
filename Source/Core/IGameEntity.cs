using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonOfAlgorithms.Source.Core;

public interface IGameEntity
{
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);
}
