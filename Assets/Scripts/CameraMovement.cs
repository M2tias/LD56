using UnityEngine;
using UnityEngine.Events;

public class CameraMovement : MonoBehaviour
{
    private float cameraMoveSpeed = 8f;
    private Vector3 mouseOffset = Vector3.zero;

    [SerializeField]
    private float maxX;
    [SerializeField]
    private float minX;
    [SerializeField]
    private float maxY;
    [SerializeField]
    private float minY;

    [SerializeField]
    private GameObject foodParticleSystem;

    private bool panningMode = false;
    private float autoPanSpeedPerUnit = 35f;
    private float currentAutoPanSpeed = 0f;
    private Vector3 panStart;
    private Vector3 panEnd;
    private float panT = 0f;

    public UnityAction<Transform> PanToTargetAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PanToTargetAction += PanToTarget;
    }

    // Update is called once per frame
    void Update()
    {
        if (panningMode)
        {
            panT += Time.deltaTime * currentAutoPanSpeed;

            float z = transform.position.z;
            Vector3 tmpPos = transform.position;

            tmpPos = Vector3.Lerp(panStart, panEnd, panT);
            tmpPos = new(tmpPos.x, tmpPos.y, z);
            transform.position = tmpPos;

            if (panT >= 1f)
            {
                panningMode = false;
                panT = 0f;
            }
        }
        else
        {
            float frameMove = cameraMoveSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.UpArrow))
            {
                transform.position = new(transform.position.x, transform.position.y + frameMove, transform.position.z);
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.DownArrow))
            {
                transform.position = new(transform.position.x, transform.position.y - frameMove, transform.position.z);
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position = new(transform.position.x - frameMove, transform.position.y, transform.position.z);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.position = new(transform.position.x + frameMove, transform.position.y, transform.position.z);
            }

            if (Input.GetMouseButtonDown(2))
            {
                mouseOffset = Input.mousePosition - Camera.main.ViewportToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(2))
            {
                transform.position = transform.position + (-1f) * Input.mousePositionDelta * 0.1f;
            }
        }

        Vector3 pos = transform.position;
        float x = Mathf.Clamp(pos.x, minX, maxX);
        float y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = new Vector3(x, y, pos.z);


        float horizontalExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
        float verticalExtent = Camera.main.orthographicSize;

        foodParticleSystem.transform.position = new Vector3(x + horizontalExtent - 1f, y - verticalExtent - 1f, foodParticleSystem.transform.position.z);

        //Debug.Log($"Min: ({minX:#.##},{minY:#.##}), Max: ({maxX:#.##}{maxY:#.##})");
    }

    public void PanToTarget(Transform target)
    {
        panEnd = target.position;// GameManager.instance.GetPanTarget();
        panStart = transform.position;
        panT = 0f;

        currentAutoPanSpeed = autoPanSpeedPerUnit / Vector3.Distance(panEnd, panStart);

        panningMode = true;
    }
}
