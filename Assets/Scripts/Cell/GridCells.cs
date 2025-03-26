// maebleme2

using System.Collections.Generic;
using System.Linq;
using Ebleme;
using Ebleme.Utility;
using UnityEngine;
using StickBlast.Models;

namespace StickBlast
{
    public class GridCells : MonoBehaviour
    {
        [SerializeField]
        private GridCell gridCellPrefab;

        [SerializeField]
        private Transform content;

        
        HashSet<Vector2Int> coordinates = new HashSet<Vector2Int>();

        private List<GridCell> cells = new List<GridCell>();

        public void SetCells()
        {
            GenerateCoordinates();
            SpawnCells();
        }

        private void GenerateCoordinates()
        {
            for (int y = 0; y < GameConfigs.Instance.BaseGridSize.y - 1; y++)
            for (int x = 0; x < GameConfigs.Instance.BaseGridSize.x - 1; x++)
                coordinates.Add(new Vector2Int(x, y));
        }

        private void SpawnCells()
        {
            
            // Line offsets
            // Left     : [0,0]
            // Right    : [+1,0]
            // Top      : [0,+1]
            // Bottom   : [0,0]
            cells = new List<GridCell>();

            foreach (Transform c in content)
                Destroy(c.gameObject);
            
            foreach (var coordinate in coordinates)
            {
                var leftLine = BaseGrid.Instance.Lines.FirstOrDefault(p => p.coordinate == coordinate && p.lineDirection == LineDirection.Vertical);
                var rightLine = BaseGrid.Instance.Lines.FirstOrDefault(p => p.coordinate == new Vector2Int(coordinate.x + 1, coordinate.y) && p.lineDirection == LineDirection.Vertical);

                var topLine = BaseGrid.Instance.Lines.FirstOrDefault(p => p.coordinate == new Vector2Int(coordinate.x, coordinate.y + 1) && p.lineDirection == LineDirection.Horizontal);
                var bottomLine = BaseGrid.Instance.Lines.FirstOrDefault(p => p.coordinate == coordinate && p.lineDirection == LineDirection.Horizontal);

                var cell = Instantiate(gridCellPrefab, content);
                cell.Set(coordinate, topLine, rightLine, bottomLine, leftLine);
                
                cells.Add(cell);
            }
        }

        public void CheckCells()
        {
            foreach (var cell in cells)
            {
                if (cell.IsLinesOccupied())
                {
                    cell.SetOccupied();
                }
                else
                {
                    cell.DeOccupie();
                    cell.DeHover();
                }
            }
        }

        public void HoverCells()
        {
            /*
            * Son gelen Item;
            * - bir gridi tamamlıyorsa o grid
            * - bir satırı tamamlıyorsa o satır
            * - bir sütunu tamamlıyorsa o sütun hover olacak
            *
            * Grid in line larını IsHoverin olmalı. Yani elde olan Item placable pozisyonda olmalı. BUraya geliyorsa zaten Item canPlaced konumundadır
            * Grid 
            *
            * 
            */
            
            foreach (var cell in cells)
            {
                if (cell.CanHover())
                {
                    cell.Hover();
                }
                else
                {
                    cell.DeHover();
                }
            }
        }
    }
}