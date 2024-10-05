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
        grid.GenerateMap(logicalTilemap, colliderTilemap);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Vector3> FindPath(Vector3 start, Vector3 end, List<Vector3> occupied)
    {
        List<Vector3Int> occupiedPosList = occupied.Select(v => WorldToTilemap(v)).ToList();
        return grid.FindPath(WorldToTilemap(start), WorldToTilemap(end), occupiedPosList);
    }

    private Vector3Int WorldToTilemap(Vector3 pos)
    {
        //Vector3 tileMapOffSet = new Vector3(0.5f, 0.5f, 0);

        return logicalTilemap.WorldToCell(pos);
    }
}

public class AStarNode
{
    public int startCost;
    public int heuristicCost;
    public List<AStarNode> Neighbours;
    public AStarNode Parent;

    private bool walkable;
    private Vector3Int tileMapPos;
    private Vector2Int aStarPos;
    private Vector3 worldPos;

    public AStarNode(Vector2Int _aStarPos, Vector3Int _tileMapPos, Vector3 _worldPos, bool _walkable)
    {
        walkable = _walkable;
        tileMapPos = _tileMapPos;
        worldPos = _worldPos;
        aStarPos = _aStarPos;
        Neighbours = new List<AStarNode>();
    }

    public int costSum
    {
        get { return startCost + heuristicCost; }
    }

    public Vector2Int AStarPos
    {
        get { return aStarPos; }
    }

    public Vector3Int TilemapPos
    {
        get { return tileMapPos; }
    }

    public bool Walkable
    {
        get { return walkable; }
    }

    public Vector3 WorldPos
    {
        get
        {
            return worldPos;
        }
    }
}

public class AStarGrid
{
    private AStarNode[,] grid;
    private int width;
    private int height;
    private int xOffset;
    private int yOffset;

    public void GenerateMap(Tilemap map, Tilemap colliderMap)
    {
        Vector3 tileMapOffSet = new Vector3(0.5f, 0.5f, 0);
        width = map.cellBounds.max.x - map.cellBounds.min.x + 1;
        height = map.cellBounds.max.y - map.cellBounds.min.y + 1;
        xOffset = map.cellBounds.min.x;
        yOffset = map.cellBounds.min.y;

        grid = new AStarNode[width, height];
        int ax = 0;

        for (int x = map.cellBounds.min.x; x < map.cellBounds.max.x; x++)
        {
            int ay = 0;
            for (int y = map.cellBounds.min.y; y < map.cellBounds.max.y; y++)
            {
                for (int z = map.cellBounds.min.z; z < map.cellBounds.max.z; z++)
                {
                    Vector2Int aStarPos = new(ax, ay);
                    Vector3Int tileMapPos = new(x, y, z);
                    Vector3 pos = map.CellToWorld(tileMapPos) + tileMapOffSet;
                    Tile groundTile = map.GetTile<Tile>(tileMapPos);
                    Tile colliderTile = colliderMap.GetTile<Tile>(tileMapPos);

                    bool walkable = colliderTile == null && groundTile != null;
                    AStarNode node = new AStarNode(aStarPos, tileMapPos, pos, walkable);
                    grid[ax, ay] = node;
                }

                ay++;
            }

            ax++;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                AStarNode node = grid[x, y];

                if (node != null)
                {
                    node.Neighbours = GetNeighbours(node);
                }
            }
        }
    }

    public List<Vector3> FindPath(Vector3Int start, Vector3Int end, List<Vector3Int> occupied)
    {
        Vector2Int startPos = GridToAStarPos(new(start.x, start.y));
        Vector2Int endPos = GridToAStarPos(new(end.x, end.y));

        AStarNode startNode = grid[startPos.x, startPos.y];
        AStarNode endNode = grid[endPos.x, endPos.y];

        if (occupied.Any(x => Vector3.Distance(endNode.TilemapPos, x) < 0.1f)) {
            endNode = endNode.Neighbours.FirstOrDefault(neighbour => !occupied.Any(y => Vector3.Distance(neighbour.TilemapPos, y) < 0.1f));
        }


        List<AStarNode> openSet = new List<AStarNode>() { startNode };
        HashSet<AStarNode> closedSet = new HashSet<AStarNode>();

        while (openSet.Count > 0)
        {
            AStarNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (
                    openSet[i].costSum < currentNode.costSum ||
                    openSet[i].costSum == currentNode.costSum &&
                    openSet[i].heuristicCost < currentNode.heuristicCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                List<AStarNode> retracedNodes = RetracePath(startNode, endNode);
                return retracedNodes.Select(x => x.WorldPos).ToList();
            }

            foreach (AStarNode neighbour in currentNode.Neighbours)
            {
                if (!neighbour.Walkable || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.startCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.startCost || !openSet.Contains(neighbour))
                {
                    neighbour.startCost = newMovementCostToNeighbour;
                    neighbour.heuristicCost = GetDistance(neighbour, endNode);
                    neighbour.Parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null; // No path
    }

    private List<AStarNode> RetracePath(AStarNode startNode, AStarNode endNode)
    {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    private int GetDistance(AStarNode nodeA, AStarNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.AStarPos.x - nodeB.AStarPos.x);
        int dstY = Mathf.Abs(nodeA.AStarPos.y - nodeB.AStarPos.y);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }

    private List<AStarNode> GetNeighbours(AStarNode node)
    {
        List<AStarNode> neighbours = new();
        List<Vector2Int> neighbourOffset = new List<Vector2Int>()
        {
            new(-1, 1),  new(0, 1),  new(1, 1),
            new(-1, 0),              new(1, 0),
            new(-1, -1), new(0, -1), new(1, -1)
        };

        foreach (Vector2Int offset in neighbourOffset)
        {
            Vector2Int neighbourPos = node.AStarPos - offset;
            bool outOfBounds = neighbourPos.x < 0 || neighbourPos.x >= width || neighbourPos.y < 0 || neighbourPos.y >= height;

            if (outOfBounds)
            {
                continue;
            }

            AStarNode neighbour = grid[neighbourPos.x, neighbourPos.y];

            if (neighbour != null)
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    private Vector2Int GridToAStarPos(Vector2Int gridPos)
    {
        return new(gridPos.x - xOffset, gridPos.y - yOffset);
    }

    private Vector2Int AStarToGridPos(Vector2Int aStarPos)
    {
        return new(aStarPos.x + xOffset, aStarPos.y + yOffset);
    }
}