using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SphereCollider))]
public class PickUpMechanic : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float _detectionRadius = 3f;
    [SerializeField] private LayerMask _itemLayer;

    private List<Item> _itemsInRange = new();
    private Item _targetItem;
    private SphereCollider _detectionTrigger;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        UpdateTargetItem(transform);
    }

    public void Initialize()
    {
        _detectionTrigger = GetComponent<SphereCollider>();
        _detectionTrigger.radius = _detectionRadius;
        _detectionTrigger.isTrigger = true;       
    }
    public void UpdateTargetItem(Transform player)
    {
        _itemsInRange.RemoveAll(item => item == null);

        _targetItem = null;

        if(_itemsInRange.Count == 0){return;}

        float bestScore = float.MaxValue;

        foreach(Item item in _itemsInRange)
        {
            float distance = Vector3.Distance(player.position,item.transform.position);

            if(distance < bestScore)
            {
                bestScore = distance;
                _targetItem = item;
            }
        }
        
    }

    public void PickUpItem(Transform itemHolder, bool keepGlobal, Item itemToPickUp)
    {

        itemToPickUp.transform.SetParent(itemHolder,keepGlobal);
        itemToPickUp.transform.SetLocalPositionAndRotation(Vector3.zero, new(0f,0f,0f,0f));
        itemToPickUp.KinematicRigidBody(true);

        if (_itemsInRange.Remove(itemToPickUp) && itemToPickUp == _targetItem)
        {
            _targetItem = null;
        }
        
    }

    public void DropItem(Transform itemHolder, bool keepGravity, Item itemToPutDown)
    {
        itemToPutDown.transform.SetParent(itemHolder,true);
        itemToPutDown.UseGravity(keepGravity);
        itemToPutDown.KinematicRigidBody(false);
        _itemsInRange.Add(itemToPutDown);
    }

    

    public Item GetTargetItem()
    {
        return _targetItem;
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _itemLayer) == 0) 
            return;

        Item item = other.GetComponent<Item>();

        if(item != null && !_itemsInRange.Contains(item)) 
            _itemsInRange.Add(item);
    }

    void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<Item>(out var item))
        {
            _itemsInRange.Remove(item);
            if(_targetItem == item)
                _targetItem = null;
        }
    }

}
