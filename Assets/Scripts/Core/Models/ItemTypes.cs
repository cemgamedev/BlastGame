namespace StickBlast.Models
{
    /// <summary>
    /// Represents different types of items in the game
    /// </summary>
    public enum ItemTypes
    {
        None = -1,
        
        // Single line items
        I = 10,
        FlatI = 20,
        I2 = 30,
        FlatI2 = 40,

        // L-shaped items
        RightBottomL = 50,
        LeftBottomL = 60,
        LeftTopL = 70,
        RightTopL = 80,

        // Double L-shaped items
        BottomLeftL2 = 90,
        BottomRightL2 = 100,
        LeftBottomL2 = 110,
        LeftTopL2 = 120,
        RightBottomL2 = 130,
        RightTopL2 = 140,
        TopLeftL2 = 150,
        TopRightL2 = 160,

        // U-shaped items
        LeftU = 170,
        TopU = 180,
        RightU = 190,
        BottomU = 200
    }
} 