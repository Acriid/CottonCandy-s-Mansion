using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpMechanic : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_2 = new(0.2f);
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private LayerMask _itemLayer;
    private Ray _lookRay;
    private RaycastHit _raycastHit;
    void OnEnable()
    {
        StartCoroutine(GetPlayerLookRay());
    }
    void OnDisable()
    {
        
    }

    public void PickUpItem()
    {

    }
    private IEnumerator GetPlayerLookRay()
    {
        while(true)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            _lookRay = _playerCamera.ScreenPointToRay(mousePos);
            if(Physics.SphereCast(_lookRay,1f,out _raycastHit, 10f,_itemLayer))
            {
                
            }
            
            yield return _waitForSeconds0_2;
        }
    }
}
