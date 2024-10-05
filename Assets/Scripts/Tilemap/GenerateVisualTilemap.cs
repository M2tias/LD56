using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateVisualTilemap : MonoBehaviour
{
    [SerializeField]
    private Tilemap logicalMap;
    [SerializeField]
    private Tilemap visualTilemap;
    [SerializeField]
    private Tile first;
    [SerializeField]
    private Tile second;
    [SerializeField]
    private Tile[] tiles;

    Dictionary<string, TileType> tileTypeMap = new();
    Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile> tileMapping;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tileTypeMap.Add(first.name, TileType.Ground);
        tileTypeMap.Add(second.name, TileType.Grass);

        tileMapping = new()
        {
            { new (TileType.Grass, TileType.Grass, TileType.Ground, TileType.Grass), tiles[0] },
            { new (TileType.Grass, TileType.Ground, TileType.Grass, TileType.Ground), tiles[1] },
            { new (TileType.Ground, TileType.Grass, TileType.Ground, TileType.Ground), tiles[2] },
            { new (TileType.Grass, TileType.Grass, TileType.Ground, TileType.Ground), tiles[3] },
            { new (TileType.Ground, TileType.Grass, TileType.Grass, TileType.Ground), tiles[4] },
            { new (TileType.Grass, TileType.Ground, TileType.Ground, TileType.Ground), tiles[5] },
            { new (TileType.Ground, TileType.Ground, TileType.Ground, TileType.Ground), tiles[6] },
            { new (TileType.Ground, TileType.Ground, TileType.Ground, TileType.Grass), tiles[7] },
            { new (TileType.Grass, TileType.Ground, TileType.Grass, TileType.Grass), tiles[8] },
            { new (TileType.Ground, TileType.Ground, TileType.Grass, TileType.Grass), tiles[9] },
            { new (TileType.Ground, TileType.Ground, TileType.Grass, TileType.Ground), tiles[10] },
            { new (TileType.Ground, TileType.Grass, TileType.Ground, TileType.Grass), tiles[11] },
            { new (TileType.Grass, TileType.Grass, TileType.Grass, TileType.Grass), tiles[12] },
            { new (TileType.Grass, TileType.Grass, TileType.Grass, TileType.Ground), tiles[13] },
            { new (TileType.Grass, TileType.Ground, TileType.Ground, TileType.Grass), tiles[14] },
            { new (TileType.Ground, TileType.Grass, TileType.Grass, TileType.Grass), tiles[15] },
        };

        bool firstX = true;

        for (int x = logicalMap.cellBounds.min.x; x < logicalMap.cellBounds.max.x-1; x++)
        {
            bool firstY = true;
            for (int y = logicalMap.cellBounds.min.y; y < logicalMap.cellBounds.max.y-1; y++)
            {
                for (int z = logicalMap.cellBounds.min.z; z < logicalMap.cellBounds.max.z; z++)
                {
                    if (firstY || firstX)
                    {
                        continue;
                    }

                    TileBase tile = logicalMap.GetTile(new Vector3Int(x, y, z));
                    if (tile == null)
                    {
                        continue;
                    }

                    TileType[] tileKeyArray = 
                        new List<Vector3Int>() {
                            new Vector3Int(x-1, y, z),
                            new Vector3Int(x, y, z),
                            new Vector3Int(x-1, y-1, z),
                            new Vector3Int(x, y-1, z)
                        }
                        .Select(v => logicalMap.GetTile<Tile>(v))
                        .Where(x => x != null)
                        .Select(t => getType(t))
                        .ToArray();

                    if (tileKeyArray.Length < 4)
                    {
                        continue;
                    }

                    Tuple<TileType, TileType, TileType, TileType> tileKey =
                        Tuple.Create(tileKeyArray[0], tileKeyArray[1], tileKeyArray[2], tileKeyArray[3]);

                    if (tileMapping.ContainsKey(tileKey))
                    {
                        visualTilemap.SetTile(new Vector3Int(x, y, z), tileMapping[tileKey]);
                    }
                    else
                    {
                        visualTilemap.SetTile(new Vector3Int(x, y, z), first);
                    }
                }

                firstY = false;
            }

            firstX = false;
        }

        if (logicalMap?.gameObject?.TryGetComponent(out TilemapRenderer tmRenderer) ?? false)
        {
            tmRenderer.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private TileType getType(Tile tile)
    {
        return tileTypeMap[tile.name]; // tileTypeMap.ContainsKey(tile.name) ? tileTypeMap[tile.name] : TileType.Ground;
    }

    private 

    enum TileType
    {
        Ground,
        Grass
    }
}
