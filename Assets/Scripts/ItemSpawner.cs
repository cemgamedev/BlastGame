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
		[SerializeField]
		private Transform boardTransform;
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

			// Get board boundaries
			Renderer boardRenderer = boardTransform.GetComponent<Renderer>();
			if (boardRenderer == null)
			{
				Debug.LogError("Board does not have a Renderer component!");
				return;
			}

			Vector3 boardMin = boardRenderer.bounds.min;
			Vector3 boardMax = boardRenderer.bounds.max;

			// Use the first itemPoint's Y position to ensure consistent Y alignment
			float spawnY = itemPoints[0].position.y;

			// Calculate spacing and start position based on board size
			float boardWidth = boardMax.x - boardMin.x;

			// Get the main SpriteRenderer of the first prefab to calculate size
			SpriteRenderer firstItemRenderer = GetMainSpriteRenderer(itemPrefabs[0].gameObject);
			if (firstItemRenderer == null)
			{
				Debug.LogError("Could not find SpriteRenderer for item!");
				return;
			}

			float itemWidth = firstItemRenderer.bounds.size.x;
			float spacing = itemWidth * 1.2f; // Add a little padding
			float startX = boardMin.x + (itemWidth / 2);

			foreach (var itemPrefab in itemPrefabs)
			{
				if (itemPrefab == null) continue;

				// Calculate X position
				float spawnX = startX + (items.Count * spacing);

				// Check if the item will be within board boundaries
				if (spawnX + (itemWidth / 2) > boardMax.x)
				{
					Debug.LogWarning("Too many items to spawn within board boundaries!");
					break;
				}

				var item = Instantiate(itemPrefab);

				// Set position with the consistent Y and constrained X
				item.transform.position = new Vector3(spawnX, spawnY, itemPoints[0].position.z);

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