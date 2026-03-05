using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonOfAlgorithms.Source.Entities;

namespace DungeonOfAlgorithms.Source.Core;

public class HUD
{
    private SpriteFont _font;

    public HUD(SpriteFont font)
    {
        _font = font;
    }

    public void Draw(SpriteBatch spriteBatch, Player player, Texture2D pixelTexture, Room currentRoom)
    {
        int collected = DungeonManager.Instance.CollectedCoins;
        int total = DungeonManager.Instance.TotalCoins;
        bool allCollected = DungeonManager.Instance.AllCoinsCollected;

        // Background semi-transparente do HUD (maior para caber moedas)
        spriteBatch.Draw(pixelTexture, new Rectangle(5, 5, 220, 95), Color.Black * 0.5f);
        spriteBatch.Draw(pixelTexture, new Rectangle(5, 5, 220, 1), Color.Gold * 0.3f);
        spriteBatch.Draw(pixelTexture, new Rectangle(5, 99, 220, 1), Color.Gold * 0.3f);
        spriteBatch.Draw(pixelTexture, new Rectangle(5, 5, 1, 95), Color.Gold * 0.3f);
        spriteBatch.Draw(pixelTexture, new Rectangle(224, 5, 1, 95), Color.Gold * 0.3f);

        // Barra de HP
        string hpLabel = "HP";
        spriteBatch.DrawString(_font, hpLabel, new Vector2(12, 12), Color.White);

        int barX = 40;
        int barY = 14;
        int barWidth = 170;
        int barHeight = 12;
        float hpPercent = player.Health / 100f;

        spriteBatch.Draw(pixelTexture, new Rectangle(barX, barY, barWidth, barHeight), Color.DarkRed * 0.8f);
        Color hpColor = hpPercent > 0.5f ? Color.LimeGreen : (hpPercent > 0.25f ? Color.Yellow : Color.Red);
        spriteBatch.Draw(pixelTexture, new Rectangle(barX, barY, (int)(barWidth * hpPercent), barHeight), hpColor);
        spriteBatch.Draw(pixelTexture, new Rectangle(barX, barY, barWidth, 1), Color.White * 0.3f);
        spriteBatch.Draw(pixelTexture, new Rectangle(barX, barY + barHeight - 1, barWidth, 1), Color.White * 0.3f);

        // Score
        string scoreText = $"Score: {player.Score}";
        spriteBatch.DrawString(_font, scoreText, new Vector2(13, 34), Color.Gold);

        // Contador de moedas
        Color coinColor = allCollected ? Color.LimeGreen : Color.Gold;
        string coinText = $"Coins: {collected}/{total}";
        spriteBatch.DrawString(_font, coinText, new Vector2(13, 56), coinColor);

        // Nome da sala
        string[] roomNames = { "", "The Stack", "The Heap", "Kernel Panic" };
        string roomName = currentRoom.Id >= 1 && currentRoom.Id <= 3 ? roomNames[currentRoom.Id] : $"Room {currentRoom.Id}";
        string roomText = $"Room {currentRoom.Id}: {roomName}";
        spriteBatch.DrawString(_font, roomText, new Vector2(13, 76), Color.LightGray * 0.8f);
    }
}
