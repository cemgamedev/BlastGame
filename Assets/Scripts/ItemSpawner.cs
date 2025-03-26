// maebleme2

using System;
using System.Collections.Generic;
using DG.Tweening;
using Ebleme;
using Ebleme.Utility;
using StickBlast;
using UnityEngine;
using StickBlast.Models;

namespace DefaultNamespace
{
    public interface IItemSpawner
    {
        void HandleItemSpawn();
        void HandleItemDestruction(Item item);
    }

    public interface IItemAnimationController
    {
        void HandleItemAnimation(List<Item> items, Transform[] itemPoints, Action onComplete);
    }

    public class ItemAnimationController : IItemAnimationController
    {
        private Sequence sequence;

        public ItemAnimationController()
        {
            // DOTween initialization moved to Awake
        }

        public void HandleInitialization()
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

    public class ItemSpawner : Singleton<ItemSpawner>, IItemSpawner
    {
        [SerializeField]
        private Transform spawnPoint;

        [SerializeField]
        private Transform[] itemPoints;

        private List<Item> items;
        private IItemAnimationController animationController;

        private void Awake()
        {
            animationController = new ItemAnimationController();
            ((ItemAnimationController)animationController).HandleInitialization();
        }

        private void Start()
        {
            if (CommonGameAssets.Instance == null)
            {
                Debug.LogError("CommonGameAssets is not initialized!");
                return;
            }
            HandleItemSpawn();
        }

        public void HandleItemSpawn()
        {
            if (CommonGameAssets.Instance == null)
            {
                Debug.LogError("CommonGameAssets is not initialized!");
                return;
            }

            var itemPrefabs = CommonGameAssets.Instance.GetRandomItems();
            if (itemPrefabs == null || itemPrefabs.Count == 0)
            {
                Debug.LogError("No item prefabs available!");
                return;
            }

            items = new List<Item>();

            foreach (var itemPrefab in itemPrefabs)
            {
                if (itemPrefab == null) continue;

                var item = Instantiate(itemPrefab);
                item.transform.position = spawnPoint.position;
                item.OnItemDestroyed += HandleItemDestruction;
                items.Add(item);
            }

            animationController.HandleItemAnimation(items, itemPoints, () =>
            {
                foreach (var item in items)
                    item.SetCanTouch();
            });
        }

        public void HandleItemDestruction(Item item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);

                if (items.Count == 0)
                {
                    HandleItemSpawn();
                }
            }
            else
            {
                Debug.LogError("Item should be in the list");
            }
        }
    }
}