namespace StickBlast.Models
{
    /// <summary>
    /// Represents different color states for items in the game
    /// </summary>
    public enum ColorTypes
    {
        /// <summary>
        /// Default state when item is not interacting
        /// </summary>
        ItemStill,

        /// <summary>
        /// State when item is not active but can be interacted with
        /// </summary>
        Passive,

        /// <summary>
        /// State when item is being hovered over
        /// </summary>
        Hover,

        /// <summary>
        /// State when item is currently active
        /// </summary>
        Active
    }
} 