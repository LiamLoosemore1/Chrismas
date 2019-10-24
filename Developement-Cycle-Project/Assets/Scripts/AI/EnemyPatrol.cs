using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyPatrol : MonoBehaviour {

    [SerializeField] private float speed;
    [SerializeField] private float idlespeed;
    [SerializeField] private float attackspeed;
    [SerializeField] private float angularspeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float distanceRange;

    [SerializeField]
    Transform _destination;

    [SerializeField]
    bool _patrolWaiting;

    [SerializeField]
    float _totalWaitTime = 3f;

    [SerializeField]
    float _switchProbability = 0.2f;

    [SerializeField]
    List<Waypoint> _patrolPoints;

    private AudioSource screamthing;


    public Image image;
    public float range;
    public Transform player;
    public Text GameOver;

    public AudioSource tickSource;

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

        tickSource = GetComponent<AudioSource>();

        screamthing = GetComponent<AudioSource>();

        GameOver.text = ("");

        image.enabled = false;

        if (_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attatched to " + gameObject.name);
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
                Debug.Log("Insufficent patrol points for basic patrolling behaviour.");
            }
        }
    }

    void FixedUpdate()
    {

        RaycastHit hit;

        Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, transform.forward * sightDist, Color.green);
        Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized * sightDist, Color.green);
        Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized * sightDist, Color.green);

        if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, transform.forward, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "Player" || Vector3.Distance(player.position, transform.position) <= distanceRange)
            {
                Attack();
                _navMeshAgent.speed = attackspeed;
            }
            else
            {
                Idle();
                _navMeshAgent.speed = idlespeed;
            }
        }
        else if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "Player" || Vector3.Distance(player.position, transform.position) <= distanceRange)
            {
                Attack();
                _navMeshAgent.speed = attackspeed;
            }
            else
            {
                Idle();
                _navMeshAgent.speed = idlespeed;
            }
        }
        else if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "Player" || Vector3.Distance(player.position, transform.position) <= distanceRange)
            {
                Attack();
                _navMeshAgent.speed = attackspeed;
            }
            else
            {
                Idle();
                _navMeshAgent.speed = idlespeed;
            }
        }
    }

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

    //NavMeshAgent _navMeshAgent;

    private void Attack()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = speed;
        _navMeshAgent.angularSpeed = angularspeed;
        _navMeshAgent.acceleration = acceleration;

        if (_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attached to " + gameObject.name);
        }
        else
        {
            SetDestinations();
        }
    }

    private void SetDestinations()
    {
        if (_destination != null)
        {
            Vector3 targetVector = _destination.transform.position;
            _navMeshAgent.SetDestination(targetVector);
        }
    }

    // Light Trigger

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "InteractiveLight")
        {
            col.gameObject.GetComponent<Light>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "InteractiveLight")
        {
            coll.gameObject.GetComponent<Light>().enabled = true; 
        }
    }

    // Game Ending Trigger

    IEnumerator GameOverWait()
    {
        screamthing.enabled = false;
        GameOver.text = ("GAME OVER");
        Time.timeScale = 0.00001f;
        //image.enabled = true;
        yield return new WaitForSecondsRealtime(5);
        //image.enabled = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Player")
        {
            tickSource.PlayScheduled(AudioSettings.dspTime + 0.25f);
            tickSource.Play();
            StartCoroutine (GameOverWait ());
        }
    }

}
