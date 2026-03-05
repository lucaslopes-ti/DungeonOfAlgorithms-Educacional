namespace DungeonOfAlgorithms.Source.Core;

public class GameManager
{
    private static GameManager _instance;
    public static GameManager Instance => _instance ??= new GameManager();

    private GameManager() { }

    public void Initialize()
    {
    }
}
