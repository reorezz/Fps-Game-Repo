using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    public Rigidbody rb;
    public CapsuleCollider cc;
    public Transform bottom, aboveBottom, center, aboveCenter, Top,Eyes,rightW,LeftW,wallPlayerPos;
    public Camera cam;
    public bool onGround, Sprinting, crouching, InAir, Onslopes, WallRunWallRight,WallRunWallLeft;
    public LayerMask GroundLayer,WallRunLayer;

    [Header("Movement")]

    public float speed;
    public float SprintSpeed,AirSpeed, CrouchSpeed,MaxSpeedOnGround,MaxSpeedInAir,CounterMul,counterMulAir,accg,acca, xsens, ysens, mouse_Sens_Multiplier;
    float x, y, mx, my, currentspeed, MaxSpeed;
    Vector2 Dir;

    [Header("jump")]
   
    public ForceMode JumpforceMode;
    public float JumpForce;
    public float jumpDelay;
    bool jupPressed, CanJum = true;

    [Header("Crouch")]
    public bool CrouchPressed;
    public float crouchForce;
    public bool Canuncrouch;
    public Vector3 Playersace, CrouchScale;
    public ForceMode CrouchFMode;
    public float CrouchSlideThreshold;//Min speed required for the player to slide;

    [Header("Slopes")]
    public float slopemultiplier;
    public float SlopeForce;
    public float MaxSlopeAngle;
    public float upWardDivider = 2.5f;

    [Header("AirDash")]
    public float dashForce;
    public float AirDashcoolDown;
    public bool CanDash,dashPressed;
    float dashPressedtime;
    float temp6;

    [Header("FOV Handler")]
    public float defaulrFOv;
    public float MaxFovNormal,MaxFOV_Sprinting;
    public float FovMultiplier,RateOfChangeOfFOV;
    float Temp7, Temp8;
    float MaxFov;

    [Header("Waall move")]
    public float wall_Move_Time;
    public float moveForceWall,wallJumpforce,MaxUpwardMovement, sidewayForcewall;
    public float MaxCamTilt;
    float wallRunTilt = 0f;

    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        cc = transform.GetComponent<CapsuleCollider>();
        Playersace = transform.localScale;
        temp6 = dashForce;
        cam.fieldOfView = defaulrFOv ;
        Temp7 = FovMultiplier;//for fov
        Temp8 = RateOfChangeOfFOV;//for fov
       
        
    }
    private void Update()
    {
        // Debug.Log(rb.velocity.magnitude);
       // Debug.Log(cam.transform.forward);
        getInput();
        RotatePlayer();
        checkCollision();
       // fOVHandler();
        
        dashTemop = transform.forward + cam.transform.forward;
        Debug.DrawRay(transform.position, dashTemop, Color.cyan);

        if (Time.time - dashPressedtime >AirDashcoolDown)
        {
            CanDash = true;
        }

        if(onGround)
        {
            jupPressed = Input.GetButton("Jump");
            CrouchPressed = Input.GetKeyDown(KeyCode.LeftControl);
            if(Input.GetKeyDown(KeyCode.LeftControl))
            {
                Crouch();
            }
            if(crouching && !Input.GetKey(KeyCode.LeftControl)&& Canuncrouch)
            {
                UnCrouch();
            }
            

            if(Input.GetKey(KeyCode.LeftShift)&&y>0f)
            {
                Sprinting = true;
                currentspeed = SprintSpeed;
                MaxSpeed = MaxSpeedOnGround + 5f;
            }else if(crouching)
            {
                currentspeed = CrouchSpeed;
             
                MaxSpeed = CrouchSpeed;
            }else
            {
                Sprinting = false;
                currentspeed = speed;
                MaxSpeed = MaxSpeedOnGround;
            }
            

        }
        if(InAir)
        {
            MaxSpeed = MaxSpeedInAir;
        }
         
        if(WallRunWallRight)
        {
            rb.useGravity = false;
            //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
        if(WallRunWallLeft)
        {
            rb.useGravity = false;
           // rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
    }

    
    private void FixedUpdate()
    {
        if(!WallRunWallRight && !WallRunWallLeft)
          rb.AddForce(Vector3.down * 1000f * Time.deltaTime);
        
      
        if(onGround)
        {
            if(jupPressed && CanJum)
            {
                JumpBaby();
            }
       
            moveTheplayer();
           counterMoveThePlayer();
        }

        else if(InAir)
        {
            InairMovemnt();
            counterAirmovemnt();
        }

        else if (WallRunWallLeft)
        {
           
            DoWallRunMovemntLeft();
        }
        else if (WallRunWallRight)
        {
           // rb.isKinematic = true;
            DoWallRunMovemntRight();
        }
    }

    void JumpBaby()
    {
        rb.drag = 1f;
       
        rb.AddForce(Vector3.up * JumpForce * Time.deltaTime, JumpforceMode);
        CanJum = false;
        if (rb.velocity.y < 0f) rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (rb.velocity.y > 0f)
        {
            rb.velocity = rb.velocity / 2f;//Vector3.Lerp(rb.velocity, rb.velocity / 2f, 4 * Time.deltaTime);
        }
        Invoke("ResetJump", jumpDelay);

       
    }
    void ResetJump()
    {
        CanJum = true;
    }
    void getInput()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");
        Dir = new Vector2(x, y);
       
    }

    /// <summary>
    /// Player rotation
    /// </summary>
    float YR = 0f;//y rotaion temp adn wall runnng camera tilt
    
    void RotatePlayer()
    {
        transform.Rotate(Vector3.up * xsens * mx * Time.deltaTime * mouse_Sens_Multiplier);
        YR -= ysens * my * mouse_Sens_Multiplier * Time.deltaTime;
        YR = Mathf.Clamp(YR, -70f, 75f);
       
        if(WallRunWallLeft || WallRunWallRight)
        {
            if (WallRunWallLeft)//turn left//make titlt value negative
            {
                if (wallRunTilt > -MaxCamTilt )
                {
                    wallRunTilt -= Time.deltaTime * 2 * MaxCamTilt;
                }
            }
            if(WallRunWallRight)
            {
                if(wallRunTilt<MaxCamTilt)
                {
                    wallRunTilt += Time.deltaTime * 2 * MaxCamTilt;
                }
            }
            

        }
        else if(wallRunTilt!=0)
        {
            if(wallRunTilt < 0f)
            {
                // wallRunTilt += Time.deltaTime * 2 * MaxCamTilt;
               wallRunTilt= Mathf.Lerp(wallRunTilt, 0f, Time.deltaTime  *wall_Move_Time);

            }else if(wallRunTilt>0f)
            {
                // wallRunTilt -= Time.deltaTime * 2 * MaxCamTilt;
               wallRunTilt= Mathf.Lerp(wallRunTilt, 0f, Time.deltaTime  *wall_Move_Time);
            }
            
        }
            Eyes.localRotation = Quaternion.Euler(YR, 0f, wallRunTilt);

    }


    /// <summary>
    /// check for collision and then udate he states according to it
    /// </summary>
    void checkCollision()
    {
        if (Physics.CheckSphere(aboveBottom.position - new Vector3(0f, 0.15f, 0f), cc.radius - 0.1f, GroundLayer) ) 
        {
            onGround = true;
            rb.drag = 2.4f;
            InAir = false;
        }
        else 
        {
            rb.drag = 0f;
            InAir = true;
            onGround = false; 
        }

        if (Physics.Raycast(Top.position, Vector3.up, 0.5f))
        {
            Canuncrouch = false;
        } else Canuncrouch = true;

        RaycastHit WallRightHit;
        Debug.DrawRay(transform.position, rightW.right, Color.white);


        if (Physics.Raycast(transform.position, rightW.right, out WallRightHit, 01f, WallRunLayer) && Input.GetKey(KeyCode.D) )
        {
            //Physics.CheckBox(rightW.position, new Vector3(0.25f, 0.4f, 0.25f), transform.rotation, WallRunLayer) 
            //   Debug.Log("We Sexy Ahhhh Right");

            //   if ( == "WallRun")
            // rb.isKinematic = true;
            rb.useGravity = false;
            WallRunWallRight = true;
            InAir = false;
            onGround = false;
        }
        else
        {
            // rb.isKinematic = false;
            rb.useGravity = true;
          //  rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            WallRunWallRight = false;
        }
      //  RaycastHit WallLeftHit;
        Debug.DrawRay(transform.position, -LeftW.right, Color.white);
        if (Physics.Raycast(transform.position, -LeftW.right, out WallRightHit, 01f, WallRunLayer) && Input.GetKey(KeyCode.A))
        {

            //  Debug.Log("We Sexy Ahhhh left");
            //  rb.isKinematic = true;
            //   if ( == "WallRun")
            rb.useGravity = false;
            WallRunWallLeft = true;
            InAir = false;
            onGround = false;
        }
        else
        {
            //  rb.isKinematic = false;
            rb.useGravity = true;
            WallRunWallLeft = false;

        }


    }


    float velx, vely,temp2,cp;
    void moveTheplayer()
    {

        if (CrouchPressed)
        {
            cp = Time.time;
            MaxSpeed = MaxSpeedOnGround + 5f;
            //  rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (Mathf.Abs(rb.velocity.z) > 0.5f)
            {
                rb.drag = 0.1f;
               
            }

        }
        if(crouching)
        {
            if(rb.velocity.magnitude > CrouchSlideThreshold)
            {
                if (Time.time - cp < 1.05f)
                {
                    rb.AddForce(transform.forward * Time.deltaTime * crouchForce, CrouchFMode);
                }
            }
           
        }

               
        if(Onslopes)
        {
            slopeMovemnent();
        }
       
        if ((x > 0 && rb.velocity.x > MaxSpeed) || (x < 0 && rb.velocity.x < -MaxSpeed)) x = 0f;
        if ((y > 0 && rb.velocity.z > MaxSpeed) || (y < 0 && rb.velocity.z < -MaxSpeed)) y = 0f;

        // Debug.Log(rb.velocity.magnitude);
        if (Mathf.Abs(x) == 0) velx = 0f;
        else velx = Mathf.Lerp(currentspeed*0.7f, currentspeed + 1.5f, 4 * Time.deltaTime);

        if (Mathf.Abs(y) == 0f) vely = 0f;
        else vely = Mathf.Lerp(currentspeed*0.8f, currentspeed + 2f, 4 * Time.deltaTime);

        

        if (rb.velocity.magnitude > MaxSpeed)
        {
            x = y = 0f;
            Dir = Vector3.zero;
            velx = vely = 0f;
            rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity/MaxSpeed*0.9f, 8 * Time.deltaTime);
            
            
        }

        if (Dir.magnitude == 0 && rb.velocity.magnitude > 0f)
        {
            if(!Input.GetButton("Jump"))
            {
                temp2 += Time.deltaTime * 9.5f;
                rb.AddForce(-rb.velocity.normalized * accg * rb.velocity.magnitude * Time.deltaTime * 3);
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero , temp2);
            }
           
           // rb.drag = 1f;
        }
        else
        {
          //  rb.drag = 0f;
            temp2 = 0f;
        }
        Vector2 temp4 = new Vector2(x, y);
         float forcappliedX = currentspeed * accg*velx*temp4.x;
         float forcappliedY= currentspeed * accg * vely * temp4.y;
        rb.AddForce(transform.forward * forcappliedY * Time.deltaTime);
       rb.AddForce(transform.right * forcappliedX * Time.deltaTime);
        
       
    }
    void counterMoveThePlayer()
    {


        Vector3 velo = transform.InverseTransformDirection(new Vector3(rb.velocity.x,0f,rb.velocity.z));
        Vector2 veddirr = new Vector2(velo.x, velo.z).normalized;
        Vector2 ip = new Vector2(x, y).normalized;
      // Debug.Log(veddirr + "  " + ip);

        if ((ip.x == 0f && Mathf.Abs(veddirr.x) > 0f) || (x < 0f && veddirr.x > 0f) || (x > 0f && veddirr.x < 0f))
        {
            rb.AddForce(transform.right * rb.velocity.magnitude * -veddirr.x * CounterMul * Time.deltaTime*Mathf.Abs(velo.x));
        }
        if ((ip.y == 0f && Mathf.Abs(veddirr.y) > 0f) || (y < 0f && veddirr.y > 0f) || (y > 0f && veddirr.y < 0f))
        {
            rb.AddForce(transform.forward * rb.velocity.magnitude * -veddirr.y * CounterMul * Time.deltaTime*Mathf.Abs(velo.z) );
        }



    }
    ForceMode implusise = ForceMode.Impulse,forceJ;
    ForceMode nomimpsive = ForceMode.Force;
    float multipler;

    Vector3 dashTemop;
    float mxtemp ;
    void InairMovemnt()
    {
        //do movemt in air;
        if ((x > 0 && rb.velocity.x > MaxSpeed) || (x < 0 && rb.velocity.x < -MaxSpeed)) x = 0f;
        if ((y > 0 && rb.velocity.z > MaxSpeed) || (y < 0 && rb.velocity.z < -MaxSpeed)) y = 0f;
        mxtemp = MaxSpeedInAir;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
         
            // add a dash
            if(CanDash)
            {
                dashPressed = true;
                float selecttime = Time.time;
                dashPressedtime = Time.time;

                // dashTemop = new Vector3(0f, transform.forward.y, transform.forward.z) + new Vector3(0f, cam.transform.forward.y, cam.transform.forward.z);//      transform.forward+ cam.transform.forward;
                dashTemop = transform.forward + cam.transform.forward;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                CanDash = false;
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, MaxFOV_Sprinting, 100*Time.deltaTime);
                if (Time.time - selecttime <1.5f)
                rb.AddForce(dashTemop* Time.deltaTime * dashForce * currentspeed*Mathf.Cos(Vector3.Angle(transform.forward,dashTemop )*Mathf.Deg2Rad), ForceMode.Impulse);
              
            }

        }
        else
        {

            MaxSpeedInAir = mxtemp;
            dashPressed = false;
            
        }
      
        if(new Vector3(rb.velocity.x,0f,rb.velocity.z).magnitude>MaxSpeedInAir)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity / 2f, 8 * Time.deltaTime);
            x = y = 0f;
        }
       
        if(rb.velocity.magnitude<5f && Dir.magnitude>0f)
        {
            forceJ = implusise;
            multipler = 0.5f;
           

        }else
        {
            forceJ = nomimpsive;
            multipler = 1.3f;
        }

        rb.AddForce(transform.forward * AirSpeed*y*acca*  Time.deltaTime*0.65f*multipler,forceJ);
        rb.AddForce(transform.right * AirSpeed  * x * acca * Time.deltaTime,forceJ);

    }
    void counterAirmovemnt()
    {
        Vector3 velo = transform.InverseTransformDirection(new Vector3(rb.velocity.x, 0f, rb.velocity.z));
        Vector2 veddirr = new Vector2(velo.x, velo.z).normalized;
        Vector2 ip = new Vector2(x, y).normalized;
        // Debug.Log(veddirr + "  " + ip);

        //i dont know why i made it

      //  if(Dir.magnitude  == 0f)
        //{
            float tempf = 2f;
            if (Input.GetButton("Jump"))
            {
                tempf = 3.5f;
            }
            else tempf = 1f;
            

            if ((ip.x == 0f && Mathf.Abs(veddirr.x) > 0f))
            {
                rb.AddForce(transform.right * rb.velocity.magnitude * -veddirr.x * counterMulAir * Time.deltaTime * Mathf.Abs(velo.x) * tempf);
            }
            if (ip.y == 0f && Mathf.Abs(veddirr.y) > 0f)
            {
                rb.AddForce(transform.forward * rb.velocity.magnitude * -veddirr.y * counterMulAir * Time.deltaTime * Mathf.Abs(velo.z) * tempf);
            }

       // }

        if ((x < 0f && veddirr.x > 0f) || (x > 0f && veddirr.x < 0f))
        {
            rb.AddForce(transform.right * rb.velocity.magnitude * -veddirr.x * counterMulAir * Time.deltaTime * Mathf.Abs(velo.x));
        }
        if ((y < 0f && veddirr.y > 0f) || (y > 0f && veddirr.y < 0f))
        {
            rb.AddForce(transform.forward * rb.velocity.magnitude * -veddirr.y * counterMulAir * Time.deltaTime * Mathf.Abs(velo.z));
        }
        if (new Vector3(rb.velocity.x,0f,rb.velocity.z).magnitude > MaxSpeedOnGround) rb.drag = 1f;









    }

    void Crouch()
    {
        crouching = true;
       transform.position = transform.position - new Vector3(0f, 0.3f, 0f);
       
        transform.localScale = CrouchScale;
      

    }
    void UnCrouch()
    {
        
        
        
            crouching = false;
            transform.localScale = Playersace;

        


    }
    float forwardAngle, normalAngle;
    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] Touchs = collision.contacts;
        foreach(ContactPoint touch in Touchs)
        {
            if (touch.otherCollider.tag == "Slopes")
            {
                Onslopes = true;

                forwardAngle = Vector3.Angle(transform.forward, touch.normal) - 90f;
                // Debug.Log(forwardAngle);
            }
            else
            {
                Onslopes = false;
            }

            
            
         
        }
        
        
    }

   
    void slopeMovemnent()
    {
        if(forwardAngle >7f && forwardAngle<50f)
        {
            if (y > 0) slopemultiplier = 1.5f;
            else slopemultiplier = 2f;
           
            rb.AddForce(transform.forward * Time.deltaTime * Dir.normalized.y * slopemultiplier * SlopeForce*Mathf.Cos(forwardAngle*Mathf.Deg2Rad));
            rb.AddForce(transform.right * Time.deltaTime * Dir.normalized.x * SlopeForce * Mathf.Sin(forwardAngle * Mathf.Deg2Rad));
            rb.AddForce(transform.up * Mathf.Sin(forwardAngle * Mathf.Deg2Rad) * slopemultiplier * SlopeForce * Dir.normalized.y/upWardDivider);
            
        }
        if(forwardAngle <-7f && forwardAngle > -50f)
        {
            if (y > 0f)
                slopemultiplier = 2f;
            else slopemultiplier = 1f;
            rb.AddForce(transform.forward * Time.deltaTime * Dir.normalized.y * slopemultiplier * SlopeForce * Mathf.Cos(forwardAngle * Mathf.Deg2Rad));
            rb.AddForce(transform.right * Time.deltaTime * Dir.normalized.x *  SlopeForce * Mathf.Sin(forwardAngle * Mathf.Deg2Rad ));
            rb.AddForce(transform.up * Mathf.Sin(forwardAngle * Mathf.Deg2Rad) * slopemultiplier * SlopeForce * Dir.normalized.y/upWardDivider);
        }
        if(Dir.y == 0f && rb.velocity.magnitude <MaxSpeedOnGround)
        {
            rb.AddForce(Vector3.down * 1000f);
        }
    }


 
    void fOVHandler()
    {
        //handel the fov withrespect to the change in velocityWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
        // as speed increase we increase fov
       if(Mathf.Abs(Dir.y)>0f || Sprinting||dashPressed)
        {
            FovMultiplier = Temp7;
            //increase fov
            if (Sprinting||dashPressed)
            {
                RateOfChangeOfFOV = Temp8 + 0.5f;
                MaxFov = MaxFOV_Sprinting;

            }
            else
            {
                MaxFov = MaxFovNormal;
            }
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, MaxFov,RateOfChangeOfFOV*Time.deltaTime);
        }else if(Dir.y ==0f||Dir.magnitude ==0f )
        {
            FovMultiplier += 0.5f;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaulrFOv, FovMultiplier * Time.deltaTime);
            
        }
     //   Debug.Log(RateOfChangeOfFOV + "  " + FovMultiplier);
        

    }


    void DoWallRunMovemntLeft()
    {
       // Debug.Log(y);
        rb.drag = 0f;
        rb.AddForce(-transform.right * Time.deltaTime * moveForceWall / 3f);
        if ( rb.velocity.magnitude<MaxSpeedOnGround+10f)
        {

            rb.AddForce(transform.forward * Time.deltaTime * moveForceWall);
           
        }
      
        if( Input.GetKey(KeyCode.D) && Input.GetButton("Jump"))
        {
            rb.AddForce(transform.forward * Time.deltaTime * wallJumpforce*2 ,ForceMode.Impulse);
            rb.AddForce(transform.right * Time.deltaTime * wallJumpforce*2,ForceMode.Impulse);
            rb.AddForce(transform.up * Time.deltaTime * MaxUpwardMovement,ForceMode.Impulse);
        }
        else if (Input.GetButton("Jump"))
        {
            rb.AddForce(transform.up * Time.deltaTime * wallJumpforce);
            rb.AddForce(-transform.right * Time.deltaTime * wallJumpforce );
            rb.AddForce(transform.forward * Time.deltaTime * wallJumpforce*2);
        }
    }
    void DoWallRunMovemntRight()
    {
       // Debug.Log("Left");
        rb.drag = 0f;
        rb.AddForce(transform.right * Time.deltaTime * moveForceWall *1.5f);
        if (rb.velocity.magnitude < MaxSpeedOnGround+10f)
        {
            rb.AddForce(transform.forward * Time.deltaTime * moveForceWall);
            
        }
        
        if (Input.GetKey(KeyCode.A) && Input.GetButton("Jump" ))
        {
            rb.AddForce(transform.forward * Time.deltaTime * wallJumpforce*2 ,ForceMode.Impulse);
            rb.AddForce(-transform.right * Time.deltaTime * wallJumpforce*2, ForceMode.Impulse);
            rb.AddForce(transform.up * Time.deltaTime *MaxUpwardMovement, ForceMode.Impulse);
        }
        else if(Input.GetButton("Jump"))
        {
            rb.AddForce(transform.up * Time.deltaTime * wallJumpforce);
            rb.AddForce(transform.right * Time.deltaTime * wallJumpforce );
            rb.AddForce(transform.forward * Time.deltaTime * wallJumpforce*2  );
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(rightW.position, new Vector3(0.25f, 0.4f, 0.25f));
        Gizmos.DrawCube(LeftW.position, new Vector3(0.25f, 0.4f, 0.25f));
        // Gizmos.DrawSphere(rightW.position, 0.3f);
        //  Gizmos.DrawSphere(LeftW.position, 0.3f);
    }

    
}
