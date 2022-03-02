using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumDialogue;
using TMPro;
using UnityEngine.InputSystem;

public class TestDialogueInterface : MonoBehaviour
{
    public QD_DialogueHandler handler;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI convoName;
    public TextMeshProUGUI messageText;
    private bool ended = false;
    private float m_inputDelay = 0.1f;
    private float m_inputTimer = 1.0f;

    private void Awake()
    {
        handler.SetConversation("The Test");
        SetText();
    }

    private void Update()
    {
        m_inputTimer = m_inputTimer <= 0 ? 0 : m_inputTimer - Time.deltaTime;

        // Don't do anything if the conversation is over
        if (ended)
            return;

        // Check if the space key is pressed and the current message is not a choice
        if (handler.currentMessageInfo.Type == QD_NodeType.Message && Mouse.current.leftButton.IsActuated() && m_inputTimer <= 0)
        {
            m_inputTimer = m_inputDelay;
            Next();
        }
    }


    private void SetText()
    {
        // Clear everything
        speakerName.text = "";
        convoName.text = "";
        messageText.gameObject.SetActive(false);
        messageText.text = "";

        // If at the end, don't do anything
        if (ended)
            return;

        // Generate choices if a choice, otherwise display the message
        if (handler.currentMessageInfo.Type == QD_NodeType.Message)
        {
            QD_Message message = handler.GetMessage();
            speakerName.text = message.SpeakerName;
            messageText.text = message.MessageText;
            QD_Conversation convo = handler.currentConversation;
            convoName.text = convo.Name;
            messageText.gameObject.SetActive(true);

        }
    }
    public void Next(int choice = -1)
    {
        Debug.Log("NEXT");

        if (ended)
            return;

        // Go to the next message
        handler.NextMessage(choice);
        // Set the new text
        SetText();
        // End if there is no next message
        if (handler.currentMessageInfo.ID < 0)
            ended = true;
    }
}
