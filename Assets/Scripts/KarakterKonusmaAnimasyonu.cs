using UnityEngine;
using UnityEngine.UI; 
using System.Collections; // Bunu ekledim çünkü karakterin aðzý bir yerde duruyor.

public class KarakterKonusmaAnimasyonu : MonoBehaviour
{
    public Sprite mouthOpenSprite;  
    public Sprite mouthClosedSprite;

    private Image mouthImage;
    private Coroutine talkingCoroutine;

   // Aðzýn açýk/kapalý kalma süresi, Emre eðer beðenmezsen inspectordan ayarlayabilirsin. Bu script i her karakterin child ýn daki gameobject e ekledim.
    public float mouthToggleSpeed = 0.1f;

    void Awake()
    {
        mouthImage = GetComponent<Image>(); // Ýnspectordan get compenent metodu ile Image bileþeinini alýyorum kodda kullandým çünkü.
        if (mouthImage == null)
        {
            enabled = false;
        }
        if (mouthOpenSprite == null || mouthClosedSprite == null)
        {
            enabled = false;
        }
    }

    public void KonusmayaBasla()
    {
        if (talkingCoroutine != null)
        {
            StopCoroutine(talkingCoroutine);
        }
        talkingCoroutine = StartCoroutine(AnimateMouthDirectly());
    }

    public void KonusmayiBitir()
    {
        if (talkingCoroutine != null)
        {
            StopCoroutine(talkingCoroutine);
            talkingCoroutine = null;
        }
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
                mouthImage.sprite = mouthOpenSprite; 
            }
            yield return new WaitForSeconds(mouthToggleSpeed);

            if (mouthImage != null && mouthClosedSprite != null)
            {
                mouthImage.sprite = mouthClosedSprite; 
            }
            yield return new WaitForSeconds(mouthToggleSpeed); // Emre burdaki yield e takýlma metodun parça parça çalýþmasýný saðladým, biraz gecikme daha hoþ durdu burayý elleme.
        }
    }
}