using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public Image Background { get => _background; }

    public TabGroup tabGroup;

    public UnityEvent OnTabSelected;
    public UnityEvent OnTabDeselected;

    private Image _background;

    private void Start()
    {
        _background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    public void Select()
    {
        OnTabSelected?.Invoke();
    }

    public void Deselect()
    {
        OnTabDeselected?.Invoke();
    }
}
