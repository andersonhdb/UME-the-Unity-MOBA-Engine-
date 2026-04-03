namespace UME.Core
{
    /// <summary>
    /// Represents the high-level phase the game is currently in.
    /// </summary>
    public enum GamePhase
    {
        None,
        Launch,
        MainMenu,
        Lobby,
        HeroBanning,
        HeroSelection,
        GameConfig,
        Loading,
        InGame,
        PostGame,
        Training,
        MapCreator
    }
}
