namespace Flow
{
    /// Carries player selections across the scene boundary (menu → loading → gameplay).
    public static class GameSession
    {
        public static GameMode SelectedMode { get; private set; } = GameMode.Normal;
        public static bool CoopRequested { get; private set; }

        public static void SelectMode(GameMode mode) => SelectedMode = mode;
        public static void SetCoop(bool coop) => CoopRequested = coop;
    }
}
