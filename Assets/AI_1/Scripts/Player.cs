using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player: MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Move Setting"), Space(10)]
    [SerializeField] private float WalkScale = 100f;
    [SerializeField] private float RunScale = 200f;
    [SerializeField] private float RotateScale = 100f;

    private float moveScale = 0f;
    private Rigidbody playerRigidbody = null;

    public Vector3 Position { get => transform.position; }

    private void Awake()
    {
        Instance = this;
        playerRigidbody = GetComponent<Rigidbody>();

    }

    private void OnEnable()
    {
        moveScale = WalkScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) moveScale = RunScale;
        if (Input.GetKeyUp(KeyCode.LeftShift)) moveScale = WalkScale;

        var horizontal = Input.GetAxis("Horizontal") * moveScale;
        var vertical = Input.GetAxis("Vertical") * moveScale;
        var rotate = Input.GetAxis("Mouse X") * RotateScale;
        transform.Rotate(0, rotate, 0);

        var forward = transform.forward * vertical;
        var right = transform.right * horizontal;

        playerRigidbody.velocity = forward + right;
    }
}
