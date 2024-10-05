using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;
    private int branches = 0;
    private int nuts = 0;
    private int berries = 0;

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

    public void Gather(Vector3 resourcePos)
    {
        // - Get interactables map
        // - Find resource from tilemap
        // - Delete/Update from tilemap
        // - Increment related resource
    }
}
