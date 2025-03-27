using System;
using System.Collections.Generic;
using DG.Tweening;
using StickBlast;
using StickBlast.Core.Interfaces;
using StickBlast.Models;
using UnityEngine;
using UniRx;
using Ebleme;

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

        public IObservable<Unit> HandleItemAnimation(List<Item> items, Transform[] itemPoints)
        {
            return Observable.Create<Unit>(observer =>
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

                    // Jel benzeri scale up animasyonu
                    sequence.Append(item.transform.DOScale(Vector3.one * 1.2f, SPAWN_SCALE_DURATION * 0.5f)
                        .SetEase(Ease.OutBack));
                    sequence.Append(item.transform.DOScale(Vector3.one, SPAWN_SCALE_DURATION * 0.5f)
                        .SetEase(Ease.OutBack));

                    // Hareket animasyonu
                    sequence.Append(item.transform.DOMove(itemPoints[index].position, SPAWN_MOVE_DURATION)
                        .SetEase(Ease.OutQuad));

                    sequence.AppendInterval(0.1f);
                    index++;
                }

                sequence.OnComplete(() =>
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });

                return Disposable.Create(() => sequence.Kill());
            });
        }

        public IObservable<Unit> HandleItemSnap(Transform itemTransform, Vector3 targetPosition)
        {
            return Observable.Create<Unit>(observer =>
            {
                var tween = itemTransform.DOMove(targetPosition, SNAP_DURATION)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        observer.OnNext(Unit.Default);
                        observer.OnCompleted();
                    });

                return Disposable.Create(() => tween.Kill());
            });
        }

        public void HandleItemHover(Transform itemTransform, bool isHovering)
        {
            float targetScale = isHovering ? HOVER_SCALE : NORMAL_SCALE;
            itemTransform.DOScale(targetScale, 0.2f)
                .SetEase(Ease.OutBack);
        }

        public IObservable<Unit> HandleItemPlacement(Transform itemTransform, Vector3 targetPosition)
        {
            return Observable.Create<Unit>(observer =>
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

                placementSequence.OnComplete(() =>
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });

                return Disposable.Create(() => placementSequence.Kill());
            });
        }
    }
} 