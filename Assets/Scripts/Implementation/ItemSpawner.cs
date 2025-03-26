using System.Collections.Generic;
using Ebleme;
using Ebleme.Utility;
using StickBlast.Core.DependencyInjection;
using StickBlast.Core.Interfaces;
using StickBlast.Models;
using UnityEngine;

namespace StickBlast.Implementation
{
    public class ItemSpawner : Singleton<ItemSpawner>, IItemSpawner
    {
        [SerializeField]
        private Transform spawnPoint;

        [SerializeField]
        private Transform[] itemPoints;

        private List<Item> items;
        private readonly IItemAnimationController animationController;

        public ItemSpawner()
        {
            animationController = ServiceLocator.Instance.GetItemAnimationController();
        }

        private void Start()
        {
            if (CommonGameAssets.Instance == null)
            {
                Debug.LogError("CommonGameAssets is not initialized!");
                return;
            }

            // Spawn point'i animation controller'a aktar
            if (spawnPoint != null)
            {
                ((ItemAnimationController)animationController).SetSpawnPoint(spawnPoint.position);
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