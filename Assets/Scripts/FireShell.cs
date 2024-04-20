using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FireShell : MonoBehaviour {

    public GameObject bullet;
    public GameObject turret;
    public GameObject rotatingTurret;
    public GameObject enemy;
    private float defaultAngle = 30f;
    private float minDistanceToTarget = 13f;
    private float maxDistanceToTarget = 22f;

    void CreateBullet()
    {
        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        Debug.Log(distance);

        float angle;

        if (distance < minDistanceToTarget)
        {
            angle = Mathf.Lerp(75f, 55f, distance / minDistanceToTarget);
        }
        else if (distance < maxDistanceToTarget)
        {
            angle = Mathf.Lerp(30f, 0f, (maxDistanceToTarget - distance) / (maxDistanceToTarget - minDistanceToTarget));
        }
        else
        {
            angle = defaultAngle;
        }

        Quaternion turretRotation = Quaternion.Euler(-angle, rotatingTurret.transform.rotation.eulerAngles.y, rotatingTurret.transform.rotation.eulerAngles.z);
        rotatingTurret.transform.rotation = turretRotation;

        GameObject newBullet = Instantiate(bullet, turret.transform.position, turret.transform.rotation);
        newBullet.GetComponent<MoveShell>().parentTurret = turret.transform;

        Quaternion bulletRotation = Quaternion.Euler(turretRotation.eulerAngles.x - 15f, turretRotation.eulerAngles.y, turretRotation.eulerAngles.z);
        newBullet.transform.rotation = bulletRotation;

        newBullet.GetComponent<MoveShell>().Fire();
    }

    void FireBullet()
    {
        Vector3 aimAt = CalculateTrajectory();
        if (aimAt != Vector3.zero)
        {
            transform.forward = CalculateTrajectory();
            CreateBullet();
        }
    }

    void Start() 
    {
        StartCoroutine(FiringRoutine());
    }

    Vector3 CalculateTrajectory()
    {
        Vector3 p = enemy.transform.position - transform.position;
        Vector3 v = enemy.transform.forward + enemy.GetComponent<Rigidbody>().velocity;
        float s = bullet.GetComponent<MoveShell>().speed;

        float a = Vector3.Dot(v, v) - s * s;
        float b = Vector3.Dot(p, v);
        float c = Vector3.Dot(p, p);
        float discriminant = b * b - a * c;

        if (discriminant < 0.1f)
            return Vector3.zero;

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float t1 = (-b - sqrtDiscriminant) / a;
        float t2 = (-b + sqrtDiscriminant) / a;

        float t;
        if (t1 < 0.0f && t2 < 0.0f)
            return Vector3.zero;
        else if (t1 < 0.0f)
            t = t2;
        else if (t2 < 0.0f)
            t = t1;
        else
            t = Mathf.Max(t1, t2);

        return t * p + v;
    }

    private IEnumerator FiringRoutine()
    {
        while (true)
        {
            FireBullet();
            yield return new WaitForSeconds(0.25f);
        }
    }
}
