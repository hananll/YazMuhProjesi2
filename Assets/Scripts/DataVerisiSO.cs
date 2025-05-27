using UnityEngine;

[CreateAssetMenu(fileName = "YeniDavaVerisi", menuName = "Oyun Verisi/Dava Verisi")]
public class DavaVerisiSO : ScriptableObject
{
    public DavaDetayVerisi detaylar; 
    // Bu script dava nýn ideal kararýný sisteme verir ve bu ideal kararlara göre barýn deðiþimi amaçlanýr.
}