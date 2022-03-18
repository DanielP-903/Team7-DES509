using System;
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
    [Tooltip("Base movement speed")]
    [SerializeField] private float m_speed;

    [Header("DEBUG")]
    [Tooltip("Current player mode/event")]
    [SerializeField] private Mode m_mode = Mode.Freeroam;
    private Mode m_goToMode = Mode.Freeroam;
    private QD_DialogueHandler handler;
    private TextMeshProUGUI speakerNameTextbox;
    private TextMeshProUGUI messageTextbox;
    private GameObject dialogueBox;
    private bool m_dialogueFinished = false;
    private GameObject m_camera;
    private const float MouseSensitivity = 0.1f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool m_moveForward;
    private bool m_moveBackward;
    private bool m_moveLeft;
    private bool m_moveRight;
    private CharacterController m_characterController;
    private GameObject m_heldObject;
    private GameManager m_gameManagerRef;
    private readonly float m_inputDelay = 0.1f;
    private readonly float m_inputDelayTalking = 0.5f;
    private float m_inputTimer;
    private bool m_walkToLock = false;
    private bool m_finishedTalking = false;
    private TeaMaker m_teaMakerRef;

    private Vector3 m_lockTalkPos;
    private Quaternion m_lockTalkRot;
    private Vector3 m_lockTeaMakePos;
    private Quaternion m_lockTeaMakeRot;
    private RecipeList m_recipeListRefPlayer;

    // Start is called before the first frame update
    void Start()
    {
        m_inputTimer = 0.1f;
        m_camera = transform.GetChild(0).gameObject;
        handler = transform.GetChild(1).gameObject.GetComponent<QD_DialogueHandler>();

        GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        dialogueBox = mainCanvas.transform.GetChild(3).gameObject;
        speakerNameTextbox = dialogueBox.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        messageTextbox = dialogueBox.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
        m_recipeListRefPlayer = GameObject.FindGameObjectWithTag("RecipeList").GetComponent<RecipeList>();

        float defaultY = transform.position.y;
        m_lockTalkPos = GameObject.FindGameObjectWithTag("Character").transform.position + new Vector3(0, 0, -2.4f);
        m_lockTalkPos = new Vector3(m_lockTalkPos.x, transform.position.y, m_lockTalkPos.z);
        m_lockTalkRot = transform.rotation;

        m_lockTeaMakePos = GameObject.FindGameObjectWithTag("Machine").transform.position + new Vector3(0, 0, -1);
        m_lockTeaMakePos = new Vector3(m_lockTeaMakePos.x, transform.position.y, m_lockTeaMakePos.z);
        m_lockTeaMakeRot = transform.rotation;

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
    void OnDrawGizmos()
    {
        var color = Color.cyan;
        color.a = 0.5f;
        Gizmos.color = color;
        m_lockTalkPos = GameObject.FindGameObjectWithTag("Character").transform.position + new Vector3(0, 0, 2.4f);
        m_lockTalkPos = new Vector3(m_lockTalkPos.x, transform.position.y, m_lockTalkPos.z);
        m_lockTalkRot = transform.rotation;

        Gizmos.DrawSphere(m_lockTalkPos, 0.5f);

        m_lockTeaMakePos = GameObject.FindGameObjectWithTag("Machine").transform.position + new Vector3(0, 0, -1);
        m_lockTeaMakePos = new Vector3(m_lockTeaMakePos.x, transform.position.y, m_lockTeaMakePos.z);
        m_lockTeaMakeRot = transform.rotation;
        color = Color.magenta;
        Gizmos.color = color;
        Gizmos.DrawSphere(m_lockTeaMakePos, 0.5f);
    }

    private void TalkingStart()
    {
        handler.SetConversation("Ordering");
        SetText();
        dialogueBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        m_inputTimer = m_inputTimer <= 0 ? 0 : m_inputTimer - Time.deltaTime;
        

        if (m_walkToLock)
        {
            Vector3 offsetPos = m_lockTalkPos;
            Quaternion offsetRot = m_lockTalkRot;
            switch (m_goToMode)
            {
                case Mode.Freeroam:
                    // n/a
                    Debug.Log("Uh oh shouldn't be here!");
                    break;
                case Mode.Talking:
                    offsetPos = m_lockTalkPos;
                    offsetRot = m_lockTalkRot;
                    break;
                case Mode.TeaMaking:
                    offsetPos = m_lockTeaMakePos;
                    offsetRot = m_lockTeaMakeRot;
                    break;
                default:
                    break;
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, offsetRot, Time.deltaTime * 4.0f);
            transform.position = Vector3.Lerp(transform.position, offsetPos, Time.deltaTime * 3.0f);
            float distance = Vector3.Distance(transform.position, offsetPos);
            if (m_goToMode == Mode.Talking)
            {
                if (distance < 0.1f)
                {
                    dialogueBox.SetActive(true);
                }
                if (distance < 0.01f)
                {
                    transform.rotation = offsetRot;
                    m_camera.transform.localRotation = offsetRot;
                    m_mode = Mode.Talking;
                    m_walkToLock = false;
                }
            }
            else if (m_goToMode == Mode.TeaMaking)
            {
                if (distance < 0.01f)
                {
                    transform.rotation = offsetRot;
                    m_camera.transform.localRotation = offsetRot;
                    m_mode = Mode.TeaMaking;
                    m_walkToLock = false;
                }
            }
        }
        HandleInput();
    }

    // Handle player's inputs
    private void HandleInput()
    {
        if (m_mode == Mode.Freeroam && !m_walkToLock)
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

    private void HandleMouseInput()
    {
        var mouse = Mouse.current;
        float mouseX = mouse.delta.x.ReadValue() * MouseSensitivity;
        float mouseY = mouse.delta.y.ReadValue() * MouseSensitivity;

        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

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
                    TeaMakingActions();
                    break;
                }
            default:
                break;
        }
        m_camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
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
            if (Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out RaycastHit hit, 100.0f))
            {
                if (hit.transform.CompareTag("Character") && m_finishedTalking == false)
                {
                    // ACTIVATE CHARACTER DIALOGUE
                    m_walkToLock = true;
                    m_goToMode = Mode.Talking;
                    m_dialogueFinished = false;
                    handler.SetConversation("Ordering");
                    SetText();

                }
                if (hit.transform.CompareTag("Machine") && !GameManager.m_hasBrewedATea && m_finishedTalking == true)
                {
                    m_goToMode = Mode.TeaMaking;
                    m_walkToLock = true;
                }
            }
        }
    }

    private void TalkingActions()
    {
        var mouse = Mouse.current;
        if (mouse.leftButton.IsActuated() && m_inputTimer <= 0 && m_finishedTalking == false)
        {
            m_inputTimer = m_inputDelayTalking;
            // GO TO NEXT DIALOGUE OPTION
            // Don't do anything if the conversation is over
            if (m_dialogueFinished)
            {
                SetText();
                return;
            }
            if (handler.currentMessageInfo.NextID == -1)
            {
                dialogueBox.SetActive(false);
                m_mode = Mode.Freeroam;
                m_goToMode = Mode.Freeroam;
                if (handler.currentConversation.Name != "Ordering")
                {
                    m_finishedTalking = false;
                }
                else
                {
                    m_finishedTalking = true;
                    GameManager.m_hasBrewedATea = false;
                }
            }
            else
            {
                Next();
            }
        }
    }

    private void TeaMakingActions()
    {
        var mouse = Mouse.current;
        if (mouse.leftButton.IsActuated() && m_inputTimer <= 0)
        {
            m_inputTimer = m_inputDelay;
            Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out RaycastHit hit, 100.0f);
            if (hit.transform != null && hit.transform.CompareTag("BrewTea"))
            {
                // Brew tea
                m_teaMakerRef.BrewTea();
                return;
            }

            if (m_heldObject != null)
            {
                if (hit.transform != null && hit.transform.CompareTag("Machine"))
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
            if (hit.transform != null && hit.transform.gameObject.GetComponent<Tea>())
            {
                // Give Tea
                m_teaMakerRef.GiveTea();
                if (m_teaMakerRef.m_teaModel.GetComponent<Tea>().m_name != GameObject.FindGameObjectWithTag("Character").GetComponent<Character>().m_favouriteRecipe + " tea")
                {
                    handler.SetConversation("Served_Neutral");
                    handler.SetMessage(handler.currentConversation.FirstMessage);
                    handler.dialogue.SetMessage(handler.currentConversation.FirstMessage, handler.GetMessage());
                    SetText();
                }
                else
                {
                    handler.SetConversation("Served_Happy");
                    handler.SetMessage(handler.currentConversation.FirstMessage);
                    handler.dialogue.SetMessage(handler.currentConversation.FirstMessage, handler.GetMessage());
                    SetText();
                }
                m_teaMakerRef.m_teaModel.GetComponent<Tea>().m_name = String.Empty;
                m_walkToLock = true;
                m_goToMode = Mode.Talking;
                GameManager.m_hasBrewedATea = true;
                m_finishedTalking = false;
                return;
            }
        }
        if (mouse.rightButton.IsActuated() && m_inputTimer <= 0)
        {
            m_inputTimer = m_inputDelay;
            Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out RaycastHit hit, 100.0f);
            if (hit.transform != null && hit.transform.gameObject.GetComponent<Tea>())
            {
                // Scrap tea
                m_teaMakerRef.m_teaModel.SetActive(false);
                m_teaMakerRef.m_teaModel.GetComponent<Tea>().m_name = String.Empty;
                //m_teaMakerRef.m_teaModel.GetComponent<Tea>().SetColour(new Color(0, 0, 0, .5f));
                //m_teaMakerRef.m_teaModel.GetComponent<Tea>().SetColour(new Color(0, 0, 0, .5f));
                return;
            }
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

    // Taken from Quantum Dialogue START -------------------
    private void SetText()
    {
        // Clear everything
        speakerNameTextbox.text = "";
        messageTextbox.gameObject.SetActive(false);
        messageTextbox.text = "";

        // If at the end, don't do anything
        if (m_dialogueFinished)
            return;

        // Generate choices if a choice, otherwise display the message
        if (handler.currentMessageInfo.Type == QD_NodeType.Message)
        {
            QD_Message message = handler.GetMessage();
            speakerNameTextbox.text = message.SpeakerName;
            messageTextbox.text = message.MessageText;
            messageTextbox.gameObject.SetActive(true);

        }
        else if (handler.currentMessageInfo.Type == QD_NodeType.Choice)
        {
            speakerNameTextbox.text = "Player";
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