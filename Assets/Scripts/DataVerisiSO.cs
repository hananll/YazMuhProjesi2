using UnityEngine;

[CreateAssetMenu(fileName = "YeniDavaVerisi", menuName = "Oyun Verisi/Dava Verisi")]
public class DavaVerisiSO : ScriptableObject
{
    public DavaDetayVerisi detaylar; 
    // Bu script dava n�n ideal karar�n� sisteme verir ve bu ideal kararlara g�re bar�n de�i�imi ama�lan�r.
}