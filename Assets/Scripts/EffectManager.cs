using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    [SerializeField]
    private GameObject heartParticlesPrefab;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateHearts(Vector3 pos)
    {
        GameObject particles = Instantiate(heartParticlesPrefab, transform, true);
        particles.transform.position = pos;
    }
}
