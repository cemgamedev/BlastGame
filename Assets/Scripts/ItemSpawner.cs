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
		private Transform[] itemPoints;
		[SerializeField]
		private Transform boardTransform;
		private List<Item> items;
		private IItemAnimationController animationController;
		private CompositeDisposable disposables = new CompositeDisposable();

		private void Awake()
		{
			animationController = new ItemAnimationController();
			((ItemAnimationController)animationController).HandleInitialization();
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

		// Helper method to get the main SpriteRenderer
		private SpriteRenderer GetMainSpriteRenderer(GameObject itemPrefab)
		{
			// First try to get the main SpriteRenderer on the prefab
			SpriteRenderer mainRenderer = itemPrefab.GetComponent<SpriteRenderer>();

			// If not found, look in children
			if (mainRenderer == null)
			{
				mainRenderer = itemPrefab.GetComponentInChildren<SpriteRenderer>();
			}

			return mainRenderer;
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

			// Spawn items at itemPoints positions
			for (int i = 0; i < itemPrefabs.Count && i < itemPoints.Length; i++)
			{
				var itemPrefab = itemPrefabs[i];
				if (itemPrefab == null) continue;

				var item = Instantiate(itemPrefab);
				Vector3 spawnPos = itemPoints[i].position;
				
				// Set initial position and spawn position
				item.transform.position = spawnPos;
				item.SetSpawnPosition(spawnPos);

				// Subscribe to item destruction using UniRx
				item.OnItemDestroyed
					.Subscribe(HandleItemDestruction)
					.AddTo(disposables);

				items.Add(item);
			}

			// Enable touch for all items immediately since they're already in position
			foreach (var item in items)
			{
				item.SetCanTouch();
			}
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