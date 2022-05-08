using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;
using UnityEngine.SceneManagement;
public class IntroPlayerController : MonoBehaviour
{
    [Tooltip("Base movement speed")]
    [SerializeField] private float m_speed;
    [Tooltip("Player Look Sensitivity")]
    [Range(0, 0.3f)]
    [SerializeField] private float MouseSensitivity = 0.1f;
    private float xRotation = 0f;
    private bool m_moveForward;
    private bool m_moveBackward;
    private bool m_moveLeft;
    private bool m_moveRight;
    private bool m_quit;
    private CharacterController m_characterController;
    private GameObject m_camera;
    private float m_quitTimer = 2.0f;
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

        if (m_quit)
        {
            m_quitTimer -= Time.deltaTime;
            if (m_quitTimer < 0)
            {
                Debug.Log("QUIT!");
                Application.Quit();
            }
        }
        else
        {
            m_quitTimer = 2.0f;
        }

        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        var mouse = Mouse.current;
        float mouseX = mouse.delta.x.ReadValue() * MouseSensitivity;
        float mouseY = mouse.delta.y.ReadValue() * MouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


        m_camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
        
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Gate"))
        {
            // Go to next scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // Input Actions
    // W
    public void Forward(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        m_moveForward = value > 0;
        //Debug.Log("Forward detected");
    }
    // S
    public void Backward(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        m_moveBackward = value > 0;
        //Debug.Log("Backward detected");
    }
    // A
    public void Left(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        m_moveLeft = value > 0;
        //Debug.Log("Left detected");
    }
    // D
    public void Right(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        m_moveRight = value > 0;
        //Debug.Log("Right detected");
    }
    // Escape
    public void Quit(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        m_quit = value > 0;
        //Debug.Log("Right detected");
    }
}