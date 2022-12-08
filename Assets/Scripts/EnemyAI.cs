using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private enum State 
    { 
        Roaming, 
        Chasing, 
        Attack
    }

    [Header("Move Setting"), Space(10)]
    [SerializeField] private float MoveSpeed = 2f;
    [SerializeField] private float RoamRadius = 5f;
    [SerializeField] private float RoamTimer = 10f;

    private float preTimer = 0f;
    private float fullTimer = 0f;

    private State enemyState = State.Roaming;
    private NavMeshAgent enemyAgent = null;

    [Header("Body Setting"), Space(10)]
    [SerializeField] private MeshRenderer EnemyRenderer = null;
    [SerializeField] private Color[] EnemyColors = null;

    private Material enemyMaterial = null;

    [Header("Target Setting"), Space(10)]
    [SerializeField] private float SearchRadius = 10f;

    private float searchRadiusX2 = 0f;

    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyMaterial = EnemyRenderer.material;
    }

    private void OnEnable()
    {
        InitializeEnemy();
    }

    private void InitializeEnemy()
    {
        enemyAgent.speed = MoveSpeed;
        searchRadiusX2 = SearchRadius * SearchRadius;

        enemyState = State.Roaming;
        ChangeStateColor(enemyState);
        InitializeTimer();
    }

    private void InitializeTimer()
    {
        preTimer = 0f;
        fullTimer = Random.Range(RoamTimer - 5f, RoamTimer + 5f);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (enemyState)
        {
            case State.Roaming:
                if (preTimer < fullTimer)
                    preTimer += Time.deltaTime;
                else
                {
                    MovePosition(transform.position + Random.insideUnitSphere * RoamRadius);
                    InitializeTimer();
                }
                FindTarget();
                break;
            case State.Chasing:
                MovePosition(Player.Instance.Position);
                LostTarget();
                break;
        }
    }

    private void MovePosition(Vector3 position)
    {
        position.y = 200;
        if (Physics.Raycast(position, -Vector3.up, out RaycastHit hit, 1000, 1))
        {
            var path = new NavMeshPath();

            enemyAgent.CalculatePath(hit.point, path);
            if (path.status == NavMeshPathStatus.PathComplete)
                enemyAgent.SetPath(path);
        }
    }

    private void FindTarget()
    {
        if ((transform.position - Player.Instance.Position).sqrMagnitude < searchRadiusX2)
        {
            enemyState = State.Chasing;
            ChangeStateColor(enemyState);
        }
    }

    private void LostTarget()
    {
        if ((transform.position - Player.Instance.Position).sqrMagnitude > searchRadiusX2)
        {
            enemyState = State.Roaming;
            ChangeStateColor(enemyState);
            InitializeTimer();
        }
    }

    private void ChangeStateColor(State state)
    {
        switch (state)
        {
            case State.Roaming:
                enemyMaterial.SetColor("_BaseColor", EnemyColors[0]);
                break;
            case State.Chasing:
                enemyMaterial.SetColor("_BaseColor", EnemyColors[1]);
                break;
        }
    }
}
