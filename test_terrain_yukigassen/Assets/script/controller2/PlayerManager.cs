using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerManager : MonoBehaviour {

    [SerializeField]
    private Camera cam;
    public float jumpforce = 38.0f;
    public float gravity = 9.8f;
    public float GravityMultiplier;

    public LayerMask RayMask;
    private RaycastHit hit;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;
    

    Animator playeranim;

    private GameObject playermodel;
    private Rigidbody rb;
    CapsuleCollider playercrouch;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playercrouch = GetComponent<CapsuleCollider>();
        playeranim = GetComponent<Animator>();
        /*flag_reload.fillAmount = 0;
        timeLeft = 0f;*/
        SetupAnimator();

    }

    //get movement vector
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Jump()
    {
        Vector3 up = new Vector3(0.0f, jumpforce, 0.0f); // script for jumping
        rb.AddForce(up * jumpforce);
        print("jump");    
    }

    public void Fall()
    {
        velocity += Physics.gravity * GravityMultiplier * Time.fixedDeltaTime;
    }

    public void Crouch()
    {
        playercrouch.height = 1.5f;
        playercrouch.center -= transform.up * 0.15f;
        print("crouch!!");
    }

    public void Up()
    {
        playercrouch.height = 1.8f;
        playercrouch.center += transform.up * 0.15f;
    }
    //get movement vector
    public void Rotation(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    //get movement vector
    public void RotationCamera(Vector3 _cameraRotation)
    {
        cameraRotation = _cameraRotation;
    }

    //run every phisics iteration
    void FixedUpdate()
    {
        MovementNormal();
        PerformRotation();
        
    }

   //movement based on velocity variable
   void MovementNormal()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    void PerformRotation()
    {
        
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if(cam!= null)
        {
            cam.transform.Rotate(-cameraRotation);
        }
    }

    void SetupAnimator()
    {
        print("animatorsetup");
        if (playermodel == null)
        {
            playeranim = GetComponentInChildren<Animator>();
            playermodel = playeranim.gameObject;
        }
        

        if (playeranim == null)
        {
            playeranim = playermodel.GetComponent<Animator>();

            playeranim.applyRootMotion = false;
        }
    }

    
}
