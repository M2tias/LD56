using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarGrid
{
    private AStarNode[,] grid;
    private int width;
    private int height;
    private int xOffset;
    private int yOffset;

    public void GenerateMap(Tilemap map, Tilemap colliderMap, Tilemap interactableTilemap, string waterName)
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
                    Tile interactableTile = interactableTilemap.GetTile<Tile>(tileMapPos);

                    bool interactable = interactableTile != null;
                    bool walkable = colliderTile == null && groundTile != null && groundTile.name != waterName;

                    AStarNode node = new AStarNode(aStarPos, tileMapPos, pos, walkable, interactable);
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

    public AStarResult FindPath(Vector3Int start, Vector3Int end, List<Vector3Int> occupied)
    {
        Vector2Int startPos = GridToAStarPos(new(start.x, start.y));
        Vector2Int endPos = GridToAStarPos(new(end.x, end.y));

        AStarNode startNode = grid[startPos.x, startPos.y];
        AStarNode endNode = grid[endPos.x, endPos.y];

        if (occupied.Any(x => Vector3.Distance(endNode.TilemapPos, x) < 0.1f))
        {
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
                return new AStarResult()
                {
                    Path = retracedNodes.Select(x => x.WorldPos).ToList(),
                    NodeType = AStarNodeType.Walkable
                };
            }

            foreach (AStarNode neighbour in currentNode.Neighbours)
            {
                bool nonEndInteractable = neighbour.Interactable && neighbour != endNode;
                bool neighbourEndNodeInteractable = neighbour.Interactable && neighbour == endNode;

                if (neighbourEndNodeInteractable)
                {
                    List<AStarNode> retracedNodes = RetracePath(startNode, currentNode);
                    return new AStarResult()
                    {
                        Path = retracedNodes.Select(x => x.WorldPos).ToList(),
                        NodeType = AStarNodeType.Interactable,
                        ActionTargetPos = neighbour.WorldPos
                    };
                }

                if (!neighbour.Walkable || closedSet.Contains(neighbour) || nonEndInteractable)
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.startCost + GetDistance(currentNode, neighbour);

                if (newMovementCostToNeighbour < neighbour.startCost || !openSet.Contains(neighbour))
                {
                    neighbour.startCost = newMovementCostToNeighbour;
                    neighbour.heuristicCost = GetDistance(neighbour, endNode);
                    neighbour.Parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return null; // No path
    }

    public void DeleteInteractable(Vector3Int tilePos)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                AStarNode node = grid[x, y];

                if (node != null && node.TilemapPos == tilePos)
                {
                    node.Walkable = true;
                    node.Interactable = false;
                }
            }
        }
    }

    public void AddInteractable(Vector3Int tilePos, string tileName, Tilemap interactableTilemap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                AStarNode node = grid[x, y];

                if (node != null && node.TilemapPos == tilePos)
                {
                    // Vector3 worldPos = interactableTilemap.CellToWorld(tilePos);
                    // AStarNode newNode = new(new(x, y), tilePos, worldPos, true, true);
                    // grid[x, y] = newNode;

                    node.Walkable = true;
                    node.Interactable = true;
                }
            }
        }
    }

    public List<AStarNode> GetNeighbours(Vector3Int pos)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                AStarNode node = grid[x, y];

                if (node != null && node.TilemapPos == pos)
                {
                    return GetNeighbours(node);
                }
            }
        }

        return new();
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