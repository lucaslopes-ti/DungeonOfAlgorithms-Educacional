// ═══════════════════════════════════════════════════════════════════════════════
// 💰 ITEM - Coisas Brilhantes que Todo Mundo Quer
// ═══════════════════════════════════════════════════════════════════════════════
// A classe base para todos os itens coletáveis do jogo.
// Moedas, poções, chaves... tudo que você pode pegar é um Item.
// Implementa IGameEntity porque precisa de Update e Draw.
// É tipo dinheiro: aparece, você pega, e ele some (da tela, não do score).
// ═══════════════════════════════════════════════════════════════════════════════

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonOfAlgorithms.Source.Core;

/// <summary>
/// Classe base para itens coletáveis. Moedas, baús, etc.
/// </summary>
public class Item : IGameEntity
{
    /// <summary>ID único do item (tipo CPF de moeda)</summary>
    public int Id { get; private set; }
    
    /// <summary>Nome do item (pra mostrar no inventário um dia)</summary>
    public string Name { get; private set; }
    
    /// <summary>Posição no mundo</summary>
    public Vector2 Position { get; private set; }
    
    /// <summary>
    /// Se o item ainda está ativo (visível/coletável).
    /// Vira false quando o player coleta.
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>Quanto vale em pontos. Dinheiro = felicidade.</summary>
    public int Value { get; private set; } = 10;
    
    // 🖼️ Textura do item
    private Texture2D _texture;
    
    /// <summary>
    /// Área de colisão do item. 16x16 pixels padrão.
    /// Se o player encostar, coleta!
    /// </summary>
    public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, 16, 16);

    /// <summary>
    /// Cria um novo item colecionável.
    /// </summary>
    /// <param name="id">ID único</param>
    /// <param name="name">Nome do item</param>
    /// <param name="texture">Sprite do item</param>
    /// <param name="position">Onde spawnar no mundo</param>
    public Item(int id, string name, Texture2D texture, Vector2 position)
    {
        Id = id;
        Name = name;
        _texture = texture;
        Position = position;
    }

    /// <summary>
    /// Atualiza o item. Por enquanto não faz nada,
    /// mas poderia ter animação de flutuar/brilhar.
    /// </summary>
    public void Update(GameTime gameTime)
    {
        // TODO: Animação de bobbing (subir e descer suavemente)
        // float yOffset = (float)System.Math.Sin(gameTime.TotalGameTime.TotalSeconds * 5) * 2;
        // Isso faria o item flutuar, mas por enquanto fica parado mesmo
    }

    /// <summary>
    /// Desenha o item na tela (se ainda estiver ativo).
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        // Só desenha se ainda não foi coletado
        if (IsActive)
            spriteBatch.Draw(_texture, Position, Color.White);
    }
}
