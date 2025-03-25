using UnityEngine;
using System.Collections.Generic;

public class StickController : MonoBehaviour
{
    [SerializeField] public GridManager gridManager;
    [SerializeField] private float snapThreshold = 0.5f;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float cellGap = 0.1f;
    [SerializeField] private Color validPlacementColor = Color.green;
    [SerializeField] private Color invalidPlacementColor = Color.red;
    
    public enum StickShape
    {
        I,
        L,
        U,
        T
    }
    
    [SerializeField] private StickShape stickShape;
    
    private bool isDragging = false;
    private Vector3 offset;
    private List<Vector2Int> occupiedCells = new List<Vector2Int>();
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool canPlace = false;
    private Vector3 dragStartPosition;
    private Camera mainCamera;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        
        // SpriteRenderer kontrolü
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"SpriteRenderer component is missing on {gameObject.name}. Please add a SpriteRenderer component.");
            enabled = false;
            return;
        }
        
        // GridManager kontrolü
        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
            if (gridManager == null)
            {
                Debug.LogError($"GridManager not found in scene. Please add a GridManager component to a GameObject in the scene.");
                enabled = false;
                return;
            }
        }
        
        originalColor = spriteRenderer.color;
    }
    
    private void OnMouseDown()
    {
        if (!enabled) return;
        
        isDragging = true;
        dragStartPosition = transform.position;
        
        // Mouse pozisyonunu dünya koordinatlarına çevir
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = transform.position.z - mainCamera.transform.position.z;
        Vector3 worldMousePos = mainCamera.ScreenToWorldPoint(mousePos);
        
        // Offset'i hesapla
        offset = transform.position - worldMousePos;
    }
    
    private void OnMouseDrag()
    {
        if (!enabled || !isDragging) return;
        
        // Mouse pozisyonunu dünya koordinatlarına çevir
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = transform.position.z - mainCamera.transform.position.z;
        Vector3 worldMousePos = mainCamera.ScreenToWorldPoint(mousePos);
        
        // Hedef pozisyonu hesapla
        Vector3 targetPosition = worldMousePos + offset;
        
        // Snap pozisyonunu hesapla
        Vector3 snapPosition = GetSnapPosition(targetPosition);
        
        // Geçici olarak snap pozisyonuna taşı
        transform.position = snapPosition;
        
        // Grid pozisyonunu hesapla
        Vector2Int gridPosition = WorldToGridPosition(snapPosition);
        
        // Yerleşim kontrolü
        canPlace = IsValidPlacement(gridPosition.x, gridPosition.y);
        UpdateVisualFeedback(canPlace);
    }
    
    private void OnMouseUp()
    {
        if (!enabled || !isDragging) return;
        
        isDragging = false;
        
        // Snap pozisyonunu hesapla
        Vector3 snapPosition = GetSnapPosition(transform.position);
        Vector2Int gridPosition = WorldToGridPosition(snapPosition);
        
        if (IsValidPlacement(gridPosition.x, gridPosition.y))
        {
            transform.position = snapPosition;
            OccupyCells(gridPosition.x, gridPosition.y);
            gridManager.CheckAndPaintCells();
            gridManager.CheckAndBlastLines();
            spriteRenderer.color = originalColor;
        }
        else
        {
            // Geçersiz yerleşim durumunda orijinal pozisyona dön
            transform.position = dragStartPosition;
            spriteRenderer.color = originalColor;
        }
    }
    
    private Vector3 GetSnapPosition(Vector3 position)
    {
        float totalCellSize = cellSize + cellGap;
        float x = Mathf.Round(position.x / totalCellSize) * totalCellSize;
        float y = Mathf.Round(position.y / totalCellSize) * totalCellSize;
        return new Vector3(x, y, 0); // Z pozisyonunu 0'a sabitle
    }
    
    private Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        float totalCellSize = cellSize + cellGap;
        int x = Mathf.RoundToInt(worldPosition.x / totalCellSize);
        int y = Mathf.RoundToInt(worldPosition.y / totalCellSize);
        return new Vector2Int(x, y);
    }
    
    private void UpdateVisualFeedback(bool isValid)
    {
        if (isValid)
        {
            spriteRenderer.color = validPlacementColor;
        }
        else
        {
            spriteRenderer.color = invalidPlacementColor;
        }
    }
    
    private bool IsValidPlacement(int centerX, int centerY)
    {
        if (gridManager == null) return false;
        
        List<Vector2Int> cellsToCheck = GetStickCells(centerX, centerY);
        
        foreach (Vector2Int cell in cellsToCheck)
        {
            // Grid sınırlarını kontrol et
            if (cell.x < 0 || cell.x >= gridManager.width || cell.y < 0 || cell.y >= gridManager.height)
            {
                return false;
            }
            
            Cell gridCell = gridManager.GetCell(cell.x, cell.y);
            if (gridCell == null || gridCell.isOccupied || gridCell.isPainted)
            {
                return false;
            }
        }
        
        return true;
    }
    
    private void OccupyCells(int centerX, int centerY)
    {
        if (gridManager == null) return;
        
        occupiedCells = GetStickCells(centerX, centerY);
        
        foreach (Vector2Int cell in occupiedCells)
        {
            Cell gridCell = gridManager.GetCell(cell.x, cell.y);
            if (gridCell != null)
            {
                gridCell.SetOccupied(true);
            }
        }
    }
    
    private List<Vector2Int> GetStickCells(int centerX, int centerY)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        
        switch (stickShape)
        {
            case StickShape.I:
                cells.Add(new Vector2Int(centerX, centerY));
                cells.Add(new Vector2Int(centerX, centerY + 1));
                cells.Add(new Vector2Int(centerX, centerY + 2));
                break;
                
            case StickShape.L:
                cells.Add(new Vector2Int(centerX, centerY));
                cells.Add(new Vector2Int(centerX, centerY + 1));
                cells.Add(new Vector2Int(centerX + 1, centerY));
                break;
                
            case StickShape.U:
                cells.Add(new Vector2Int(centerX, centerY));
                cells.Add(new Vector2Int(centerX + 1, centerY));
                cells.Add(new Vector2Int(centerX, centerY + 1));
                cells.Add(new Vector2Int(centerX + 1, centerY + 1));
                break;
                
            case StickShape.T:
                cells.Add(new Vector2Int(centerX, centerY));
                cells.Add(new Vector2Int(centerX - 1, centerY));
                cells.Add(new Vector2Int(centerX + 1, centerY));
                cells.Add(new Vector2Int(centerX, centerY + 1));
                break;
        }
        
        return cells;
    }
} 