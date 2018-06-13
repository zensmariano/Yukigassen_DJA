using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Snowball : MonoBehaviour {

    public Transform spawnPosition;
    public Transform handshoot;
    public Transform player;
    public GameObject snowball;
    public int maxCount;
    public Camera cam;
    public float ray_range;
    public KeyCode interact;
    public bool isShooting = false;
    public bool isReloading = false;
    public bool kill = false;
    public float time;
    public float reloadTime = 1f;
    public float throwForce = 1000f;
    public float throwDistance = 0f;
    public bool holdingball = false;

    public Animator animshoot;

    public Image forcebar;
    public float maxTime = 25f;
    public static float timeLeft;
    public static float forcespeed = 5f;

    [Header("Data")]
    public int data_amount_snowball = 0;
    public Text data_text_snowball;
    public Text full_snowball;

    void Start()
    {
        animshoot = GetComponent<Animator>();
        data_text_snowball.text = data_amount_snowball.ToString();
        for (int i = 0; i<= 50; i++)
        {
           snowball = Instantiate(snowball, spawnPosition.position, spawnPosition.rotation);
        }
      //  forcebar = GetComponent<Image>();
        forcebar.fillAmount = 0f;
        timeLeft = 0f;

    }

    void Update()
    {
        if(holdingball == true)
        {
            snowball.transform.position = handshoot.transform.position;
            isShooting = false;
            
        }
        Ray ray_Cast = cam.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit ray_Hit;

        if (Physics.Raycast(ray_Cast, out ray_Hit, ray_range))
        {
            if (ray_Hit.collider.tag == "PickupItem")
            {

                if (Input.GetButton("take"))
                {

                    if(data_amount_snowball == 1)
                    {

                        snowball = Instantiate(snowball, handshoot.position, handshoot.rotation);
                        snowball.GetComponent<Rigidbody>().useGravity = false;
                        holdingball = true;
                        
                        
                    }
                    if(data_amount_snowball < maxCount)
                    {
                        data_amount_snowball++;
                        Destroy(ray_Hit.collider.gameObject);
                        data_text_snowball.text = data_amount_snowball.ToString();
                    }else
                    {
                        full_snowball.text = "FULL";  
                    }
                }
            }
        }

        if (isReloading)
        {
            return;
        }
        if (data_amount_snowball <= 0)
        {
            Debug.Log("no ammo");
            return;
        }

        if (Input.GetButton("shoot"))
        {
            

            if (holdingball)
            {
                    print("timershoot");
                    timeLeft += Time.deltaTime * forcespeed;
                    forcebar.fillAmount = (timeLeft / maxTime)*2;
                    animshoot.SetBool("shoot", true);
            }
        }

        if(Input.GetButtonUp("shoot") && timeLeft > 0)
        {
            //animshoot.SetBool("shoot", true);
            shootanim();
            forcebar.fillAmount = 0f;
            timeLeft = 0f;
            //isShooting = true;
        }

                /*snowball.transform.position = handshoot.transform.position + handshoot.transform.forward * throwDistance;
                snowball.transform.rotation = handshoot.transform.rotation;*/
                
           


            // GameObject ball = Instantiate(snowball, transform.position, transform.rotation);
       

    }

    private void shootanim()
    {
        Shoot(throwForce * (timeLeft / 2));   
    }

    IEnumerator Reload()
    {
        isReloading = true;
        holdingball = false;
        animshoot.SetBool("shoot", false);
        print("reload");

        yield return new WaitForSeconds(reloadTime);
        if(data_amount_snowball > 0)
        {
            snowball = Instantiate(snowball, handshoot.position, handshoot.rotation);
            snowball.GetComponent<Rigidbody>().useGravity = false;

            holdingball = true;
        }
        //snowball.transform.position = handshoot.transform.position + handshoot.transform.forward * throwDistance;

        isReloading = false;
    }

    


        
        /*else if (ballcollide.gameObject.tag == "amigo")
        {
            amigo.data_amount_snowball++;
        }*/
    /*IEnumerator Snowball_Destruction(float time)
    {
        print("snowball_destruction");
        Vector3 originalScale = snowball.transform.localScale;
        Vector3 destinationScale = new Vector3(0.2f, 0.2f, 0.2f);

        float currentTime = 0.0f;

        do
        {
            snowball.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);

        Destroy(this.gameObject);
    }*/

    public void Shoot(float force)
    {
        //RaycastHit hit;
        holdingball = false;
        snowball.GetComponent<Rigidbody>().useGravity = true;
        snowball.GetComponent<Rigidbody>().AddForce(cam.transform.forward * force);
        data_amount_snowball--;
        data_text_snowball.text = data_amount_snowball.ToString();
        //StartCoroutine(Snowball_Destruction(time));
        StartCoroutine(Reload());
        

        /*   if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
           {

               Debug.Log(hit.transform);
               if(hit.rigidbody != null)
                   {
                       hit.rigidbody.AddForce(-hit.normal * hitForce);
                   }
           }*/
    }
}
