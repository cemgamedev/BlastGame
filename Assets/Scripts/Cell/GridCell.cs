// maebleme2

using System;
using System.Collections.Generic;
using System.Linq;
using Ebleme;
using UnityEngine;
using StickBlast.Models;

namespace StickBlast
{
    public class GridCell : MonoBehaviour
    {
        private Vector2Int coordinate;
        public Vector2Int Coordinate => coordinate;

        private SpriteRenderer spriteRenderer;
        private Color hideColor = new Color(0, 0, 0, 0);

        private HashSet<BaseLine> gridLines;
        private bool IsOccupied;
        private bool IsHovered;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Hide();
        }

        public void Set(Vector2Int coordinate, BaseLine topLine, BaseLine rightLine, BaseLine bottomLine, BaseLine leftLine)
        {
            this.coordinate = coordinate;

            // lines = new Dictionary<Direction, BaseLine>();
            gridLines = new HashSet<BaseLine>
            {
                topLine,
                rightLine,
                bottomLine,
                leftLine
            };


            Vector3 center = Vector3.zero;
            foreach (var line in gridLines)
            {
                center += line.transform.position;
            }

            center /= gridLines.Count;

            // Nesneyi merkeze yerleştir
            transform.position = center;

            // Genişliği ve yüksekliği hesapla
            float width = Mathf.Abs((leftLine.transform.position - rightLine.transform.position).x);
            float height = Mathf.Abs((topLine.transform.position - bottomLine.transform.position).y);

            // Scale değerlerini ayarla
            spriteRenderer.size = new Vector2(width, height);
        }

        public bool IsLinesOccupied()
        {
            foreach (var line in gridLines)
            {
                if (!line.IsOccupied)
                {
                    return false;
                }
            }

            return true;
        }

        public void SetOccupied()
        {
            IsOccupied = true;
            IsHovered = false;

            Show();
        }

        public void DeOccupie()
        {
            // if (IsOccupied)
            // {
            //     foreach (var line in gridLines)
            //     {
            //         line.DeHover();
            //         line.DeOccupied();
            //     }
            // }

            IsOccupied = false;
            IsHovered = false;

            Hide();
        }

        public bool CanHover()
        {
            // can be deleted
            if (IsOccupied)
                return false;

            // line lar ya occupied olmalı yada hovering olmalı
            bool canHover = false;
            foreach (var line in gridLines)
            {
                canHover = line.IsHovering || line.IsOccupied;

                if (!canHover)
                    break;
            }

            return canHover;
        }

        public void Hover()
        {
            IsHovered = true;
            spriteRenderer.color = GameConfigs.Instance.GridHoverColor;
        }

        public void DeHover()
        {
            if (IsHovered)
            {
                IsHovered = false;
                Hide();
            }
        }

        private void Show()
        {
            spriteRenderer.color = GameConfigs.Instance.ActiveColor;
        }

        private void Hide()
        {
            spriteRenderer.color = hideColor;
        }

        public void ReColor(ColorTypes status)
        {
            switch (status)
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
    }
}