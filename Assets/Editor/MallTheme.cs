using UnityEngine;

namespace KrazyKartEditor
{
    /// Per-floor art and content description used by MallBuilder. Keeping this as plain data
    /// means retheming a floor never touches the geometry code.
    public class MallTheme
    {
        public Core.FloorId Id;
        public string DisplayName;
        public Color FloorColor;
        public Color WallColor;
        public Color AccentColor;
        public bool CopsEnabled;
        public int FirstCopAt;
        public int SecondCopAt;
        public int FormationAt;

        /// Large obstacles laid out in aisles. Higher MC, act as the destruction clusters.
        public string[] LargeProps;
        /// Small scatter dressing. Cheap MC, keeps chains alive between the big hits.
        public string[] SmallProps;
        /// How dense this floor is, 0..1. Scales from sparse B1 to packed F7.
        public float Density;

        private const string PropRoot = "Assets/ThirdPartyAssets/SimpleShopInteriors/_prefabs/Props/";

        public static string Path(string propName) => PropRoot + propName + ".prefab";

        /// Bottom-to-top running order of the mall, following LD_floors.md.
        public static MallTheme[] All()
        {
            return new[]
            {
                new MallTheme
                {
                    Id = Core.FloorId.B1, DisplayName = "B1 · Basement Parking",
                    FloorColor = new Color(0.24f, 0.24f, 0.26f),
                    WallColor = new Color(0.32f, 0.32f, 0.34f),
                    AccentColor = new Color(0.95f, 0.78f, 0.2f),
                    // Tutorial floor: teaches the impact loop with nothing chasing you.
                    CopsEnabled = false, FirstCopAt = 99, SecondCopAt = 99, FormationAt = 99,
                    LargeProps = new[] { "SI_Prop_Container_01", "SI_Prop_TrashBins_01", "SI_Prop_ConcreteBench_01" },
                    SmallProps = new[] { "SI_Prop_CarboardBox_01", "SI_Prop_CarboardBox_02", "SI_Prop_SignCaution_01", "SI_Prop_RubbishBin_01" },
                    Density = 0.45f
                },
                new MallTheme
                {
                    Id = Core.FloorId.F1, DisplayName = "F1 · Food Court",
                    FloorColor = new Color(0.62f, 0.5f, 0.38f),
                    WallColor = new Color(0.86f, 0.72f, 0.55f),
                    AccentColor = new Color(1f, 0.45f, 0.2f),
                    CopsEnabled = true, FirstCopAt = 4, SecondCopAt = 9, FormationAt = 16,
                    LargeProps = new[] { "SI_Prop_ChairTables_01", "SI_Prop_Table_01", "SI_Prop_KitchenCounter_01", "SI_Prop_Coffee_Kiosk", "SI_Prop_Hotdog_Kiosk" },
                    SmallProps = new[] { "SI_Prop_FastfoodTray_01", "SI_Prop_CupStack_01", "SI_Prop_Plate_Stack_01", "SI_Prop_Chair_01", "SI_Prop_Stool" },
                    Density = 0.6f
                },
                new MallTheme
                {
                    Id = Core.FloorId.F2, DisplayName = "F2 · Fashion & Clothing",
                    FloorColor = new Color(0.55f, 0.5f, 0.58f),
                    WallColor = new Color(0.88f, 0.84f, 0.9f),
                    AccentColor = new Color(0.95f, 0.3f, 0.6f),
                    CopsEnabled = true, FirstCopAt = 4, SecondCopAt = 8, FormationAt = 14,
                    LargeProps = new[] { "SI_Prop_ClothesRack_01", "SI_Prop_ClothesRack_02", "SI_Prop_Shirt_Shelf_01", "SI_Prop_ShoeShelf_01" },
                    SmallProps = new[] { "SI_Prop_Mannequin_01", "SI_Prop_Mannequin_02", "SI_Prop_Shoe_Box_01", "SI_Prop_Shirts_Table_01" },
                    Density = 0.65f
                },
                new MallTheme
                {
                    Id = Core.FloorId.F3, DisplayName = "F3 · Supermarket",
                    FloorColor = new Color(0.7f, 0.72f, 0.7f),
                    WallColor = new Color(0.9f, 0.93f, 0.9f),
                    AccentColor = new Color(0.25f, 0.8f, 0.4f),
                    CopsEnabled = true, FirstCopAt = 3, SecondCopAt = 7, FormationAt = 13,
                    LargeProps = new[] { "SI_Prop_Shelf_Isle_Preset_01", "SI_Prop_Shelf_Isle_Preset_02", "SI_Prop_ProduceSection_01", "SI_Prop_Freezer_01" },
                    SmallProps = new[] { "SI_Prop_ShoppingTrolley_01", "SI_Prop_ShoppingTrolley_03", "SI_Prop_Basket_Stack_01", "SI_Prop_Shelf_01" },
                    Density = 0.72f
                },
                new MallTheme
                {
                    Id = Core.FloorId.F4_5, DisplayName = "F4 · Electronics",
                    FloorColor = new Color(0.3f, 0.34f, 0.42f),
                    WallColor = new Color(0.5f, 0.58f, 0.72f),
                    AccentColor = new Color(0.3f, 0.75f, 1f),
                    CopsEnabled = true, FirstCopAt = 3, SecondCopAt = 6, FormationAt = 11,
                    LargeProps = new[] { "SI_Prop_LaptopShelf_01", "SI_Prop_RecordShelf_01", "SI_Prop_Vending_Machine", "SI_Prop_Stereo_02" },
                    SmallProps = new[] { "SI_Prop_Monitor_01", "SI_Prop_HeadPhoneStand", "SI_Prop_CD_Holder", "SI_Prop_CD_Case" },
                    Density = 0.75f
                },
                new MallTheme
                {
                    Id = Core.FloorId.F6, DisplayName = "F6 · Sports & Toys",
                    FloorColor = new Color(0.35f, 0.5f, 0.45f),
                    WallColor = new Color(0.7f, 0.85f, 0.8f),
                    AccentColor = new Color(1f, 0.85f, 0.2f),
                    CopsEnabled = true, FirstCopAt = 3, SecondCopAt = 6, FormationAt = 10,
                    LargeProps = new[] { "SI_Prop_ArcadeMachine_01", "SI_Prop_Arcade_ClawMachine_01", "SI_Prop_Arcade_AirHockey", "SI_Prop_Arcade_Pinball", "SI_Prop_Arcade_SkeeballMachine_01" },
                    SmallProps = new[] { "SI_Prop_Toy_Bear_01", "SI_Prop_Toy_Dog_01", "SI_Prop_Toy_Pig_01", "SI_Prop_Toy_Rabbit_01", "SI_Prop_Toy_Turtle_01" },
                    Density = 0.8f
                },
                new MallTheme
                {
                    Id = Core.FloorId.F7, DisplayName = "F7 · Home & Furniture",
                    FloorColor = new Color(0.5f, 0.42f, 0.35f),
                    WallColor = new Color(0.82f, 0.76f, 0.68f),
                    AccentColor = new Color(1f, 0.35f, 0.15f),
                    CopsEnabled = true, FirstCopAt = 2, SecondCopAt = 5, FormationAt = 9,
                    LargeProps = new[] { "SI_Prop_Cabinet_01", "SI_Prop_Table_01", "SI_Prop_Bench_01", "SI_Prop_Counter_01" },
                    SmallProps = new[] { "SI_Prop_Chair_01", "SI_Prop_Plant_06", "SI_Prop_Plant_12", "SI_Prop_WallDeco_01" },
                    Density = 0.85f
                },
                new MallTheme
                {
                    Id = Core.FloorId.Boss, DisplayName = "MANAGER'S OFFICE",
                    FloorColor = new Color(0.2f, 0.18f, 0.2f),
                    WallColor = new Color(0.3f, 0.26f, 0.28f),
                    AccentColor = new Color(1f, 0.2f, 0.15f),
                    // The clock is the threat here, not cops.
                    CopsEnabled = false, FirstCopAt = 99, SecondCopAt = 99, FormationAt = 99,
                    LargeProps = new[] { "SI_Prop_Safe_01", "SI_Prop_Counter_01", "SI_Prop_Cabinet_01" },
                    SmallProps = new[] { "SI_Prop_Briefcase", "SI_Prop_MoneyStack_01", "SI_Prop_Chair_01" },
                    Density = 0.5f
                }
            };
        }
    }
}
