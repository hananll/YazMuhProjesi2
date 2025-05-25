using UnityEngine;

[CreateAssetMenu(fileName = "KonusmaMetni", menuName = "Veri/Konusma Metni")]
public class KonusmaMetniData : ScriptableObject
{
    [TextArea(3, 10)] 
    public string metin;
    public string konusmaciAdi;
    public Sprite konusmaciPortresi;
    public AudioClip sesDosyasi;
}