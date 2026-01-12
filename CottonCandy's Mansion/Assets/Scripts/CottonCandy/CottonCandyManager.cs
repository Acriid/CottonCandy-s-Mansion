using System.Collections;
using UnityEngine;

public class CottonCandyManager : MonoBehaviour
{
    public static CottonCandyManager Instance {get; private set;}

    [SerializeField] private CottonCandySO _cottonCandySO;
    [SerializeField] private GameObject _offerPrefab;
    [SerializeField] private GameObject _poolParent;
    [SerializeField] private int _offerPoolSize;

    private GenericPool<CottonCandyOffer> _pool;
    private CottonCandyOffer _instance;

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
            Debug.Log("Failed to load pool");
        }
        ShowOffer(_cottonCandySO.GetOffer());
    }

    private void ShowOffer(Offer offerToShow)
    {
        _instance = _pool.Get();
        _instance.InitializeOffer(offerToShow);
    }
    public void HideOffer()
    {
        _pool.Return(_instance);
    }
}
