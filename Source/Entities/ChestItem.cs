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
/// O baú do tesouro - objetivo final do jogo!
/// Coletar isso = VITÓRIA! 🏆
/// </summary>
public class ChestItem : Item
{
    /// <summary>Se o bau esta destrancado (todas as moedas coletadas)</summary>
    public bool IsUnlocked { get; set; } = false;

    /// <summary>
    /// Cria um novo baú do tesouro.
    /// ID 999 porque ele é lendário, único, especial.
    /// </summary>
    public ChestItem(Texture2D texture, Vector2 position)
        : base(999, "Treasure Chest", texture, position)
    {
    }
}
