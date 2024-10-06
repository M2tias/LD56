using UnityEngine;
using UnityEngine.UI;

public class BuildingProgressUI : MonoBehaviour
{
    [SerializeField]
    private Image progressMask;
    private float progress = 0f;

    private Canvas canvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    public void Initialize()
    {
    }

    // Update is called once per frame
    void Update()
    {
        progressMask.fillAmount = progress;
    }
}
