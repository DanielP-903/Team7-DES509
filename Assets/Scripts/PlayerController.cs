using QuantumTek.QuantumDialogue;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

enum Mode
{
    Freeroam, Talking, TeaMaking
}

public class PlayerController : MonoBehaviour
{

    [SerializeField] private QD_DialogueHandler handler;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject dialogueBox;
    private bool m_dialogueFinished = false;
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
    private readonly float m_inputDelay = 0.1f;
    private readonly float m_inputDelayTalking = 0.5f;
    private float m_inputTimer;
    private TeaMaker m_teaMakerRef;
    [SerializeField] private Mode m_mode = Mode.Freeroam;
    public int lastFrame = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_inputTimer = 0.1f;
        m_camera = transform.GetChild(0).gameObject;

        if (GameObject.FindGameObjectWithTag("GameManager"))
        {
            m_gameManagerRef = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
        else
        {
            Debug.LogError("ERROR: Game Manager has no tag assigned!");
            Debug.DebugBreak();
        }

        if (GameObject.FindGameObjectWithTag("Machine"))
        {
            m_teaMakerRef = GameObject.FindGameObjectWithTag("Machine").GetComponent<TeaMaker>();
        }
        else
        {
            Debug.LogError("ERROR: Tea Machine has no tag assigned!");
            Debug.DebugBreak();
        }

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
        TalkingStart();
    }

    private void TalkingStart()
    {
        handler.SetConversation("Ordering");
        SetText();
        dialogueBox.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        m_inputTimer = m_inputTimer <= 0 ? 0 : m_inputTimer - Time.deltaTime;
        HandleInput();
    }

    // Handle player's inputs
    private void HandleInput()
    {
        if (m_mode == Mode.Freeroam)
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

    private void FreeroamActions()
    {
        var mouse = Mouse.current;
        if (mouse.leftButton.IsActuated() && m_inputTimer <= 0)
        {
            m_inputTimer = m_inputDelay;
            Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out RaycastHit hit, 100.0f);

            if (m_heldObject != null)
            {
                if (hit.transform.CompareTag("Machine"))
                {
                    // Add held object to tea machine
                    m_teaMakerRef.AddIngredient(FindIngredient());
                    m_heldObject.GetComponent<Ingredient>().IsHeld = false;
                    Destroy(m_heldObject.gameObject);
                    m_heldObject = null;
                }
                return;
            }
            if (hit.transform != null && hit.transform.gameObject.GetComponent<Ingredient>())
            {
                if (m_heldObject == null)
                {
                    // Hold an object
                    m_heldObject = Instantiate(hit.transform.gameObject);
                    m_heldObject.GetComponent<Ingredient>().IsHeld = true;
                }
                return;
            }
        }
        if (mouse.rightButton.IsActuated() && m_inputTimer <= 0)
        {
            m_inputTimer = m_inputDelay;
            if (m_heldObject != null)
            {
                // Discard held object
                m_heldObject.GetComponent<Ingredient>().IsHeld = false;
                Destroy(m_heldObject.gameObject);
                m_heldObject = null;
            }
            if (m_heldObject == null)
            {
                // Remove item from teapot
                m_teaMakerRef.RemoveIngredient();
            }
        }
    }

    private void TalkingActions()
    {
        if (lastFrame != handler.currentMessageInfo.ID)
        {
            Debug.Log("GOTCHA!!");
        }
        Debug.Log("Before: " + handler.currentMessageInfo.ID);
        var mouse = Mouse.current;
        if (mouse.leftButton.IsActuated() && m_inputTimer <= 0)
        {
            m_inputTimer = m_inputDelayTalking;
            // GO TO NEXT DIALOGUE OPTION
            // Don't do anything if the conversation is over
            if (m_dialogueFinished)
            {
                // dialogueBox.SetActive(false);

                //Go to next conversation (via index)
                int nextConvoIndex = handler.currentConversationIndex + 1 < handler.dialogue.Conversations.Count ? handler.currentConversationIndex + 1 : 0;

                handler.SetConversation(handler.dialogue.GetConversation(nextConvoIndex+1).Name);
                //handler.NextMessage

                // FIRST MESSAGE IS NOT READ
                m_dialogueFinished = false;
                SetText();
               // Next();
                return;
            }
            // Check if the space key is pressed and the current message is not a choice
            if (handler.currentMessageInfo.Type == QD_NodeType.Message)
            {
                if (handler.currentMessageInfo.NextID == -1)
                {
                    m_dialogueFinished = true;
                }
                else
                {
                    //QD_Message msg = handler.dialogue.GetMessage(handler.GetNextID(handler.currentMessageInfo.ID));
                    //handler.currentMessageInfo = new QD_MessageInfo(msg.ID, msg.NextMessage, QD_NodeType.Message);
                    Next();
                }
            }
        }
        Debug.Log("After: " + handler.currentMessageInfo.ID);
        lastFrame = handler.currentMessageInfo.ID;
    }

    // Taken from Quantum Dialogue START -------------------
    private void SetText()
    {
        // Clear everything
        speakerName.text = "";
        messageText.gameObject.SetActive(false);
        messageText.text = "";

        // If at the end, don't do anything
        if (m_dialogueFinished)
            return;

        // Generate choices if a choice, otherwise display the message
        if (handler.currentMessageInfo.Type == QD_NodeType.Message)
        {
            QD_Message message = handler.GetMessage();
            speakerName.text = message.SpeakerName;
            messageText.text = message.MessageText;
            messageText.gameObject.SetActive(true);

        }
        else if (handler.currentMessageInfo.Type == QD_NodeType.Choice)
        {
            speakerName.text = "Player";
        }
    }
    public void Next(int choice = -1)
    {
        if (m_dialogueFinished)
            return;

        // Go to the next message
        handler.NextMessage(choice);
        // Set the new text
        SetText();
        // End if there is no next message
        if (handler.currentMessageInfo.ID < 0)
            m_dialogueFinished = true;
    }
    // Taken from Quantum Dialogue END -------------------

    private void HandleMouseInput()
    {
        var mouse = Mouse.current;
        float mouseX = mouse.delta.x.ReadValue() * MouseSensitivity;
        float mouseY = mouse.delta.y.ReadValue() * MouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        m_camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);


        switch (m_mode)
        {
            case Mode.Freeroam:
                {
                    FreeroamActions();
                    break;
                }
            case Mode.Talking:
                {
                    TalkingActions();
                    break;
                }
            case Mode.TeaMaking:
                {
                    break;
                }
            default:
                break;
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