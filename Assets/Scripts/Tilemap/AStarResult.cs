using System.Collections.Generic;
using UnityEngine;

public class AStarResult
{
    public List<Vector3> Path { get; set; }
    public AStarNodeType NodeType { get; set; }
    public Vector3 ActionTargetPos { get; set; }
}

public enum AStarNodeType
{
    Walkable, // nothing interesting
    Interactable,
    Unwalkable,
    Enemy
}
