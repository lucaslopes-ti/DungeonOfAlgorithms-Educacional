// ═══════════════════════════════════════════════════════════════════════════════
// DECOR OBJECT - Os Móveis do Dungeon
// ═══════════════════════════════════════════════════════════════════════════════
// Caixas, barris, potes... tudo que você pode (ou não) destruir.
// Esses objetos têm colisão - o player não atravessa eles.
// São tipo os móveis da sua casa: só servem pra você bater o dedinho.
// ═══════════════════════════════════════════════════════════════════════════════

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonOfAlgorithms.Source.Entities;

/// <summary>
/// Objeto decorativo com colisão. Caixas, barris, etc.
/// Não se move, não ataca, mas atrapalha sua passagem.
/// </summary>
public class DecorObject
{
    /// <summary>Posição no mundo (X, Y)</summary>
    public Vector2 Position { get; private set; }
    
    /// <summary>Retângulo de origem na spritesheet (se usar)</summary>
    public Rectangle SourceRect { get; private set; }
    
    // 🖼️ Textura do objeto
    private Texture2D _texture;
    
    /// <summary>
    /// Área de colisão - baseada no tamanho do sprite.
    /// Se você bater nisso, não passa!
    /// </summary>
    public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, SourceRect.Width, SourceRect.Height);

    /// <summary>
    /// Construtor para spritesheet (recorta uma parte da imagem).
    /// Útil quando você tem várias caixas numa imagem só.
    /// </summary>
    /// <param name="texture">A spritesheet com os objetos</param>
    /// <param name="position">Onde colocar o objeto no mundo</param>
    /// <param name="sourceRect">Qual parte da spritesheet usar</param>
    public DecorObject(Texture2D texture, Vector2 position, Rectangle sourceRect)
    {
        _texture = texture;
        Position = position;
        SourceRect = sourceRect;
    }
    
    /// <summary>
    /// Construtor para textura individual (usa a imagem toda).
    /// Mais simples quando cada objeto é um arquivo separado.
    /// </summary>
    /// <param name="texture">A textura do objeto (imagem completa)</param>
    /// <param name="position">Onde colocar o objeto no mundo</param>
    public DecorObject(Texture2D texture, Vector2 position)
    {
        _texture = texture;
        Position = position;
        // Usa a textura inteira
        SourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
    }

    /// <summary>
    /// Desenha o objeto na tela. Simples assim.
    /// </summary>
    /// <param name="spriteBatch">O pincel do MonoGame</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, SourceRect, Color.White);
    }
}
