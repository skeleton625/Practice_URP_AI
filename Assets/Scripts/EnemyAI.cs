using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private enum State 
    { 
        Roaming, 
        Chasing, 
        Attack,
        AttackWait,
    }

    [Header("Move Setting"), Space(10)]
    [SerializeField] private float MoveSpeed = 2f;
    [SerializeField] private float RoamRadius = 5f;
    [SerializeField] private float RoamTimer = 10f;

    private float preRoamTimer = 0f;
    private float fullRoamTimer = 0f;

    private State enemyState = State.Roaming;
    private NavMeshAgent enemyAgent = null;

    [Header("Body Setting"), Space(10)]
    [SerializeField] private MeshRenderer EnemyRenderer = null;
    [SerializeField] private Color[] EnemyColors = null;

    private Material enemyMaterial = null;

    [Header("Target Setting"), Space(10)]
    [SerializeField] private float SearchRadius = 10f;
    [SerializeField] private float EscapeRadius = 15f;
    [SerializeField] private float AttackRadius = 2f;
    [SerializeField] private float AttackTimer = 0f;
    [SerializeField] private float AttackWaitTimer = 0f;

    private float searchRadiusX2 = 0f;
    private float escapeRadiusX2 = 0f;
    private float attackRadiusX2 = 0f;

    private float preAttackTimer = 0f;
    private float preAttackWaitTimer = 0f;

    #region Enemy AI Initialize Functions
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
        escapeRadiusX2 = EscapeRadius * EscapeRadius;
        attackRadiusX2 = AttackRadius * AttackRadius;

        enemyState = State.Roaming;
        ChangeStateColor(enemyState);
        InitializeTimer();
    }

    private void InitializeTimer()
    {
        preRoamTimer = 0f;
        fullRoamTimer = Random.Range(RoamTimer - 5f, RoamTimer + 5f);
    }
    #endregion

    #region Enemy AI Update Functions
    // Update is called once per frame
    private void Update()
    {
        switch (enemyState)
        {
            case State.Roaming:
                if (preRoamTimer < fullRoamTimer)
                    preRoamTimer += Time.deltaTime;
                else
                {
                    MovePosition(transform.position + Random.insideUnitSphere * RoamRadius);
                    InitializeTimer();
                }
                FindTarget();
                break;
            case State.Chasing:
                MovePosition(Player.Instance.Position);

                if ((transform.position - Player.Instance.Position).sqrMagnitude < attackRadiusX2)
                {
                    enemyState = State.Attack;
                    enemyAgent.isStopped = true;
                    ChangeStateColor(enemyState);
                    preAttackTimer = 0;
                }
                LostTarget();
                break;
            case State.Attack:
                if (preAttackTimer < AttackTimer)
                    preAttackTimer += Time.deltaTime;
                else
                {
                    enemyState = State.AttackWait;
                    ChangeStateColor(enemyState);
                    preAttackWaitTimer = 0;
                }
                break;
            case State.AttackWait:
                if (preAttackWaitTimer < AttackWaitTimer)
                    preAttackWaitTimer += Time.deltaTime;
                else
                {
                    enemyState = State.Roaming;
                    enemyAgent.isStopped = false;
                    ChangeStateColor(enemyState);
                    preRoamTimer = fullRoamTimer;
                }
                break;
        }
    }
    #endregion

    #region Enemy AI Move Functions
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
    #endregion

    #region Enemy AI Target Functions
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
        if ((transform.position - Player.Instance.Position).sqrMagnitude > escapeRadiusX2)
        {
            enemyState = State.Roaming;
            ChangeStateColor(enemyState);
            InitializeTimer();
        }
    }
    #endregion

    #region Enemy AI State Functions
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
            case State.Attack:
                enemyMaterial.SetColor("_BaseColor", EnemyColors[2]);
                break;
            case State.AttackWait:
                enemyMaterial.SetColor("_BaseColor", EnemyColors[3]);
                break;
        }
    }
    #endregion
}
