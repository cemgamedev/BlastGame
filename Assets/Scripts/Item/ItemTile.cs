// maebleme2

using Ebleme;
using StickBlast.Grid;
using UnityEngine;
using StickBlast.Models;

namespace StickBlast
{
    public class ItemTile:TileController
    {
        public Moveable Moveable { get; private set; }
        
        public Item Item { get; private set; }
       
        private Collider2D collider2d;
        private Vector3 startPosition;
        private SpriteRenderer spriteRenderer;

        
        private void Awake()
        {
            startPosition = transform.position;
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            Moveable = GetComponent<Moveable>();
            collider2d = GetComponent<Collider2D>();
        }

        public void SetActiveCollider(bool active)
        {
            collider2d.enabled = active;
        }

        public void SetItem(Item item)
        {
            Item = item;
        }
       
        public void BackToStartPosition()
        {
            transform.position = startPosition;
        }

        public BaseTile SetPositionToHit()
        {
            var hit = Moveable.Hit();
           
            var baseTile = hit.transform.GetComponent<BaseTile>();
           
            var target = hit.transform.position;
            target.z = 0f;
            transform.position = target;
           
            SetActiveCollider(false);

            return baseTile;
        }

        public void ReColor(ColorTypes type)
        {
            switch (type)
            {
                case ColorTypes.ItemStill:
                    spriteRenderer.color = GameConfigs.Instance.ItemStillColor;
                    break;
                case ColorTypes.Passive:
                    spriteRenderer.color = GameConfigs.Instance.LinePassiveColor;
                    break;
                case ColorTypes.Hover:
                    spriteRenderer.color = GameConfigs.Instance.HoverColor;
                    break;
                case ColorTypes.Active:
                    spriteRenderer.color = GameConfigs.Instance.ActiveColor;
                    break;
                default:
                    break;
            }
        }
        
        public RaycastHit2D Hit()
        {
            return Physics2D.Raycast(transform.position, Vector3.forward, 30, GameConfigs.Instance.BaseTileLayer);
        }
    }
}