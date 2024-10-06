using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;
    [SerializeField]
    private Sprite NutSprite;
    [SerializeField]
    private List<Sprite> BerrySprites;
    [SerializeField]
    private Sprite BranchSprite;

    private Dictionary<string, int> tileAmounts = new Dictionary<string, int>();

    // how much you get from one tile and what's the max amount for mouse
    private int amountPerTile = 5;

    private int branches = 0;
    private int nuts = 0;
    private int berries = 0;

    private int test = 3;

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

    public bool Gather(Selectable mouse, Vector3 resourcePos)
    {
        Vector3Int tilePos = AStarManager.instance.WorldToTilemap(resourcePos);
        string tileName = AStarManager.instance.GetTileName(tilePos);
        string dictKey = $"{tileName}|({tilePos.x},{tilePos.y},{tilePos.z})";

        if (!tileAmounts.ContainsKey(dictKey))
        {
            tileAmounts.Add(dictKey, amountPerTile);
        }

        if (tileAmounts[dictKey] > 1)
        {
            tileAmounts[dictKey]--;
        }
        else
        {
            tileAmounts.Remove(dictKey);
            AStarManager.instance.DeleteInteractable(tilePos);
        }

        if (NutSprite.name == tileName)
        {
            nuts++;
        }
        else if (BerrySprites.Select(x => x.name).Contains(tileName))
        {
            berries++;
        }
        else if (BranchSprite.name == tileName)
        {
            branches++;
        }

        mouse.Gather();
        return tileAmounts.ContainsKey(dictKey);
    }

    public int MaxResources()
    {
        return amountPerTile;
    }
}
