using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

[RequireComponent(typeof(CanvasGroup))]
public class FloatingJoystickManager : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Joystick Parçaları")]
    public RectTransform joystickArea;
    public GameObject stickObject;
    
    private RectTransform _stickRect;
    private RectTransform _parentRect;
    private OnScreenStick _stickComponent;
    private CanvasGroup _areaCanvasGroup;

    void Awake()
    {
        _parentRect = GetComponent<RectTransform>();
        _stickRect = stickObject.GetComponent<RectTransform>();
        _stickComponent = stickObject.GetComponent<OnScreenStick>();
        _areaCanvasGroup = joystickArea.GetComponent<CanvasGroup>();

        HideJoystick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        joystickArea.anchoredPosition = localPoint;
        _stickRect.anchoredPosition = Vector2.zero;

        ShowJoystick();

        _stickComponent.OnPointerDown(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _stickComponent.OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _stickComponent.OnPointerUp(eventData);
        HideJoystick();
    }

    private void ShowJoystick()
    {
        _areaCanvasGroup.alpha = 1f;
        _areaCanvasGroup.blocksRaycasts = false;
    }

    private void HideJoystick()
    {
        _areaCanvasGroup.alpha = 0f;
        _areaCanvasGroup.blocksRaycasts = false;
    }
}