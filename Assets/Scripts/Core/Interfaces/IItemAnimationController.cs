using System;
using System.Collections.Generic;
using StickBlast.Models;
using UnityEngine;

namespace StickBlast.Core.Interfaces
{
    public interface IItemAnimationController
    {
        void HandleItemAnimation(List<Item> items, Transform[] itemPoints, Action onComplete);
    }
} 