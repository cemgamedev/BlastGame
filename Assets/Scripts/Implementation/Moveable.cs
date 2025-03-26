using UnityEngine;
using StickBlast.Core.Interfaces;
using StickBlast.Models;
using StickBlast.Core.DependencyInjection;

namespace StickBlast.Implementation
{
    public class Moveable : MonoBehaviour, IMoveable, IDraggable
    {
        [SerializeField]
        private LayerMask layerMask;

        private Vector2 offset;
        private ItemTile itemTile;
        private bool canMove = true;
        private IItemAnimationController animationController;
        private bool isDragging;
        
        public bool CanMove 
        { 
            get => canMove;
            set => canMove = value;
        }

        private void Start()
        {
            itemTile = GetComponent<ItemTile>();
            animationController = ServiceLocator.Instance.GetItemAnimationController();
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
                var targetPosition = itemTile.Item.GetPosition();
                animationController.HandleItemPlacement(transform, targetPosition, () =>
                {
                    itemTile.Item.AssingItemTilesToGridTiles();
                    BaseGrid.Instance.CheckGrid();
                });
            }
            else
            {
                animationController.HandleItemSnap(transform, itemTile.Item.GetOriginalPosition(), () =>
                {
                    itemTile.Item.ReleaseAll();
                });
            }
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