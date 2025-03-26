using System.Collections.Generic;
using Ebleme.Utility;
using StickBlast;
using UnityEngine;

namespace Ebleme
{
    [CreateAssetMenu(fileName = "GameConfigs", menuName = "Ebleme/GameConfigs", order = 0)]
    public class GameConfigs : SingletonScriptableObject<GameConfigs>
    {
        [SerializeField]
        private LayerMask baseTileLayer;

        [SerializeField]
        private LayerMask baseLineLayer;

        [SerializeField]
        private Vector2Int baseGridSize;
        
        [Header("Scales")]
        [SerializeField]
        private Vector2 itemStillScale;
        
        [SerializeField]
        private Vector2 itemDragScale = new Vector2(1,1);

        [Header("Grid")]
        [SerializeField]
        private float gridFillDuration = .25f;

        [SerializeField]
        private Color linePassiveColor;
        
        [SerializeField]
        private Color hoverColor; 
        
        [SerializeField]
        private Color activeColor;  
        
        [SerializeField]
        private Color gridHoverColor;

        [Header("Item")]
        [SerializeField]
        private Color itemStillColor;

        [SerializeField]
        private int itemSpawnCount = 3;

        [SerializeField]
        private float itemMoveAnimDuration = .5f;

        [SerializeField]
        private float itemMoveAnimInterval = .5f;

        [SerializeField]
        private List<ItemSpawnProbability> itemSpawnProbabilities;

        
        
    
        public Vector2Int BaseGridSize => baseGridSize;
        public Vector2 ItemStillScale => itemStillScale;
        public Vector2 ItemDragScale => itemDragScale;
        
        public Color LinePassiveColor => linePassiveColor;
        public Color ActiveColor => activeColor;
        public Color GridHoverColor => gridHoverColor;
        public Color HoverColor => hoverColor;
        public Color ItemStillColor => itemStillColor;
        
        public LayerMask BaseTileLayer => baseTileLayer;
        public LayerMask BaseLineLayer => baseLineLayer;

        public float GridFillDuration => gridFillDuration;

        public int ItemSpawnCount => itemSpawnCount;

        public float ItemMoveAnimDuration => itemMoveAnimDuration;
        public float ItemMoveAnimInterval => itemMoveAnimInterval;

        public List<ItemSpawnProbability> ItemSpawnProbabilities => itemSpawnProbabilities;

    }
}