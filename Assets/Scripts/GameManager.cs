using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mousePrefab;
    [SerializeField]
    private Transform mouseUiPanelContainer;
    [SerializeField]
    private GameObject mouseUIPrefab;

    public List<Selectable> Selectables;

    public static GameManager instance;

    private float secondsPerBerry = 30;
    private float lastFood;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Selectables = FindObjectsByType<Selectable>(FindObjectsSortMode.None).ToList();
        lastFood = Time.time + secondsPerBerry; // 30s of mercy at the start
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            InstantiateMouse();
        }

        if (Time.time - secondsPerBerry > lastFood)
        {
            lastFood = Time.time;
            EffectManager.instance.PlayEatingFood();
        }
    }

    public Selectable InstantiateMouse()
    {
        GameObject uiObj = Instantiate(mouseUIPrefab, mouseUiPanelContainer, true);
        MouseUIPanel mouseUIPanel = uiObj.GetComponent<MouseUIPanel>();

        GameObject obj = Instantiate(mousePrefab, null, true);
        obj.transform.position = new Vector3(0.5f, 0.5f, 0);
        Selectable selectable = obj.GetComponent<Selectable>();
        Selectables.Add(selectable);
        MouseUI ui = obj.GetComponent<MouseUI>();

        ui.Initialize(mouseUIPanel);
        return selectable;
    }

    public void MoveSelectable(Vector3 target)
    {
        List<Vector3> occupied = new();

        foreach (Selectable selectable in Selectables.Where(x => x.IsSelected()))
        {
            AStarResult pathResult = AStarManager.instance.FindPath(selectable.transform.position, target, occupied);

            if (pathResult != null && pathResult?.Path != null)
            {
                if (pathResult.Path.Count == 0 && pathResult.NodeType != AStarNodeType.Interactable)
                {
                    return;
                }

                if (pathResult.Path.Count == 0)
                {
                    occupied.Add(selectable.transform.position);
                }
                else
                {
                    occupied.Add(pathResult.Path.Last());
                }

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
                ResourceType type = ResourceManager.instance.GetResourceTileType(pathResult.ActionTargetPos);


                if (type == ResourceType.None || selected.CanGather(type))
                {

                    selected.Attack();
                    yield return new WaitForSeconds(attackTime);

                    while (ResourceManager.instance.Interact(selected, pathResult.ActionTargetPos))
                    {
                        yield return new WaitForSeconds(attackTime);
                    }

                    selected.Idle();
                }
            }
        }
    }
}
