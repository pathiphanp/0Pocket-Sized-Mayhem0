using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] protected float _radius = 1f;
    [SerializeField] protected List<GameObject> _civilians;

    public virtual void SpawnObject(GameObject civilian)
    {
        Vector3 position = transform.position;
        Vector3 offset = Vector3.ClampMagnitude(new Vector3(Random.Range(-_radius, _radius), 0f, Random.Range(-_radius, _radius)), _radius);
        Instantiate(civilian, position + offset, Quaternion.Euler(0, 0, 0));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
