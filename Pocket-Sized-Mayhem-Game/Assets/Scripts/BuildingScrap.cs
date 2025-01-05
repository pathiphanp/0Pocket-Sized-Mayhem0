using System.Collections;
using Interface;
using UnityEngine;

public class BuildingScrap : MonoBehaviour, TakeDamage
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool canCheck = true;
    public bool canDestroy = false;
    Collider bColl;//BuildingScrap Collider
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bColl = GetComponent<Collider>();
    }
    // Update is called once per frame
    void Update()
    {
        if (rb != null && canCheck)
        {
            if (rb.velocity.y == 0)
            {
                canCheck = false;
                canDestroy = true;
                rb.Sleep();
                rb.velocity = Vector3.zero;
            }
        }
    }
    public void Explode(float _force, Vector3 _direction)
    {
        transform.SetParent(null);
        rb.AddForce(_direction.normalized * _force, ForceMode.Impulse);
        StartCoroutine(DelayCheck());
    }
    IEnumerator DelayCheck()
    {
        yield return new WaitForSeconds(0.5f);
        canCheck = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TakeDamage>() != null && canCheck)
        {
            if (other.GetComponent<TakeDamage>().ThisType() == TargetType.NPC)
            {
                other.GetComponent<TakeDamage>().TakeDamage();
            }
        }
    }

    public bool TakeDamage()
    {
        if (canDestroy)
        {
            Destroy(this.gameObject);
        }
        return true;
    }

    public TargetType ThisType()
    {
        return TargetType.Building;
    }
}
