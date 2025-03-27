using UnityEngine;
using DG.Tweening;

namespace StickBlast
{
    public class BlastParticle : MonoBehaviour
    {
        private ParticleSystem particleSystem;
        private bool hasPlayed;
        private float duration = 0.5f;
        private float delay = 0.1f;

        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        public void Play(Color color, Vector3 position, float sequenceDelay, System.Action onComplete)
        {
            if (hasPlayed) return;
            hasPlayed = true;

            transform.position = position;
            
            // Particle system rengini ayarla
            var main = particleSystem.main;
            main.startColor = color;

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(sequenceDelay);
            sequence.OnComplete(() => {
                particleSystem.Play();
                // Particle system bittiğinde objeyi yok et
                //Destroy(gameObject, particleSystem.main.duration);
                onComplete?.Invoke();
            });
        }
    }
} 