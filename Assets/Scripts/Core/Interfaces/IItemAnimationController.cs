using System;
using System.Collections.Generic;
using StickBlast.Models;
using UnityEngine;
using UniRx;

namespace StickBlast.Core.Interfaces
{
    public interface IItemAnimationController
    {
        void HandleInitialization();
        void SetSpawnPoint(Vector3 position);
        IObservable<Unit> HandleItemAnimation(List<Item> items, Transform[] itemPoints);
        IObservable<Unit> HandleItemSnap(Transform itemTransform, Vector3 targetPosition);
        void HandleItemHover(Transform itemTransform, bool isHovering);
        IObservable<Unit> HandleItemPlacement(Transform itemTransform, Vector3 targetPosition);
    }
} 