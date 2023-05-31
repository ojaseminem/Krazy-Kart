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

    public enum LevelDifficulty
    {
        Easy,
        Average,
        Hard
    }

    //UI
    public enum Windows
    {
        Briefing,
        GameUi,
        Task,
        GameOver,
    }

    public enum ItemType
    {
        Bananas,
        Watermelon,
        Pear,
        Pineapple,
        Eggs,
        Ketchup,
        MilkBottles,
        WineBottles,
        Pickle,
        Colgate,
        ChewingGum,
        Cornflakes,
        Meat,
        ChickToy,
        StuffedToy,
        Headphones,
        Boots,
        Cake,
        Cookies,
        FoodTray,
        NeckPiece,
        Pant,
        Pastries,
        Rings,
        Shoes,
        Slippers,
        Shirt,
        Watches,
        WaterBottles,
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