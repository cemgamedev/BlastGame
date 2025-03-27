using System;
using System.Collections.Generic;
using DG.Tweening;
using BlastRoot;
using BlastRoot.Utility;
using StickBlast;
using UnityEngine;
using StickBlast.Models;
using UniRx;
using StickBlast.Core.Interfaces;
using StickBlast.Implementation;
using Cysharp.Threading.Tasks;

namespace StickBlast.Implementation
{
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
                Debug.LogError("ItemCollectionCustomAsset is not initialized!");
                return;
            }
            HandleItemSpawn();
        }

        public void HandleItemSpawn()
        {
            if (CommonGameAssets.Instance == null)
            {
                Debug.LogError("ItemCollectionCustomAsset is not initialized!");
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

            HandleItemAnimationAsync(items).Forget();
        }

        private async UniTaskVoid HandleItemAnimationAsync(List<Item> items)
        {
            await animationController.HandleItemAnimationAsync(items, itemPoints);
            foreach (var item in items)
                item.SetCanTouch();
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