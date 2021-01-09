using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovemnt : MonoBehaviour
{   //basic requirement that are needed
     Rigidbody rb;
     CapsuleCollider PlayerCollider;
    public Transform Cam;
    public Transform Groundcheckposition;
    public GameObject Holder;
   

    //basic movement 
    float x, y, Mx, My;
    public LayerMask Ground;
    public float ForwardVelocity;
    public float BackWardVelocity;
    public float sidewaysVelocity;
    public float sprintSpeed;
    public float PlayerIdeaVelocityWithNoInput;
    float PlayerCurrentVelocity;
    public ForceMode PlayerforceMode;
    public float MouseSensx;
    public float MouseSensY;
    public float MouseRotationMultiplier = 1f;
    float mouseYRot;
    public float MaxVelocity = 16f;
    public bool PlayerOnGround;
  public   Vector2 relativeVelocity;
    public float JumpForce;
    
    Vector3 zero = new Vector3(0f, 0f, 0f);

   // settingScript settings;
  public   float sprintmaxvel;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = transform.GetComponent<Rigidbody>();
        PlayerCollider = transform.GetComponent<CapsuleCollider>();
        PlayerCurrentVelocity = PlayerIdeaVelocityWithNoInput;
        playerHeight = PlayerCollider.height;
        playerScale = Holder.transform.localScale;
        maxvelholder = MaxVelocity;
        temp = CounterforceMul;
        

    }
    private void Update()
    {
        //take input from user
        input();


        //rotate the player and camera according to the mouse input
        RotationOfPlayer();

        //To AssignPlayer Velocity according to the input
        if(!sprintButtonPressed && !isCrouching)
        {
            MaxVelocity = maxvelholder;
            if (y > 0f) PlayerCurrentVelocity = ForwardVelocity;
            if (y < 0f) PlayerCurrentVelocity = BackWardVelocity;
            if (Mathf.Abs(x) > 0f) PlayerCurrentVelocity = sidewaysVelocity;
        }
        if(sprintButtonPressed)
        {
            MaxVelocity = sprintmaxvel;
            PlayerCurrentVelocity = sprintSpeed;
        }
        
       
      //  if (Mathf.Abs(x) < Mathf.Epsilon && Mathf.Abs(y) < Mathf.Epsilon) PlayerCurrentVelocity = 0f;
       //print(Mathf.Abs(x));


        




    }
    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * 1000);
      //  Debug.Log(rb.velocity.magnitude);
        if (JumpButtonPressed && PlayerOnGround)
        {
            Jump();
        }

        Movement();
       // Debug.Log(rb.velocity.magnitude);
        FindRelativeVelocity();

        // CounterMovement();

      
        counterMov();
    }
    
    //taking input
    bool JumpButtonPressed;
    bool sprintButtonPressed;
   
    bool idRunning;
    bool isJumping= false;
    float maxvelholder;
    void input()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        Mx = Input.GetAxis("Mouse X");
        My = Input.GetAxis("Mouse Y");

        JumpButtonPressed = Input.GetButton("Jump");
        sprintButtonPressed = Input.GetKey(KeyCode.LeftShift);
        isCrouching = Input.GetKey(KeyCode.LeftControl);
        //   couchingButtonPresses = Input.GetKeyDown(KeyCode.LeftControl);
        //  isCrouchingReleased = Input.GetKeyUp(KeyCode.LeftControl);
     
       if(Input.GetKeyDown(KeyCode.LeftControl) && PlayerOnGround&& !crouchigstarttwd)
        {
            crouchtime = Time.time;
            crouchigstarttwd = true;
      Startcrouching();
           
        }
       if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            crouchigstarttwd = false;
          stopCrouching();
            
        }
      
    }

    //crouching
    [Header("Crouching")]
     Vector3 playerScale;
     float playerHeight;
    public Vector3 reduceScale;
    public float crouchSpeed;
    public ForceMode CrouchForceMode;
    public bool isCrouching;
    bool crouchigstarttwd;
    public float crouchspeedthreshold;
    float tempvar = 1f;
    float crouchtime;
    float earliervelo;
    Vector3 velo;
    float temp;
    void Startcrouching()
    {
        velo = rb.velocity * 0.8f;
        earliervelo = rb.velocity.magnitude;
        crouchigstarttwd = true;
        PlayerCollider.height = 1f;
        Holder.transform.localScale = reduceScale;
        isCrouching = true;
        // transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
        if (earliervelo > crouchspeedthreshold && Mathf.Abs(y)<Mathf.Epsilon )
        {
            Vector3 dir = rb.velocity.normalized;
            dir.y = 0f;
            rb.AddForce(dir * crouchSpeed * tempvar*Time.deltaTime, CrouchForceMode);
       
        }
      //  rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity *0.9f, 5f);
    }
       

    
    void stopCrouching()
    {
        earliervelo = crouchspeedthreshold;
        crouchigstarttwd = false;
        tempvar = 0f;
        isCrouching = false;
        PlayerCollider.height = 2f;
        Holder.transform.localScale = playerScale;
       // transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
    }
    public ForceMode JumpForceModel;
    public float jumpdelay;
    void Jump()
    {
        if (!isJumping && !isCrouching)
        {

            isJumping = true;
            rb.AddForce(Vector3.up * Time.deltaTime * JumpForce*3f, JumpForceModel); 
            if(normaltotouchjump)
             rb.AddForce(normalangletouch * Time.deltaTime * JumpForce/6f, JumpForceModel);
            Invoke("Jumpagain", jumpdelay);
            if(rb.velocity.y<0)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(rb.velocity.x, 0f, rb.velocity.z), 1.5f);
            }
            if(rb.velocity.y>0f)
            {
                Vector3 tempv = rb.velocity;
                rb.velocity = Vector3.Lerp(rb.velocity, tempv / 2, 1f);
            }
        }
    } 
    void Jumpagain()
    {
        isJumping = false;
    }

    //rotating player body and camera
    void RotationOfPlayer()
    {
       
        transform.Rotate(Vector3.up * MouseSensx * Time.deltaTime * Mx*MouseRotationMultiplier);
        mouseYRot -= My * Time.deltaTime * MouseSensY * MouseRotationMultiplier;
        mouseYRot = Mathf.Clamp(mouseYRot, -80f, 80f);
      //  Debug.Log(mouseYRot);
        Cam.localRotation = Quaternion.Euler(mouseYRot,0f, 0f);//new Vector3(mouseYRot, 0f, 0f);
    }
    //playeemovemnt like running ,walking,jumping, Couching 
    public float slopevelocity=1f;
    public bool normaltotouchjump = false;
    void Movement()
    {
        if (rb.velocity.magnitude > MaxVelocity) x = y = 0f;
        //i dont kno if its countering the diagonal velocity but lets see what happens 

       
            Vector2 NormalizingInput = new Vector2(x, y).normalized;
            //  Vector2 normalinput = new Vector2(x, y);

            // Debug.Log(NormalizingInput+"  "+normalinput);

            //rb.AddForce(NormalizingInput * PlayerCurrentVelocity * Time.deltaTime, PlayerforceMode);

          //  RaycastHit hit;
            // if(Physics.SphereCast(transform.position,PlayerCollider.radius*0.95f,Vector3.down,out hit,(PlayerCollider.height/2)-PlayerCollider.radius+0.5f,Ground))
            if (Physics.CheckSphere(Groundcheckposition.position, 0.15f, Ground))
            {


                //     Debug.Log("we hit ground");
                PlayerOnGround = true;

            }
            else
            {
                PlayerOnGround = false;
            }
            float multipliernormal = 1f;
            float multiplierAdvamceMovement = 1f;
            if (!PlayerOnGround)
            {

                multipliernormal = 0.7f;
                multiplierAdvamceMovement = 0.7f;
            }
            else if(!PlayerOnGround && Mathf.Abs(y)>0.1f)
        {
            multipliernormal = 0.8f;
            multiplierAdvamceMovement = 0.8f;
        }
            if (normalAngle > 5f && normalAngle < 45f)
            {
            float mulsplope=1.5f;
                if (PlayerOnGround)
                {
                    if(rb.velocity.magnitude<2f)
                    {
                    mulsplope = 10f;
                    }
                else
                {
                    mulsplope = 1f;
                }

                    if (Mathf.Abs(y) > Mathf.Epsilon)
                    {
                        
                          if(forwardandtouchangle<-5f && y>Mathf.Epsilon)
                        {
                            rb.AddForce(transform.up * Time.deltaTime * Mathf.Sin(forwardandtouchangle * Mathf.Deg2Rad) * slopevelocity * y * mulsplope*0.2f, ForceMode.VelocityChange);
                            rb.AddForce(transform.forward * Time.deltaTime * Mathf.Cos(forwardandtouchangle * Mathf.Deg2Rad) * slopevelocity * y * mulsplope*0.05f, ForceMode.VelocityChange);
                        PlayerCurrentVelocity = 50f;
                        rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity *0.97f, 2f);
                        CounterforceMul = 5f;
                        }
                          else
                        {
                        CounterforceMul = temp;
                            rb.AddForce(transform.up * Time.deltaTime * Mathf.Sin(forwardandtouchangle * Mathf.Deg2Rad) * slopevelocity * y * mulsplope, ForceMode.VelocityChange);
                            rb.AddForce(transform.forward * Time.deltaTime * Mathf.Cos(forwardandtouchangle * Mathf.Deg2Rad) * slopevelocity * y * mulsplope, ForceMode.VelocityChange);
                        }
                        //rb.AddForce(transform.forward * Time.deltaTime * Mathf.Cos(forwardandtouchangle * Mathf.Deg2Rad) * slopevelocity * y*mulsplope, ForceMode.VelocityChange);
                    }

                    else if (Mathf.Abs(x) > Mathf.Epsilon)
                    {
                        rb.AddForce(transform.up * Time.deltaTime * Mathf.Sin(forwardandtouchangle * Mathf.Deg2Rad) * slopevelocity * x*mulsplope, ForceMode.VelocityChange);
                        rb.AddForce(transform.right * Time.deltaTime * Mathf.Cos(forwardandtouchangle * Mathf.Deg2Rad) * slopevelocity * x*mulsplope, ForceMode.VelocityChange);
                    }

                    slopemul = 0.6f;
                    normaltotouchjump = true;
                }
               
            }
            else
            {
            CounterforceMul = temp;
                normaltotouchjump = false;
                slopemul = 1f;
            }

            if (isCrouching)
            {
                multiplierAdvamceMovement = 0.3f;
                multipliernormal = 0.3f;
                MaxVelocity = 15f;
            PlayerCurrentVelocity = 100f;
            if(rb.velocity.magnitude> MaxVelocity)
            {
                rb.velocity=Vector3.Lerp(rb.velocity, rb.velocity / 1.5f, 1f);
            }
            }
            else
            {
                MaxVelocity = maxvelholder;
            }
            if(sprintButtonPressed)
            {if(!isCrouching)
               PlayerCurrentVelocity = sprintSpeed;
            }

            rb.AddForce(transform.forward * Time.deltaTime * PlayerCurrentVelocity * NormalizingInput.y * multiplierAdvamceMovement * multipliernormal, PlayerforceMode);
            rb.AddForce(transform.right * Time.deltaTime * PlayerCurrentVelocity * NormalizingInput.x * multipliernormal, PlayerforceMode);

        
    }

   
    void FindRelativeVelocity()
    {
        //if (rb.velocity.magnitude > MaxVelocity || (Mathf.Abs(x)<0.01f && Mathf.Abs(y)<0.01f)) rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector3(0f,0f,0f),ref zero,0.15f);
        float look_Angle = transform.eulerAngles.y;
        float AngleBettwenXandYVelovity = Vector3.SignedAngle(Vector3.forward, new Vector3(rb.velocity.x, 0f, rb.velocity.z), Vector3.up);

        // float temp = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;
        // float temp2 = Mathf.DeltaAngle(look_Angle, temp);
        //Debug.Log(temp1 + "   " + temp2);
        // Debug.Log(Mathf.Cos(AngleBettwenXandYVelovity));

        //here we check where is out angle exactly with the drection in which we are loking wrt to our velocity
        float temp1 = Mathf.DeltaAngle(look_Angle, AngleBettwenXandYVelovity);//this is for z velocity ie forward
        float temp2 = 90 - temp1;//this is for x velocity ie sideways;
        //Debug.Log(temp1 + "  " + temp2);
        float velocityMag = rb.velocity.magnitude;
        float xV = velocityMag * Mathf.Cos(temp2*Mathf.Deg2Rad);
        float yv = velocityMag * Mathf.Cos(temp1 * Mathf.Deg2Rad);
        relativeVelocity = new Vector2(xV, yv);
      //  Debug.Log(relativeVelocity);
       
    }

    #region movement Counter 1
    public float slowNessMultiplier;
Vector3 zero1 = new Vector3(0f, 0f, 0f);
    float stpmul;
    public float slownessmulstandard;
   
    void CounterMovement()
    {
       if(Mathf.Abs(x)<0.01f)
        {
            if (Mathf.Abs(relativeVelocity.x) > 0.01f)
            {

                //  rb.velocity=  Vector3.SmoothDamp(rb.velocity, new Vector3(0, rb.velocity.y, rb.velocity.z), ref zero, 0.1f);
                if (PlayerOnGround)
                    rb.AddForce(transform.right * Time.deltaTime * -relativeVelocity.x * slowNessMultiplier,ForceMode.VelocityChange);
                else if(!PlayerOnGround)
                    rb.AddForce(transform.right * Time.deltaTime * -relativeVelocity.x * (slowNessMultiplier-2f), ForceMode.VelocityChange);
              //  Debug.Log("1");
                //  Debug.Log(relativeVelocity);
            }
        }
       if(Mathf.Abs(y)<0.01f)
        {
            if(Mathf.Abs(relativeVelocity.y)>0.01f)
            {
                // Debug.Log("2");
                //  rb.velocity=  Vector3.SmoothDamp(rb.velocity, new Vector3(rb.velocity.x, rb.velocity.y, 0f), ref zero1, 0.1f);
                // Debug.Log(relativeVelocity);
                if (PlayerOnGround)
                    rb.AddForce(transform.forward * Time.deltaTime * -relativeVelocity.y * slowNessMultiplier, ForceMode.VelocityChange);
                else if (!PlayerOnGround)
                    rb.AddForce(transform.forward * Time.deltaTime * -relativeVelocity.y * (slowNessMultiplier - 2f), ForceMode.VelocityChange);
            }
        }
       if((Mathf.Abs(x)<Mathf.Epsilon &&Mathf.Abs(y)<Mathf.Epsilon)&&Mathf.Abs(relativeVelocity.x)>Mathf.Epsilon && Mathf.Abs(relativeVelocity.y)>Mathf.Epsilon)
        {
           if(PlayerOnGround)
            rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.zero, ref zero, 1f);
           else
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.zero, ref zero, 1.5f);
            }
        }
        float subtractor = 0f;
        if (!PlayerOnGround) subtractor = 0.2f;
        else subtractor = 0f;
        if ((y>0f && relativeVelocity.y<0.01f)||(y<0.01f && relativeVelocity.y>-0.01f))
        {
            rb.AddForce(transform.forward * Time.deltaTime * -relativeVelocity.y * (slownessmulstandard-subtractor), ForceMode.VelocityChange);
        }
       if((x>0.01f && relativeVelocity.x<-0.01f) || (x<-0.01f && relativeVelocity.x>0.01f))
        {
            rb.AddForce(transform.right * Time.deltaTime * -relativeVelocity.x * (slownessmulstandard - subtractor), ForceMode.VelocityChange);
        }

    }
    #endregion
    float normalAngle;
    Vector3  normalangletouch;
    float forwardandtouchangle;
    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] p = collision.contacts;
        foreach(ContactPoint q in p )
        {
            normalangletouch = q.normal;
            // Debug.Log(q.normal);
            if (q.otherCollider.tag == "Slopes")
            {
                normalangletouch = q.normal;
            }
            else
            {
                normalangletouch = new Vector3(0f, 1f, 0f);
            }
            normalAngle = Vector3.Angle(Vector3.up, q.normal);
         forwardandtouchangle = Vector3.Angle(transform.forward, q.normal)-90f;
            // Debug.Log(normalAngle);
         //  Debug.Log(forwardandtouchangle);
        }
    }


    [Header("movement counter attributes")]
   public  float CounterforceMul=0.5f;
    public ForceMode counterorcemode;
    float slopemul=1f;
    void counterMov()
    {
        if(PlayerOnGround)
        {
            if((x<0.01f && relativeVelocity.x>-0.01f) || (x>-0.01 && relativeVelocity.x<-0.01f) || (Mathf.Abs(x)<Mathf.Epsilon && Mathf.Abs(relativeVelocity.x)>0.01f ))
            {
                rb.AddForce(transform.right * Time.deltaTime * -relativeVelocity.x * CounterforceMul*PlayerCurrentVelocity*slopemul, counterorcemode);
            }

            if ((y < Mathf.Epsilon && relativeVelocity.y > 0.01f) || (y > Mathf.Epsilon && relativeVelocity.y < -0.01f) || (Mathf.Abs(y) < Mathf.Epsilon && Mathf.Abs(relativeVelocity.y) > 0.01f))
            {
                rb.AddForce(transform.forward * Time.deltaTime * -relativeVelocity.y * CounterforceMul*PlayerCurrentVelocity*slopemul, counterorcemode);
            }
        }
        
    }




}
