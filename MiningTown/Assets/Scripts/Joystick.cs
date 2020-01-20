using System;
using UnityEngine;
using UnityEngine.EventSystems;

//https://github.com/ashwaniarya/Unity3D-Simple-Mobile-Joystick
public class Joystick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public static Action<Vector2> OnJoystickMove;
    [SerializeField] private RectTransform outerCircleRect;
    [SerializeField] private RectTransform innerKnobRect;
    [SerializeField] private float knobCircumference = 100;
    private Vector2 knobPosition;
    private Vector2 initialPosition;
    private bool isTouched = false;

    private void Start()
    {
        initialPosition = outerCircleRect.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        knobPosition = new Vector2((eventData.position.x - outerCircleRect.position.x) / ((outerCircleRect.rect.size.x - innerKnobRect.rect.size.x) / 2),
            (eventData.position.y - outerCircleRect.position.y) / ((outerCircleRect.rect.size.y - innerKnobRect.rect.size.y) / 2));
        knobPosition = (knobPosition.magnitude > 1.0f) ? knobPosition.normalized : knobPosition;
        OnJoystickMove?.Invoke(knobPosition);
        innerKnobRect.anchoredPosition = new Vector2(knobPosition.x * knobCircumference, knobPosition.y * knobCircumference);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        innerKnobRect.anchoredPosition = knobPosition = Vector2.zero;
        OnJoystickMove?.Invoke(Vector2.zero);
    }
}