using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class FearAreaControl : MonoBehaviour
{
    [SerializeField] List<GameObject> target = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Fear>() != null)
        {
            target.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Fear>() != null)
        {
            target.Remove(other.gameObject);
        }
    }

    public void CallFear()
    {
        if (target.Count > 0)
        {
            foreach (GameObject tg in target)
            {
                if (tg != null)
                {
                    tg.GetComponent<Fear>().AddFear();
                }
            }
        }
        target.Clear();
    }
}
