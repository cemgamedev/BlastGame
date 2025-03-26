using System;
using System.Collections.Generic;
using DG.Tweening;
using Ebleme;
using StickBlast.Core.Interfaces;
using StickBlast.Models;
using UnityEngine;

namespace StickBlast.Implementation
{
    public class ItemAnimationController : IItemAnimationController
    {
        private Sequence sequence;

        public ItemAnimationController()
        {
            sequence = DOTween.Sequence();
        }

        public void HandleItemAnimation(List<Item> items, Transform[] itemPoints, Action onComplete)
        {
            if (sequence == null)
            {
                sequence = DOTween.Sequence();
            }

            sequence.Kill();

            int index = 0;
            foreach (var item in items)
            {
                sequence.Append(item.transform.DOMove(itemPoints[index].position, GameConfigs.Instance.ItemMoveAnimDuration));
                sequence.AppendInterval(GameConfigs.Instance.ItemMoveAnimInterval);
                index++;
            }

            sequence.OnComplete(() => onComplete?.Invoke());
        }
    }
} 