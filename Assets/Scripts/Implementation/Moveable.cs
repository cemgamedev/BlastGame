using UnityEngine;
using StickBlast.Core.Interfaces;
using StickBlast.Models;
using StickBlast.Core.DependencyInjection;
using UniRx;
using Cysharp.Threading.Tasks;

namespace StickBlast.Implementation
{
    public class Moveable : MonoBehaviour, IMoveable, IDraggable
    {
        [SerializeField]
        private LayerMask layerMask;

        private Vector2 offset;
        private ItemFeatureProvider itemTile;
        private bool canMove = true;
        private IItemAnimationController animationController;
        private bool isDragging;
        private CompositeDisposable disposables = new CompositeDisposable();
        
        public bool CanMove 
        { 
            get => canMove;
            set => canMove = value;
        }

        private void Start()
        {
            itemTile = GetComponent<ItemFeatureProvider>();
            animationController = ServiceLocator.Instance.GetItemAnimationController();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        public void OnStartMove()
        {
            if (!canMove) return;
            isDragging = true;
            var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = itemTile.Item.GetPosition() - target;
            itemTile.Item.SetMovingScale();
            animationController.HandleItemHover(transform, true);
        }

        public void OnMove(Vector2 position)
        {
            if (!canMove || !isDragging) return;
            Vector2 target = Camera.main.ScreenToWorldPoint(position);
            target += offset;
            itemTile.Item.SetPosition(target);
        }

        public void OnEndMove()
        {
            if (!canMove || !isDragging) return;
            isDragging = false;
            animationController.HandleItemHover(transform, false);

            var allowSetToGrid = itemTile.Item.AllowSetToGrid();
            if (allowSetToGrid)
            {
                HandleItemPlacementAsync().Forget();
            }
            else
            {
                HandleItemSnapAsync().Forget();
            }
        }

        private async UniTaskVoid HandleItemPlacementAsync()
        {
            canMove = false;
            await animationController.HandleItemPlacementAsync(transform, itemTile.Item.GetGridPosition());
            itemTile.Item.SetToGrid();
        }

        private async UniTaskVoid HandleItemSnapAsync()
        {
            canMove = false;
            await animationController.HandleItemSnapAsync(transform, itemTile.Item.GetSpawnPosition());
            canMove = true;
        }

        public void OnDragStart(Vector2 position)
        {
            OnStartMove();
        }

        public void OnDrag(Vector2 position)
        {
            OnMove(position);
        }

        public void OnDragEnd(Vector2 position)
        {
            OnEndMove();
        }

        public RaycastHit2D Hit()
        {
            return Physics2D.Raycast(transform.position, Vector3.forward, 30, layerMask);
        }
    }
} 