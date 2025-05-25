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

    // --- SES ÝÇÝN EKLENEN KISIM ---
    public AudioSource diyalogAudioSource; // Maðdur ve Hakim'in seslerini çalacak AudioSource
    // --- SES ÝÇÝN EKLENEN KISIM SONU ---

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

        // Diyalog metinleri kontrolü
        if (magdurDiyalogMetinleri == null || magdurDiyalogMetinleri.Count == 0)
        {
            Debug.LogError("Maðdur Diyalog Metinleri Inspector'da atanmamýþ veya boþ!");
        }
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

        // --- SES ÝÇÝN EKLENEN KISIM: Konuþma baþladýðýnda önceki sesi durdur ---
        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }
        // --- SES ÝÇÝN EKLENEN KISIM SONU ---

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

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

            // --- SES ÝÇÝN EKLENEN KISIM: Hýzlý týklamada ses hala çalýyorsa durdur ---
            if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
            {
                diyalogAudioSource.Stop();
            }
            // --- SES ÝÇÝN EKLENEN KISIM SONU ---

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

        // --- SES ÝÇÝN EKLENEN KISIM: Yeni konuþma baþladýðýnda önceki sesi durdur ---
        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }
        // --- SES ÝÇÝN EKLENEN KISIM SONU ---

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

            // --- SES ÝÇÝN EKLENEN KISIM: Ýlgili ses dosyasýný çal ---
            if (diyalogAudioSource != null && mevcutKonusma.sesDosyasi != null)
            {
                diyalogAudioSource.clip = mevcutKonusma.sesDosyasi;
                diyalogAudioSource.Play();
            }
            else if (mevcutKonusma.sesDosyasi == null)
            {
                Debug.LogWarning($"Maðdur diyaloðu için ses dosyasý atanmamýþ: Index {mevcutMetinIndex}");
            }
            // --- SES ÝÇÝN EKLENEN KISIM SONU ---

            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(magdurMetinText, mevcutKonusma.metin, magdurDevamEtButon));
        }
        else if (mevcutKonusma.konusmaciAdi == "Hakim")
        {
            hakimKonusmaPanel.SetActive(true);
            hakimAdiText.text = mevcutKonusma.konusmaciAdi;

            // Hakim için animasyon kontrolü varsa burada baþlatýn
            // if (hakimAnimationControl != null)
            // {
            //    hakimAnimationControl.KonusmayaBasla();
            //    currentSpeakerAnimationControl = hakimAnimationControl;
            // }

            // --- SES ÝÇÝN EKLENEN KISIM: Ýlgili ses dosyasýný çal ---
            if (diyalogAudioSource != null && mevcutKonusma.sesDosyasi != null)
            {
                diyalogAudioSource.clip = mevcutKonusma.sesDosyasi;
                diyalogAudioSource.Play();
            }
            else if (mevcutKonusma.sesDosyasi == null)
            {
                Debug.LogWarning($"Hakim diyaloðu için ses dosyasý atanmamýþ: Index {mevcutMetinIndex}");
            }
            // --- SES ÝÇÝN EKLENEN KISIM SONU ---

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

        // --- SES ÝÇÝN EKLENEN KISIM: Metin animasyonu bittiðinde sesi durdur ---
        // Eðer ses hala çalýyorsa (yani metinden daha uzunsa), durdur.
        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }
        // --- SES ÝÇÝN EKLENEN KISIM SONU ---

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

        // --- SES ÝÇÝN EKLENEN KISIM: Diyalog bittiðinde sesi durdur ---
        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }
        // --- SES ÝÇÝN EKLENEN KISIM SONU ---

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

        ButonlariAc(); // magdurButon da burada tekrar aktif edilecek.
        mevcutMetinIndex = 0;
        magdurDevamEtButon.gameObject.SetActive(false);
        hakimDevamEtButon.gameObject.SetActive(false);
    }

    void ButonlariKapat()
    {
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