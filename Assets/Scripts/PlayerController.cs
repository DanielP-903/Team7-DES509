using System;
using System.Collections.Generic;
using QuantumTek.QuantumDialogue;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

enum Mode
{
    Freeroam, Talking, TeaMaking
}

public class PlayerController : MonoBehaviour
{
    [Tooltip("Base movement speed")]
    [SerializeField] private float m_speed;
    [Tooltip("Player Look Sensitivity")]
    [Range(0, 0.3f)]
    [SerializeField] private float MouseSensitivity = 0.1f;
    [Tooltip("Talking Skip/Next Delay")]
    [Range(0.1f, 0.5f)]
    [SerializeField] private float m_inputDelayTalking = 0.1f;

    [Header("DEBUG")]
    [Tooltip("Current player mode/event")]
    [SerializeField] private Mode m_mode = Mode.Freeroam;

    [HideInInspector] public bool m_finishedTalking = false;
    [HideInInspector] public bool m_teaAnimFlag;
    [HideInInspector] public List<QD_Message> messageList;
    private bool m_mouseDown = false;
    private TextMeshProUGUI m_tooltipText;
    private GameObject m_tooltipObject;
    private Mode m_goToMode = Mode.Freeroam;
    private QD_DialogueHandler handler;
    private TextMeshProUGUI speakerNameTextbox;
    private TextMeshProUGUI messageTextbox;
    private GameObject dialogueBox;
    private bool m_dialogueFinished = false;
    private GameObject m_camera;
    private float xRotation = 0f;
    private bool m_moveForward;
    private bool m_moveBackward;
    private bool m_moveLeft;
    private bool m_moveRight;
    private bool m_quit;
    private CharacterController m_characterController;
    private GameObject m_heldObject;
    private GameManager m_gameManagerRef;
    private readonly float m_inputDelay = 0.1f;
    private float m_quitTimer = 2.0f;
    private float m_inputTimer;
    private bool m_walkToLock = false;
    private TeaMaker m_teaMakerRef;
    public Vector3 m_lockTalkPos;
    private Quaternion m_lockTalkRot;
    public Vector3 m_lockTeaMakePos;
    private Quaternion m_lockTeaMakeRot;
    private bool m_playerLockTurn = false;
    private GameObject m_recipeBookUI;
    private bool m_lookingAtBook = false;
    private float m_lockTimer = 2.5f;
    [HideInInspector] public AudioSource sfx_pouring, sfx_remove, sfx_normal, sfx_newRecipe;

    // Start is called before the first frame update
    void Start()
    {
        messageList = new List<QD_Message>();
        m_inputTimer = 0.1f;
        m_camera = transform.GetChild(0).gameObject;
        handler = transform.GetChild(1).gameObject.GetComponent<QD_DialogueHandler>();

        sfx_pouring = m_camera.transform.GetChild(2).GetComponent<AudioSource>();
        sfx_remove = m_camera.transform.GetChild(3).GetComponent<AudioSource>();
        sfx_normal = m_camera.transform.GetChild(4).GetComponent<AudioSource>();
        sfx_newRecipe = m_camera.transform.GetChild(5).GetComponent<AudioSource>();

        GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        dialogueBox = mainCanvas.transform.GetChild(3).gameObject;
        speakerNameTextbox = dialogueBox.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        messageTextbox = dialogueBox.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();

        m_recipeBookUI = mainCanvas.transform.GetChild(6).gameObject;

        m_tooltipObject = mainCanvas.transform.GetChild(4).gameObject;
        m_tooltipText = m_tooltipObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        float defaultY = transform.position.y;
        
        //m_lockTalkPos = new Vector3(-5.93f, transform.position.y, 0.44f);
        //m_lockTalkRot = transform.rotation;

        //m_lockTeaMakePos = GameObject.FindGameObjectWithTag("Machine").transform.position + new Vector3(0, 0, -1);
        //m_lockTeaMakePos = new Vector3(m_lockTeaMakePos.x, transform.position.y, m_lockTeaMakePos.z);
        //m_lockTeaMakeRot = transform.rotation;
        //m_lockTeaMakeRot.Set(m_lockTeaMakeRot.x, 0, m_lockTeaMakeRot.z,1);

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

        m_tooltipObject.SetActive(false);
        m_teaMakerRef.m_teaModel.GetComponent<MeshCollider>().enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        TalkingStart();
    }

    void OnDrawGizmos()
    {
        var color = Color.cyan;
        color.a = 0.5f;
        Gizmos.color = color;
        //m_lockTalkPos = new Vector3(-5.93f, transform.position.y, 0.44f);
        //m_lockTalkRot = transform.rotation;

        Gizmos.DrawSphere(m_lockTalkPos, 0.5f);

        //m_lockTeaMakePos = GameObject.FindGameObjectWithTag("Machine").transform.position + new Vector3(0, 0, -1);
        //m_lockTeaMakePos = new Vector3(m_lockTeaMakePos.x, transform.position.y, m_lockTeaMakePos.z);
        //m_lockTeaMakeRot = transform.rotation;
        color = Color.magenta;
        color.a = 0.5f;
        Gizmos.color = color;
        Gizmos.DrawSphere(m_lockTeaMakePos, 0.5f);
    }

    private void TalkingStart()
    {
        handler.dialogue = m_gameManagerRef.currentCharacter.currentDialogue;
        handler.SetConversation("Ordering");
        SetText();
        dialogueBox.SetActive(false);
        messageList.Add(handler.GetMessage());

    }

    // Update is called once per frame
    void Update()
    {
        m_inputTimer = m_inputTimer <= 0 ? 0 : m_inputTimer - Time.deltaTime;
        HandleInput();

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
            transform.position = Vector3.Lerp(transform.position, offsetPos, Time.deltaTime * 3.0f);
            float distance = Vector3.Distance(transform.position, offsetPos);
            if (m_goToMode == Mode.Talking)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, offsetRot, Time.deltaTime * 3.0f);
                if (distance < 0.1f)
                {
                    dialogueBox.SetActive(true);
                }
                if (distance < 0.05f)
                {
                    m_teaMakerRef.m_teaModel.GetComponent<Tea>().IsHeld = false;
                    StartCoroutine(m_teaMakerRef.m_teaModel.GetComponent<Tea>().PlayAnim());
                    m_mode = Mode.Talking;
                    m_walkToLock = false;
                    m_playerLockTurn = false;
                }
            }
            else if (m_goToMode == Mode.TeaMaking)
            {
                m_lockTimer = m_lockTimer <= 0 ? 0 : m_lockTimer - Time.deltaTime;
                if (Quaternion.Angle(transform.rotation, offsetRot) > 0.5f && m_playerLockTurn == true && m_lockTimer > 0)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, offsetRot, Time.deltaTime * 3.0f);
                }
                else
                {
                    m_playerLockTurn = false;
                }
                if (distance < 0.05f)
                {
                    m_lockTimer = 2.5f;
                    m_mode = Mode.TeaMaking;
                    m_walkToLock = false;
                    m_playerLockTurn = false;
                }
            }
        }
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
        if (!m_lookingAtBook)
        {
            CheckReleased();

            var mouse = Mouse.current;
            float mouseX = mouse.delta.x.ReadValue() * MouseSensitivity;
            float mouseY = mouse.delta.y.ReadValue() * MouseSensitivity;

            xRotation -= mouseY;
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
                    m_playerLockTurn = true;
                    m_goToMode = Mode.Talking;
                    m_dialogueFinished = false;
                    handler.dialogue = m_gameManagerRef.currentCharacter.currentDialogue;
                    handler.SetConversation("Ordering");
                    SetText();
                    QD_Message currMessage = handler.GetMessage();
                    if (currMessage != null)
                    {
                        Material newExpression = currMessage.Expression;
                        if (newExpression != null)
                        {
                            m_gameManagerRef.currentCharacter.SetExpression(newExpression);
                        }
                        else
                        {
                            m_gameManagerRef.currentCharacter.SetExpression(m_gameManagerRef.currentCharacter.characterScriptableObject.material);
                        }
                    }
                    else
                    {
                        m_gameManagerRef.currentCharacter.SetExpression(m_gameManagerRef.currentCharacter.characterScriptableObject.material);
                    }
                }
                if (hit.transform.CompareTag("Machine") && !GameManager.m_hasBrewedATea && m_finishedTalking == true)
                {
                    m_goToMode = Mode.TeaMaking;
                    m_walkToLock = true;
                    m_playerLockTurn = true;
                }
            }
        }
    }

    private void TalkingActions()
    {
        var mouse = Mouse.current;
        if (mouse.leftButton.IsActuated() && m_inputTimer <= 0 && m_finishedTalking == false && m_mouseDown == false)
        {
            m_mouseDown = true;
            m_inputTimer = m_inputDelayTalking;
            if (handler.currentMessageInfo.NextID != -1)
            {
                int nextID = handler.GetNextID(handler.GetMessage().ID);
                Material newExpression = handler.dialogue.GetMessage(nextID).Expression;
                if (newExpression != null)
                {
                    m_gameManagerRef.currentCharacter.SetExpression(newExpression);
                }
            }
            // GO TO NEXT DIALOGUE OPTION
            // Don't do anything if the conversation is over
            if (m_dialogueFinished)
            {
                SetText();
                return;
            }
            if (handler.currentMessageInfo.NextID == -1)
            {

                if (handler.currentConversation.Name != "Ordering")
                {
                    // TODO
                    // Set neutral convo next after a narrative response!
                    //handler.SetConversation("Served_Neutral");
                    if (IsQuips())
                    {
                        dialogueBox.transform.GetChild(0).GetComponent<Image>().sprite = m_gameManagerRef.MessageBox;
                        messageTextbox.fontSize = 16;
                        handler.dialogue = m_gameManagerRef.currentCharacter.currentDialogue;
                        handler.SetConversation("Served_Neutral");
                        handler.SetMessage(handler.currentConversation.FirstMessage);
                        handler.dialogue.SetMessage(handler.currentConversation.FirstMessage, handler.GetMessage());
                        SetText();
                        return;
                    }
                    m_finishedTalking = false;
                    m_teaMakerRef.ResetTea();
                    m_teaAnimFlag = true;

                    // End of interaction, go away
                    m_gameManagerRef.currentCharacter.leaving = true;
                    m_gameManagerRef.currentCharacter.isAvailable = false;

                    // Go to next dialogue state
                    if (m_gameManagerRef.currentCharacter.CharacterStages[m_gameManagerRef.currentCharacterName.ToString()] == 0 || handler.currentConversation.Name == "Served_Happy")
                    {
                        if (m_gameManagerRef.currentCharacter.CharacterStages[m_gameManagerRef.currentCharacterName.ToString()] < 3)
                        {
                            m_gameManagerRef.currentCharacter.CharacterStages[m_gameManagerRef.currentCharacterName.ToString()]++;
                        }
                    }


                    //m_gameManagerRef.currentCharacter.currentDialogue = m_gameManagerRef.currentCharacter.characterScriptableObject.dialogues[m_gameManagerRef.currentCharacter.CharacterStages[m_gameManagerRef.currentCharacterName.ToString()]];
                    //handler.dialogue = m_gameManagerRef.currentCharacter.currentDialogue;

                    messageList.Clear();
                }
                else
                {
                    m_finishedTalking = true;
                    m_teaAnimFlag = false;
                    GameManager.m_hasBrewedATea = false;
                }
                dialogueBox.SetActive(false);
                m_mode = Mode.Freeroam;
                m_goToMode = Mode.Freeroam;
            }
            else
            {
                Next();
            }
        }
    }
    private bool IsQuips()
    {
        switch (handler.currentConversation.Name)
        {
            case "Cool_TooMuch":
            {
                return true;
            }
            case "Sour_TooMuch":
            {
                return true;
            }
            case "Aromatic_TooMuch":
            {
                return true;
            }
            case "Warm_TooMuch":
            {
                return true;
            }
            case "Sweet_TooMuch":
            {
                return true;
            }
            case "Aromatic_TooLittle":
            {
                return true;
            }
            case "Cool_TooLittle":
            {
                return true;
            }
            case "Warm_TooLittle":
            {
                return true;
            }
            case "Sour_TooLittle":
            {
                return true;
            }
            case "Sweet_TooLittle":
            {
                return true;
            }
            default:
            {
                return false;
            }
        }
    }
    private void CheckReleased()
    {
        if (m_mouseDown && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            m_mouseDown = false;
        }
    }

    public GameObject FindIngredient()
    {
        foreach (var ingredient in m_gameManagerRef.m_ingredientList)
        {
            if (ingredient.GetComponent<Ingredient>().m_type == m_heldObject.GetComponent<Ingredient>().m_type)
            {
               return ingredient;
            }
        }
        return null;
    }

    public GameObject FindIngredient(string name)
    {
        foreach (var ingredient in m_gameManagerRef.m_ingredientList)
        {
            if (ingredient.GetComponent<Ingredient>().m_type.ToString() == name)
            {
                return ingredient;
            }
        }
        return null;
    }

    private void TeaMakingActions()
    {
        var mouse = Mouse.current;
        if (mouse.leftButton.IsActuated() && m_inputTimer <= 0)
        {
            m_inputTimer = m_inputDelay;
            Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out RaycastHit hit, 100.0f);
            if (hit.transform != null && hit.transform.CompareTag("BrewTea") && m_teaMakerRef.Total > 0)
            {
                // Brew tea
                m_teaMakerRef.BrewTea();
                m_teaMakerRef.m_teaModel.GetComponent<MeshCollider>().enabled = true;
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
            if (hit.transform != null && hit.transform.gameObject.GetComponent<Tea>() && m_teaMakerRef.hasClickedBrew)
            {
                m_teaMakerRef.m_teaModel.GetComponent<MeshCollider>().enabled = false;
                // Give Tea
                m_teaMakerRef.GiveTea();
                if (m_teaMakerRef.m_teaModel.GetComponent<Tea>().m_name != GameObject.FindGameObjectWithTag("Character").GetComponent<Character>().characterScriptableObject.favouriteRecipe)
                {
                    if (m_gameManagerRef.currentCharacter.characterScriptableObject.quipDialogue != null)
                    {
                        dialogueBox.transform.GetChild(0).GetComponent<Image>().sprite = m_gameManagerRef.MessageBoxNoName;
                        messageTextbox.fontSize = 12.0f;
                        handler.dialogue = m_gameManagerRef.currentCharacter.characterScriptableObject.quipDialogue;
                        handler.SetConversation(GetQuip());
                    }
                    else
                    {
                        handler.SetConversation("Served_Neutral");
                    }
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

            if (hit.transform != null && hit.transform.CompareTag("RecipeBook"))
            {
                // Display UI
                m_recipeBookUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                m_lookingAtBook = true;
                return;
            }
           
        }
        if (mouse.rightButton.IsActuated() && m_inputTimer <= 0)
        {
            m_inputTimer = m_inputDelay;
            Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out RaycastHit hit, 100.0f);
            if (hit.transform != null && hit.transform.gameObject.GetComponent<Tea>())
            {
                // Scrap tea (Unused)
                //m_teaMakerRef.m_teaModel.SetActive(false);
                //m_teaMakerRef.m_teaModel.GetComponent<Tea>().m_name = String.Empty;
                //m_teaMakerRef.ResetTea();
                //return;
            }

            if (m_heldObject == null && hit.transform != null && hit.transform.gameObject.GetComponent<TeaMaker>() && m_teaMakerRef.Total > 0)
            {
                // Remove item from teapot
                m_teaMakerRef.RemoveIngredient();
                sfx_remove.Play();
            }
            if (m_heldObject != null)
            {
                sfx_remove.Play();
                // Discard held object
                m_heldObject.GetComponent<Ingredient>().IsHeld = false;
                Destroy(m_heldObject.gameObject);
                m_heldObject = null;
            }
        }

        // TOOLTIPS
        Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out RaycastHit constantHit, 100.0f);
        if (constantHit.transform == null)
        {
            // Not hovering over ingredient
            m_tooltipObject.SetActive(false);
        }
        else
        {
            if (constantHit.transform.gameObject.GetComponent<Ingredient>())
            {
                m_tooltipObject.SetActive(true);
                Ingredient values = constantHit.transform.gameObject.GetComponent<Ingredient>();
                //m_tooltipText.text = values.m_type.ToString() + "\n- " + values.m_description;
                m_tooltipText.text = "<size=14><b>" + values.m_name + "</b></size>" + "\n\n " + values.m_description;
            }
            else if (constantHit.transform.CompareTag("Machine") && !m_teaMakerRef.m_teaModel.GetComponent<MeshCollider>().enabled && !m_teaMakerRef.hasClickedBrew)
            {
                m_tooltipObject.SetActive(true);
                m_tooltipText.text = "<size=16><b>Contains: </b></size>";
                foreach (var item in constantHit.transform.gameObject.GetComponent<TeaMaker>().m_container)
                {
                    if (item.Value != 0)
                    {
                        m_tooltipText.text +=
                            "\n<size=14>" + m_teaMakerRef.FindIngredient((GameObject) item.Key).GetComponent<Ingredient>().m_name + " x " + item.Value.ToString() + "</size>";
                    }
                }
            }
            else if (constantHit.transform.gameObject.GetComponent<Tea>() && m_teaMakerRef.hasClickedBrew)
            {
                m_tooltipObject.SetActive(true);
                if (m_teaMakerRef.m_teaModel.GetComponent<Tea>() != null && !string.IsNullOrEmpty(m_teaMakerRef.m_teaModel.GetComponent<Tea>().m_name))
                {
                    m_tooltipText.text = "<size=16>A Cup of <b>" + m_teaMakerRef.m_teaModel.GetComponent<Tea>().m_name + "</b> Tea\n</size>";
                }
                else
                {
                    m_tooltipText.text = "<size=16>A Regular Cup of Tea</size>";
                }

                foreach (var item in m_teaMakerRef.m_container)
                {
                    if (item.Value != 0)
                    {
                        m_tooltipText.text +=
                            "<size=14>\n" + m_teaMakerRef.FindIngredient((GameObject)item.Key).GetComponent<Ingredient>().m_name + " x " + item.Value.ToString() + "</size>";
                    }
                }
            }
            else if (constantHit.transform.CompareTag("BrewTea") && !m_teaMakerRef.hasClickedBrew && m_teaMakerRef.Total > 0)
            {
                m_tooltipObject.SetActive(true);
                m_tooltipText.text = "<size=24><b>Click to brew </b></size>";
            }
            else
            {
                m_tooltipObject.SetActive(false);
            }
        }
    }

    private string GetQuip()
    {
        List<string> quips = new List<string>();

        Recipe recipe = FindRecipe(GameObject.FindGameObjectWithTag("Character").GetComponent<Character>().characterScriptableObject.favouriteRecipe);

        //foreach (var item in m_teaMakerRef.m_teaModel.GetComponent<Tea>().ingredients)
        foreach (var item in recipe.m_ingredients)
        {
            string quip;
            if (item.Value != 0)
            {
                if (m_teaMakerRef.m_teaModel.GetComponent<Tea>().ingredients.ContainsKey(item.Key) )
                {
                    if (m_teaMakerRef.m_teaModel.GetComponent<Tea>().ingredients[item.Key] != item.Value)
                    {
                        if (m_teaMakerRef.m_teaModel.GetComponent<Tea>().ingredients[item.Key] > item.Value)
                        {
                            // TOO MUCH of ...
                            quip = DetermineQuip((item.Key as Ingredient).m_type);
                            quip += "_TooMuch";
                            quips.Add(quip);
                        }
                        if (m_teaMakerRef.m_teaModel.GetComponent<Tea>().ingredients[item.Key] < item.Value)
                        {
                            // TOO LITTLE of...
                            quip = DetermineQuip((item.Key as Ingredient).m_type);
                            quip += "_TooLittle";
                            quips.Add(quip);
                        }
                    }
                }
                else
                {
                    // Does not contain ingredient at all so: TOO LITTLE OF ...
                    //GameObject ing = ((GameObject)GameObject.FindObjectOfType(item.Key.GetType())).transform.parent.gameObject;
                    GameObject ing = FindIngredient(item.Key.name);
                    if (ing != null)
                    {
                        quip = DetermineQuip(ing.GetComponent<Ingredient>().m_type);
                        quip += "_TooLittle";
                        quips.Add(quip);
                    }
                }
            }
        }

        // Got the list of available discrepencies between fav recipe and tea
        // Now return the first one

        if (quips.Count > 0)
        {
            return quips[0];
        }
        else
        {
            Debug.LogError("No quips found");
            return "";
        }
    }

    private string DetermineQuip(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.Breezeleaf:
                {
                    return "Cool";
                }
            case IngredientType.Cindershard:
                {
                    return "Warm";
                }
            case IngredientType.Ebonstraw:
                {
                    return "Sweet";
                }
            case IngredientType.GlowLime:
                {
                    return "Sour";
                }
            case IngredientType.HeartOfRose:
                {
                    return "Aromatic";
                }
            case IngredientType.PurpleCrystal:
                {
                    return "Aromatic";
                }
            default:
                return "NONE";
        }
    }


    private Recipe FindRecipe(string name)
    {

        foreach (var item in m_teaMakerRef.m_recipeListRef.m_recipes)
        {
            if (item.m_name == name)
            {
                return item;
            }
        }

        return null;
    }

    public void ExitBook()
    {
        m_recipeBookUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        m_lookingAtBook = false;
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

        if (IsQuips())
        {
            messageTextbox.text += "[";
        }
       
        // Generate choices if a choice, otherwise display the message
        if (handler.currentMessageInfo.Type == QD_NodeType.Message)
        {
            QD_Message message = handler.GetMessage();
            speakerNameTextbox.text = message.SpeakerName;
            messageTextbox.text += message.MessageText;
            messageTextbox.gameObject.SetActive(true);

        }
        else if (handler.currentMessageInfo.Type == QD_NodeType.Choice)
        {
            speakerNameTextbox.text = "Player";
        }

        if (IsQuips())
        {
            messageTextbox.text += "]";
        }
    }
    public void Next(int choice = -1)
    {
        if (m_dialogueFinished)
            return;

        // Go to the next message
        handler.NextMessage(choice);
        messageList.Add(handler.GetMessage());

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
    // Escape
    public void Quit(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        m_quit = value > 0;
        //Debug.Log("Right detected");
    }
}