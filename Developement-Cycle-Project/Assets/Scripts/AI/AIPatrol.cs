using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AIPatrol : MonoBehaviour
{

    // AI Changeable Values

    [SerializeField]
    private float _aiSpeed; // Sets AI Movement Speed

    [SerializeField]
    private float _aiIdleSpeed; // Sets AI Idle Speed

    [SerializeField]
    private float _aiAttackSpeed; // Sets AI Attack Speed

    [SerializeField]
    private float _aiAcceleration; // Sets AI Acceleration

    [SerializeField]
    private float _aiTurningSpeed; // Sets AI Turning Speed

    [SerializeField]
    private float _aiStoppingDistance; // Sets AI Stopping Distance

    [SerializeField]
    private float _aiDistanceRange; // Sets AI Distance Range

    // --------------------

    [SerializeField]
    bool _patrolWaiting;

    [SerializeField]
    float _totalWaitTime = 3f;

    [SerializeField]
    float _switchProbability = 0.2f;

    [SerializeField]
    List<Waypoint> _patrolPoints;

    [SerializeField]
    Transform _destination;

    public Transform player;

    NavMeshAgent _navMeshAgent;
    int _currentPatrolIndex;
    bool _travelling;
    bool _waiting;
    bool _patrolForward;
    float _waitTimer;
    public float heightMultiplier;
    public float sightDist = 10;

    void Start()
    {

        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attached to " + gameObject.name);
        }
        else
        {
            if (_patrolPoints != null && _patrolPoints.Count >= 2)
            {
                _currentPatrolIndex = 0;
                SetDestination();
            } 
            else
            {
                Debug.LogError("Insufficient patrol point count!");
            }
        }

    }

    // --------------------------------------------------------------------------- //
    // AI SIGHT & FOV
    // --------------------------------------------------------------------------- //

    void FixedUpdate()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, transform.forward * sightDist, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized * sightDist, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized * sightDist, Color.red);


        if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, transform.forward, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "Player" || Vector3.Distance(player.position, transform.position) <= _aiDistanceRange)
            {
                Attack();
                _navMeshAgent.speed = _aiAttackSpeed;
            }
            else
            {
                Idle();
                _navMeshAgent.speed = _aiIdleSpeed;
            }
        }
        else if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized, out hit, sightDist))
        { 
            if (hit.collider.gameObject.tag == "Player" || Vector3.Distance(player.position, transform.position) <= _aiDistanceRange)
            {
                Attack();
                _navMeshAgent.speed = _aiAttackSpeed;
            }
            else
            {
                Idle();
                _navMeshAgent.speed = _aiIdleSpeed;
            }
        }
        else if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "Player" || Vector3.Distance(player.position, transform.position) <= _aiDistanceRange)
            {
                Attack();
                _navMeshAgent.speed = _aiAttackSpeed;
            }
            else
            {
                Idle();
                _navMeshAgent.speed = _aiIdleSpeed;
            }
        }
    }

    // --------------------------------------------------------------------------- //
    // AI STATES
    // --------------------------------------------------------------------------- //

    private void Idle()
    {

        if (_travelling && _navMeshAgent.remainingDistance <= 1.0f)
        {
            _travelling = false;

            if (_patrolWaiting)
            {
                _waiting = true;
                _waitTimer = 0f;
            }
            else
            {
                ChangePatrolPoint();
                SetDestination();
            }

            if (_waiting)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= _totalWaitTime)
                {
                    _waiting = false;

                    ChangePatrolPoint();
                    SetDestination();
                }
            }

        }

    }

    private void Attack()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = _aiSpeed;
        _navMeshAgent.angularSpeed = _aiTurningSpeed;
        _navMeshAgent.acceleration = _aiAcceleration;

        if (_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attatched to " + gameObject.name);
        }
        else
        {
            SetDestinations();
        }
    }

    // --------------------------------------------------------------------------- //

    private void SetDestinations()
    {
        if (_destination != null)
        {
            Vector3 targetVector = _destination.transform.position;
            _navMeshAgent.SetDestination(targetVector);
        }
    }

    private void SetDestination()
    {

        if (_patrolPoints != null)
        {
            Vector3 targetVector = _patrolPoints[_currentPatrolIndex].transform.position;
            _navMeshAgent.SetDestination(targetVector);
            _travelling = true;
        }

    }

    private void ChangePatrolPoint()
    {

        if (UnityEngine.Random.Range(0f, 1f) <= _switchProbability)
        {
            _patrolForward = !_patrolForward;
        }

        if (_patrolForward)
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Count;
        }
        else
        {
            if (--_currentPatrolIndex < 0)
            {
                _currentPatrolIndex = _patrolPoints.Count - 1;
            }
        }

    }

    IEnumerator GameOverWait()
    {
        Time.timeScale = 0.000001f;

        yield return new WaitForSecondsRealtime(10);

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            //Debug.LogWarning("Collision Successful!");

            //Debug.LogError("...");

            StartCoroutine(GameOverWait());
        }
    }

}
