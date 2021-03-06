using UnityEngine;
using UnityEngine.UI;

public class weaponSA : MonoBehaviour
{
    // Menu control
    private menu menuEscape;
    // Audio effect of the shot when fired
    [SerializeField] public AudioSource weaponAS;
    [SerializeField] public AudioClip weaponAC;
    // Weapon details
    public short weaponType = 1; // 0: Primary, 1: Secondary, 2: Special
    public string weaponName = "Nerf Pistol";
    // Weapon technical stats
    [SerializeField] int magazineAmmo = 12;
    [SerializeField] int reserveAmmo = 48;
    public int reserveAux = 0;
    private int maxMagazineAmmo = 12;
    private int maxReserveAmmo = 60;
    private float damage = 18f;
    private float range = 100.03f;
    // Status booleans
    [SerializeField] bool isReloading;
    [SerializeField] bool isShooting;
    [SerializeField] bool isWeaponEnabled;
    // Timings
    private float nextShootTime = 0.00000001f;
    private float nextShootDelay = 0.2440f;
    private float finishReloadingTime = 0.0000001f;
    private float reloadDelay = 0.7f;
    // Shooting targets
    public Camera firstPersonCamera;
    public bool debugHit = true;
    [SerializeField] GameObject debugHitGameObject;
    // GUI
    public bool showAmmoOnHUD = true;
    private Text ammoText;
    
    private void Start()
    {
        isWeaponEnabled = true;
        isShooting = false;
        isReloading = false;
        firstPersonCamera = GameObject.Find("Camera").GetComponent<Camera>();
    }
    private void Update()
    {
        if (menuEscape.isOpen) return;
        if (Input.GetKeyDown(KeyCode.R) || magazineAmmo == 0)
        {
            if (magazineAmmo < maxMagazineAmmo)
            {
                if (reserveAmmo <= 0)
                {
                    return;
                }
                reserveAmmo += magazineAmmo;
                if (reserveAmmo >= maxMagazineAmmo)
                {
                    magazineAmmo = maxMagazineAmmo;
                    reserveAux = reserveAmmo - magazineAmmo;
                    reserveAmmo = reserveAux;
                    isReloading = true;
                } else if (reserveAmmo < maxMagazineAmmo && reserveAmmo > 0)
                {
                    magazineAmmo = reserveAmmo;
                    reserveAmmo = 0;
                    isReloading = true;
                }
            }
            finishReloadingTime = Time.time + reloadDelay;
        }

        if (isReloading)
        {
            if (finishReloadingTime > Time.time)
            {
                return;
            }
            isReloading = false;
        }

        if (!WeaponShootIsOnCooldown(nextShootTime))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isWeaponEnabled && !isReloading)
                {
                    isShooting = WeaponFire();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
        }
        
        if (showAmmoOnHUD)
        {
            ammoText.text = magazineAmmo.ToString() + "/" + reserveAmmo.ToString();
        }
    }
    
    
    private void OnEnable()
    {
        menuEscape = GetComponentInParent<menu>();
        if (showAmmoOnHUD)
        {
            ammoText = GameObject.Find("Ammo").GetComponent<Text>();
            ammoText.enabled = true;
        }
        if (magazineAmmo > maxMagazineAmmo) magazineAmmo = maxMagazineAmmo;
        isWeaponEnabled = true;
        if (weaponAC != null && weaponAS != null)
        {
            weaponAS.clip = weaponAC;
        }
    }

    private void OnDisable()
    {
        isWeaponEnabled = false;
        GameObject.Find("Ammo").GetComponent<Text>().enabled = false;
    }

    private bool WeaponShootIsOnCooldown(float nextShoot)
    {
        if (Time.time < nextShoot)
        {
            return true;
        }
        return false;
    }

    private bool WeaponFire()
    {
        if (magazineAmmo >= 1)
        {
            Shoot();
            weaponAS.Play();
            magazineAmmo--;
            nextShootTime = Time.time + nextShootDelay;
            return true;
        }
        return false;
    }

    private void Shoot()
    {
        Vector3 nextBulletRayPosition = firstPersonCamera.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(firstPersonCamera.transform.position, nextBulletRayPosition, out hit, range))
        {
            if (hit.transform.tag == "OtherPlayer")
            {
                hit.transform.GetComponent<health>().SetDamageOnHealth(damage);
            }
            if (debugHit)
            {
                Instantiate(debugHitGameObject, hit.point, Quaternion.identity);
            }
        }
    }

    public void SetReloadDelay(float t)
    {
        reloadDelay = t;
    }

    public void SetNextShootDelay(float t)
    {
        nextShootDelay = t;
    }

    public void SetDamage(float d)
    {
        damage = d;
    }

    public void SetRange(float r)
    {
        range = r;
    }

    public void SetWeaponAmmo(int curMag, int curRes, int maxMag, int maxRes)
    {
        maxMagazineAmmo = maxMag;
        maxReserveAmmo = maxRes;
        magazineAmmo = curMag;
        reserveAmmo = curRes;
    }

}
