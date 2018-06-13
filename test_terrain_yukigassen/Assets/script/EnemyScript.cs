using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

enum enemyStatus
{
    Idle,
    Wander,
    Chase,
    Attack,
    Flag
}

public class EnemyScript : MonoBehaviour
{

    public bool walkYa = false;
    public bool runYa = false;
    public bool enemydeath = false;
    public bool attackYa = false;
    public bool enemywin = false;
    public float wanderRadius = 15.0f;
    public float PTargetRadius = 0.5f;
    private float rotatespeed = 2.0f;
    Vector3 wanderTarget;
    Vector3 PlayerTarget;

    public bool isReloading = false;

    enemyStatus status = enemyStatus.Idle;

    NavMeshAgent agent;
    Animator animator;

    [SerializeField]
    public float maxTime = 60.0f;
    public static float timeLeft;
    public static float flagspeed = 1.0f;
    public Image flag_reload;

    public float followDistance;
    public float FlagDistance;
    public float shootdistance;
    private Transform player;
    private Transform flag;

    public GameObject snowball_enemy;
    public Transform handshoot_enemy;
    public bool holdingball = false;

    private float timershoot = 0;
    private float reloadtimer = 0;
    private float timerreact = 0;
    public float reload;
    public float coldown;
    public float react;
    public Image game_over;

    // Use this for initialization
    void Start()
    {
        print("inimigo");
        holdingball = true;
        game_over.enabled = false;
        snowball_enemy = Instantiate(snowball_enemy, handshoot_enemy.position, handshoot_enemy.rotation);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        flag = GameObject.FindGameObjectWithTag("flag").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        StartWander();


    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("walk", walkYa);
        animator.SetBool("run", runYa);

        timershoot += Time.deltaTime;
        reloadtimer += Time.deltaTime;
        timerreact += Time.deltaTime;

        followDistance = Random.Range(6.0f, 10.0f);
        FlagDistance = Random.Range(1.5f, 3.5f);
        shootdistance = Random.Range(5.0f, 10.0f);

        if (holdingball == true)
        {
            snowball_enemy.transform.position = handshoot_enemy.transform.position;

        }

        switch (status)
        {
            case enemyStatus.Wander:
                if (((transform.position - player.position).magnitude < followDistance))
                {
                    setChase();
                }
                if (((transform.position - player.position).magnitude < shootdistance))
                {
                    setAttack();
                }
                else if ((transform.position - wanderTarget).magnitude < 1f)
                    StartWander();
                walkYa = true;

                break;
            case enemyStatus.Idle:
                StartWander();
                break;
            case enemyStatus.Chase:
                if (((transform.position - player.position).magnitude < followDistance))
                {
                    setAttack();

                }
                else
                {
                    if ((transform.position - player.position).magnitude > followDistance)
                    {
                        runYa = false;

                    }
                    else
                    {
                        agent.SetDestination(player.position);
                        runYa = true;
                    }
                }

                break;
            case enemyStatus.Attack:
                react = Random.Range(2.0f, 7.0f);
                attackYa = true;
                HandleRotate();
                if (timerreact >= react)
                {
                    animator.GetBool("attack");
                    if (timershoot >= coldown)
                    {
                        if (attackYa == true)
                        {
                            shootanim();
                            timershoot = 0;
                        }

                    }
                }

                if ((transform.position - player.position).magnitude > shootdistance)
                {
                    setIdle();
                }
                break;

            case enemyStatus.Flag:
                if (((transform.position - flag.position).magnitude < FlagDistance))
                {
                    GoFlag();
                }
                else
                {
                    if ((transform.position - flag.position).magnitude < FlagDistance || (transform.position - player.position).magnitude < followDistance)
                    {
                        runYa = false;
                        setChase();

                    }
                    else
                    {
                        agent.SetDestination(flag.position);
                        runYa = true;
                    }
                }

                break;
        }

    }

    private void OnTriggerStay(Collider flag)
    {
        //se estiver inimigos e amigos dentro n da count(fazer)
        if (flag.gameObject.tag == "flag")
        {
            timeLeft += Time.deltaTime * flagspeed;
            flag_reload.fillAmount = (timeLeft / maxTime);

            if (flag_reload.fillAmount == 1)
            {
                enemywin = true;
                game_over.enabled = true;
                Cursor.lockState = CursorLockMode.None;
                Destroy(flag.gameObject, 1.0f);
                //points == fillAmount(%)(maximos(10p))
            }
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "PickupItem")
        {
            print("hit_enemy");
            enemydeath = true;
            animator.SetBool("death", true);
            //animacao morte
            Destroy(this.gameObject);
        }
    }

    void StartWander()
    {
        print("aiwander");
        float angle = Random.Range(0, 2 * Mathf.PI);
        wanderTarget = flag.position + wanderRadius * new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        NavMeshHit myHit;
        if (NavMesh.SamplePosition(wanderTarget, out myHit, 10, -1))
        {
            walkYa = true;
            wanderTarget = myHit.position;
            agent.SetDestination(wanderTarget);
            agent.isStopped = false;
            status = enemyStatus.Wander;
        }
    }

    void setIdle()
    {
        status = enemyStatus.Idle;
    }

    void setChase()
    {

        agent.isStopped = false;
        status = enemyStatus.Chase;
        if ((transform.position - player.position).magnitude < followDistance)
        {
            agent.SetDestination(player.position);
        }

    }

    void GoFlag()
    {
        agent.isStopped = false;
        status = enemyStatus.Flag;
        if ((transform.position - flag.position).magnitude < FlagDistance)
        {
            agent.SetDestination(flag.position);
        }
        else
        {
            setIdle();
        }
    }

    void setAttack()
    {
        
        agent.isStopped = true;
        float angledot = Random.Range(0, 2 * Mathf.PI);
        PlayerTarget = player.position + PTargetRadius * new Vector3(Mathf.Sin(angledot), 0, Mathf.Cos(angledot));
        status = enemyStatus.Attack;
        agent.SetDestination(player.position);
        if ((transform.position - player.position).magnitude > shootdistance)
        {
            setIdle();
        }
    }

    void Shoot()
    {
        //RaycastHit hit;
        float force = Random.Range(55.0f, 85.0f);
        holdingball = false;
        snowball_enemy.GetComponent<Rigidbody>().useGravity = true;
        snowball_enemy.GetComponent<Rigidbody>().AddForce(this.transform.forward * force);
        if(reloadtimer >= reload)
        {
            Reload();
            reloadtimer = 0;
        }
        //StartCoroutine(Reload());
    }

    void Reload()
    {
        isReloading = true;
        holdingball = false;
        snowball_enemy = Instantiate(snowball_enemy, handshoot_enemy.position, handshoot_enemy.rotation);
        snowball_enemy.GetComponent<Rigidbody>().useGravity = false;
        holdingball = true;
        //snowball.transform.position = handshoot.transform.position + handshoot.transform.forward * throwDistance;

        isReloading = false;
    }

    private void shootanim()
    {
        Shoot();
    }

    void HandleRotate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.position - this.transform.position), rotatespeed * Time.deltaTime);
    }

    /*IEnumerator Reload()
    {
        isReloading = true;
        holdingball = false;
        print("reloadenemy");

        yield return new WaitForSeconds(reloadTime);
        snowball_enemy = Instantiate(snowball_enemy, handshoot_enemy.position, handshoot_enemy.rotation);
        snowball_enemy.GetComponent<Rigidbody>().useGravity = false;
        holdingball = true;
        //snowball.transform.position = handshoot.transform.position + handshoot.transform.forward * throwDistance;

        isReloading = false;
    }*/
}
