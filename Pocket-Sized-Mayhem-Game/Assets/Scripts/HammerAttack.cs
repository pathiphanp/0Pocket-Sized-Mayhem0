using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class HammerAttack : MonoBehaviour
{
    [SerializeField] GameObject fearAear;
    [SerializeField] List<GameObject> target = new List<GameObject>();
    private void Start()
    {

    }
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
            //Can Kill
            foreach (GameObject tg in target)
            {
                if (tg.gameObject != null)
                {
                    if (tg.GetComponent<TakeDamage>().TakeDamage() == TargetType.NPC)
                    {
                        GameManager._instance.AddPointPlayerKill();
                    }
                    tg.GetComponent<TakeDamage>().TakeDamage();
                    fearAear.transform.position = transform.position;
                    fearAear.GetComponent<FearAreaControl>().CallFear();
                }
            }
        }
        target.Clear();
    }

}
