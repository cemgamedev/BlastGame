using UnityEngine;

namespace StickBlast.Core.Interfaces
{
    public interface IMoveable
    {
        bool CanMove { get; set; }
        void OnStartMove();
        void OnMove(Vector2 position);
        void OnEndMove();
        RaycastHit2D Hit();
    }
} 