using System.Collections;
using Interface;
using UnityEngine;
using UnityEngine.Pool;

public class NPC_ObjectPool : MonoBehaviour
{
    [Header("Object Prefab")]
    [SerializeField] private GameObject prefab;
    [Header("SetObjectPool")]
    public int maxPoolSize = 10;
    public int stackDefaultCapacity = 5;

    [SerializeField] GameObject pool;
    private IObjectPool<GameObject> _pool;
    public IObjectPool<GameObject> Pool
    {
        get
        {
            if (_pool == null)
            {
                _pool = new ObjectPool<GameObject>(
                    CreatedPoolItem,
                    OnTakeFromPool,
                    OnReturnedToPool,
                    OnDestroyPoolObject,
                    true,
                    stackDefaultCapacity,
                    maxPoolSize
                );
            }
            return _pool;
        }
    }

    private GameObject CreatedPoolItem()
    {
        GameObject _npcInstan = Instantiate(prefab);
        SetObjectPool<IObjectPool<GameObject>> _npc = _npcInstan.GetComponent<SetObjectPool<IObjectPool<GameObject>>>();
        _npc.AddPool(Pool);
        return _npcInstan;
    }

    private void OnTakeFromPool(GameObject _npc)
    {

    }

    private void OnReturnedToPool(GameObject _npc)
    {
        _npc.SetActive(false);
    }
    private void OnDestroyPoolObject(GameObject _npc)
    {
        Destroy(_npc.gameObject);
    }
    private void OnDestroy()
    {
        if (_pool != null)
        {
            _pool.Clear();
        }
    }
    private void Start()
    {
        SpawnPool();
    }
    private void SpawnPool()
    {
        for (int i = 0; i < maxPoolSize; i++)
        {
            GameObject enemy = Pool.Get();
            enemy.SetActive(false);
            enemy.transform.position = pool.transform.position;
            enemy.SetActive(true);
        }
    }
}
