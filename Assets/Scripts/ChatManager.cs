using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq; // List.All() ve List.Any() için gerekli

public class ChatManager : MonoBehaviour
{
    [Header("UI Referanslarý")]
    public TextMeshProUGUI chatHistoryText;
    public TMP_InputField chatInputField;
    public GameObject chatPanel;

    [Header("Chat Verisi")]
    [Tooltip("Soru-cevap verilerini içeren ScriptableObject.")]
    public ChatResponsesData chatData;

    

    void Awake()
    {
        if (chatData == null) //Scriotable object atanýp atanmama kontrolü yaptým.
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
            chatHistoryText.text = "Hukuk Danýþmaný: Vermeniz gereken ceza 30.000 TL Adli Para Cezasýydý. Baþka sorularýnýz varsa yardýmcý olabilirim.";
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
        chatHistoryText.text = currentDisplay + "\n\n" + "<color=#cc0000>Hukuk Danýþmaný:</color> " + botResponse;

        chatInputField.text = "";
        chatInputField.ActivateInputField();
    }

    private string GetBotResponse(string playerInput)
    {
        string cleanInput = playerInput.ToLower().Trim();

        
        ChatResponsesData.ChatEntry bestMatch = null;
        int maxMatchedKeywords = 0; // Kaç tane anahtar kelimenin eþleþtiðine baktým.
        int bestMatchLength = 0;    // Eþleþen kelimelerin uzunluðuna da baktýk(en çok eþlenen daha iyi bir cevaptýr)

        if (chatData != null && chatData.entries != null)
        {
            foreach (var entry in chatData.entries)
            {
                int currentMatchedKeywords = 0;
                string matchedPhrase = "";

                // Her bir triggerKeyword'ü kontrol ettim
                foreach (string keyword in entry.triggerKeywords)
                {
                    string cleanKeyword = keyword.ToLower().Trim();
                    if (cleanInput.Contains(cleanKeyword))
                    {
                        currentMatchedKeywords++;
                        if (cleanKeyword.Length > matchedPhrase.Length) // En uzun eþleþen kelime öbeðini buldum
                        {
                            matchedPhrase = cleanKeyword;
                        }
                    }
                }

                // Eþleþen anahtar kelime sayýsý sýfýrdan büyükse ve daha iyi bir eþleþmeyse kaydet
                if (currentMatchedKeywords > 0)
                {
                    // Daha fazla kelime eþleþiyorsa VEYA ayný sayýda kelime eþleþip daha uzun bir anahtar kelimeyse
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
            // Hiçbir eþleþme bulunamazsa varsayýlan bir cevap olarak bunu tanýmladým.
            return "Sorunuzu tam olarak anlayamadým. Türk Ceza Kanunu'na veya karara iliþkin daha spesifik bir konuda bilgi vermeye çalýþabilirim.";
        }
    }
}