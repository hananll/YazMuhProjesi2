// Script Ad�: BarYoneticisi.cs
using UnityEngine;
using UnityEngine.UI;

public class BarYoneticisi : MonoBehaviour
{
    [Header("Slider Referanslar�")]
    public Slider kamuoyuGuvenSlider;
    public Slider hukukGuvenSlider;

    private float maksimumDeger = 100f;
    private float minimumDeger = 0f;

    public static BarYoneticisi Ornek { get; private set; }

    public static float kayitliKamuoyuDegeri = 100f;
    public static float kayitliHukukDegeri = 100f;

    void Awake()
    {
        // Singleton deseni, sahnede sadece bir �rnek kalacak �ekilde
        if (Ornek == null) Ornek = this;
        else if (Ornek != this) Destroy(gameObject);
    }

    void Start()
    {
        // Sliderlar statik kay�tlardan ba�lat�l�r
        SetKamuoyuGuvenDegeri(kayitliKamuoyuDegeri);
        SetHukukGuvenDegeri(kayitliHukukDegeri);

        // Slider referanslar� kontrol�
        if (kamuoyuGuvenSlider == null)
            Debug.LogError("HATA: BarYoneticisi - Kamuoyu G�ven Slider'� ATANMAMI�!");
        if (hukukGuvenSlider == null)
            Debug.LogError("HATA: BarYoneticisi - Hukuk G�ven Slider'� ATANMAMI�!");

        BarlariBaslangicDurumunaGetir();
    }

    public void BarlariBaslangicDurumunaGetir()
    {
        if (kamuoyuGuvenSlider != null)
        {
            kamuoyuGuvenSlider.minValue = minimumDeger;
            kamuoyuGuvenSlider.maxValue = maksimumDeger;
            kamuoyuGuvenSlider.value = maksimumDeger;
            kamuoyuGuvenSlider.interactable = false;
        }
        if (hukukGuvenSlider != null)
        {
            hukukGuvenSlider.minValue = minimumDeger;
            hukukGuvenSlider.maxValue = maksimumDeger;
            hukukGuvenSlider.value = maksimumDeger;
            hukukGuvenSlider.interactable = false;
        }
    }

    public void NihaiKararaGoreBarlariGuncelle(
        int oyuncununVerdigiHapisGun, float oyuncununVerdigiParaCezasi, bool oyuncuBeraatMiVerdi,
        int davaninIdealiHapisGun, float davaninIdealiParaCezasi, bool davaninIdealiBeraatMiydi)
    {
        float kamuoyuDegisimMiktari = 0f;
        float hukukDegisimMiktari = 0f;

        if (oyuncuBeraatMiVerdi)
        {
            if (davaninIdealiBeraatMiydi)
            {
                kamuoyuDegisimMiktari = 5f;
                hukukDegisimMiktari = 10f;
            }
            else
            {
                kamuoyuDegisimMiktari = -35f;
                hukukDegisimMiktari = -50f;
            }
        }
        else
        {
            if (davaninIdealiBeraatMiydi)
            {
                kamuoyuDegisimMiktari = -40f;
                hukukDegisimMiktari = -60f;
            }
            else
            {
                float hapisFarkiEtkisi = 0f;
                if (davaninIdealiHapisGun > 0)
                {
                    float farkOrani = Mathf.Abs(oyuncununVerdigiHapisGun - davaninIdealiHapisGun) / (float)davaninIdealiHapisGun;
                    hapisFarkiEtkisi = Mathf.Clamp01(farkOrani) * -20f;
                }
                else if (oyuncununVerdigiHapisGun > 0)
                {
                    hapisFarkiEtkisi = -25f;
                }

                float paraFarkiEtkisi = 0f;
                if (davaninIdealiParaCezasi > 1.0f)
                {
                    float farkOrani = Mathf.Abs(oyuncununVerdigiParaCezasi - davaninIdealiParaCezasi) / davaninIdealiParaCezasi;
                    paraFarkiEtkisi = Mathf.Clamp01(farkOrani) * -20f;
                }
                else if (oyuncununVerdigiParaCezasi > 0)
                {
                    paraFarkiEtkisi = -25f;
                }

                kamuoyuDegisimMiktari = hapisFarkiEtkisi * 0.6f + paraFarkiEtkisi * 0.4f;
                hukukDegisimMiktari = hapisFarkiEtkisi * 0.4f + paraFarkiEtkisi * 0.6f;

                bool hapisYakin = Mathf.Abs(oyuncununVerdigiHapisGun - davaninIdealiHapisGun) <= Mathf.Max(10, davaninIdealiHapisGun * 0.1f);
                bool paraYakin = Mathf.Abs(oyuncununVerdigiParaCezasi - davaninIdealiParaCezasi) <= Mathf.Max(100, davaninIdealiParaCezasi * 0.1f);
                if (hapisYakin && paraYakin)
                {
                    kamuoyuDegisimMiktari += Random.Range(1f, 3f);
                    hukukDegisimMiktari += Random.Range(3f, 6f);
                }
            }
        }

        DegistirKamuoyuGuven(kamuoyuDegisimMiktari);
        DegistirHukukGuven(hukukDegisimMiktari);

        // �stersen log a�abilirsin
        // Debug.Log($"Nihai karar sonras� g�ncelleme -> Kamuoyu: {kamuoyuGuvenSlider.value}, Hukuk: {hukukGuvenSlider.value}");
    }

    public void SetKamuoyuGuvenDegeri(float yeniDeger)
    {
        float clamped = Mathf.Clamp(yeniDeger, minimumDeger, maksimumDeger);
        if (kamuoyuGuvenSlider != null)
        {
            kamuoyuGuvenSlider.value = clamped;
        }
        kayitliKamuoyuDegeri = clamped;
    }

    public void DegistirKamuoyuGuven(float miktar)
    {
        if (kamuoyuGuvenSlider != null)
        {
            SetKamuoyuGuvenDegeri(kamuoyuGuvenSlider.value + miktar);
        }
    }

    public void SetHukukGuvenDegeri(float yeniDeger)
    {
        float clamped = Mathf.Clamp(yeniDeger, minimumDeger, maksimumDeger);
        if (hukukGuvenSlider != null)
        {
            hukukGuvenSlider.value = clamped;
        }
        kayitliHukukDegeri = clamped;
    }

    public void DegistirHukukGuven(float miktar)
    {
        if (hukukGuvenSlider != null)
        {
            SetHukukGuvenDegeri(hukukGuvenSlider.value + miktar);
        }
    }
}
