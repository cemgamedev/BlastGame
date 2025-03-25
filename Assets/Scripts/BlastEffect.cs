using UnityEngine;
using System.Collections;

public class BlastEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem blastParticles;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeIntensity = 0.5f;
    [SerializeField] private GameObject blastSpritePrefab;
    [SerializeField] private float blastSpriteDuration = 0.5f;
    
    private Camera mainCamera;
    
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    
    public void PlayBlastEffect(Vector3 position)
    {
        // Spawn particles
        if (blastParticles != null)
        {
            ParticleSystem particles = Instantiate(blastParticles, position, Quaternion.identity);
            Destroy(particles.gameObject, particles.main.duration);
        }
        
        // Spawn blast sprite
        if (blastSpritePrefab != null)
        {
            GameObject blastSprite = Instantiate(blastSpritePrefab, position, Quaternion.identity);
            StartCoroutine(FadeOutSprite(blastSprite));
        }
        
        // Camera shake
        StartCoroutine(ShakeCamera());
    }
    
    private IEnumerator FadeOutSprite(GameObject spriteObject)
    {
        SpriteRenderer spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        float elapsed = 0f;
        
        while (elapsed < blastSpriteDuration)
        {
            float alpha = 1 - (elapsed / blastSpriteDuration);
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        Destroy(spriteObject);
    }
    
    private IEnumerator ShakeCamera()
    {
        Vector3 originalPosition = mainCamera.transform.localPosition;
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            
            mainCamera.transform.localPosition = new Vector3(x, y, originalPosition.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        mainCamera.transform.localPosition = originalPosition;
    }
} 