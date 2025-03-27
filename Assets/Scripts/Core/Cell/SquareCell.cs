using System;
using System.Collections.Generic;
using System.Linq;
using BlastRoot;
using UnityEngine;
using StickBlast.Models;
using DG.Tweening;
using UnityEngine.Audio;
using System.Collections;

namespace StickBlast
{
    public class SquareCell : MonoBehaviour
    {
        [SerializeField]
        public BlastParticle blastParticlePrefab;

        [SerializeField] 
        public SpriteRenderer starRenderer;

        [SerializeField] 
        private ParticleSystem starBonusParticle;

        private Vector2Int coordinate;
        public Vector2Int Coordinate => coordinate;

        private SpriteRenderer spriteRenderer;
        private Color hideColor = new Color(0, 0, 0, 0);
        private Tween blinkTween;

        private HashSet<Grain> gridLines;
        public bool IsOccupied;
        private bool IsHovered;
        private bool hasPlayedScaleAnimation;
        private bool hasPlayedBlastEffect;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (starRenderer != null)
            {
                starRenderer.enabled = false; // Ensure star starts disabled
            }
            else
            {
                Debug.LogError("Star Renderer is not assigned in inspector!");
            }
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
        public void ActivateStar()
        {
            Debug.Log("ActivateStar called");
            if (starRenderer != null)
            {
                Debug.Log("Star renderer found, enabling...");
                starRenderer.enabled = true;
                starRenderer.gameObject.SetActive(true); // Make sure the game object is also active
            }
            else
            {
                Debug.LogError("Star Renderer is null!");
            }
        }

        public void PlayStarBonusParticle()
        {
            if (starBonusParticle != null)
            {
                starBonusParticle.gameObject.SetActive(true);
                starBonusParticle.Play();
                StartCoroutine(DisableParticleAfterPlay());
            }
        }

        private IEnumerator DisableParticleAfterPlay()
        {
            starRenderer.gameObject.SetActive(false);
            yield return new WaitForSeconds(starBonusParticle.main.duration);
            starBonusParticle.gameObject.SetActive(false);
        }

        public void Initialize(Vector2Int coordinate, Grain topLine, Grain rightLine, Grain bottomLine, Grain leftLine)
        {
            this.coordinate = coordinate;

            gridLines = new HashSet<Grain>
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
                transform.DOScale(Vector3.one, 0.25f)
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
            hasPlayedBlastEffect = false;
        }

        public void PlayBlastEffect(float sequenceDelay, System.Action onComplete)
        {
            if (hasPlayedBlastEffect || !IsOccupied) return;
            hasPlayedBlastEffect = true;

            // Create a sequence for the shake and blast effect
            Sequence sequence = DOTween.Sequence();
            
            // Add shake effect
            sequence.Append(transform.DOShakePosition(0.3f, 0.2f, 10, 90, false, true)
                .SetEase(Ease.OutElastic));
            
            // Add blast effect after shake
            sequence.AppendCallback(() => {
                blastParticlePrefab.Play(GameConfigs.Instance.ActiveColor, transform.position, sequenceDelay, onComplete);
				//blastParticlePrefab.GetComponent<AudioSource>().Play();
			});
          
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

        public bool HasLine(Grain line)
        {
            return gridLines.Contains(line);
        }

        public void UpdateColor(ColorTypes status)
        {
        }
    }
}