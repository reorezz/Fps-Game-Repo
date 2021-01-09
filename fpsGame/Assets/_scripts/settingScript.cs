using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class settingScript : MonoBehaviour
{
    public GameObject setting_pannel;
    bool issettingPressed=false;
    PlayerMovemnt playermov;
    public Slider mox, moy;
    public Slider countermov;
    public Slider forw, backwa, sidewa,sprint,crouch;
    // Start is called before the first frame update
    void Start()
    {
        playermov = transform.GetComponent<PlayerMovemnt>();
        mox.value = playermov.MouseSensx;
        moy.value = playermov.MouseSensY;
        countermov.value = playermov.CounterforceMul;
        forw.value = playermov.ForwardVelocity;
        backwa.value = playermov.BackWardVelocity;
        sidewa.value = playermov.sidewaysVelocity;
        sprint.value = playermov.sprintSpeed;
        crouch.value = playermov.crouchSpeed;
        setting_pannel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            issettingPressed = !issettingPressed;
            if(issettingPressed)
            {
               
             //   Debug.Log(issettingPressed);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;

                Time.timeScale = 0f;
                setting_pannel.SetActive(true);

            }else
            if(!issettingPressed)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1f;
                setting_pannel.SetActive(false);
            }
        }
    }
    public void changeMousex(float mousexvalue)
    {
        playermov.MouseSensx = mousexvalue;
    }
    public void changeMousey(float mouseyal)
    {
        playermov.MouseSensY = mouseyal;
    }
    public void counterMovementval(float counterm)
    {
        playermov.CounterforceMul = counterm;
    }
    public void changeforwardvel(float forwardvel)
    {
        playermov.ForwardVelocity = forwardvel;
    }
    public void chageBackwardvel(float backwa)
    {
        playermov.BackWardVelocity = backwa;
    }
    public void changesidewaysvel(float sidewaysvel)
    {
        playermov.sidewaysVelocity = sidewaysvel;
    }
    public void chnaglesprintvalur(float sprintal)
    {
        playermov.sprintSpeed = sprintal;
    }
    public void changecrouchspeed(float crouchsll)
    {
        playermov.crouchSpeed = crouchsll;
    }
}
