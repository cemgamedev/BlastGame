// maebleme2

using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Level", menuName = "BlastRoot/Level", order = 0)]
    public class LevelPreset : ScriptableObject
    {
        [SerializeField]
        private Vector2Int baseGridSize;

        [SerializeField]
        private Color linePassiveColor;
        
        [SerializeField]
        private Color hoverColor; 
        
        [SerializeField]
        private Color activeColor;

        [Header("Item")]
        [SerializeField]
        private Color itemStillColor;
    
        public Vector2Int BaseGridSize => baseGridSize;
        
        public Color LinePassiveColor => linePassiveColor;
        public Color ActiveColor => activeColor;
        public Color HoverColor => hoverColor;
        public Color ItemStillColor => itemStillColor;
    }
}