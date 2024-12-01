using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class HammerAttack : MonoBehaviour
{
    [SerializeField] GameObject fearAear;
    [SerializeField] List<GameObject> target = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<TakeDamage>() != null)
        {
            target.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<TakeDamage>() != null)
        {
            target.Remove(other.gameObject);
        }
    }
    public void CallHammerAttack()
    {
        if (target.Count > 0)
        {
            foreach (GameObject tg in target)
            {
                if (tg != null)
                {
                    if (tg.GetComponent<TakeDamage>() != null)
                    {
                        if (tg.GetComponent<TakeDamage>().TakeDamage() != TargetType.Guard)
                        {
                            tg.gameObject.SetActive(false);
                            tg.gameObject.SetActive(true);
                            tg.GetComponent<TakeDamage>().TakeDamage();
                            fearAear.transform.position = transform.position;
                            fearAear.GetComponent<FearAreaControl>().CallFear();
                        }
                    }
                }
            }
        }
        target.Clear();
    }
}
