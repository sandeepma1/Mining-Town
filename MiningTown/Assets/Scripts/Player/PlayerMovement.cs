using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_moveSpeed = 1;
    [SerializeField] private float m_interpolation = 10;
    private Transform mainCamera;
    private Animator m_animator;
    private Vector3 m_currentDirection = Vector3.zero;
    private float currentV = 0;
    private float currentH = 0;
    private Vector2 inputAxis = Vector2.zero;

    private void Start()
    {
        mainCamera = Camera.main.transform;
        Joystick.OnJoystickMove += OnJoystickMove;
        m_animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        Joystick.OnJoystickMove -= OnJoystickMove;
    }

    private void OnJoystickMove(Vector2 position)
    {
        inputAxis = position;
        m_animator.SetBool("Grounded", inputAxis != Vector2.zero);
    }

    private void FixedUpdate()
    {
        currentV = Mathf.Lerp(currentV, inputAxis.y, Time.deltaTime * m_interpolation);
        currentH = Mathf.Lerp(currentH, inputAxis.x, Time.deltaTime * m_interpolation);
        Vector3 direction = mainCamera.forward * currentV + mainCamera.right * currentH;
        direction.y = 0;
        direction = direction.normalized;
        if (direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);
            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;
            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }
    }
}