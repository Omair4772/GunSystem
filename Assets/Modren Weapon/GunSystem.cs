using UnityEngine;
using TMPro;
using EZCameraShake;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor.PackageManager;

/*using UnityEngine.Animations.Rigging;*/

public class GunSystem : MonoBehaviour
{
    [Header("Weapon Status")]
    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize , bulletsPerTap;
    int presentAmmunation, bulletsShot;

    [Header("For Pistol")]
    public bool IsSingleShotRifle;


    [Header("Bullet Elements")]
    //Bullet
    public GameObject bullet;
    //Bullet Force
    public float shootForce, upWardForce;

    //bools 
    bool startShooting, readyToShoot, setReloading;
    public bool allowInvoke = true;

    [Header("Recoil")]
    public Rigidbody playerRB;
    public float recoilforce;

    [Header("Refrence")]
    //Reference
    public Transform Rifle_Parent_hand;
    public GameObject WeaponUI;
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    [Header("Graphics")]
    //Graphics
    public GameObject BloodEffect;
    public GameObject[] HitEffects;

    public ParticleSystem muzzleFlash;
    public float camShakeMagnitude = 1f, camShakeDuration =1f;
    public TextMeshProUGUI text;

    [Header("Animator Controllor")]
    public Animator animator;

    [Header("Animation Rigger")]
   /* [SerializeField]  Rig shootRig; */
    [HideInInspector] public float ShotRigWeight;

    [Header("Weapon Sounds")]
    public AudioSource PlayFireSound;
    public AudioSource FireStop;
    public AudioSource ReloadingSound;

    private void Awake()
    {
        ShotRigWeight = 1f;
        WeaponUI.SetActive(true);
        presentAmmunation = magazineSize;
        transform.SetParent(Rifle_Parent_hand);
        setReloading = false;
        readyToShoot = true;
    }
    private void Update()
    {

        //SetText
        text.SetText(presentAmmunation + "/" + magazineSize);
        
        //My INPUT FOR Shooting
        MyInput();

    }
    private void MyInput()
    {

        //Start Shooting
        if (startShooting && readyToShoot && !setReloading && presentAmmunation > 0)
        {
            print("RETUREND SHOOT");
            //set Bullet Shot 0 To Clear Garbeg from This Variable (it's AmmoPerTab)
            bulletsShot = 0;

            //SingleShot Weapon is here
            if (IsSingleShotRifle)
            {
                bulletsShot = bulletsPerTap;
                print("Feel Like Single Shot weapon");
                                 // Add Animation for Reloading
                Shoot();
            }
            //weapons is Here
            if(!IsSingleShotRifle)
            {
                bulletsShot = bulletsPerTap;
                print("Feel Like Auto Weapons");
                Shoot();
            }

/*
            //Animation Rigging
            shootRig.weight = Mathf.Lerp(shootRig.weight, ShotRigWeight, Time.deltaTime * 20f);
*/
        }

        // Reloading 
        if (magazineSize > 0 && presentAmmunation <= 0 || setReloading && presentAmmunation < magazineSize && magazineSize > 0)
        {
            StartCoroutine(Reload());
        }
        if (magazineSize <= 0 && setReloading)
        {
            print("Can Not Reload Now Because Your Are Out of Ammo");
        }
        else
        {
            print("Returned");
            return;
        }

    }
    private void Shoot()
    {
        PlayFireSound.Play();
        /*Sounds.playAkmFire();*/
        readyToShoot = false;
        muzzleFlash.Play();

        //Btn Desabler
        if (IsSingleShotRifle)
        {
          StartCoroutine(fireBtnDesabler());
        }


#region Finiding Mid Point of Screen To Move Bullets That Way

        //Find The Exect Hit Point using Raycast        Vector3(0.5,0.5,0) = middle of screen
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //ray to middle of screen
        RaycastHit Hit;
        #endregion

#region If (RayHitSomething){ CalculateDistance, CreateBullet, SpreadBullets/OrNot, AddForceToBullet, ThrowBullets, BounceBullets}

        //check if ray Hit Something
        Vector3 targetHitted;
        if (Physics.Raycast(ray, out Hit))
        {
            targetHitted = Hit.point;
        }
        else
        {
            targetHitted = ray.GetPoint(75); //Making Point far from player
        }

        //calculate distance from player to hitted point
        Vector3 directionwithoutspread = targetHitted - attackPoint.position;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 directionwithspread = directionwithoutspread + new Vector3(x, y, 0); //Give New Position               /*fpsCam.transform.forward + new Vector3(x, y, 0);*/

        //Instantiate Bullet/projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        Destroy(currentBullet, 1f);

        //Rotate Bullet to shoot Direction
        currentBullet.transform.forward = directionwithspread.normalized;

        //Add force to Bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionwithspread.normalized * shootForce, ForceMode.Impulse);
        //Bouncing Bullets
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upWardForce, ForceMode.Impulse);
#endregion

#region Functions For Things Hitted By Bullets
        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, directionwithspread, out rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("opponent"))
            {
                print("Enimie HITTED");
                //Graphics
                GameObject ImpactDamage = Instantiate(BloodEffect, rayHit.point, Quaternion.LookRotation(rayHit.normal));  //Euler(0, 180, 0)
                Destroy(ImpactDamage, 2f);

                // Damage Enimie
                /*rayHit.collider.GetComponent<Zombies2>.ZombiesHitDamage(damage);*/  //Zombies Getting Damage*/
                /*                ZombiesAISystem zombiesAISystem = rayHit.transform.GetComponent<ZombiesAISystem>();
                                  Zombie2 zombie2 = rayHit.transform.GetComponent<Zombie2>();

                                        if (zombiesAISystem != null || zombie2 != null)
                                        {
                                                zombiesAISystem.ZombiesHitDamage(giveDamageOf);
                                                GameObject ImpactDamage = Instantiate(BloodEffect, rayHit.point, Quaternion.LookRotation(hitInfo.normal));
                                                Destroy(ImpactDamage, 1f);

                                        }


                */

                
            }
            if (!rayHit.collider.CompareTag("opponent"))
            {
                print("Other Objects HITTED");

                int i = Random.Range(0, HitEffects.Length - 1);
                GameObject ImpactGlass = Instantiate(HitEffects[i], rayHit.point, Quaternion.LookRotation(rayHit.normal));
                Destroy(ImpactGlass, 1f);
            }

        }

#endregion

        //camera shake
        CameraShaker.Instance.ShakeOnce(camShakeDuration, camShakeMagnitude, .1f, 1f);

        presentAmmunation--;
        bulletsShot--;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            //Add Recoil To player
            playerRB.AddForce(-directionwithspread.normalized * recoilforce, ForceMode.Impulse);
        }

        //If want to shoot More Then One Bullet Per Tab Make Sure To repeat Shoot Function
        if (bulletsShot > 1 && presentAmmunation > 0)          //bulletsShot < bulletsPerTap && 
        {
            Invoke("Shoot", timeBetweenShots);
        }

    }
    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    //UI For Shooting
    public void startFiring()
    {
        if (!setReloading)
        {
            startShooting = true;
        }
        
    }
     
    public void stopFiring()
    {
        if (!IsSingleShotRifle)
        {
            PlayFireSound.Stop();
        }
        if (!setReloading) { FireStop.Play(); }
        startShooting = false;
    }

    //Fire Btn Disabler
    IEnumerator fireBtnDesabler()
    {
        
        yield return new WaitForSeconds(timeBetweenShots);
    }

    //Reload with Button
    public void ReloadNow()
    {
        if(presentAmmunation != magazineSize)
        {
            setReloading = true;
        }
        else
        {
            return;
        }
        
    }

    //Reload Main Function
    IEnumerator Reload()
    {

        ReloadingSound.Play();
        startShooting = false;
        ShotRigWeight = 0f;
        muzzleFlash.Stop();
        setReloading = true;
        Debug.Log("Reloading...");
/*        animator.SetBool("PistolReload", true);*/

        yield return new WaitForSeconds(reloadTime);

        Debug.Log("Reloading Completed...");
        /*        animator.SetBool("PistolReload", false);*/
        presentAmmunation = magazineSize;
        setReloading = false;

    }
}
