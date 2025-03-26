using StickBlast.Models;

namespace StickBlast.Core.Interfaces
{
    public interface IItemSpawner
    {
        void HandleItemSpawn();
        void HandleItemDestruction(Item item);
    }
} 