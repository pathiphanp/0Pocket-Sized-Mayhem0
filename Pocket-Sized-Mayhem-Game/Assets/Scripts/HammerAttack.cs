using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class HammerAttack : MonoBehaviour
{
    Collider coll;
    [SerializeField] GameObject fearAear;
    [SerializeField] public GameObject checkGuardAear;
    [SerializeField] public List<GameObject> target = new List<GameObject>();
    [SerializeField] public List<GameObject> checkHumans = new List<GameObject>();
    bool onAddFear = false;
    private void Start()
    {
        coll = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<TakeDamage>() != null)
        {
            target.Add(other.gameObject);
            if (other.gameObject.GetComponent<TakeDamage>().ThisType() == TargetType.NPC)
            {
                checkHumans.Add(other.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<TakeDamage>() != null)
        {
            target.Remove(other.gameObject);
            if (other.gameObject.GetComponent<TakeDamage>().ThisType() == TargetType.NPC)
            {
                checkHumans.Remove(other.gameObject);
            }
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
                    if (tg.GetComponent<TakeDamage>().TakeDamage() && !onAddFear)
                    {
                        onAddFear = true;
                        fearAear.transform.position = transform.position;
                        fearAear.GetComponent<FearAreaControl>().CallFear();
                    }
                }
            }
        }
        target.Clear();
        checkHumans.Clear();
        onAddFear = false;
        coll.enabled = false;
        coll.enabled = true;
    }
    public void CheckHaveHumanInAttackArea()
    {
        if (checkHumans.Count > 0)
        {
            checkGuardAear.SetActive(true);
        }
    }
}
