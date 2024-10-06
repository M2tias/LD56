using UnityEngine;
using UnityEngine.UI;

public class MouseUIPanel : MonoBehaviour
{
    [SerializeField]
    public Image resourceUI1;

    [SerializeField]
    public Image resourceUI2;

    [SerializeField]
    public Image hpUI1;

    [SerializeField]
    public Image hpUI2;

    [SerializeField]
    private Button panButton;

    private Transform target;
    private CameraMovement cameraMovement;

    public void Initialize(Transform target)
    {
        cameraMovement = Camera.main.GetComponent<CameraMovement>();
        this.target = target;
    }

    public void Plop()
    {
        cameraMovement.PanToTarget(target);
    }
}
