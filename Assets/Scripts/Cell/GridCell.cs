using System;
using System.Collections.Generic;
using System.Linq;
using Ebleme;
using UnityEngine;
using StickBlast.Models;
using DG.Tweening;

namespace StickBlast
{
    public class GridCell : MonoBehaviour
    {
        private Vector2Int coordinate;
        public Vector2Int Coordinate => coordinate;

        private SpriteRenderer spriteRenderer;
        private Color hideColor = new Color(0, 0, 0, 0);
        private Tween blinkTween;

        private HashSet<BaseLine> gridLines;
        private bool IsOccupied;
        private bool IsHovered;
        private bool hasPlayedScaleAnimation;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            SetVisibility(false);
        }

        private void OnDestroy()
        {
            if (blinkTween != null)
            {
                blinkTween.Kill();
                blinkTween = null;
            }
        }

        public void Initialize(Vector2Int coordinate, BaseLine topLine, BaseLine rightLine, BaseLine bottomLine, BaseLine leftLine)
        {
            this.coordinate = coordinate;

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

            transform.position = center;

            float width = Mathf.Abs((leftLine.transform.position - rightLine.transform.position).x);
            float height = Mathf.Abs((topLine.transform.position - bottomLine.transform.position).y);

            spriteRenderer.size = new Vector2(width, height);
        }

        public bool CheckLineOccupation()
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
            spriteRenderer.color = GameConfigs.Instance.ActiveColor;

            if (!hasPlayedScaleAnimation)
            {
                transform.localScale = Vector3.zero;
                transform.DOScale(Vector3.one, 1f)
                    .SetEase(Ease.Linear);
                hasPlayedScaleAnimation = true;
            }
        }

        public void ClearOccupation()
        {
            IsOccupied = false;
            IsHovered = false;
            spriteRenderer.color = hideColor;
            hasPlayedScaleAnimation = false;
        }

        public bool CanBeHovered()
        {
            if (IsOccupied)
                return false;

            bool canHover = false;
            foreach (var line in gridLines)
            {
                canHover = line.IsHovering || line.IsOccupied;

                if (!canHover)
                    break;
            }

            return canHover;
        }

        public void SetHover()
        {
            IsHovered = true;
        }

        public void ClearHover()
        {
            if (IsHovered)
            {
                IsHovered = false;
            }
        }

        private void SetVisibility(bool isVisible)
        {
            if (IsOccupied)
            {
                spriteRenderer.color = GameConfigs.Instance.ActiveColor;
            }
            else
            {
                spriteRenderer.color = hideColor;
            }
        }

        public void UpdateColor(ColorTypes status)
        {
        }
    }
}