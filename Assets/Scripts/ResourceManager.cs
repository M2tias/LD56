using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    // Interactive sprites/tileMap names
    [SerializeField]
    private Sprite NutSprite;
    [SerializeField]
    private List<Sprite> BerrySprites;
    [SerializeField]
    private Sprite BranchSprite;
    [SerializeField]
    private Sprite RepositorySprite;
    [SerializeField]
    private Sprite BuildingSprite;

    [SerializeField]
    private Sprite LoveHutSprite;

    // UI Texts
    [SerializeField]
    private Text berryText;
    [SerializeField]
    private Text branchText;
    [SerializeField]
    private Text nutText;

    private Dictionary<string, int> tileAmounts = new Dictionary<string, int>();
    private Dictionary<string, ResourceType> resourceTypes = new Dictionary<string, ResourceType>();
    private Dictionary<string, float> buildingProgress = new();

    private List<Selectable> loveHutMice = new();

    // how much you get from one tile and what's the max amount for mouse
    private int amountPerTile = 5;
    private int buildAmount = 20;
    private int loveHutMiceAmount = 2;

    private int branches = 0;
    private int nuts = 0;
    private int berries = 10;
    private int winScene = 3;
    private int loseScene = 2;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resourceTypes.Add(NutSprite.name, ResourceType.Nut);
        BerrySprites.ForEach(b => resourceTypes.Add(b.name, ResourceType.Berry));
        resourceTypes.Add(BranchSprite.name, ResourceType.Branch);
    }

    // Update is called once per frame
    void Update()
    {
        berryText.text = $"{berries}/100";
        branchText.text = $"{branches}/100";
        nutText.text = $"{nuts}/100";

        if (nuts >= 100)
        {
            Invoke("Victory", 1f);
        }

        if (berries < 0)
        {
            Invoke("Lose", 1f);
        }
    }

    private void Victory()
    {
        SceneManager.LoadScene(winScene);
    }

    private void Lose()
    {
        SceneManager.LoadScene(loseScene);
    }

    public bool Interact(Selectable mouse, Vector3 resourcePos)
    {
        Vector3Int tilePos = AStarManager.instance.WorldToTilemap(resourcePos);
        string tileName = AStarManager.instance.GetTileName(tilePos);

        if (tileName == RepositorySprite.name)
        {
            (ResourceType type, int amount) = mouse.Dump();

            switch (type)
            {
                case ResourceType.Berry:
                    berries += amount;
                    break;
                case ResourceType.Branch:
                    branches += amount;
                    break;
                case ResourceType.Nut:
                    nuts += amount;
                    break;
            }

            StartCoroutine(PopDumpEffect(resourcePos, amount));

            // Dump everything at once and it's done
            return false;
        }
        else if (tileName == BuildingSprite.name)
        {
            return Build(mouse, tilePos, tileName);
        }
        else if (tileName == LoveHutSprite.name)
        {
            return Copulate(mouse, tilePos, tileName);
        }
        else
        {
            return Gather(mouse, tilePos, tileName);
        }
    }

    private IEnumerator PopDumpEffect(Vector3 resourcePos, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            EffectManager.instance.InstantiatePlusOne(resourcePos + Random.Range(-0.5f, 0.5f) * Vector3.right);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public ResourceType GetResourceTileType(Vector3 resourcePos)
    {
        Vector3Int tilePos = AStarManager.instance.WorldToTilemap(resourcePos);
        string tileName = AStarManager.instance.GetTileName(tilePos);
        if (resourceTypes.TryGetValue(tileName, out ResourceType type))
        {
            return type;
        }

        return ResourceType.None;
    }

    public void EatFood()
    {
        berries--;
    }

    private bool Copulate(Selectable mouse, Vector3Int tilePos, string tileName)
    {
        if (loveHutMice.Count() < loveHutMiceAmount - 1)
        {
            mouse.gameObject.SetActive(false);
            loveHutMice.Add(mouse);
        }
        else if (loveHutMice.Count() == loveHutMiceAmount - 1)
        {
            if (GameManager.instance.Selectables.Count < 8)
            {
                EffectManager.instance.InstantiateHearts(AStarManager.instance.TilemapToWorld(tilePos) + Vector3.up * 0.5f);
                Selectable newMouse = GameManager.instance.InstantiateMouse();
                loveHutMice.Add(newMouse);
            }

            loveHutMice.Add(mouse);
            List<AStarNode> neighbours = AStarManager.instance.GetTileNeighbours(tilePos);

            loveHutMice.ForEach(m => m.gameObject.SetActive(true));
            Queue<Selectable> miceQueue = new(loveHutMice);

            foreach (AStarNode node in neighbours)
            {
                if (miceQueue.TryDequeue(out Selectable m))
                {
                    m.transform.position = node.WorldPos;
                }
                else
                {
                    break;
                }
            }

            loveHutMice = new();
        }

        return false;

    }

    private bool Build(Selectable mouse, Vector3Int tilePos, string tileName)
    {
        string dictKey = $"{tileName}|({tilePos.x},{tilePos.y},{tilePos.z})";

        if (!buildingProgress.ContainsKey(dictKey))
        {
            buildingProgress.Add(dictKey, 0);
        }

        if (buildingProgress[dictKey] >= buildAmount)
        {
            buildingProgress.Remove(dictKey);
            AStarManager.instance.DeleteInteractable(tilePos);
            AStarManager.instance.AddInteractable(tilePos, LoveHutSprite.name);
            return false;
        }

        if (branches > 0)
        {
            EffectManager.instance.InstantiateMinusOne(AStarManager.instance.TilemapToWorld(tilePos) + Vector3.up * 0.5f);
            branches--;
            buildingProgress[dictKey]++;
        }
        else if (branches <= 0)
        {
            return false;
        }

        return true;
    }

    private bool Gather(Selectable mouse, Vector3Int tilePos, string tileName)
    {
        if (tileName == null)
        {
            return false;
        }

        string dictKey = $"{tileName}|({tilePos.x},{tilePos.y},{tilePos.z})";

        if (!tileAmounts.ContainsKey(dictKey))
        {
            tileAmounts.Add(dictKey, amountPerTile);
        }

        if (tileAmounts[dictKey] > 1)
        {
            EffectManager.instance.InstantiatePlusOne(AStarManager.instance.TilemapToWorld(tilePos));
            tileAmounts[dictKey]--;
        }
        else
        {
            tileAmounts.Remove(dictKey);
            AStarManager.instance.DeleteInteractable(tilePos);
        }

        // This happens when resource dumping
        // if (NutSprite.name == tileName)
        // {
        //     nuts++;
        // }
        // else if (BerrySprites.Select(x => x.name).Contains(tileName))
        // {
        //     berries++;
        // }
        // else if (BranchSprite.name == tileName)
        // {
        //     branches++;
        // }

        if (resourceTypes.TryGetValue(tileName, out ResourceType type))
        {
            mouse.Gather(type);
        }

        return tileAmounts.ContainsKey(dictKey);
    }

    public int MaxResources()
    {
        return amountPerTile;
    }
}

public enum ResourceType
{
    None,
    Berry,
    Branch,
    Nut
}
