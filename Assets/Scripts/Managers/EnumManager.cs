namespace Managers
{
    public class EnumManager
    {

    }

    public enum GameState
    {
        PreRequisites,
        Briefing,
        Playing,
        Task,
        ScoreCalculation,
        GameOver,
    }

    public enum MallState
    {
        Empty,
        Constructing,
        Transitioning,
        ReachedManagerFloor,
        Deconstructing
    }

    public enum FloorTriggerType
    {
        PassThrough,
        EntryTrigger,
        ExitTrigger
    }

    public enum TaskState
    {
        PreTask,
        MidTask,
        PostTask
    }

    public enum LevelDifficulty
    {
        Easy,
        Average,
        Hard
    }

    public enum EconomyType
    {
        Coin,
        Token
    }

    //UI
    public enum Windows
    {
        Briefing,
        GameUi,
        Task,
        GameOver,
    }

    public enum Popups
    {
        ObjectSelectionConfirmation
    }

    public enum StoreType
    {
        Groceries,
        Arcade,
        Music,
        FastFood,
        Jewelry,
        Clothing,
        Bakery
    }

    public enum AdsType
    {
        Interstitial,
        Rewarded,
        Banner
    }
}