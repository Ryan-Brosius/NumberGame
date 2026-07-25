using System.Collections.Generic;
using UnityEngine;

public class TurretCode : MonoBehaviour
{
    [Header("target")]
    [SerializeField] private Transform cursorTransform;
    [Header("bullet")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 5f;
    [SerializeField] private int maxBullets = 5;
    private List<GameObject> activeBullets = new List<GameObject>();

    void Update()
    {
        Vector3 direction = cursorTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        if (direction.x < 0)
        {
            //transform.localScale = new Vector3(1.4f, -1.4f, 1.4f);
        }
        else
        {
            //transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
        activeBullets.RemoveAll(bullet => bullet == null);
    }

    void Fire()
    {
        if (activeBullets.Count >= maxBullets)
        {
            GameObject oldestBullet = activeBullets[0];

            if (oldestBullet != null)
            {
                Destroy(oldestBullet);
            }

            activeBullets.RemoveAt(0);
        }
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );
        activeBullets.Add(bullet);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.up * bulletSpeed;
        }
        Destroy(bullet, bulletLifetime);
    }
}