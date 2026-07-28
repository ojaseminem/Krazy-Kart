namespace Flow
{
    public enum GameMode
    {
        /// Full run: B1 → floors → boss escape. MC converts to escape time.
        Normal,

        /// No cops, no boss timer. Free destruction sandbox.
        FreeRun
    }

    public enum MenuPanelType
    {
        Settings,
        Coop,
        Socials,
        Credits,
        Quit
    }
}
