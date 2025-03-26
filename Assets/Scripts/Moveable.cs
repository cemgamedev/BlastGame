using UnityEngine;
using UnityEngine.EventSystems;

namespace StickBlast
{
    public interface IMoveable
    {
        bool CanMove { get; set; }
        void OnStartMove();
        void OnMove(Vector2 position);
        void OnEndMove();
        RaycastHit2D Hit();
    }

    public interface IDraggable
    {
        void OnDragStart(Vector2 position);
        void OnDrag(Vector2 position);
        void OnDragEnd(Vector2 position);
    }

    public class Moveable : MonoBehaviour, IMoveable, IDraggable
    {
        [SerializeField]
        private LayerMask layerMask;

        private Vector2 offset;
        // private Vector3 startPosition;

        // private Item item;
        private ItemTile itemTile;
        private bool canMove = true;
        
        public bool CanMove 
        { 
            get => canMove;
            set => canMove = value;
        }

        private void Start()
        {
            // startPosition = transform.position;
            itemTile = GetComponent<ItemTile>();
        }

        public void OnStartMove()
        {
            if (!canMove) return;
            var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = itemTile.Item.GetPosition() - target;
            itemTile.Item.SetMovingScale();
        }

        public void OnMove(Vector2 position)
        {
            if (!canMove) return;
            Vector2 target = Camera.main.ScreenToWorldPoint(position);
            target += offset;
            itemTile.Item.SetPosition(target);
        }

        public void OnEndMove()
        {
            if (!canMove) return;
            var allowSetToGrid = itemTile.Item.AllowSetToGrid();

            if (allowSetToGrid)
            {
                itemTile.Item.AssingItemTilesToGridTiles();
                BaseGrid.Instance.CheckGrid();
            }
            else
            {
                itemTile.Item.ReleaseAll();
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

        // #region Pointers
        //
        // public void OnPointerDown(PointerEventData eventData)
        // {
        //     var target = Camera.main.ScreenToWorldPoint(eventData.position);
        //     offset = itemTile.Item.GetPosition() - target;
        //     
        //     itemTile.Item.SetMovingScale();
        // }
        //
        // public void OnDrag(PointerEventData eventData)
        // {
        //     Vector2 target = Camera.main.ScreenToWorldPoint(eventData.position);
        //     target += offset;
        //
        //     itemTile.Item.SetPosition(target);
        // }
        //
        // public void OnPointerUp(PointerEventData eventData)
        // {
        //     var allowSetToGrid = itemTile.Item.AllowSetToGrid();
        //
        //     if (allowSetToGrid)
        //     {
        //         itemTile.Item.AssingItemTilesToGridTiles();
        //         BaseGrid.Instance.CheckGrid();
        //     }
        //     else
        //     {
        //         itemTile.Item.ReleaseAll();
        //     }
        // }
        //
        // #endregion

        #region Manager

        // private bool AllowSetToGrid()
        // {
        //     var allowSetToGrid = true;
        //
        //     foreach (var tile in gridManager.Tiles)
        //     {
        //         if (!tile.gameObject.activeSelf) continue;
        //
        //         var myTile = (MyTile)tile;
        //
        //         var hit = myTile.Moveable.Hit();
        //         if (!hit)
        //         {
        //             allowSetToGrid = false;
        //             break;
        //
        //         }
        //
        //         // base tile ın üstünde herhangi birşey var mı kontrolü
        //         var baseTile = hit.transform.GetComponent<MyTile>();
        //         if (baseTile.OnMyTile)
        //         {
        //             allowSetToGrid = false;
        //             break;
        //         }
        //     }
        //
        //     return allowSetToGrid;
        // }

        // private void SetPositionAll()
        // {
        //     foreach (var tile in gridManager.Tiles)
        //     {
        //         if (!tile.gameObject.activeSelf)
        //             continue;
        //
        //         var myTile = (MyTile)tile;
        //         myTile.Moveable.SetPositionToHit();
        //     }
        // }

        // private void BackToStartPositionAll()
        // {
        //     foreach (var tile in gridManager.Tiles)
        //     {
        //         if (!tile.gameObject.activeSelf) continue;
        //         var myTile = (MyTile)tile;
        //         myTile.Moveable.BackToStartPosition();
        //     }
        // }

        #endregion


        // private void SetPositionToHit()
        // {
        //     var hit = Hit();
        //     var baseTile = hit.transform.GetComponent<MyTile>();
        //     baseTile.OnMyTile = myTile;
        //     var target = hit.transform.position;
        //     target.z = 0.5f;
        //     transform.position = target;
        //     myTile.SetActiveCollider(false);
        // }


        // private void BackToStartPosition()
        // {
        //     transform.position = startPosition;
        // }

        public RaycastHit2D Hit()
        {
            return Physics2D.Raycast(transform.position, Vector3.forward, 30, layerMask);
        }

        // private void FixedUpdate()
        // {
        //     var hit = Hit();
        //
        //     Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 20, hit ? Color.yellow : Color.white);
        // }
    }
}