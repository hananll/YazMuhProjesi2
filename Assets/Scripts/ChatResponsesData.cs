using UnityEngine;
using System.Collections.Generic; // List i�in gerekli

[CreateAssetMenu(fileName = "ChatResponsesData", menuName = "Chat/Chat Responses Data")]
public class ChatResponsesData : ScriptableObject
{
    [System.Serializable] // Unity Editor'da g�r�nmesini sa�lamak i�in tan�mlad�m.
    public class ChatEntry
    {
        
        public List<string> triggerKeywords = new List<string>(); // Tek string yerine bir liste yapt�m.

        [TextArea(3, 10)] 
        public string responseText;
    }

    // T�m soru-cevap �iftlerini tutacak bir liste tan�mlamas� yapt�m. 
    public List<ChatEntry> entries = new List<ChatEntry>();

    
}