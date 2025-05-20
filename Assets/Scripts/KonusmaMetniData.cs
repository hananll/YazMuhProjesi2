using UnityEngine;

[CreateAssetMenu(fileName = "KonusmaMetni", menuName = "Veri/Konusma Metni")]
public class KonusmaMetniData : ScriptableObject
{
    [TextArea(3, 10)] // Inspector'da daha b�y�k bir metin alan� sa�lar
    public string metin;
    public string konusmaciAdi;
    public Sprite konusmaciPortresi; // �ste�e ba�l�
    // �leride metinle ilgili ba�ka veriler ekleyebilirsiniz (�rne�in, bir sonraki metnin ID'si, ses dosyas� vb.).
}