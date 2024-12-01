using System.Collections;
using Interface;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BuildingScrap : MonoBehaviour, TakeDamage
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool canCheck = true;
    public bool canDestroy = false;
    Collider bColl;//BuildingScrap Collider
    [SerializeField] float _y;
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
            _y = rb.velocity.y;
            if (rb.velocity.y == 0)
            {
                canCheck = false;
                canDestroy = true;
                rb.Sleep();
                rb.velocity = Vector3.zero;
            }
        }
    }
    public void Explode(float _force)
    {
        transform.SetParent(null);
        Vector3 direction = Random.insideUnitCircle.normalized;
        rb.AddForce(direction * _force, ForceMode.Impulse);
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
            if (other.GetComponent<TakeDamage>().TakeDamage() == TargetType.NPC)
            {
                other.GetComponent<TakeDamage>().TakeDamage();
            }
        }
    }

    public TargetType TakeDamage()
    {
        if (canDestroy)
        {
            Destroy(this.gameObject);
        }
        return TargetType.Building;
    }
}
