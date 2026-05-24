using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

[RequireComponent(typeof(CanvasGroup))]
public class FloatingJoystickManager : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Joystick Elements")]
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
        
        _areaCanvasGroup.blocksRaycasts = false;
        
        HideJoystick();
    }

    //Bu objenin bağlı olduğu yere dokunulursa joystick'in görünür olmasını sağlayan kod parçası
    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        joystickArea.anchoredPosition = localPoint;
        _stickRect.anchoredPosition = Vector2.zero;

        ShowJoystick();

        _stickComponent.OnPointerDown(eventData);
    }

    //Eğer ki haraket ederse oradaki stick'e haber veriyor. Stick'te bu değerin değişmesi ile Movement Input'unun değerini değiştirerek haraket haberi veriyor.
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
    }

    private void HideJoystick()
    {
        _areaCanvasGroup.alpha = 0f;
    }
}