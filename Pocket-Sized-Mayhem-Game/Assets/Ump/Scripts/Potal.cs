using UnityEngine;

public class Potal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<NPCnormal>() != null)
        {
            GameManager._instance.AddPointHumansEscaped();
        }
    }
}