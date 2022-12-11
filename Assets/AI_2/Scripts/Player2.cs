using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player2 : MonoBehaviour
{
    [Header("Move Setting"), Space(10)]
    [SerializeField] private float WalkScale = 100f;

    private float moveScale = 0f;
    private Rigidbody playerRigidbody = null;
    private Camera mainCamera = null;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        moveScale = WalkScale;
    }

    private void Update()
    {
        var horizontal = Input.GetAxis("Horizontal") * moveScale;
        var vertical = Input.GetAxis("Vertical") * moveScale;
        var position = GetCursorPosition();

        transform.LookAt(position + Vector3.up * transform.position.y);
        playerRigidbody.velocity = new Vector3(horizontal, 0, vertical);
    }

    private Vector3 GetCursorPosition()
    {
        var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.y);
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }
}
