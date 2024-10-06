using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    [SerializeField]
    private GameObject heartParticlesPrefab;

    [SerializeField]
    private GameObject plusOneParticlesPrefab;

    [SerializeField]
    private GameObject minusOneParticlesPrefab;

    [SerializeField]
    private GameObject eatingFoodParticlesPrefab;

    [SerializeField]
    private ParticleSystem eatingFoodParticles;

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

    public void InstantiatePlusOne(Vector3 pos)
    {
        GameObject particles = Instantiate(plusOneParticlesPrefab, transform, true);
        particles.transform.position = pos;
    }

    public void InstantiateMinusOne(Vector3 pos)
    {
        GameObject particles = Instantiate(minusOneParticlesPrefab, transform, true);
        particles.transform.position = pos;
    }

    public void InstantiateEatingFood(Vector3 pos)
    {
        GameObject particles = Instantiate(eatingFoodParticlesPrefab, transform, true);
        particles.transform.position = pos;
    }

    public void PlayEatingFood()
    {
        eatingFoodParticles.Play();
    }

}
