// maebleme2

using System;
using System.Collections.Generic;
using DG.Tweening;
using Ebleme;
using Ebleme.Utility;
using StickBlast;
using UnityEngine;
using StickBlast.Models;
using UniRx;
using StickBlast.Core.Interfaces;
using StickBlast.Implementation;

namespace DefaultNamespace
{
    public interface IItemSpawner
    {
        void HandleItemSpawn();
        void HandleItemDestruction(Item item);
    }

    public class ItemSpawner : Singleton<ItemSpawner>, IItemSpawner
    {
        [SerializeField]
        private Transform spawnPoint;

        [SerializeField]
        private Transform[] itemPoints;

        private List<Item> items;
        private IItemAnimationController animationController;
        private CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            animationController = new ItemAnimationController();
            ((ItemAnimationController)animationController).HandleInitialization();
            ((ItemAnimationController)animationController).SetSpawnPoint(spawnPoint.position);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
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
                
                // Subscribe to item destruction using UniRx
                item.OnItemDestroyed
                    .Subscribe(HandleItemDestruction)
                    .AddTo(disposables);
                
                items.Add(item);
            }

            animationController.HandleItemAnimation(items, itemPoints)
                .Subscribe(_ =>
                {
                    foreach (var item in items)
                        item.SetCanTouch();
                })
                .AddTo(disposables);
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