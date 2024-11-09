using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<NpcCivilian>() != null)
        {
            GameManager._instance.AddPointHumansEscaped();
            other.gameObject.GetComponent<NpcCivilian>().TakeDamage();
        }
    }
}