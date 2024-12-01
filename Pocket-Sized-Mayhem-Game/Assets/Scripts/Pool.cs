using Interface;
using UnityEngine;
using UnityEngine.Pool;

public class Pool : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<SetObjectPool<IObjectPool<GameObject>>>().SetUpStartGame();
    }
}
