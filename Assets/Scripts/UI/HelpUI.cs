using UnityEngine;

public class HelpUI : MonoBehaviour
{
    [SerializeField]
    private GameObject helpPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleHelp()
    {
        helpPanel.SetActive(!helpPanel.activeSelf);
    }
}
