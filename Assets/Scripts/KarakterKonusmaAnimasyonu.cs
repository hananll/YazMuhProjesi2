using UnityEngine;
using UnityEngine.UI; // Image bile�eni i�in
using System.Collections; // Coroutine i�in

public class KarakterKonusmaAnimasyonu : MonoBehaviour
{
    public Sprite mouthOpenSprite;   // A�z�n a��k hali resmi
    public Sprite mouthClosedSprite; // A�z�n kapal� hali resmi

    private Image mouthImage;
    private Coroutine talkingCoroutine;

    // A��z�n a��k/kapal� kalma s�resi
    public float mouthToggleSpeed = 0.1f;

    void Awake()
    {
        mouthImage = GetComponent<Image>();
        if (mouthImage == null)
        {
            Debug.LogError(gameObject.name + ": Image bile�eni bulunamad�! KarakterKonusmaAnimasyonu script'ini bir UI Image objesine atay�n.");
            enabled = false;
        }
        if (mouthOpenSprite == null || mouthClosedSprite == null)
        {
            Debug.LogError(gameObject.name + ": A��z a��k/kapal� resimleri Inspector'da atanmad�!");
            enabled = false;
        }
    }

    public void KonusmayaBasla()
    {
        // E�er zaten konu�ma coroutine'i �al���yorsa durdur
        if (talkingCoroutine != null)
        {
            StopCoroutine(talkingCoroutine);
        }
        // Yeni konu�ma coroutine'ini ba�lat
        talkingCoroutine = StartCoroutine(AnimateMouthDirectly());
    }

    public void KonusmayiBitir()
    {
        // Konu�ma coroutine'ini durdur
        if (talkingCoroutine != null)
        {
            StopCoroutine(talkingCoroutine);
            talkingCoroutine = null;
        }
        // A�z� kapal� resme getir
        if (mouthImage != null && mouthClosedSprite != null)
        {
            mouthImage.sprite = mouthClosedSprite;
        }
    }

    IEnumerator AnimateMouthDirectly()
    {
        // Sonsuz d�ng�de a�z� a��p kapat
        while (true)
        {
            if (mouthImage != null && mouthOpenSprite != null)
            {
                mouthImage.sprite = mouthOpenSprite; // A�z� a�
            }
            yield return new WaitForSeconds(mouthToggleSpeed);

            if (mouthImage != null && mouthClosedSprite != null)
            {
                mouthImage.sprite = mouthClosedSprite; // A�z� kapat
            }
            yield return new WaitForSeconds(mouthToggleSpeed);
        }
    }
}