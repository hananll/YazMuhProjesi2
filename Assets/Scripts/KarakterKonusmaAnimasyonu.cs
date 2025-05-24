using UnityEngine;
using UnityEngine.UI; // Image bileþeni için
using System.Collections; // Coroutine için

public class KarakterKonusmaAnimasyonu : MonoBehaviour
{
    public Sprite mouthOpenSprite;   // Aðzýn açýk hali resmi
    public Sprite mouthClosedSprite; // Aðzýn kapalý hali resmi

    private Image mouthImage;
    private Coroutine talkingCoroutine;

    // Aðýzýn açýk/kapalý kalma süresi
    public float mouthToggleSpeed = 0.1f;

    void Awake()
    {
        mouthImage = GetComponent<Image>();
        if (mouthImage == null)
        {
            Debug.LogError(gameObject.name + ": Image bileþeni bulunamadý! KarakterKonusmaAnimasyonu script'ini bir UI Image objesine atayýn.");
            enabled = false;
        }
        if (mouthOpenSprite == null || mouthClosedSprite == null)
        {
            Debug.LogError(gameObject.name + ": Aðýz açýk/kapalý resimleri Inspector'da atanmadý!");
            enabled = false;
        }
    }

    public void KonusmayaBasla()
    {
        // Eðer zaten konuþma coroutine'i çalýþýyorsa durdur
        if (talkingCoroutine != null)
        {
            StopCoroutine(talkingCoroutine);
        }
        // Yeni konuþma coroutine'ini baþlat
        talkingCoroutine = StartCoroutine(AnimateMouthDirectly());
    }

    public void KonusmayiBitir()
    {
        // Konuþma coroutine'ini durdur
        if (talkingCoroutine != null)
        {
            StopCoroutine(talkingCoroutine);
            talkingCoroutine = null;
        }
        // Aðzý kapalý resme getir
        if (mouthImage != null && mouthClosedSprite != null)
        {
            mouthImage.sprite = mouthClosedSprite;
        }
    }

    IEnumerator AnimateMouthDirectly()
    {
        // Sonsuz döngüde aðzý açýp kapat
        while (true)
        {
            if (mouthImage != null && mouthOpenSprite != null)
            {
                mouthImage.sprite = mouthOpenSprite; // Aðzý aç
            }
            yield return new WaitForSeconds(mouthToggleSpeed);

            if (mouthImage != null && mouthClosedSprite != null)
            {
                mouthImage.sprite = mouthClosedSprite; // Aðzý kapat
            }
            yield return new WaitForSeconds(mouthToggleSpeed);
        }
    }
}