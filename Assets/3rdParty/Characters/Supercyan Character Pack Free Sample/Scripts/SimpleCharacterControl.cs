using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterControl : MonoBehaviour
{
    [SerializeField] private float m_moveSpeed = 1;
    [SerializeField] private float m_interpolation = 10;
    private bool m_wasGrounded;
    private bool m_isGrounded;
    private Animator m_animator;
    private Rigidbody m_rigidBody;
    private Vector3 m_currentDirection = Vector3.zero;
    private List<Collider> m_collisions = new List<Collider>();
    private Transform mainCamera;
    private float m_currentV = 0;
    private float m_currentH = 0;

    private void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        mainCamera = Camera.main.transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

    private void Update()
    {
        m_animator.SetBool("Grounded", m_isGrounded);
        DirectUpdate();
        m_wasGrounded = m_isGrounded;
    }

    private void DirectUpdate()
    {
        //m_currentV = Mathf.Lerp(m_currentV, Joystick.Vertical, Time.deltaTime * m_interpolation);
        //m_currentH = Mathf.Lerp(m_currentH, Joystick.Horizontal, Time.deltaTime * m_interpolation);
        Vector3 direction = mainCamera.forward * m_currentV + mainCamera.right * m_currentH;
        direction.y = 0;
        direction = direction.normalized * direction.magnitude;
        if (direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);
            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;
            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }
    }
}