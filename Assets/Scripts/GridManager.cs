using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField] public int width = 10;
    [SerializeField] public int height = 10;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float cellGap = 0.1f; // Hücreler arası boşluk
    [SerializeField] private GameObject cellPrefab;
    
    private Cell[,] grid;
    
    private void Start()
    {
        CreateGrid();
    }
    
    private void CreateGrid()
    {
        grid = new Cell[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Hücreler arası boşluğu hesaba katarak pozisyon hesaplama
                Vector3 position = new Vector3(
                    x * (cellSize + cellGap), 
                    y * (cellSize + cellGap), 
                    0
                );
                GameObject cellObject = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                cellObject.name = $"Cell_{x}_{y}";
                
                Cell cell = cellObject.GetComponent<Cell>();
                grid[x, y] = cell;
            }
        }
    }
    
    public Cell GetCell(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return grid[x, y];
        }
        return null;
    }
    
    public bool IsCellValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    
    public void CheckAndPaintCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!grid[x, y].isPainted && IsCellSurrounded(x, y))
                {
                    grid[x, y].SetPainted(true);
                }
            }
        }
    }
    
    private bool IsCellSurrounded(int x, int y)
    {
        Cell currentCell = grid[x, y];
        if (currentCell.isOccupied) return false;
        
        // Check all four directions
        bool left = x > 0 && grid[x - 1, y].isOccupied;
        bool right = x < width - 1 && grid[x + 1, y].isOccupied;
        bool bottom = y > 0 && grid[x, y - 1].isOccupied;
        bool top = y < height - 1 && grid[x, y + 1].isOccupied;
        
        return left && right && bottom && top;
    }
    
    public void CheckAndBlastLines()
    {
        // Check rows
        for (int y = 0; y < height; y++)
        {
            bool isRowComplete = true;
            for (int x = 0; x < width; x++)
            {
                if (!grid[x, y].isPainted)
                {
                    isRowComplete = false;
                    break;
                }
            }
            if (isRowComplete)
            {
                BlastRow(y);
            }
        }
        
        // Check columns
        for (int x = 0; x < width; x++)
        {
            bool isColumnComplete = true;
            for (int y = 0; y < height; y++)
            {
                if (!grid[x, y].isPainted)
                {
                    isColumnComplete = false;
                    break;
                }
            }
            if (isColumnComplete)
            {
                BlastColumn(x);
            }
        }
    }
    
    private void BlastRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            grid[x, y].SetPainted(false);
            grid[x, y].SetOccupied(false);
        }
    }
    
    private void BlastColumn(int x)
    {
        for (int y = 0; y < height; y++)
        {
            grid[x, y].SetPainted(false);
            grid[x, y].SetOccupied(false);
        }
    }
} 