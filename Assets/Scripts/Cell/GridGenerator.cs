// maebleme2

using System.Collections.Generic;
using System.Linq;
using Ebleme;
using Ebleme.Utility;
using UnityEngine;
using StickBlast.Models;
using System.Collections;

namespace StickBlast
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField]
        private SquareCell gridCellPrefab;

        [SerializeField]
        private Transform content;

        
        HashSet<Vector2Int> coordinates = new HashSet<Vector2Int>();

        private List<SquareCell> cells = new List<SquareCell>();

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
            cells = new List<SquareCell>();

            foreach (Transform c in content)
                Destroy(c.gameObject);
            
            foreach (var coordinate in coordinates)
            {
                var leftLine = BaseGrid.Instance.Lines.FirstOrDefault(p => p.coordinate == coordinate && p.lineDirection == LineDirection.Vertical);
                var rightLine = BaseGrid.Instance.Lines.FirstOrDefault(p => p.coordinate == new Vector2Int(coordinate.x + 1, coordinate.y) && p.lineDirection == LineDirection.Vertical);

                var topLine = BaseGrid.Instance.Lines.FirstOrDefault(p => p.coordinate == new Vector2Int(coordinate.x, coordinate.y + 1) && p.lineDirection == LineDirection.Horizontal);
                var bottomLine = BaseGrid.Instance.Lines.FirstOrDefault(p => p.coordinate == coordinate && p.lineDirection == LineDirection.Horizontal);

                var cell = Instantiate(gridCellPrefab, content);
                cell.Initialize(coordinate, topLine, rightLine, bottomLine, leftLine);
                
                cells.Add(cell);
            }
        }

        public void CheckCells()
        {
            foreach (var cell in cells)
            {
                if (cell.CheckLineOccupation())
                {
                    cell.SetOccupied();
                    CheckBlastCondition();

				}
            }
        }
        [Sirenix.OdinInspector.Button]
		public void CheckBlastCondition()
		{
			float delayBetweenBlasts = 0.1f; // Sesler arasında gecikme

			Dictionary<int, List<SquareCell>> rows = new Dictionary<int, List<SquareCell>>();
			Dictionary<int, List<SquareCell>> columns = new Dictionary<int, List<SquareCell>>();

			foreach (var cell in cells)
			{
				if (!rows.ContainsKey(cell.Coordinate.y))
					rows[cell.Coordinate.y] = new List<SquareCell>();
				rows[cell.Coordinate.y].Add(cell);

				if (!columns.ContainsKey(cell.Coordinate.x))
					columns[cell.Coordinate.x] = new List<SquareCell>();
				columns[cell.Coordinate.x].Add(cell);
			}

			// Satırlar (sağdan sola)
			foreach (var row in rows.Values)
			{
				if (row.Count >= 5 && row.All(c => c.IsOccupied))
				{
					StartCoroutine(PlayRowBlastEffect(row, delayBetweenBlasts));
				}
			}

			// Sütunlar (aşağıdan yukarı)
			foreach (var column in columns.Values)
			{
				if (column.Count >= 5 && column.All(c => c.IsOccupied))
				{
					StartCoroutine(PlayColumnBlastEffect(column, delayBetweenBlasts));
				}
			}
		}

		private IEnumerator PlayRowBlastEffect(List<SquareCell> row, float delayBetweenBlasts)
		{
			for (int i = 0; i < row.Count; i++)
			{
				var audioSource = row[i].blastParticlePrefab.GetComponent<AudioSource>();
				audioSource.Play();
				int currentIndex = i;
				row[i].PlayBlastEffect(currentIndex * delayBetweenBlasts, () => { });
				yield return new WaitForSeconds(delayBetweenBlasts); // Delay before the next sound plays
			}
		}

		private IEnumerator PlayColumnBlastEffect(List<SquareCell> column, float delayBetweenBlasts)
		{
			for (int i = 0; i < column.Count; i++)
			{
				var audioSource = column[i].blastParticlePrefab.GetComponent<AudioSource>();
				audioSource.Play();
				int currentIndex = i;
				column[i].PlayBlastEffect(currentIndex * delayBetweenBlasts, () => { });
				yield return new WaitForSeconds(delayBetweenBlasts); // Delay before the next sound plays
			}
		}

		public void HoverCells()
        {
            foreach (var cell in cells)
            {
                if (cell.CanBeHovered() && cell.CheckLineOccupation())
                {
                    cell.SetHover();
                }
                else
                {
                    cell.ClearHover();
                }
            }
        }

        public void ClearCells()
        {
            foreach (var cell in cells)
            {
                cell.ClearOccupation();
            }
        }

        public List<SquareCell> GetCells()
        {
            return cells;
        }
    }
}