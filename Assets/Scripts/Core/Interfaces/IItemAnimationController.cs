using System;
using System.Collections.Generic;
using StickBlast.Models;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

namespace StickBlast.Core.Interfaces
{
    public interface IItemAnimationController
    {
        void HandleInitialization();
        void SetSpawnPoint(Vector3 position);
        
        // Async methods
        UniTask HandleItemAnimationAsync(List<Item> items, Transform[] itemPoints);
        UniTask HandleItemSnapAsync(Transform itemTransform, Vector3 targetPosition);
        UniTask HandleItemPlacementAsync(Transform itemTransform, Vector3 targetPosition);
        
        // Legacy Observable methods
        IObservable<Unit> HandleItemAnimation(List<Item> items, Transform[] itemPoints);
        IObservable<Unit> HandleItemSnap(Transform itemTransform, Vector3 targetPosition);
        IObservable<Unit> HandleItemPlacement(Transform itemTransform, Vector3 targetPosition);
        
        void HandleItemHover(Transform itemTransform, bool isHovering);
    }
} 