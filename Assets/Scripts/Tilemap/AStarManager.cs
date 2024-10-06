using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarManager : MonoBehaviour
{

    [SerializeField]
    private Tilemap logicalTilemap; // Ground map with the actual logical tiles

    [SerializeField]
    private Tilemap colliderTilemap;

    [SerializeField]
    private Tilemap visualTilemap; // Visual layer of logical map

    [SerializeField]
    private Tilemap interactableTilemap;

    [SerializeField]
    private Tilemap fogTilemap;

    [SerializeField]
    private List<DynamicInteractives> dynamicInteractives;

    [SerializeField]
    private Sprite logicalWater;

    private int fogRadius = 5;

    public static AStarManager instance;
    private AStarGrid grid;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grid = new AStarGrid();
        grid.GenerateMap(logicalTilemap, colliderTilemap, interactableTilemap, logicalWater.name);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public AStarResult FindPath(Vector3 start, Vector3 end, List<Vector3> occupied)
    {
        List<Vector3Int> occupiedPosList = occupied.Select(v => WorldToTilemap(v)).ToList();
        return grid.FindPath(WorldToTilemap(start), WorldToTilemap(end), occupiedPosList);
    }

    public Vector3Int WorldToTilemap(Vector3 pos)
    {
        return logicalTilemap.WorldToCell(pos);
    }

    public Vector3 TilemapToWorld(Vector3Int pos)
    {
        return logicalTilemap.WorldToCell(pos);
    }

    public string GetTileName(Vector3 pos)
    {
        Vector3Int tilePos = WorldToTilemap(pos);
        return interactableTilemap.GetTile<Tile>(tilePos)?.name;
    }

    public string GetTileName(Vector3Int pos)
    {
        return interactableTilemap.GetTile<Tile>(pos)?.name;
    }

    public void DeleteInteractable(Vector3Int pos)
    {
        interactableTilemap.SetTile(pos, null);
        grid.DeleteInteractable(pos);
    }

    public List<AStarNode> GetTileNeighbours(Vector3Int pos)
    {
        return grid.GetNeighbours(pos);
    }

    public void AddInteractable(Vector3Int tilePos, string tileName)
    {
        Tile tile = dynamicInteractives.FirstOrDefault(x => x.tileSprite.name == tileName)?.tile;

        if (tile != null)
        {
            interactableTilemap.SetTile(tilePos, tile);
            Debug.Log("Set tile");
            grid.AddInteractable(tilePos, tileName, interactableTilemap);
        }
        else
        {
            Debug.LogError($"Tile {tileName} was not configured in DynamicInteractives!");
        }
    }

    public void UpdateFog(Vector3 pos)
    {
        Vector3Int tilePos = fogTilemap.WorldToCell(pos);

        for (int i = -fogRadius; i < fogRadius; i++)
        {
            for (int j = -fogRadius; j < fogRadius; j++)
            {
                Vector3Int fogTilePos = tilePos + new Vector3Int(i, j, 0);

                if (Vector3Int.Distance(tilePos, fogTilePos) < fogRadius)
                {
                    fogTilemap.SetTile(fogTilePos, null);
                }
            }
        }
    }
}

[Serializable]
public class DynamicInteractives
{
    public Sprite tileSprite;
    public Tile tile;
}
