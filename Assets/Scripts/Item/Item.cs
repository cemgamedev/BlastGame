// maebleme2

using System;
using System.Collections.Generic;
using System.Linq;
using Ebleme;
using StickBlast.Grid;
using UnityEngine;
using UnityEngine.EventSystems;
using StickBlast.Models;
using DG.Tweening;

namespace StickBlast
{
    public class Item : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField]
        private ItemTypes itemType;

        [Header("Line")]
        [SerializeField]
        private ItemLine linePrefab;

        [SerializeField]
        private Transform linesContent;

        [SerializeField]
        private List<ItemLine> lines;

        public ItemTypes ItemType => itemType;
        
        private Vector2 startScale;
        private Vector3 startPosition;
        private Vector2 offset;

        private List<ItemTile> tiles = new List<ItemTile>();
        private List<BaseLine> baseLinesHit;

        private bool canPlaced;
        private bool canTouch = true;

        public event Action<Item> OnItemDestroyed; 

        private void Start()
        {
            startScale = GameConfigs.Instance.ItemStillScale;
            
            transform.localScale = startScale;
            startPosition = transform.position;

            SetTilesList();
            DrawLines();
            Recolor(ColorTypes.ItemStill);
        }

        public void SetCanTouch()
        {
            canTouch = true;
            startPosition = transform.position;
        }

        // Sets Item tiles
        private void SetTilesList()
        {
            foreach (Transform c in transform)
            {
                if (c.gameObject.activeSelf)
                {
                    var tile = c.GetComponent<ItemTile>();
                    if (tile == null) continue;

                    tile.SetItem(this);
                    tiles.Add(tile);
                }
                else
                {
                    Destroy(c.gameObject);
                }
            }
        }

        #region Color

        private void Recolor(ColorTypes colorType)
        {
            RecolorItems(colorType);
            RecolorLines(colorType);
        }

        private void RecolorItems(ColorTypes colorType)
        {
            foreach (var tile in tiles)
            {
                tile.ReColor(colorType);
            }
        }

        private void RecolorLines(ColorTypes colorType)
        {
            foreach (var line in lines)
            {
                line.ReColor(colorType);
            }
        }

        #endregion

        #region Lines

        private void DrawLines()
        {
            foreach (Transform c in linesContent)
                Destroy(c.gameObject);

            lines = new List<ItemLine>();
            for (int i = tiles.Count - 1; i >= 0; i--)
            {
                var tile = tiles[i];
                // Your code here
                var right = tile.GetNeighbour(Direction.Right);

                if (right && right.gameObject.activeSelf)
                {
                    DrawLine((ItemTile)tile, (ItemTile)right, LineDirection.Horizontal);
                }

                var up = tile.GetNeighbour(Direction.Up);

                if (up && up.gameObject.activeSelf)
                {
                    DrawLine((ItemTile)tile, (ItemTile)up, LineDirection.Vertical);
                }
            }

            linesContent.transform.localPosition = new Vector3(linesContent.transform.localPosition.x, linesContent.transform.localPosition.y, 0.1f);
        }

        private void DrawLine(ItemTile tileA, ItemTile tileB, LineDirection lineDirection)
        {
            if (tileA == null || tileB == null) return;

            Transform pointA = tileA.transform, pointB = tileB.transform;

            float distance = Vector2.Distance(pointA.position, pointB.position);

            var line = Instantiate(linePrefab, linesContent);
            line.transform.position = (pointA.position + pointB.position) / 2;

            Vector2 direction = pointB.position - pointA.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            line.transform.rotation = Quaternion.Euler(0, 0, angle);

            Vector3 scale = line.transform.localScale;
            scale.x = distance / line.GetComponent<SpriteRenderer>().bounds.size.x;
            line.transform.localScale = scale;

            line.Set(tileA.coordinate, lineDirection, tileA, tileB);

            lines.Add(line);
        }

        #endregion

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        public void SetMovingScale()
        {
            transform.DOScale(GameConfigs.Instance.ItemDragScale,0.15f)
                .SetEase(Ease.InElastic);
        }

        public bool AllowSetToGrid()
        {
            return canPlaced;
        }

        public void AssingItemTilesToGridTiles()
        {
            foreach (var tile in tiles)
            {
                var baseTile = tile.SetPositionToHit();
                if (baseTile != null)
                {
                    baseTile.SetOccupied();
                }
            }
        }

        public void ReleaseAll()
        {
            foreach (var tile in tiles)
            {
                tile.BackToStartPosition();
                tile.SetActiveCollider(true);
            }
        }

        private void CanPlacable()
        {
            canPlaced = true;

            // Önceki Line Hitleri ile yenilerini karşılaştırıyoruz ki öncekilerin hover ını kapatabilelim
            var newHitLines = GetBaseLineHits();
            
            if (newHitLines != baseLinesHit)
                BaseGrid.Instance.DeHover(baseLinesHit);

            baseLinesHit = newHitLines;

            if (baseLinesHit.Count == lines.Count)
            {
                foreach (var baseLine in baseLinesHit)
                {
                    // Eğer hit olan line zaten occupied ise yerleştirilemez
                    if (baseLine.IsOccupied)
                    {
                        canPlaced = false;
                        break;
                    }
                }
            }
            else
                canPlaced = false;
        }

        private List<BaseLine> GetBaseLineHits()
        {
            var list = new List<BaseLine>();

            foreach (var line in lines)
            {
                var hit = line.Hit();

                if (hit && hit.transform != null)
                {
                    var baseLine = hit.transform.GetComponent<BaseLine>();
                    if (baseLine.lineDirection == line.lineDirection)
                        list.Add(baseLine);
                }
            }

            return list;
        }

        #region Pointers

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!canTouch)
                return;
            
            var target = Camera.main.ScreenToWorldPoint(eventData.position);
            offset = transform.position - target;

            SetMovingScale();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!canTouch)
                return;
            
            Vector2 target = Camera.main.ScreenToWorldPoint(eventData.position);
            target += offset;

            transform.position = target;

            CanPlacable();

            if (canPlaced)
                BaseGrid.Instance.Hover(baseLinesHit);
            else
                BaseGrid.Instance.DeHover(baseLinesHit);

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (canPlaced)
            {
                // Item ı grid e işler
                BaseGrid.Instance.PutItemToGrid(baseLinesHit);

                // Item ı yok eder
                OnItemDestroyed?.Invoke(this);
                Destroy(gameObject);

                // Dolu olan Grid Cell leri kontrol eder. Ardından tüm Grid i kontrol eder.
                BaseGrid.Instance.CheckCells(() =>
                {
                    BaseGrid.Instance.CheckGrid();
                });
            }
            else
            {
                transform.localScale = startScale;
                transform.position = startPosition;
            }

            baseLinesHit = null;
        }

        #endregion

        public Vector3 GetOriginalPosition()
        {
            return startPosition;
        }

        public Vector3 GetTargetPosition()
        {
            // İlk tile'ın pozisyonunu hedef pozisyon olarak kullanıyoruz
            if (tiles.Count > 0)
            {
                return tiles[0].transform.position;
            }
            return transform.position;
        }
    }
}