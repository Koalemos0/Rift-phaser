using UnityEngine;

public class ThrowableSwitch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] weapons;

    [Header("Keys")]
    [SerializeField] private KeyCode[] keys;

    [Header("Settings")]
    [SerializeField] private float switchTime;

    private int selectedWeapon;
    private float timeSinceLastSwitch;

    void Start()
    {
        setWeapon();
        select(selectedWeapon);
    }

    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]) && timeSinceLastSwitch >= switchTime){
                selectedWeapon = i;
            }
        }
        if (previousSelectedWeapon != selectedWeapon) select(selectedWeapon);

        timeSinceLastSwitch += Time.deltaTime;
    }


    private void select(int weaponIndex)
    {
        for (int i = 0; i < weapons.Length; i++) 
        {
            weapons[i].gameObject.SetActive(i == weaponIndex);
        }

        timeSinceLastSwitch = 0f;
        onWeaponSelected();
    }

    private void onWeaponSelected()
    {
        print("Select new weapon... ");
    }

    private void setWeapon()
    {
        weapons = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            weapons[i] = transform.GetChild(i);       
        } 

        if(keys == null)
        {
            keys = new KeyCode[weapons.Length];
        }
    }
}
