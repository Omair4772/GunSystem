using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioSource footStepSound;
    public AudioSource SprintSound;
    public AudioSource jumpSound;
    public AudioSource Punching;
    public AudioSource AkmFire;
    public AudioSource NOAmmo;
    public AudioSource PistolFire;
    public AudioSource Reloading;
    public AudioSource Aiming;
    public AudioSource PlayerDie;


/*    public FixedJoystick joystick;*/

    // Update is called once per frame
    void Update()
    {

        //AUDIO CONTROL OF FOOTSTEPS

/*
        //Walking Sound
        if(joystick.Vertical != 0 || joystick.Horizontal != 0)
        {
            footStepSound.enabled = true;
            jumpSound.Pause();

        }
        else
        {

            footStepSound.enabled = false;
        }
       */
    }

    //Running Sound
    public void playSprint()
    {

        jumpSound.Pause();
        SprintSound.Play();
    }
    // Stop Running Sound when button is released
    public void stopSprint()
    {
        SprintSound.Pause();
    }

    //Jumping Sound
    public void playjump()
    {

        SprintSound.Pause();
        jumpSound.Play();
    }

    // Stop Jump Sound when button is released
    public void stopJump()
    {
        jumpSound.Pause();
    }




    //punch Sound
    public void playpunch()
    {
        Punching.Play();
        AkmFire.Pause();
        PistolFire.Pause();
        Reloading.Pause();
    }



    //Aim Sound
    public void playAim()
    {
        Aiming.Play();
    }
    //stop Aiming Sound
    public void stopAim()
    {
        Aiming.Pause();
    }



    //AKM Sound
    public void playAkmFire()
    {
        AkmFire.Play();
        PistolFire.Pause();
        Punching.Pause();
        Reloading.Pause();
    }

    // Stop AKM
    public void stopAkmFire()
    {
        AkmFire.Pause();
        AkmFire.Stop();
    }


    //PISTOL FIRE 
    public void _PistolFire()
    {
        PistolFire.Play();
        AkmFire.Pause();
        Punching.Pause();
    }
    // STOP PISTAL SOUND
    public void StopPistol()
    {
        PistolFire.Pause();
    }


    //Reload Sound
    public void playReloading()
    {
        Reloading.Play();
        AkmFire.Pause();
        AkmFire.Stop();
        PistolFire.Pause();
        PistolFire.Stop();
        Punching.Pause();
    }
    // Stop reload
    public void stopReloading()
    {
        Reloading.Pause();
    }

    public void Player_DIE()
    {
        stopAim();
        stopAkmFire();
        stopJump();
        stopReloading();
        stopSprint();
        StopPistol();
        PlayerDie.Play();
    }
}
