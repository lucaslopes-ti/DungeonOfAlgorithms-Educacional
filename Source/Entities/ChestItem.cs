// ═══════════════════════════════════════════════════════════════════════════════
// 📦 CHEST ITEM - O Baú do Tesouro (O Grande Prêmio!)
// ═══════════════════════════════════════════════════════════════════════════════
// Este é o baú que todo mundo quer achar. É tipo achar dinheiro no bolso
// da calça que você não lavava há 3 meses. Satisfação pura.
// Herda de Item porque também é coletável, mas é ESPECIAL.
// ═══════════════════════════════════════════════════════════════════════════════

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonOfAlgorithms.Source.Core;

namespace DungeonOfAlgorithms.Source.Entities;

/// <summary>
/// O bau do tesouro - objetivo final do jogo!
/// Usa apenas um frame da spritesheet (16x24).
/// </summary>
public class ChestItem : Item
{
    private const int FRAME_W = 16;
    private const int FRAME_H = 24;

    /// <summary>Se o bau esta destrancado (todas as moedas coletadas)</summary>
    public bool IsUnlocked { get; set; } = false;

    /// <summary>Hitbox baseada no tamanho de um frame</summary>
    public override Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, FRAME_W, FRAME_H);

    public ChestItem(Texture2D texture, Vector2 position)
        : base(999, "Treasure Chest", texture, position)
    {
    }

    /// <summary>
    /// Desenha apenas um frame da spritesheet.
    /// Frame 0 = fechado, frame 2 = aberto.
    /// </summary>
    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;
        int frame = IsUnlocked ? 2 : 0;
        var src = new Rectangle(frame * FRAME_W, 0, FRAME_W, FRAME_H);
        spriteBatch.Draw(_texture, new Vector2(Position.X, Position.Y + _bobOffset), src, Color.White);
    }
}
