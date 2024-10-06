using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraMovement : MonoBehaviour
{
    private float cameraMoveSpeed = 8f;
    private Vector3 mouseOffset = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
}
