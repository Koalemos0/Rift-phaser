using UnityEngine;
using UnityEngine.ProBuilder;

public class Throwing : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;
    public Transform parentobj;
    public ProjectileHandler projectileHandler;

    [Header("Settings")]
    [SerializeField] private int currentAmmo;
    public int maxAmmo;
    public float throwCooldown;
    public float reloadTime;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;

    bool reloading;
    private void Start()
    {
        projectileHandler = GetComponent<ProjectileHandler>();
        currentAmmo = maxAmmo;
        readyToThrow = true;
        reloading = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(throwKey) && readyToThrow && currentAmmo > 0 && !reloading)
        {
            Throw();
        }
        if (Input.GetKeyDown(reloadKey))
        {
            reloading = true;
            Invoke(nameof(Reload), reloadTime);
        }
    }

    public void Throw()
    {
        readyToThrow = false;

        //instantiate obj to throw
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation, parentobj); // original object, attack point transform, quaterion rotation, parent 

        //get rb component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        //calculate direction
        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        //add force
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        //add force to rb
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        currentAmmo--;

        //throw cd
        Invoke(nameof(ResetThrow), throwCooldown);

        projectileHandler.DeleteOldestProjectile();
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

    private void Reload()
    {
        currentAmmo = maxAmmo;
        reloading = false;
    }

    private void DestroyProjectile()
    {
       
    }
}
