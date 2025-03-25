using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public class StickButton
    {
        public Button button;
        public StickController.StickShape shape;
        public GameObject stickPrefab;
    }
    
    [SerializeField] private List<StickButton> stickButtons;
    [SerializeField] private Text scoreText;
    [SerializeField] private GridManager gridManager;
    
    private int score = 0;
    
    private void Start()
    {
        SetupButtons();
        UpdateScore(0);
    }
    
    private void SetupButtons()
    {
        foreach (StickButton stickButton in stickButtons)
        {
            stickButton.button.onClick.AddListener(() => SpawnStick(stickButton.stickPrefab));
        }
    }
    
    private void SpawnStick(GameObject stickPrefab)
    {
        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPosition.z = 0;
        
        GameObject stick = Instantiate(stickPrefab, spawnPosition, Quaternion.identity);
        StickController controller = stick.GetComponent<StickController>();
        controller.gridManager = gridManager;
    }
    
    public void UpdateScore(int points)
    {
        score += points;
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
} 