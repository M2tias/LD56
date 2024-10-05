using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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
        grid.GenerateMap(logicalTilemap, colliderTilemap, interactableTilemap);
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
}
