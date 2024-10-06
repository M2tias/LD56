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

    private Selectable selectable;

    public void Initialize(Transform target, Selectable selectable)
    {
        this.selectable = selectable;
        cameraMovement = Camera.main.GetComponent<CameraMovement>();
        this.target = target;
    }

    public void Plop()
    {
        GameManager.instance.Selectables.ForEach(selectable => selectable.Deselect());
        selectable.Select();
        cameraMovement.PanToTarget(target);
    }
}
