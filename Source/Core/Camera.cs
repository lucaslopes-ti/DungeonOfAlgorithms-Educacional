using Microsoft.Xna.Framework;

namespace DungeonOfAlgorithms.Source.Core;

/// <summary>
/// A câmera do jogo. Segue o player com dedicação de cachorro.
/// </summary>
public class Camera
{
    /// <summary>
    /// A matriz de transformação - matemática pesada que faz a mágica acontecer.
    /// Basicamente diz ao MonoGame onde desenhar tudo.
    /// </summary>
    public Matrix Transform { get; private set; }
    
    /// <summary>
    /// Nível de zoom. 3x porque somos míopes e pixel art é minúsculo.
    /// Aumente se precisar de óculos, diminua se tiver visão de águia.
    /// </summary>
    public float Zoom { get; set; } = 3.0f;

    /// <summary>
    /// Faz a câmera seguir um alvo (geralmente o player, a não ser que você seja masoquista).
    /// </summary>
    /// <param name="target">Posição do alvo a ser seguido</param>
    /// <param name="screenWidth">Largura da tela em pixels</param>
    /// <param name="screenHeight">Altura da tela em pixels</param>
    public void Follow(Vector2 target, int screenWidth, int screenHeight)
    {
        // Move tudo na direção OPOSTA ao alvo (assim ele fica no centro)
        var position = Matrix.CreateTranslation(
            -target.X,
            -target.Y,
            0);

        // Coloca o "ponto de vista" no centro da tela
        var offset = Matrix.CreateTranslation(
            screenWidth / 2,
            screenHeight / 2,
            0);
        
        // Aplica o zoom (faz tudo ficar GRANDE)
        var scale = Matrix.CreateScale(Zoom, Zoom, 1);

        // Combina tudo numa matrix só - ordem importa!
        // Primeiro posiciona, depois escala, depois centraliza
        Transform = position * scale * offset;
    }
}
