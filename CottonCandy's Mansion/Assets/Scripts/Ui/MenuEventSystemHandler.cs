using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class MenuEventSystemHandler : MonoBehaviour
{
    [SerializeField] private List<Selectable> _selectables;
    [SerializeField] protected Selectable _firstSelected;
    [SerializeField] protected InputReader _inputReader;
    [SerializeField] protected Selectable _lastSelected;
    public virtual void OnEnable()
    {
        StartCoroutine(SelectAfterDelay());
        _inputReader.EnableNavigateAction();
        _inputReader.EnableSubmitAction();
        SubscribeToEvents();
    }
    public virtual void OnDisable()
    {
        _inputReader.DisableNavigateAction();
        _inputReader.DisableSubmitAction();
        UnsubscribeFromEvents();
    }
    protected virtual IEnumerator SelectAfterDelay()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);
    }
    public virtual void Awake()
    {
        foreach(Selectable selectable in _selectables)
        {
            AddSelectionListners(selectable);
        }
    }
    protected virtual void AddSelectionListners(Selectable selectable)
    {
        if(!selectable.gameObject.TryGetComponent<EventTrigger>(out var trigger))
        {
            trigger = selectable.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry SelectEntry = new() { eventID = EventTriggerType.Select};
        SelectEntry.callback.AddListener(OnSelect);
        trigger.triggers.Add(SelectEntry); 

        EventTrigger.Entry DeselectEntry = new() { eventID = EventTriggerType.Deselect};
        DeselectEntry.callback.AddListener(OnDeselect);
        trigger.triggers.Add(DeselectEntry);

        EventTrigger.Entry PointerEnter = new() {eventID = EventTriggerType.PointerEnter};
        PointerEnter.callback.AddListener(OnPointerEnter);
        trigger.triggers.Add(PointerEnter);

        EventTrigger.Entry PointerExit = new() {eventID = EventTriggerType.PointerExit};
        PointerExit.callback.AddListener(OnPointerExit);
        trigger.triggers.Add(PointerExit);
    }

    public void OnSelect(BaseEventData eventData)
    {
        _lastSelected = eventData.selectedObject.GetComponent<Selectable>();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        
    }
    public void OnPointerEnter(BaseEventData eventData)
    {
        if (eventData is PointerEventData pointerEventData)
        {
            Selectable sel = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
            if(sel == null)
            {
                sel = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
            }

            pointerEventData.selectedObject = sel.gameObject;
        }
    } 
    public void OnPointerExit(BaseEventData eventData)
    {
        if (eventData is PointerEventData pointerEventData)
        {
            pointerEventData.selectedObject = null;
        }
    }

    protected virtual void OnNavigate()
    {
        if(EventSystem.current.currentSelectedGameObject == null && _lastSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(_lastSelected.gameObject);
        }
    }
    protected virtual void OnSubmit()
    {
        if(EventSystem.current.currentSelectedGameObject.TryGetComponent<Button>(out var currentButton))
        {
            currentButton.onClick.Invoke();
        }
        
    }
    private void SubscribeToEvents()
    {
        _inputReader.OnNavigate += OnNavigate;
        _inputReader.OnSubmit += OnSubmit;
    }
    private void UnsubscribeFromEvents()
    {
        _inputReader.OnNavigate -= OnNavigate;
        _inputReader.OnSubmit -= OnSubmit;
    }
}
