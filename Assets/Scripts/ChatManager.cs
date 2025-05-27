using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq; // List.All() ve List.Any() i�in gerekli

public class ChatManager : MonoBehaviour
{
    [Header("UI Referanslar�")]
    public TextMeshProUGUI chatHistoryText;
    public TMP_InputField chatInputField;
    public GameObject chatPanel;

    [Header("Chat Verisi")]
    [Tooltip("Soru-cevap verilerini i�eren ScriptableObject.")]
    public ChatResponsesData chatData;

    

    void Awake()
    {
        if (chatData == null) //Scriotable object atan�p atanmama kontrol� yapt�m.
        {
            Debug.LogError("ChatResponsesData ScriptableObject is not assigned to ChatManager! Please assign it in the Inspector.", this);
        }

        if (chatPanel != null)
        {
            chatPanel.SetActive(false);
        }
    }

    public void OpenChatPanel()
    {
        if (chatPanel != null)
        {
            chatPanel.SetActive(true);
            chatHistoryText.text = "Hukuk Dan��man�: Vermeniz gereken ceza 30.000 TL Adli Para Cezas�yd�. Ba�ka sorular�n�z varsa yard�mc� olabilirim.";
            chatInputField.ActivateInputField();
        }
    }

    public void SendChatMessage()
    {
        string playerMessage = chatInputField.text;

        if (string.IsNullOrWhiteSpace(playerMessage))
        {
            return;
        }

        chatHistoryText.text = "<color=#007acc>Sen:</color> " + playerMessage; 

        string botResponse = GetBotResponse(playerMessage);
        string currentDisplay = chatHistoryText.text;
        chatHistoryText.text = currentDisplay + "\n\n" + "<color=#cc0000>Hukuk Dan��man�:</color> " + botResponse;

        chatInputField.text = "";
        chatInputField.ActivateInputField();
    }

    private string GetBotResponse(string playerInput)
    {
        string cleanInput = playerInput.ToLower().Trim();

        
        ChatResponsesData.ChatEntry bestMatch = null;
        int maxMatchedKeywords = 0; // Ka� tane anahtar kelimenin e�le�ti�ine bakt�m.
        int bestMatchLength = 0;    // E�le�en kelimelerin uzunlu�una da bakt�k(en �ok e�lenen daha iyi bir cevapt�r)

        if (chatData != null && chatData.entries != null)
        {
            foreach (var entry in chatData.entries)
            {
                int currentMatchedKeywords = 0;
                string matchedPhrase = "";

                // Her bir triggerKeyword'� kontrol ettim
                foreach (string keyword in entry.triggerKeywords)
                {
                    string cleanKeyword = keyword.ToLower().Trim();
                    if (cleanInput.Contains(cleanKeyword))
                    {
                        currentMatchedKeywords++;
                        if (cleanKeyword.Length > matchedPhrase.Length) // En uzun e�le�en kelime �be�ini buldum
                        {
                            matchedPhrase = cleanKeyword;
                        }
                    }
                }

                // E�le�en anahtar kelime say�s� s�f�rdan b�y�kse ve daha iyi bir e�le�meyse kaydet
                if (currentMatchedKeywords > 0)
                {
                    // Daha fazla kelime e�le�iyorsa VEYA ayn� say�da kelime e�le�ip daha uzun bir anahtar kelimeyse
                    if (currentMatchedKeywords > maxMatchedKeywords ||
                       (currentMatchedKeywords == maxMatchedKeywords && matchedPhrase.Length > bestMatchLength))
                    {
                        maxMatchedKeywords = currentMatchedKeywords;
                        bestMatchLength = matchedPhrase.Length;
                        bestMatch = entry;
                    }
                }
            }
        }

        if (bestMatch != null)
        {
            return bestMatch.responseText;
        }
        else
        {
            // Hi�bir e�le�me bulunamazsa varsay�lan bir cevap olarak bunu tan�mlad�m.
            return "Sorunuzu tam olarak anlayamad�m. T�rk Ceza Kanunu'na veya karara ili�kin daha spesifik bir konuda bilgi vermeye �al��abilirim.";
        }
    }
}