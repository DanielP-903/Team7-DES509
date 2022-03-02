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
    private GameManager m_gameManagerRef;
    private float m_inputDelay = 0.1f;
    private float m_inputTimer;

    private TeaMaker m_teaMakerRef;
    
    // Start is called before the first frame update
    void Start()
    {
        m_gameManagerRef = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (GameObject.FindGameObjectWithTag("Machine"))
        {
            m_teaMakerRef = GameObject.FindGameObjectWithTag("Machine").GetComponent<TeaMaker>();
        }
        else
        {
            Debug.LogError("ERROR: Tea making machine has no tag assigned!");
            Debug.DebugBreak();
        }
        m_inputTimer = 0.1f;
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
        m_inputTimer = m_inputTimer <= 0 ? 0 : m_inputTimer - Time.deltaTime;
        // Debug.Log(m_inputTimer);
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

    private GameObject FindIngredient()
    {
        GameObject g = new GameObject();

        foreach (var ingredient in m_gameManagerRef.m_ingredientList)
        {
            if (ingredient.GetComponent<Ingredient>().m_type == m_heldObject.GetComponent<Ingredient>().m_type)
            {
                g = ingredient;
            }
        }

        return g;
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

        if (mouse.leftButton.IsActuated() && m_inputTimer <= 0)
        {
            m_inputTimer = m_inputDelay;
            RaycastHit hit;
            Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out hit, 100.0f);
            //if (hit.transform != null)
            //{
            //    Debug.Log("I HIT: " + hit.transform.name);
            //}

            if (m_heldObject != null)
            {
                if (hit.transform.tag == "Machine")
                {
                    m_teaMakerRef.AddIngredient(FindIngredient());
                    m_heldObject.GetComponent<Ingredient>().IsHeld = false;
                    Destroy(m_heldObject.gameObject);
                    m_heldObject = null;
                    //if ()
                    //{
                    //    m_heldObject.GetComponent<Ingredient>().IsHeld = false;
                    //    Destroy(m_heldObject.gameObject);
                    //    m_heldObject = null;
                    //    Debug.Log("Ingredient successfully added!");
                    //}
                    //else
                    //{
                    //    Debug.Log("Ingredient adding failed!");
                    //}
                }
                return;
            }
            //m_heldObject = GameObject.FindGameObjectWithTag("Debug");
            if (hit.transform != null && hit.transform.gameObject.GetComponent<Ingredient>())
            {
                if (m_heldObject == null)
                {
                    m_heldObject = Instantiate(hit.transform.gameObject);
                    m_heldObject.GetComponent<Ingredient>().IsHeld = true;
                }
                return;
            }
            else if (hit.transform != null && m_heldObject == null)
            {
                // Remove item from teapot
                //m_teaMakerRef.RemoveIngredient();
            }
        }
        if (mouse.rightButton.IsActuated() && m_inputTimer <= 0)
        {
            if (m_heldObject != null)
            {
                // Discard held object
                m_heldObject.GetComponent<Ingredient>().IsHeld = false;
                Destroy(m_heldObject.gameObject);
                m_heldObject = null;
            }
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
}
