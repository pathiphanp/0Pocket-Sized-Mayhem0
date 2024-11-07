using UnityEngine;

public class Potal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Civilian>() != null)
        {
            GameManager._instance.AddPointHumansEscaped();
            other.gameObject.GetComponent<Civilian>().TakeDamage();
        }
    }
}