    using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    [Header("References")]
    public GameObject[] projectiles;

    [Header("Settings")]
    public int maxSpawnedProjectiles;

    void Start()
    {
        
    }

    void Update()
    {
        SetProjectiles();
        if(projectiles.Length > maxSpawnedProjectiles)
        {
            DeleteOldestProjectile();
        }
    }

    private void SetProjectiles()
    {
        projectiles = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            projectiles[i] = transform.GetChild(i).gameObject;
        }
    }

    public void DeleteOldestProjectile()
    {
        Destroy(projectiles[0]);
    }

}
