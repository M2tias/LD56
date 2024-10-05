using UnityEngine;

public class Selectable : MonoBehaviour
{
    [SerializeField]
    private GameObject selectBorder;
    [SerializeField]
    private GameObject hpUi;

    private bool isSelected = false;

    void Start()
    {
        if (selectBorder == null)
        {
            Debug.LogError($"{transform.name} is selectable but doesn't have selectBorder object assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        selectBorder.SetActive(isSelected);
        hpUi.SetActive(isSelected);
    }

    public void Select()
    {
        isSelected = true;
    }

    public void Deselect()
    {
        isSelected = false;
    }
}
