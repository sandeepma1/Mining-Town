using System;
using UnityEngine;
using UnityEngine.EventSystems;

//https://github.com/ashwaniarya/Unity3D-Simple-Mobile-Joystick
public class Joystick : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public static Action<Vector2, bool> OnJoystickMove;
    [SerializeField] private RectTransform glowArc;
    [SerializeField] private RectTransform outerCircleRect;
    [SerializeField] private RectTransform innerKnobRect;
    [SerializeField] private float knobCircumference = 100;
    [SerializeField] private Canvas canvas;
    private Vector2 knobPosition;
    private Vector2 defaultPosition;
    private Vector3 m_currentDirection = Vector3.zero;

    private void Start()
    {
        defaultPosition = new Vector2(Screen.width / 2, Screen.width / 3);
        OnJoystickMove?.Invoke(Vector2.zero, false);
        RestJoystick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        outerCircleRect.anchoredPosition = canvas.ScreenToCanvasPosition(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        RestJoystick();
    }

    public float angleOffset = 90;
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
        innerKnobRect.anchoredPosition = knobPosition = Vector2.zero;
        OnJoystickMove?.Invoke(Vector2.zero, false);
        RestJoystick();
    }

    private void RestJoystick()
    {
        outerCircleRect.anchoredPosition = canvas.ScreenToCanvasPosition(defaultPosition);
        glowArc.localEulerAngles = Vector3.zero;

    }
}