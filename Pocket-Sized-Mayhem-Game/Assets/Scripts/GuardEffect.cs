using System.Collections.Generic;
using Interface;
using UnityEngine;

public class GuardEffect : MonoBehaviour
{
    [SerializeField] List<SetGuardEffectProtect> listGuardProtect = new List<SetGuardEffectProtect>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SetGuardEffectProtect>() != null && other.GetComponent<TakeDamage>().ThisType() != TargetType.Guard)
        {
            other.GetComponent<SetGuardEffectProtect>().AddGuardEffect();
            listGuardProtect.Add(other.GetComponent<SetGuardEffectProtect>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<SetGuardEffectProtect>() != null)
        {
            other.GetComponent<SetGuardEffectProtect>().RemoveGuardEffect();
            listGuardProtect.Remove(other.GetComponent<SetGuardEffectProtect>());
        }
    }
    public void OffGuard()
    {
        if (listGuardProtect.Count > 0)
        {
            foreach (SetGuardEffectProtect lgp in listGuardProtect)
            {
                lgp.RemoveGuardEffect();
            }
            listGuardProtect.Clear();
        }
        Destroy(this.gameObject);
    }
}
