using UnityEngine;

namespace StickBlast.Core.Interfaces
{
    public interface IDraggable
    {
        void OnDragStart(Vector2 position);
        void OnDrag(Vector2 position);
        void OnDragEnd(Vector2 position);
    }
} 