using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerManager))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5.0f;
    public bool jump;
    public bool crouch;
    private bool playerdeath;
    public float minimumvert = -45.0f;
    public float maximumvert = 45.0f;
    public Transform ttransform;

    [SerializeField]
    public float maxTime = 60.0f;
    public static float timeLeft;
    public static float flagspeed = 1.5f;
    public Image flag_reload;
    public Image game_over;
    public Image Win;

    [SerializeField]
    private float lookSensitivity = 3.0f;
    public bool isCrouch;
    public bool isJumping;
    public bool sprint;


    public GameObject player;
    Animator animator;
    
    private PlayerManager manager;
    private FriendScript friend;
    private EnemyScript enemy;
    private Snowball snowball;

    CursorLockMode LockMode;

    void Start()
    {
        SetCursorState();
        manager = GetComponent<PlayerManager>();
        animator = GetComponent<Animator>();
        snowball = GetComponent<Snowball>();
        isCrouch = false;
        isJumping = false;
        sprint = false;
        crouch = false;
        flag_reload.fillAmount = 0;
        game_over.enabled = false;
        Win.enabled = false;
        timeLeft = 0f;
        

    }

    void Update()
    {

        animator.SetBool("walk", Input.GetAxis("Vertical") > 0);
        animator.SetBool("walkback", Input.GetAxis("Vertical") < 0);
        animator.SetBool("walkR", Input.GetAxis("Horizontal") > 0);
        animator.SetBool("walkL", Input.GetAxis("Horizontal") < 0);

        //calculate movement velocity as a 3dvector
        float _xmov = Input.GetAxisRaw("Horizontal");
        float _zmov = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _xmov;
        Vector3 _moveVertical = transform.forward * _zmov;


        //final movement vector
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * speed;

        //apply movemnt
        manager.Move(_velocity);

        //calculate rotation
        float yRot = Input.GetAxisRaw("Mouse X");
        yRot = Mathf.Clamp(yRot, minimumvert, maximumvert);

        Vector3 _rotation = new Vector3(0.0f, yRot, 0.0f) * lookSensitivity;

        //Apply rotation
        manager.Rotation(_rotation);

        //calculate rotation
        float xRot = Input.GetAxisRaw("Mouse Y");
        //xRot = Mathf.Clamp(xRot, minimumvert, maximumvert);

        Vector3 _camerarotation = new Vector3(xRot, 0.0f, 0.0f) * lookSensitivity;


        //Apply rotation
        manager.RotationCamera(_camerarotation);

        if(Input.GetButton("dodger"))
        {
            animator.SetTrigger("DodgeR");
        }

        if(Input.GetButton("dodgel"))
        {
            animator.SetTrigger("DodgeL");
        }

        if (Input.GetButtonDown("Jump") /*|| isJumping == false*/)
        {
            if (OnGround())
            {
                //animator.SetTrigger("isJump");
                manager.Jump();
                //PlayJumpSound();
                jump = false;
                isJumping = true;
            }
        }

        if (Input.GetButtonUp("Jump") /*|| isJumping == false*/)
        {
            if (!OnGround())
            {
                manager.Fall();
                jump = true;
                isJumping = false;
            }

        }

        if (Input.GetButtonDown("crouch") /*|| isCrouch == false*/)
        {
            crouch = true;
            animator.SetBool("crouch", true);
            manager.Crouch();
            speed = 1.5f;
        }

        if (Input.GetButtonUp("crouch") /*|| isCrouch == true*/)
        {
            animator.SetBool("crouch", false);
            crouch = false;
            manager.Up();
            speed = 2.5f;
        }


        if (Input.GetButtonDown("sprint") /*&& isCrouch == false*/)
        {
            sprint = true;
            animator.SetBool("sprint", true);
            speed = 4.0f;
        }
        if (Input.GetButtonUp("sprint"))
        {
            sprint = false;
            animator.SetBool("sprint", false);
            speed = 2.5f;
        }

       

    }

    void OnTriggerStay(Collider flag)
     {
        //flag_reload.fillAmount = friend.flag_reload.fillAmount;  
          if (flag.gameObject.tag == "flag")
                {
                    timeLeft += Time.deltaTime * flagspeed;
                    flag_reload.fillAmount = (timeLeft / maxTime);
                   
                    if(flag_reload.fillAmount == 1)
                        {

                           Win.enabled = true;
                           Cursor.lockState = CursorLockMode.None;
                           Destroy(flag.gameObject, 1.0f);
                            //points == fillAmount(%)(maximos(10p))
                        }
                }
        }

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "snowball_enemy")
        {
            print("hit_player");
            playerdeath = true;
            game_over.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            //Destroy(this.gameObject);
            //animator.SetBool("death", true);
            //animacao morte

        }
    }

    bool OnGround()
    {
        Vector3 origin = ttransform.position;
        origin.y += 0.6f;
        Vector3 dir = -Vector3.up;
        float dis = 0.7f;
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, dis))
        {
            Vector3 tp = hit.point;
            ttransform.position = tp;
            return true;
        }
        return false;
    }

    

    // Apply requested cursor state
    void SetCursorState()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
