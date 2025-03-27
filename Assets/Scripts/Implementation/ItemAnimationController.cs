using System;
using System.Collections.Generic;
using DG.Tweening;
using StickBlast;
using StickBlast.Core.Interfaces;
using StickBlast.Models;
using UnityEngine;
using UniRx;
using BlastRoot;
using Cysharp.Threading.Tasks;

namespace StickBlast.Implementation
{
    public class ItemAnimationController : IItemAnimationController
    {
        private Sequence sequence;
        private const float SPAWN_SCALE_DURATION = 0.4f;
        private const float SPAWN_MOVE_DURATION = 0.5f;
        private const float SNAP_DURATION = 0.2f;
        private const float HOVER_SCALE = 1.1f;
        private const float NORMAL_SCALE = 1f;
        private const float PLACEMENT_BOUNCE_HEIGHT = 0.8f;
        private const float PLACEMENT_BOUNCE_DURATION = 0.4f;

        private Vector3 spawnPoint;

        public ItemAnimationController()
        {
            spawnPoint = Vector3.zero;
        }

        public void HandleInitialization()
        {
            sequence = DOTween.Sequence();
        }

        public void SetSpawnPoint(Vector3 position)
        {
            spawnPoint = position;
        }

        public async UniTask HandleItemAnimationAsync(List<Item> items, Transform[] itemPoints)
        {
            if (sequence == null)
            {
                sequence = DOTween.Sequence();
            }

            sequence.Kill();

            int index = 0;
            foreach (var item in items)
            {
                // Spawn animasyonu
                item.transform.localScale = Vector3.zero;
                item.transform.position = spawnPoint;

                sequence.Join(item.transform.DOScale(Vector3.one, SPAWN_SCALE_DURATION)
                    .SetEase(Ease.OutBack));

                sequence.Join(item.transform.DOMove(itemPoints[index].position, SPAWN_MOVE_DURATION)
                    .SetEase(Ease.OutBack));

                index++;
            }

            await sequence.Play().AsyncWaitForCompletion();
        }

        public void HandleItemHover(Transform itemTransform, bool isHovering)
        {
            float targetScale = isHovering ? HOVER_SCALE : NORMAL_SCALE;
            itemTransform.DOScale(Vector3.one * targetScale, 0.2f).SetEase(Ease.OutBack);
        }

        public async UniTask HandleItemSnapAsync(Transform itemTransform, Vector3 targetPosition)
        {
            await itemTransform.DOMove(targetPosition, SNAP_DURATION)
                .SetEase(Ease.OutBack)
                .AsyncWaitForCompletion();
        }

        public async UniTask HandleItemPlacementAsync(Transform itemTransform, Vector3 targetPosition)
        {
            Sequence placementSequence = DOTween.Sequence();

            // İlk bounce
            placementSequence.Append(itemTransform.DOMove(targetPosition + Vector3.up * PLACEMENT_BOUNCE_HEIGHT, PLACEMENT_BOUNCE_DURATION * 0.4f)
                .SetEase(Ease.OutQuad));

            // İkinci bounce (daha küçük)
            placementSequence.Append(itemTransform.DOMove(targetPosition + Vector3.up * (PLACEMENT_BOUNCE_HEIGHT * 0.5f), PLACEMENT_BOUNCE_DURATION * 0.3f)
                .SetEase(Ease.OutQuad));

            // Son bounce (en küçük)
            placementSequence.Append(itemTransform.DOMove(targetPosition + Vector3.up * (PLACEMENT_BOUNCE_HEIGHT * 0.25f), PLACEMENT_BOUNCE_DURATION * 0.2f)
                .SetEase(Ease.OutQuad));

            // Son pozisyona gel
            placementSequence.Append(itemTransform.DOMove(targetPosition, PLACEMENT_BOUNCE_DURATION * 0.1f)
                .SetEase(Ease.InQuad));

            // Scale animasyonları
            placementSequence.Join(itemTransform.DOScale(Vector3.one * 1.2f, PLACEMENT_BOUNCE_DURATION * 0.2f)
                .SetEase(Ease.OutQuad));
            placementSequence.Join(itemTransform.DOScale(Vector3.one * 0.9f, PLACEMENT_BOUNCE_DURATION * 0.2f)
                .SetEase(Ease.InQuad)
                .SetDelay(PLACEMENT_BOUNCE_DURATION * 0.2f));
            placementSequence.Join(itemTransform.DOScale(Vector3.one, PLACEMENT_BOUNCE_DURATION * 0.2f)
                .SetEase(Ease.OutBack)
                .SetDelay(PLACEMENT_BOUNCE_DURATION * 0.4f));

            await placementSequence.Play().AsyncWaitForCompletion();
        }

        // Legacy Observable methods for backward compatibility
        public IObservable<Unit> HandleItemAnimation(List<Item> items, Transform[] itemPoints)
        {
            return HandleItemAnimationAsync(items, itemPoints).ToObservable().Select(_ => Unit.Default);
        }

        public IObservable<Unit> HandleItemSnap(Transform itemTransform, Vector3 targetPosition)
        {
            return HandleItemSnapAsync(itemTransform, targetPosition).ToObservable().Select(_ => Unit.Default);
        }

        public IObservable<Unit> HandleItemPlacement(Transform itemTransform, Vector3 targetPosition)
        {
            return HandleItemPlacementAsync(itemTransform, targetPosition).ToObservable().Select(_ => Unit.Default);
        }
    }
} 