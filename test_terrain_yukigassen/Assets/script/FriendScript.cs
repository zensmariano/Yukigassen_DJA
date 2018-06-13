using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

enum friendStatus
{
    Idle,
    Wander,
    Chase,
    Attack,
    Flag
}


public class FriendScript : MonoBehaviour {

    public bool walkYa = false;
    public bool runYa = false;
    public bool enemydeath = false;
    public bool attackYa = false;
    public float wanderRadius = 15.0f;
    public float ETargetRadius = 0.5f;
    private float Arotatespeed = 2.0f;
    Vector3 wanderTarget;
    Vector3 enemyTarget;

    friendStatus status = friendStatus.Idle;

    NavMeshAgent agentfriend;
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
    private Transform amigo;
    private Transform flag;
    private Transform enemy;
    private PlayerController playercontroller;

    public GameObject snowball_amigo;
    public Transform handshoot_amigo;
    public bool Aholdingball = false;

    public bool AisReloading = false;
    private float Atimershoot = 0;
    private float Areloadtimer = 0;
    private float Atimerreact = 0;
    public float Areload;
    public float Acoldown;
    private float Areact;

    // Use this for initialization
    void Start()
    {
        print("inimigo");
        snowball_amigo = Instantiate(snowball_amigo, handshoot_amigo.position, handshoot_amigo.rotation);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        flag = GameObject.FindGameObjectWithTag("flag").GetComponent<Transform>();
        enemy = GameObject.FindGameObjectWithTag("inimigo").GetComponent<Transform>();
        amigo = GameObject.FindGameObjectWithTag("amigo").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        agentfriend = GetComponent<NavMeshAgent>();
        StartWander();

    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("walk", walkYa);
        animator.SetBool("run", runYa);

        Atimershoot += Time.deltaTime;
        Areloadtimer += Time.deltaTime;
        Atimerreact += Time.deltaTime;

        followDistance = Random.Range(6.0f, 10.0f);
        FlagDistance = Random.Range(1.5f, 3.5f);
        shootdistance = Random.Range(5.0f, 10.0f);

        if (Aholdingball == true)
        {
            snowball_amigo.transform.position = handshoot_amigo.transform.position;

        }

        switch (status)
        {
            case friendStatus.Wander:
                if (((amigo.position - enemy.position).magnitude < followDistance))
                {
                    setChase();
                }
                if (((transform.position - enemy.position).magnitude < shootdistance))
                {
                    setAttack();
                }
                else if ((amigo.position - wanderTarget).magnitude < 1.5f)
                { 
                    StartWander();
                    walkYa = true;
                }


                break;
            case friendStatus.Idle:
                StartWander();
                break;
            case friendStatus.Chase:
                if (((amigo.position - enemy.position).magnitude < shootdistance))
                {
                    setAttack();
                }
                else
                {
                    if ((amigo.position - enemy.position).magnitude > followDistance)
                    {
                        runYa = false;
                        setIdle();

                    }
                    else
                    {
                        agentfriend.SetDestination(player.position);
                        runYa = true;
                    }
                }

                break;
            case friendStatus.Attack:
                Areact = Random.Range(2.0f, 7.0f);
                attackYa = true;
                AHandleRotate();
                if (Atimerreact >= Areact)
                {
                    //animator.GetBool("attack");
                    if (Atimershoot >= Acoldown)
                    {
                        if (attackYa == true)
                        {
                            shootanim();
                            Atimershoot = 0;
                        }

                    }
                }

                if ((transform.position - player.position).magnitude > shootdistance)
                {
                    setIdle();
                }
                break;

            case friendStatus.Flag:
                if (((amigo.position - flag.position).magnitude < FlagDistance))
                {
                    GoFlag();
                }
                else if ((amigo.position - flag.position).magnitude > FlagDistance || (amigo.position - enemy.position).magnitude < followDistance)
                    {
                        runYa = false;
                        setChase();

                    }
                    else
                    {
                        setIdle();
                    }

                break;
        }

    }

    private void OnTriggerStay(Collider flag)
    {
        //flag_reload.fillAmount = playercontroller.flag_reload.fillAmount;

        //if (flag.gameObject.tag == "flag")
        //{
            //timeLeft += Time.deltaTime * flagspeed;
            //flag_reload.fillAmount = (timeLeft / maxTime);

            //if (flag_reload.fillAmount == 1)
            //{
                //Destroy(flag.gameObject, 1.0f);
                //points == fillAmount(%)(maximos(10p))
           // }
            //animator.SetTrigger("take");
            //GameManager.instance.PickObject();
        //}
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "PickupItem")
        {
            print("hit_friend");
            //ammo++
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
            agentfriend.SetDestination(wanderTarget);
            agentfriend.isStopped = false;
            status = friendStatus.Wander;
        }
    }

    void setIdle()
    {
        status = friendStatus.Idle;
    }

    void setChase()
    {

        agentfriend.isStopped = false;
        status = friendStatus.Chase;
        if ((amigo.position - enemy.position).magnitude < followDistance)
        {
            agentfriend.SetDestination(enemy.position);
        }

    }

    void GoFlag()
    {
        agentfriend.isStopped = false;
        status = friendStatus.Flag;
        if ((amigo.position - flag.position).magnitude < FlagDistance)
        {
            agentfriend.SetDestination(flag.position);
        }
        else
        {
            setIdle();
        }
    }

    void setAttack()
    {
        animator.GetBool("attack");
        float angledot = Random.Range(0, 2 * Mathf.PI);
        enemyTarget = player.position + ETargetRadius * new Vector3(Mathf.Sin(angledot), 0, Mathf.Cos(angledot));
        agentfriend.isStopped = true;
        //status = friendStatus.Attack;
        if ((amigo.position - enemy.position).magnitude > shootdistance)
        {
            setIdle();
        }
    }

    void AShoot()
    {
        //RaycastHit hit;
        float force = Random.Range(55.0f, 85.0f);
        Aholdingball = false;
        snowball_amigo.GetComponent<Rigidbody>().useGravity = true;
        snowball_amigo.GetComponent<Rigidbody>().AddForce(this.transform.forward * force);
        if (Areloadtimer >= Areload)
        {
            AReload();
            Areloadtimer = 0;
        }
        //StartCoroutine(Reload());
    }

    void AReload()
    {
        AisReloading = true;
        Aholdingball = false;
        snowball_amigo = Instantiate(snowball_amigo, handshoot_amigo.position, handshoot_amigo.rotation);
        snowball_amigo.GetComponent<Rigidbody>().useGravity = false;
        Aholdingball = true;
        //snowball.transform.position = handshoot.transform.position + handshoot.transform.forward * throwDistance;

        AisReloading = false;
    }

    private void shootanim()
    {
        AShoot();
    }

    void AHandleRotate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(enemy.position - this.transform.position), Arotatespeed * Time.deltaTime);
    }
}
