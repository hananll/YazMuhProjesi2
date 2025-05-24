using UnityEngine;

public class OzelImlecYoneticisi : MonoBehaviour
{
    [Tooltip("Kullanılacak özel imleç görseli (Texture2D)")]
    public Texture2D imlecGorseli;

    [Tooltip("İmlecin 'tıklama noktasının' görselin sol üst köşesine göre X ve Y ofseti")]
    public Vector2 aktifNokta = Vector2.zero;

    [Tooltip("İmleç modu: Donanımsal veya Yazılımsal")]
    public CursorMode imlecModu = CursorMode.Auto;

    void Start()
    {
        
        OzelImleciAyarla();
    }

    public void OzelImleciAyarla()
    {
        if (imlecGorseli != null)
        {
            Cursor.SetCursor(imlecGorseli, aktifNokta, imlecModu);
        }
        else
        {
            Debug.LogError("OzelImlecYoneticisi: İmleç Görseli (imlecGorseli) Inspector'dan atanmamış!");
        }
    }

    void OnApplicationQuit()
    {
        Cursor.SetCursor(null, Vector2.zero, imlecModu);
    }
}