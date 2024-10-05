using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    public int startCost;
    public int heuristicCost;
    public List<AStarNode> Neighbours;
    public AStarNode Parent;

    private bool walkable;
    private bool interactable;
    private Vector3Int tileMapPos;
    private Vector2Int aStarPos;
    private Vector3 worldPos;

    public AStarNode(Vector2Int _aStarPos, Vector3Int _tileMapPos, Vector3 _worldPos, bool _walkable, bool _interactable)
    {
        walkable = _walkable;
        tileMapPos = _tileMapPos;
        worldPos = _worldPos;
        aStarPos = _aStarPos;
        Neighbours = new List<AStarNode>();
        interactable = _interactable;
    }

    public bool Walkable { get => walkable; set => walkable = value; }
    public bool Interactable { get => interactable; set => interactable = value; }

    public int costSum => startCost + heuristicCost;
    public Vector2Int AStarPos => aStarPos;
    public Vector3Int TilemapPos => tileMapPos;
    public Vector3 WorldPos => worldPos;
}