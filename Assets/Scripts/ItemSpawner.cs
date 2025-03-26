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
        void SpawnItems();
        void OnItemDestroyed(Item item);
    }

    public interface IItemAnimationController
    {
        void AnimateItems(List<Item> items, Transform[] itemPoints, Action onComplete);
    }

    public class ItemAnimationController : IItemAnimationController
    {
        private Sequence sequence;

        public ItemAnimationController()
        {
            // DOTween initialization moved to Awake
        }

        public void Initialize()
        {
            sequence = DOTween.Sequence();
        }

        public void AnimateItems(List<Item> items, Transform[] itemPoints, Action onComplete)
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
            ((ItemAnimationController)animationController).Initialize();
        }

        private void Start()
        {
            if (CommonGameAssets.Instance == null)
            {
                Debug.LogError("CommonGameAssets is not initialized!");
                return;
            }
            SpawnItems();
        }

        public void SpawnItems()
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
                item.OnItemDestroyed += OnItemDestroyed;
                items.Add(item);
            }

            animationController.AnimateItems(items, itemPoints, () =>
            {
                foreach (var item in items)
                    item.SetCanTouch();
            });
        }

        public void OnItemDestroyed(Item item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);

                if (items.Count == 0)
                {
                    SpawnItems();
                }
            }
            else
            {
                Debug.LogError("Item should be in the list");
            }
        }
    }
}