using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Selectable> Selectables;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Selectables = FindObjectsByType<Selectable>(FindObjectsSortMode.None).ToList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveSelectable(Vector3 target)
    {
        List<Vector3> occupied = new();

        foreach (Selectable selectable in Selectables.Where(x => x.IsSelected()))
        {
            AStarResult pathResult = AStarManager.instance.FindPath(selectable.transform.position, target, occupied);
            if (pathResult != null || pathResult.Path != null || pathResult.Path.Count == 0)
            {
                occupied.Add(pathResult.Path.Last());

                Coroutine ongoing = selectable.GetCurrentCoroutine();
                if (ongoing != null)
                {
                    StopCoroutine(ongoing);
                }

                selectable.SetTargetInteractable(pathResult.NodeType == AStarNodeType.Interactable);
                selectable.SetOngoingCoroutine(StartCoroutine(StepSelected(selectable, pathResult)));
            }
        }
    }

    IEnumerator StepSelected(Selectable selected, AStarResult pathResult)
    {
        float moveTime = 0.5f;
        float attackTime = 1f;

        if (pathResult?.Path != null)
        {
            foreach (Vector3 step in pathResult.Path)
            {
                if (Time.time - selected.LastMoveTime < moveTime)
                {
                    yield return new WaitForSeconds(Time.time - selected.LastMoveTime);
                }

                selected.LastMoveTime = Time.time;
                selected.transform.position = step;
                yield return new WaitForSeconds(moveTime);
            }

            // TODO: Path is done. If we need to interact, do it here?
            if (selected.TargetIsInteractable())
            {
                selected.Attack();
                yield return new WaitForSeconds(attackTime);
                while (ResourceManager.instance.Gather(pathResult.ActionTargetPos))
                {
                    yield return new WaitForSeconds(attackTime);
                }

                selected.Idle();
            }
        }
    }
}
