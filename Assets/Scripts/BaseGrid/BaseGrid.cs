// maebleme2

using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Ebleme;
using Ebleme.Utility;
using StickBlast.Grid;
using UnityEngine;
using StickBlast.Models;

namespace StickBlast
{
    public class BaseGrid : Singleton<BaseGrid>
    {
        [SerializeField]
        private GridManager gridManager;

        [SerializeField]
        private GridCells gridCells;


        [SerializeField]
        private BaseLine linePrefab;

        [SerializeField]
        private Transform linesContent;

        private List<BaseLine> lines;
        public List<BaseLine> Lines => lines;


        private HashSet<BaseLine> linesToRemove;
        private HashSet<BaseTile> tilesToRemove;

        private void Start()
        {
            DrawLines();

            gridCells.SetCells();
        }

        #region Line Draw

        private void DrawLines()
        {
            lines = new List<BaseLine>();
            for (int i = gridManager.Tiles.Count - 1; i >= 0; i--)
            {
                var tile = gridManager.Tiles[i];

                var right = tile.GetNeighbour(Direction.Right);
                if (right)
                    DrawLine((BaseTile)tile, (BaseTile)right, LineDirection.Horizontal);

                var up = tile.GetNeighbour(Direction.Up);
                if (up)
                    DrawLine((BaseTile)tile, (BaseTile)up, LineDirection.Vertical);
            }

            linesContent.transform.localPosition = new Vector3(linesContent.transform.localPosition.x, linesContent.transform.localPosition.y, 0.1f);
        }

        private void DrawLine(BaseTile tileA, BaseTile tileB, LineDirection lineDirection)
        {
            if (tileA == null || tileB == null) return;

            Vector2 pointA = tileA.transform.position, pointB = tileB.transform.position;

            float distance = Vector2.Distance(pointA, pointB);

            var line = Instantiate(linePrefab, linesContent);
            line.transform.position = (Vector2)(pointA + pointB) / 2;

            Vector2 direction = pointB - pointA;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            line.transform.rotation = Quaternion.Euler(0, 0, angle);

            Vector3 scale = line.transform.localScale;
            scale.x = distance / line.GetComponent<SpriteRenderer>().bounds.size.x;
            line.transform.localScale = scale;

            line.Set(tileA.coordinate, lineDirection, tileA, tileB);

            line.ReColor(ColorTypes.Passive);

            lines.Add(line);
        }

        #endregion

        public void CheckCells(Action onCompleted)
        {
            gridCells.CheckCells();

            DOVirtual.DelayedCall(GameConfigs.Instance.GridFillDuration, () => { onCompleted?.Invoke(); });
        }
        

        #region Check grid fullness

        public void CheckGrid()
        {
            linesToRemove = new HashSet<BaseLine>();
            tilesToRemove = new HashSet<BaseTile>();

            CheckHorizontal();
            CheckVertical();

            foreach (var line in linesToRemove)
            {
                line.DeOccupied();
                line.DeHover();
            }

            foreach (var tile in tilesToRemove)
                tile.DeOccupied();

            // clear cells
            gridCells.CheckCells();
        }

        private void CheckHorizontal()
        {
            for (int row = 0; row < GameConfigs.Instance.BaseGridSize.y; row++)
            {
                if (IsFullRowHorizontal(row) && IsFullRowVertical(row)) // Row full both horizontal
                {
                    if (row + 1 < GameConfigs.Instance.BaseGridSize.y)
                        if (IsFullRowHorizontal(row + 1)) // Row full both horizontal
                        {
                            Debug.Log($"Row {row} to row {row + 1} is full");

                            var horizontalLines = GetHorizontalLinesByRow(row);
                            var verticalLines = GetVerticalLinesByRow(row);
                            var horizontalLines2 = GetHorizontalLinesByRow(row + 1);

                            foreach (var line in horizontalLines)
                                linesToRemove.Add(line);

                            foreach (var line in verticalLines)
                                linesToRemove.Add(line);

                            foreach (var line in horizontalLines2)
                                linesToRemove.Add(line);

                            var tiles1 = GetHorizontalTiles(row);
                            var tiles2 = GetHorizontalTiles(row + 1);

                            foreach (var tile in tiles1)
                                tilesToRemove.Add(tile);

                            foreach (var tile in tiles2)
                                tilesToRemove.Add(tile);
                        }
                }
            }
        }

        private void CheckVertical()
        {
            for (int column = 0; column < GameConfigs.Instance.BaseGridSize.x; column++)
            {
                if (IsFullHorizontalColumn(column) && IsFullVerticalColumn(column)) // Row full both horizontal
                {
                    if (column + 1 < GameConfigs.Instance.BaseGridSize.x)
                        if (IsFullVerticalColumn(column + 1)) // Row full both horizontal
                        {
                            Debug.Log($"Column {column} to column {column + 1} is full");

                            var horizontalLines = GetHorizontalLinesByColumn(column);
                            var verticalLines = GetVerticalLinesByColumn(column);
                            var verticaLines2 = GetVerticalLinesByColumn(column + 1);

                            foreach (var line in horizontalLines)
                                linesToRemove.Add(line);

                            foreach (var line in verticalLines)
                                linesToRemove.Add(line);

                            foreach (var line in verticaLines2)
                                linesToRemove.Add(line);

                            var tiles1 = GetVerticalTiles(column);
                            var tiles2 = GetVerticalTiles(column + 1);

                            foreach (var tile in tiles1)
                                tilesToRemove.Add(tile);

                            foreach (var tile in tiles2)
                                tilesToRemove.Add(tile);
                        }
                }
            }
        }

        private HashSet<BaseLine> GetHorizontalLinesByRow(int row)
        {
            // Yatay
            HashSet<BaseLine> lines = new HashSet<BaseLine>();
            for (int i = 0; i < GameConfigs.Instance.BaseGridSize.x - 1; i++)
            {
                var line = GetLine(i, row, LineDirection.Horizontal);
                lines.Add(line);
            }

            return lines;
        }

        private HashSet<BaseLine> GetVerticalLinesByRow(int row)
        {
            // Dikey
            HashSet<BaseLine> lines = new HashSet<BaseLine>();

            for (int i = 0; i < GameConfigs.Instance.BaseGridSize.x; i++)
            {
                var line = GetLine(i, row, LineDirection.Vertical);
                lines.Add(line);
            }

            return lines;
        }

        private HashSet<BaseLine> GetHorizontalLinesByColumn(int column)
        {
            // Yatay
            HashSet<BaseLine> lines = new HashSet<BaseLine>();
            for (int i = 0; i < GameConfigs.Instance.BaseGridSize.y; i++)
            {
                var line = GetLine(column, i, LineDirection.Horizontal);
                lines.Add(line);
            }

            return lines;
        }

        private HashSet<BaseLine> GetVerticalLinesByColumn(int column)
        {
            // Dikey
            HashSet<BaseLine> lines = new HashSet<BaseLine>();

            for (int i = 0; i < GameConfigs.Instance.BaseGridSize.y - 1; i++)
            {
                var line = GetLine(column, i, LineDirection.Vertical);
                lines.Add(line);
            }

            return lines;
        }

        private HashSet<BaseTile> GetHorizontalTiles(int row)
        {
            // Yatay
            HashSet<BaseTile> tiles = new HashSet<BaseTile>();
            for (int i = 0; i < GameConfigs.Instance.BaseGridSize.x; i++)
            {
                var tile = GetTile(i, row);
                tiles.Add(tile);
            }

            return tiles;
        }

        private HashSet<BaseTile> GetVerticalTiles(int column)
        {
            // Dikey
            HashSet<BaseTile> tiles = new HashSet<BaseTile>();
            for (int i = 0; i < GameConfigs.Instance.BaseGridSize.y; i++)
            {
                var tile = GetTile(column, i);
                tiles.Add(tile);
            }

            return tiles;
        }

        private bool IsFullRowVertical(int row)
        {
            // Dikey
            for (int i = 0; i < GameConfigs.Instance.BaseGridSize.x; i++)
            {
                var line = GetLine(i, row, LineDirection.Vertical);
                if (line == null || !line.IsOccupied) return false;
            }

            return true;
        }

        private bool IsFullVerticalColumn(int column)
        {
            // Dikey
            for (int i = 0; i < GameConfigs.Instance.BaseGridSize.y - 1; i++)
            {
                var line = GetLine(column, i, LineDirection.Vertical);
                if (line == null || !line.IsOccupied) return false;
            }

            return true;
        }

        private bool IsFullHorizontalColumn(int column)
        {
            // Yatay
            for (int i = 0; i < GameConfigs.Instance.BaseGridSize.y; i++)
            {
                var line = GetLine(column, i, LineDirection.Horizontal);
                if (line == null || !line.IsOccupied) return false;
            }

            return true;
        }

        private bool IsFullRowHorizontal(int row)
        {
            // Yatay
            for (int i = 0; i < GameConfigs.Instance.BaseGridSize.x - 1; i++)
            {
                var line = GetLine(i, row, LineDirection.Horizontal);
                if (line == null || !line.IsOccupied) return false;
            }


            return true;
        }

        private BaseLine GetLine(int x, int y, LineDirection direction)
        {
            return lines.SingleOrDefault(p => p.coordinate == new Vector2Int(x, y) && p.lineDirection == direction);
        }

        private BaseTile GetTile(int x, int y)
        {
            return (BaseTile)gridManager.Tiles.SingleOrDefault(p => p.coordinate == new Vector2Int(x, y));
        }

        #endregion


        public void PutItemToGrid(List<BaseLine> lines)
        {
            if (lines == null)
                return;

            foreach (var line in lines)
            {
                foreach (var tileController in line.ConnectedTiles)
                {
                    var tile = (BaseTile)tileController;
                    tile.SetOccupied();
                }

                line.SetOccupied();
            }
        }

        public void Hover(List<BaseLine> hoverLines)
        {
            if (hoverLines != null)
                foreach (var line in hoverLines)
                {
                    line.Hover();

                    foreach (var tileController in line.ConnectedTiles)
                    {
                        var tile = (BaseTile)tileController;
                        tile.ReColor(ColorTypes.Hover);
                    }
                }

            gridCells.HoverCells();
        }

        public void DeHover(List<BaseLine> hoverLines)
        {
            if (hoverLines != null)
                foreach (var line in hoverLines)
                {
                    line.DeHover();

                    foreach (var tileController in line.ConnectedTiles)
                    {
                        var tile = (BaseTile)tileController;
                        tile.DeHover();
                    }
                }
            
            gridCells.HoverCells();
        }
    }
}