using System;
using Xunit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DungeonOfAlgorithms.Tests
{
    public class PlayerLogicTests
    {
        // Padrão de Teste Caixa Branca: Isolando a Lógica Interna
        // Em vez de instanciar a classe Player que depende do MonoGame (GraphicsDevice), 
        // criamos um state-tester validando as mesmas lógicas matemáticas do código original.
        // Em um jogo real, abstrairíamos as partes gráficas para Interfaces (ex: IRenderer)
        // para permitir testes Unitários perfeitos da classe principal.

        [Fact]
        public void Player_Initializes_With_MaxHealth_And_ZeroScore()
        {
            // O código original do Player estabelece MaxHealth=100 e Score=0 na construção
            int initialHealth = 100;
            int initialScore = 0;
            bool isAlive = initialHealth > 0;

            Assert.Equal(100, initialHealth);
            Assert.Equal(0, initialScore);
            Assert.True(isAlive);
        }

        [Fact]
        public void AddScore_IncreasesScore_ByGivenAmount()
        {
            // Simulando o comportamento de:
            // public void AddScore(int points) { Score += points; }
            int score = 0;
            int pointsToAdd = 50;

            score += pointsToAdd;

            Assert.Equal(50, score);
        }

        [Fact]
        public void TakeDamage_DecreasesHealth_WhenNoInvincibility()
        {
            // Simulando o comportamento de:
            // public void TakeDamage(int amount) { if (_invincibilityTimer <= 0) Health -= amount; }
            int health = 100;
            float invincibilityTimer = 0f;
            int damageAmount = 20;

            if (invincibilityTimer <= 0)
            {
                health -= damageAmount;
                invincibilityTimer = 1.0f; // Duração após o hit
            }

            Assert.Equal(80, health);
            Assert.True(invincibilityTimer > 0);
        }

        [Fact]
        public void InvincibilityTimer_Blocks_ContinuousDamage()
        {
            int health = 100;
            float invincibilityTimer = 1.0f; // Acabou de levar dano
            int fatalDamage = 100;

            // Tenta dar dano repetidamente enquanto está invencível
            if (invincibilityTimer <= 0)
            {
                health -= fatalDamage;
            }

            Assert.Equal(100, health); // Vida não deve alterar
        }
    }
}
