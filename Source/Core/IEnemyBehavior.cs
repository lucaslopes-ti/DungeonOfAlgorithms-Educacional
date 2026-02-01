// ═══════════════════════════════════════════════════════════════════════════════
// INTERFACE DE COMPORTAMENTO DE INIMIGO - A Mente do Monstro
// ═══════════════════════════════════════════════════════════════════════════════
// Design Pattern: Strategy
// Esta interface define como um inimigo PENSA e AGE.
// Diferentes implementações = diferentes personalidades de monstros.
// É tipo Hogwarts, mas pra mobs: Slimes são da Lufa-Lufa, Ghosts da Sonserina.
// ═══════════════════════════════════════════════════════════════════════════════

using DungeonOfAlgorithms.Source.Entities;

namespace DungeonOfAlgorithms.Source.Core;

/// <summary>
/// Interface Strategy para comportamentos de inimigos.
/// Implemente isso pra criar monstros com personalidades diferentes.
/// </summary>
public interface IEnemyBehavior
{
    /// <summary>
    /// Atualiza o comportamento do inimigo a cada frame.
    /// Aqui é onde a IA (ou a falta dela) acontece.
    /// </summary>
    /// <param name="enemy">O inimigo que vai se comportar</param>
    /// <param name="player">O player (alvo principal de todo monstro)</param>
    /// <param name="gameTime">Tempo de jogo (pra calcular movimento)</param>
    void Update(Enemy enemy, Player player, Microsoft.Xna.Framework.GameTime gameTime);
}
