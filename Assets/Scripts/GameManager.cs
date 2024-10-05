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
        // Selectable selected = Selectables.FirstOrDefault(x => x.IsSelected());
        // 
        // if (selected != null)
        // {
        //     List<Vector3> path = AStarManager.instance.FindPath(selected.transform.position, target);
        //     StartCoroutine(StepSelected(selected, path));
        // }

        List<Vector3> occupied = new();

        foreach (Selectable selectable in Selectables.Where(x => x.IsSelected()))
        {
            List<Vector3> path = AStarManager.instance.FindPath(selectable.transform.position, target, occupied);
            if (path != null)
            {
                occupied.Add(path.Last());
                StartCoroutine(StepSelected(selectable, path));
            }
        }
    }

    IEnumerator StepSelected(Selectable selected, List<Vector3> path)
    {
        if (path != null)
        {
            foreach (Vector3 step in path)
            {
                selected.transform.position = step;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
