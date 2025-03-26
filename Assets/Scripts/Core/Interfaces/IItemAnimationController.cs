using System;
using System.Collections.Generic;
using StickBlast.Models;
using UnityEngine;

namespace StickBlast.Core.Interfaces
{
    public interface IItemAnimationController
    {
        void HandleItemAnimation(List<Item> items, Transform[] itemPoints, Action onComplete);
        void HandleItemSnap(Transform itemTransform, Vector3 targetPosition, Action onComplete = null);
        void HandleItemHover(Transform itemTransform, bool isHovering);
        void HandleItemPlacement(Transform itemTransform, Vector3 targetPosition, Action onComplete = null);
    }
} 