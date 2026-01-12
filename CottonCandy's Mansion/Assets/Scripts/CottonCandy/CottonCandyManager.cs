using System.Collections;
using UnityEngine;

public class CottonCandyManager : MonoBehaviour
{
    public static CottonCandyManager Instance {get; private set;}

    [SerializeField] private GameObject _offerPrefab;
    [SerializeField] private GameObject _poolParent;
    [SerializeField] private int _offerPoolSize;

    private GenericPool<CottonCandyOffer> _pool;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _pool = PoolManager.Instance.GetPool<CottonCandyOffer>(_offerPrefab,_offerPoolSize);
        if(_pool == null)
        {
            Debug.Log("Failed");
        }
        else
        {
            _pool.Get();
        }
    }

}
