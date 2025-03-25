using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isOccupied = false;
    public bool isPainted = false;
    
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite paintedSprite;
    [SerializeField] private Sprite occupiedSprite;
    
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
        UpdateVisual();
    }
    
    public void SetPainted(bool painted)
    {
        isPainted = painted;
        UpdateVisual();
    }
    
    private void UpdateVisual()
    {
        if (isPainted)
        {
            spriteRenderer.sprite = paintedSprite;
        }
        else if (isOccupied)
        {
            spriteRenderer.sprite = occupiedSprite;
        }
        else
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }
} 