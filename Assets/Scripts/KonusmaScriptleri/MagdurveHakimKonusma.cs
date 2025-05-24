using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// KonusmaMetniData ScriptableObject'i ayrý bir dosyada tanýmlý olduðu için,
// bu script içinde tekrar TANIMLANMAMALI.

public class MagdurveHakimKonusma : MonoBehaviour
{
    // Maðdur'un aðýz animasyon kontrol script'ine referans
    public KarakterKonusmaAnimasyonu magdurAnimationControl;
    // Hakim için de animasyon kontrolü ekleyecekseniz (eðer ona da aðýz yapýsý kurduysanýz):
    // public KarakterKonusmaAnimasyonu hakimAnimationControl;


    // Hakim Konuþma UI Elemanlarý
    public GameObject hakimKonusmaPanel;
    public TextMeshProUGUI hakimMetinText;
    public TextMeshProUGUI hakimAdiText;
    public Button hakimDevamEtButon;

    // Maðdur Konuþma UI Elemanlarý
    public GameObject magdurKonusmaPanel;
    public TextMeshProUGUI magdurMetinText;
    public TextMeshProUGUI magdurAdiText;
    public Button magdurDevamEtButon;
    public Button magdurKapatButon;
    public Button magdurButon; // Maðdur'un kendi diyalogunu baþlatan buton

    public float harfHiz = 0.05f;

    // Diyalog verileri (KonusmaMetniData ScriptableObject'ini kullanýyor)
    public List<KonusmaMetniData> magdurDiyalogMetinleri;

    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;
    private KarakterKonusmaAnimasyonu currentSpeakerAnimationControl = null; // Aktif konuþanýn animasyonunu yönetmek için

    // Diðer diyalog yöneticilerine referanslar (buton etkileþimi için)
    public SanikveHakimKonusma sanikveHakimKonusma;
    public SavciKonusma savciKonusma;
    public SanýkAvukatýKonusma sanýkAvukatýKonusma;
    public MagdurAvukatiKonusma magdurAvukatiKonusma;

    void Start()
    {
        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

        magdurButon.onClick.AddListener(KonusmayiBaslat);
        magdurKapatButon.onClick.AddListener(KonusmayiBitir);
        magdurDevamEtButon.onClick.AddListener(SonrakiMetniGoster);
        hakimDevamEtButon.onClick.AddListener(SonrakiMetniGoster);

        magdurDevamEtButon.gameObject.SetActive(false);
        hakimDevamEtButon.gameObject.SetActive(false);
    }

    void KonusmayiBaslat()
    {
        if (magdurDiyalogMetinleri == null || magdurDiyalogMetinleri.Count == 0)
        {
            Debug.LogWarning("Maðdur diyalog listesi boþ veya atanmamýþ!");
            return;
        }

        // Önceki konuþanýn animasyonunu durdur (eðer varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

        // magdurButon'un interactable durumu bu fonksiyonda artýk deðiþtirilmeyecek.
        // Sadece diðer butonlarý kapatacaðýz.
        ButonlariKapat();

        mevcutMetinIndex = 0;
        MevcutMetniGoster();
    }

    void SonrakiMetniGoster()
    {
        // Eðer metin hala yazýlýyorsa, yazmayý tamamla
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;

            // Metni anýnda tamamla
            KonusmaMetniData currentData = magdurDiyalogMetinleri[mevcutMetinIndex];
            if (currentData.konusmaciAdi == "Maðdur" && magdurMetinText != null) magdurMetinText.text = currentData.metin;
            else if (currentData.konusmaciAdi == "Hakim" && hakimMetinText != null) hakimMetinText.text = currentData.metin;

            // Hýzlý týklamada metin yazýmý tamamlandýðýnda aðzý kapat
            if (currentSpeakerAnimationControl != null)
            {
                currentSpeakerAnimationControl.KonusmayiBitir();
            }

            // Butonlarý tekrar aktif et (metin yazma tamamlandý)
            magdurDevamEtButon.gameObject.SetActive(true);
            magdurDevamEtButon.interactable = true;
            hakimDevamEtButon.gameObject.SetActive(true);
            hakimDevamEtButon.interactable = true;
            return;
        }

        mevcutMetinIndex++;
        if (mevcutMetinIndex < magdurDiyalogMetinleri.Count)
        {
            MevcutMetniGoster();
        }
        else
        {
            KonusmayiBitir();
        }
    }

    void MevcutMetniGoster()
    {
        // Önceki metin animasyonunu durdur
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;
        }
        // Önceki konuþanýn animasyonunu durdur (eðer varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);
        magdurDevamEtButon.gameObject.SetActive(false);
        hakimDevamEtButon.gameObject.SetActive(false);

        KonusmaMetniData mevcutKonusma = magdurDiyalogMetinleri[mevcutMetinIndex];

        if (mevcutKonusma.konusmaciAdi == "Maðdur")
        {
            magdurKonusmaPanel.SetActive(true);
            magdurAdiText.text = mevcutKonusma.konusmaciAdi;

            // Maðdur animasyonunu baþlat
            if (magdurAnimationControl != null)
            {
                magdurAnimationControl.KonusmayaBasla();
                currentSpeakerAnimationControl = magdurAnimationControl; // Aktif konuþan yap
            }
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(magdurMetinText, mevcutKonusma.metin, magdurDevamEtButon));
        }
        else if (mevcutKonusma.konusmaciAdi == "Hakim")
        {
            hakimKonusmaPanel.SetActive(true);
            hakimAdiText.text = mevcutKonusma.konusmaciAdi;
            // Hakim için animasyon kontrolü varsa burada baþlatýn
            // if (hakimAnimationControl != null)
            // {
            //     hakimAnimationControl.KonusmayaBasla();
            //     currentSpeakerAnimationControl = hakimAnimationControl;
            // }
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(hakimMetinText, mevcutKonusma.metin, hakimDevamEtButon));
        }
        else
        {
            Debug.LogWarning("Bilinmeyen konuþmacý: " + mevcutKonusma.konusmaciAdi + " (Maðdur Diyalog Scriptinde) Ýndeks: " + mevcutMetinIndex);
        }
    }

    IEnumerator MetniHarfHarfGoster(TextMeshProUGUI metinAlani, string metin, Button devamButonu)
    {
        int harfSayisi = 0;
        metinAlani.text = "";

        while (harfSayisi < metin.Length)
        {
            harfSayisi++;
            metinAlani.text = metin.Substring(0, harfSayisi);
            yield return new WaitForSeconds(harfHiz);
        }

        // Metin yazýmý tamamlandýðýnda konuþma animasyonunu durdur
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
        }

        devamButonu.gameObject.SetActive(true);
        devamButonu.interactable = true;
    }

    void KonusmayiBitir()
    {
        // Aktif olan son konuþanýn animasyonunu durdur
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

        ButonlariAc(); // magdurButon da burada tekrar aktif edilecek.
        mevcutMetinIndex = 0;
        magdurDevamEtButon.gameObject.SetActive(false);
        hakimDevamEtButon.gameObject.SetActive(false);
    }

    void ButonlariKapat()
    {
        // magdurButon.interactable = false; // BU SATIR YORUM SATIRI YAPILDI / SÝLÝNDÝ

        if (sanikveHakimKonusma != null && sanikveHakimKonusma.mikrofonButon != null)
            sanikveHakimKonusma.mikrofonButon.interactable = false;
        if (savciKonusma != null && savciKonusma.savciButon != null)
            savciKonusma.savciButon.interactable = false;
        if (sanýkAvukatýKonusma != null && sanýkAvukatýKonusma.sanikAvukatiButon != null)
            sanýkAvukatýKonusma.sanikAvukatiButon.interactable = false;
        if (magdurAvukatiKonusma != null && magdurAvukatiKonusma.magdurAvukatiButon != null)
            magdurAvukatiKonusma.magdurAvukatiButon.interactable = false;
    }

    void ButonlariAc()
    {
        // magdurButon'u daima true olacaðý için sadece diðerlerini açýyoruz.
        // Ancak bu fonksiyon KonusmayiBitir() içinde çaðrýldýðý için
        // ve magdurButon'un durumunu Start'ta veya baþka bir yerde yönetiyorsanýz
        // burada deðiþtirmemek daha iyi olabilir.
        // Eðer Start'ta magdurButon'u kapattýysanýz, burada tekrar açmanýz gerekir.
        // Þu anki koda göre magdurButon'u her zaman true tuttuðumuz için bu satýr gereksiz olabilir.
        magdurButon.interactable = true;

        if (sanikveHakimKonusma != null && sanikveHakimKonusma.mikrofonButon != null)
            sanikveHakimKonusma.mikrofonButon.interactable = true;
        if (savciKonusma != null && savciKonusma.savciButon != null)
            savciKonusma.savciButon.interactable = true;
        if (sanýkAvukatýKonusma != null && sanýkAvukatýKonusma.sanikAvukatiButon != null)
            sanýkAvukatýKonusma.sanikAvukatiButon.interactable = true;
        if (magdurAvukatiKonusma != null && magdurAvukatiKonusma.magdurAvukatiButon != null)
            magdurAvukatiKonusma.magdurAvukatiButon.interactable = true;
    }
}