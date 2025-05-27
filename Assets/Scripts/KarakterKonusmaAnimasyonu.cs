using UnityEngine;
using UnityEngine.UI; 
using System.Collections; // Bunu ekledim ��nk� karakterin a�z� bir yerde duruyor.

public class KarakterKonusmaAnimasyonu : MonoBehaviour
{
    public Sprite mouthOpenSprite;  
    public Sprite mouthClosedSprite;

    private Image mouthImage;
    private Coroutine talkingCoroutine;

   // A�z�n a��k/kapal� kalma s�resi, Emre e�er be�enmezsen inspectordan ayarlayabilirsin. Bu script i her karakterin child �n daki gameobject e ekledim.
    public float mouthToggleSpeed = 0.1f;

    void Awake()
    {
        mouthImage = GetComponent<Image>(); // �nspectordan get compenent metodu ile Image bile�einini al�yorum kodda kulland�m ��nk�.
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
        // Sonsuz d�ng�de a�z� a��p kapat
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
            yield return new WaitForSeconds(mouthToggleSpeed); // Emre burdaki yield e tak�lma metodun par�a par�a �al��mas�n� sa�lad�m, biraz gecikme daha ho� durdu buray� elleme.
        }
    }
}