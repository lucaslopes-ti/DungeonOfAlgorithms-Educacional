// ═══════════════════════════════════════════════════════════════════════════════
// 👹 ENEMY - Os Vilões do Dungeon
// ═══════════════════════════════════════════════════════════════════════════════
// Design Pattern: Strategy (comportamento intercambiável)
// Inimigos são as criaturas que tentam te matar. Slimes, Ghosts, etc.
// Cada um tem um comportamento diferente (definido por IEnemyBehavior).
// Eles têm animação, causam dano e colidem com paredes.
// São tipo os NPCs do trabalho que te atrapalham.
// ═══════════════════════════════════════════════════════════════════════════════

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using DungeonOfAlgorithms.Source.Core;

namespace DungeonOfAlgorithms.Source.Entities;

/// <summary>
/// Classe base para inimigos. Usa Strategy Pattern pro comportamento.
/// Cada tipo de inimigo pode ter IA diferente.
/// </summary>
public class Enemy : IGameEntity
{
    // ═══════════════════════════════════════════════════════════════════════════
    // 📊 PROPRIEDADES PÚBLICAS
    // ═══════════════════════════════════════════════════════════════════════════
    
    /// <summary>ID único do inimigo (tipo CPF de monstro)</summary>
    public int Id { get; private set; }
    
    /// <summary>Posição no mundo. Set público pra o behavior poder mudar.</summary>
    public Vector2 Position { get; set; }
    
    /// <summary>Velocidade de movimento (mais lenta que o player, senão é injusto)</summary>
    public float Speed { get; set; } = 50f;
    
    /// <summary>Quanto dano causa quando encosta no player</summary>
    public int Damage { get; set; } = 10;
    
    // ═══════════════════════════════════════════════════════════════════════════
    // 🎨 ANIMAÇÃO
    // ═══════════════════════════════════════════════════════════════════════════
    
    // Dicionário com texturas de animação
    private Dictionary<string, Texture2D> _textures;
    private string _currentAnimation;
    private int _currentFrame;
    private double _timer;
    
    private const double FRAME_TIME = 0.15;   // Velocidade da animação
    private const int FRAME_WIDTH = 32;        // Largura de cada frame
    private const int FRAME_HEIGHT = 32;       // Altura de cada frame
    
    private SpriteEffects _flipEffect = SpriteEffects.None;
    
    // ═══════════════════════════════════════════════════════════════════════════
    // STATE
    // ═══════════════════════════════════════════════════════════════════════════
    
    /// <summary>Direções que o inimigo pode olhar</summary>
    private enum FaceDirection { Down, Up, Left, Right }
    private FaceDirection _facing = FaceDirection.Down;
    private bool _isMoving = false;

    /// <summary>
    /// O comportamento atual do inimigo (Strategy Pattern).
    /// Define como o inimigo age: patrulha, persegue, etc.
    /// </summary>
    private IEnemyBehavior _behavior;
    
    /// <summary>
    /// Área de colisão do inimigo. 24x24, um pouco menor que o sprite.
    /// Centralizada no sprite de 32x32.
    /// </summary>
    public Rectangle Bounds => new Rectangle((int)Position.X + 4, (int)Position.Y + 4, 24, 24);

    /// <summary>
    /// Cria um novo inimigo.
    /// </summary>
    /// <param name="id">ID único</param>
    /// <param name="textures">Texturas de animação</param>
    /// <param name="startPosition">Posição inicial</param>
    /// <param name="behavior">Comportamento (Strategy)</param>
    public Enemy(int id, Dictionary<string, Texture2D> textures, Vector2 startPosition, IEnemyBehavior behavior)
    {
        Id = id;
        _textures = textures;
        Position = startPosition;
        _behavior = behavior;
        _currentAnimation = "Down_Idle";
    }

    /// <summary>
    /// Muda o comportamento do inimigo em runtime.
    /// Útil pra fazer um monstro calmo virar agressivo.
    /// </summary>
    public void ChangeBehavior(IEnemyBehavior newBehavior)
    {
        _behavior = newBehavior;
    }

    /// <summary>
    /// Update básico (exigido pela interface, mas não usado).
    /// </summary>
    public void Update(GameTime gameTime)
    {
        // Implementação vazia - usamos o overload com Player e Tilemap
    }
    
    /// <summary>
    /// Update completo com referência ao Player e Tilemap.
    /// </summary>
    /// <param name="gameTime">Tempo de jogo</param>
    /// <param name="player">O player (alvo da maioria dos comportamentos)</param>
    /// <param name="tilemap">O mapa (pra colisão com paredes)</param>
    public void Update(GameTime gameTime, Player player, Tilemap tilemap = null)
    {
        // Guarda posição anterior (pra reverter se bater na parede)
        Vector2 prevPos = Position;
        
        // Executa o comportamento (Strategy Pattern em ação!)
        _behavior.Update(this, player, gameTime);
        
        // Verifica colisão com paredes - reverte se bateu
        if (tilemap != null && tilemap.IsColliding(Bounds))
        {
            Position = prevPos; // "Ops, parede! Volta!"
        }
        
        // Calcula se moveu pra atualizar animação
        Vector2 delta = Position - prevPos;
        _isMoving = delta.LengthSquared() > 0.0001f;
        
        // 🎨 Atualiza direção baseado no movimento
        if (_isMoving)
        {
            // Determina direção principal do movimento
            if (Math.Abs(delta.X) > Math.Abs(delta.Y))
            {
                // Movimento horizontal é maior
                _facing = delta.X > 0 ? FaceDirection.Right : FaceDirection.Left;
            }
            else
            {
                // Movimento vertical é maior
                _facing = delta.Y > 0 ? FaceDirection.Down : FaceDirection.Up;
            }
        }
        
        UpdateAnimation(gameTime);
    }
    
    /// <summary>
    /// Atualiza a animação baseado na direção e estado de movimento.
    /// </summary>
    private void UpdateAnimation(GameTime gameTime)
    {
        _flipEffect = SpriteEffects.None;
        string suffix = _isMoving ? "" : "_Idle";
        
        // Escolhe a animação baseado na direção
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
                _flipEffect = SpriteEffects.FlipHorizontally; // Espelha pra direita
                break;
            case FaceDirection.Left:
                _currentAnimation = "Side" + suffix;
                _flipEffect = SpriteEffects.None;
                break;
        }
        
        // Avança os frames
        _timer += gameTime.ElapsedGameTime.TotalSeconds;
        if (_timer >= FRAME_TIME)
        {
            _timer = 0;
            _currentFrame++;
            if (_currentFrame >= 6) // 6 frames por animação
                _currentFrame = 0;
        }
    }

    /// <summary>
    /// Desenha o inimigo na tela.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        // Tenta usar a animação atual, senão usa fallbacks
        string animToPlay = _currentAnimation;
        if (!_textures.ContainsKey(animToPlay)) animToPlay = "Down_Idle";
        if (!_textures.ContainsKey(animToPlay)) animToPlay = "Down";
        
        // Só desenha se tiver a textura
        if (_textures.ContainsKey(animToPlay))
        {
            Texture2D texture = _textures[animToPlay];
            Rectangle sourceRect = new Rectangle(_currentFrame * FRAME_WIDTH, 0, FRAME_WIDTH, FRAME_HEIGHT);
            spriteBatch.Draw(texture, Position, sourceRect, Color.White, 0f, Vector2.Zero, 1f, _flipEffect, 0f);
        }
    }
}
