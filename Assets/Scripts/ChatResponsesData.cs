using UnityEngine;
using System.Collections.Generic; // List için gerekli

[CreateAssetMenu(fileName = "ChatResponsesData", menuName = "Chat/Chat Responses Data")]
public class ChatResponsesData : ScriptableObject
{
    [System.Serializable] // Unity Editor'da görünmesini saðlamak için tanýmladým.
    public class ChatEntry
    {
        
        public List<string> triggerKeywords = new List<string>(); // Tek string yerine bir liste yaptým.

        [TextArea(3, 10)] 
        public string responseText;
    }

    // Tüm soru-cevap çiftlerini tutacak bir liste tanýmlamasý yaptým. 
    public List<ChatEntry> entries = new List<ChatEntry>();

    
}