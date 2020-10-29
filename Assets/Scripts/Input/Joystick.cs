using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//https://github.com/ashwaniarya/Unity3D-Simple-Mobile-Joystick
public class Joystick : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public static Action<Vector2, bool> OnJoystickMove;
    public static Action OnKnobButtonClicked;
    public static Action<bool> OnToggleJoystick;
    public static Action OnJoystickReset;
    [SerializeField] private Image mainPanel;
    [SerializeField] private RectTransform glowArc;
    [SerializeField] private RectTransform outerCircleRect;
    [SerializeField] private RectTransform innerKnobRect;
    [SerializeField] private float knobCircumference = 100;
    [SerializeField] private Canvas mainParentCanvas;
    private Vector2 knobPosition;
    private Vector2 defaultPosition;
    public float angleOffset = 90;
    private Vector2 lastClickPosition;

    private void Awake()
    {
        OnToggleJoystick += ToggleJoystick;
        OnJoystickReset += RestJoystick;
    }

    private void Start()
    {
        defaultPosition = new Vector2(Screen.width / 2, Screen.width / 3);
        OnJoystickMove?.Invoke(Vector2.zero, false);
        RestJoystick();
    }

    private void OnDestroy()
    {
        OnToggleJoystick -= ToggleJoystick;
        OnJoystickReset -= RestJoystick;
    }

    private void ToggleJoystick(bool isEnabled)
    {
        outerCircleRect.gameObject.SetActive(isEnabled);
        mainPanel.raycastTarget = isEnabled;
        PlayerInteraction.OnToggleColliderTrigger?.Invoke(!isEnabled);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastClickPosition = eventData.position;
        outerCircleRect.anchoredPosition = mainParentCanvas.ScreenToCanvasPosition(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (lastClickPosition == eventData.position)
        {
            OnKnobButtonClicked?.Invoke();
            lastClickPosition = Vector2.zero;
        }
        RestJoystick();
    }

    public void OnDrag(PointerEventData eventData)
    {
        knobPosition = new Vector2((eventData.position.x - outerCircleRect.position.x) / ((outerCircleRect.rect.size.x - innerKnobRect.rect.size.x) / 2),
            (eventData.position.y - outerCircleRect.position.y) / ((outerCircleRect.rect.size.y - innerKnobRect.rect.size.y) / 2));
        knobPosition = (knobPosition.magnitude > 1.0f) ? knobPosition.normalized : knobPosition;
        OnJoystickMove?.Invoke(knobPosition, true);
        innerKnobRect.anchoredPosition = new Vector2(knobPosition.x * knobCircumference, knobPosition.y * knobCircumference);

        //Rotate GlowArc
        Vector3 dir = Vector2.zero - knobPosition;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        glowArc.localEulerAngles = new Vector3(0, 0, angle + angleOffset);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        RestJoystick();
    }

    public void RestJoystick()
    {
        innerKnobRect.anchoredPosition = knobPosition = Vector2.zero;
        OnJoystickMove?.Invoke(Vector2.zero, false);
        outerCircleRect.anchoredPosition = mainParentCanvas.ScreenToCanvasPosition(defaultPosition);
        glowArc.localEulerAngles = Vector3.zero;
    }

}