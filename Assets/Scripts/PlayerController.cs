using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GameObject m_camera;
    private const float MouseSensitivity = 0.1f;
    private float xRotation = 0f;
    private bool m_moveForward;
    private bool m_moveBackward;
    private bool m_moveLeft;
    private bool m_moveRight;
    private CharacterController m_characterController;
    [Tooltip("Base movement speed")]
    [SerializeField] private float m_speed;
    private GameObject m_heldObject;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = transform.GetChild(0).gameObject;
        if (TryGetComponent(out CharacterController charController))
        {
            m_characterController = charController;
        }
        else
        {
            Debug.LogError("ERROR: Player has no CharacterController component!");
            Debug.DebugBreak();
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    // Handle player's inputs
    private void HandleInput()
    {
        if (m_moveForward || m_moveBackward)
        {
            Vector3 move = m_moveForward ? (m_speed * Time.deltaTime * transform.forward) : (m_speed / 1.5f * Time.deltaTime * -transform.forward);
            m_characterController.Move(move);
        }
        if (m_moveLeft || m_moveRight)
        {
            Vector3 move = m_moveLeft ? (m_speed * Time.deltaTime * -transform.right) : (m_speed * Time.deltaTime * transform.right);
            m_characterController.Move(move);
        }
        HandleMouseInput();
    }

    // Handle look movement
    private void HandleMouseInput()
    {
        var mouse = Mouse.current;
        float mouseX = mouse.delta.x.ReadValue() * MouseSensitivity;
        float mouseY = mouse.delta.y.ReadValue() * MouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        m_camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);

        if (mouse.leftButton.IsActuated())
        {
            m_heldObject = GameObject.FindGameObjectWithTag("Debug");
            if (!m_heldObject.GetComponent<Ingredient>().IsHeld)
            {
                m_heldObject.GetComponent<Ingredient>().IsHeld = true;
            }
            else
            {
                m_heldObject.GetComponent<Ingredient>().IsHeld = false;
                m_heldObject = null;
            }
        }

        Debug.Log(m_heldObject != null);
    }


    // Input Actions
    // W
    public void Forward(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        m_moveForward = value > 0;
        Debug.Log("Forward detected");
    }
    // S
    public void Backward(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        m_moveBackward = value > 0;
        Debug.Log("Backward detected");
    }
    // A
    public void Left(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        m_moveLeft = value > 0;
        Debug.Log("Left detected");
    }
    // D
    public void Right(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        m_moveRight = value > 0;
        Debug.Log("Right detected");
    }
}
