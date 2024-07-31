using TMPro;
using UnityEngine;

public class modrenWeapon : MonoBehaviour
{
    //Bullet
    public GameObject bullet;

    //Bullet Force
    public float shootForce, upWardForce;

    //Gun State
    public float timeBtwShooting,spread,reloadTime,timeBtwShots;
    public int magazineSize, bulletsPerTab;

    //Graphics
    public GameObject MuzzleFlash;
    public TextMeshProUGUI ammoDisplay;

    public bool allowButtonhold;

    int bulletleft, bulletshot;

    //Bools
    bool shooting, readyToShot, reloading;

    //Refrences
    public Camera FpsCam;
    public Transform attackPoint;

    //Fixing Bug
    public bool allowInvoke = true;



    private void Awake()
    {
        //Make Sure Magazine is Full
        bulletleft = magazineSize;
        readyToShot = true;
    }

    private void Update()
    {
        myInput();

        //set Ammo Display if Exist
        if(ammoDisplay != null)
        {
            ammoDisplay.SetText(bulletleft / bulletsPerTab + " / " + magazineSize / bulletsPerTab);
        }
    }


    void myInput()
    {
        //Check If allowed To hold FireBtn and Take Corresponing Bullet
        if (allowButtonhold)
        {
            //Mouse0 is mouse left btn
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            //true till Hold
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        // Reloading 
        if (Input.GetKeyDown(KeyCode.R) && bulletleft < magazineSize && !reloading)
        {
            Reload();
        }
        // Auto Reload when bullets becomes 0
        if(readyToShot && shooting && bulletleft <= 0 && !reloading)
        {
            Reload();
        }

        //shooting
        if(readyToShot && shooting && !reloading && bulletleft > 0)
        {
            //set Bullet Shot 0
            bulletshot = 0;

            shoot();
        }
    }

    void shoot()
    {
        readyToShot = false;

        //Find The Exect Hit Point using Raycast        Vector3(0.5,0.5,0) = middle of screen
        Ray ray = FpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //ray to middle of screen
        RaycastHit Hit;

        //check if ray Hit Something

        Vector3 targetHitted;
        if(Physics.Raycast(ray, out Hit)) 
        {
            targetHitted = Hit.point;
        }
        else
        {
            targetHitted = ray.GetPoint(75); //Making Point far from player
        }

        //calculate distance from player to hitted point
        Vector3 directionwithoutspread = targetHitted - attackPoint.position;

        //make bullet spread
        float xSpread = Random.Range(-spread, spread);
        float ySpread = Random.Range(-spread, spread);

        //calculate new Direction with spread
        Vector3 directionwithspread = directionwithoutspread + new Vector3(xSpread, ySpread, 0); //Give New Position

        //Instantiate Bullet/projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        Destroy(currentBullet, 1f);

        //Rotate Bullet to shoot Direction
        currentBullet.transform.forward = directionwithspread.normalized;

        //Add force to Bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionwithspread.normalized * shootForce, ForceMode.Impulse);
        //Bouncing Bullets
        currentBullet.GetComponent<Rigidbody>().AddForce(FpsCam.transform.up * upWardForce, ForceMode.Impulse);

        //Muzzle Flash
        if(MuzzleFlash != null)
        {
            Instantiate(MuzzleFlash, attackPoint.position, Quaternion.identity);
        }

        bulletleft--;
        bulletshot++;

        // Invoke ResetShot Function ( if Not Already Invoked)
        if (allowInvoke)
        {

            Invoke("ResetShot", timeBtwShooting);
            allowInvoke = false;
        }

        //If want to shoot More Then One Bullet Per Tab Make Sure To repeat Shoot Function
        if(bulletshot < bulletsPerTab && bulletleft > 0)
        {
            Invoke("shoot", timeBtwShooting);
        }
    }

    private void ResetShot()
    {
        readyToShot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinish", reloadTime);
    }

    private void ReloadFinish()
    {
        bulletleft = magazineSize;
        reloading = false;
    }
}

