using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

// KonusmaMetniData ScriptableObject'i ayrý bir dosyada tanýmlý olduðu için,
// bu script içinde tekrar TANIMLANMAMALI.

public class MagdurAvukatiKonusma : MonoBehaviour
{
    // Maðdur Avukatý'nýn aðýz animasyon kontrol script'ine referans
    public KarakterKonusmaAnimasyonu magdurAvukatiAnimationControl;

    public GameObject magdurAvukatiKonusmaPanel;
    public TextMeshProUGUI magdurAvukatiMetinText;
    public TextMeshProUGUI magdurAvukatiAdiText;
    public Button magdurAvukatiKapatButon;
    public Button magdurAvukatiButon; // Kendi diyalogunu baþlatan buton
    public Button magdurAvukatiDevamEtButon;

    // --- SES ÝÇÝN EKLENEN KISIM ---
    public AudioSource magdurAvukatiAudioSource; // Maðdur Avukatý'nýn seslerini çalacak AudioSource
    // --- SES ÝÇÝN EKLENEN KISIM SONU ---

    public float harfHiz = 0.05f;

    // Diyalog verileri (KonusmaMetniData ScriptableObject'ini kullanýyor)
    public List<KonusmaMetniData> magdurAvukatiDiyalogMetinleri;

    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;
    // Hangi karakterin animasyonunun o an aktif olduðunu tutmak için
    private KarakterKonusmaAnimasyonu currentSpeakerAnimationControl = null;

    public SanikveHakimKonusma sanikveHakimKonusma;
    public SavciKonusma savciKonusma;
    public SanýkAvukatýKonusma sanýkAvukatýKonusma;
    public MagdurveHakimKonusma magdurKonusma;
    public TokmakSistemiYoneticisi tokmakSistemiYoneticisi;

    void Start()
    {
        magdurAvukatiKonusmaPanel.SetActive(false); // Paneli baþlangýçta kapat

        // Buton olaylarýný dinlemeye baþla
        magdurAvukatiButon.onClick.AddListener(KonusmayiBaslat);
        magdurAvukatiKapatButon.onClick.AddListener(KonusmayiBitir);
        magdurAvukatiDevamEtButon.onClick.AddListener(SonrakiMetniGoster);

        magdurAvukatiDevamEtButon.gameObject.SetActive(false); // Devam et butonunu baþlangýçta kapat

        // Diyalog metinleri kontrolü
        if (magdurAvukatiDiyalogMetinleri == null || magdurAvukatiDiyalogMetinleri.Count == 0)
        {
            Debug.LogError("Maðdur Avukatý Diyalog Metinleri Inspector'da atanmamýþ veya boþ!");
        }
    }

    void KonusmayiBaslat()
    {
        if (magdurAvukatiDiyalogMetinleri == null || magdurAvukatiDiyalogMetinleri.Count == 0)
        {
            Debug.LogWarning("Maðdur Avukatý diyalog listesi boþ veya atanmamýþ!");
            return;
        }

        // Önceki konuþanýn animasyonunu durdur (eðer varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        // --- SES ÝÇÝN EKLENEN KISIM: Konuþma baþladýðýnda önceki sesi durdur ---
        if (magdurAvukatiAudioSource != null && magdurAvukatiAudioSource.isPlaying)
        {
            magdurAvukatiAudioSource.Stop();
        }
        // --- SES ÝÇÝN EKLENEN KISIM SONU ---

        magdurAvukatiKonusmaPanel.SetActive(true); // Paneli aktif et
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

            KonusmaMetniData currentData = magdurAvukatiDiyalogMetinleri[mevcutMetinIndex];
            magdurAvukatiMetinText.text = currentData.metin; // Metni anýnda tamamla

            // Hýzlý týklamada metin yazýmý tamamlandýðýnda aðzý kapat
            if (currentSpeakerAnimationControl != null)
            {
                currentSpeakerAnimationControl.KonusmayiBitir();
            }

            // --- SES ÝÇÝN EKLENEN KISIM: Hýzlý týklamada ses hala çalýyorsa durdur ---
            if (magdurAvukatiAudioSource != null && magdurAvukatiAudioSource.isPlaying)
            {
                magdurAvukatiAudioSource.Stop();
            }
            // --- SES ÝÇÝN EKLENEN KISIM SONU ---

            // Butonlarý tekrar aktif et (metin yazma tamamlandý)
            magdurAvukatiDevamEtButon.gameObject.SetActive(true);
            magdurAvukatiDevamEtButon.interactable = true;
            return; // Metin tamamlandýðý için sonraki adýma geçmeden çýk
        }

        mevcutMetinIndex++;
        if (mevcutMetinIndex < magdurAvukatiDiyalogMetinleri.Count)
        {
            MevcutMetniGoster();
        }
        else
        {
            KonusmayiBitir(); // Tüm metinler gösterildiyse konuþmayý bitir.
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
        if (magdurAvukatiAudioSource != null && magdurAvukatiAudioSource.isPlaying)
        {
            magdurAvukatiAudioSource.Stop();
        }
        // --- SES ÝÇÝN EKLENEN KISIM SONU ---

        magdurAvukatiDevamEtButon.gameObject.SetActive(false); // Metine baþladýðýmýzda devam et butonuna týklayamamayý saðladýk.

        if (mevcutMetinIndex < magdurAvukatiDiyalogMetinleri.Count)
        {
            KonusmaMetniData mevcutKonusma = magdurAvukatiDiyalogMetinleri[mevcutMetinIndex];

            magdurAvukatiMetinText.text = ""; // Metin alanýný temizle
            magdurAvukatiAdiText.text = mevcutKonusma.konusmaciAdi;

            // Maðdur Avukatý animasyonunu baþlat
            if (magdurAvukatiAnimationControl != null)
            {
                magdurAvukatiAnimationControl.KonusmayaBasla();
                currentSpeakerAnimationControl = magdurAvukatiAnimationControl; // Aktif konuþan yap
            }

            // --- SES ÝÇÝN EKLENEN KISIM: Ýlgili ses dosyasýný çal ---
            if (magdurAvukatiAudioSource != null && mevcutKonusma.sesDosyasi != null)
            {
                magdurAvukatiAudioSource.clip = mevcutKonusma.sesDosyasi;
                magdurAvukatiAudioSource.Play();
            }
            else if (mevcutKonusma.sesDosyasi == null)
            {
                Debug.LogWarning($"Maðdur Avukatý diyaloðu için ses dosyasý atanmamýþ: Index {mevcutMetinIndex}");
            }
            // --- SES ÝÇÝN EKLENEN KISIM SONU ---

            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(magdurAvukatiMetinText, mevcutKonusma.metin, magdurAvukatiDevamEtButon));
        }
        else
        {
            KonusmayiBitir(); // Tüm metinler gösterildiyse konuþmayý bitir.
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
        if (magdurAvukatiAudioSource != null && magdurAvukatiAudioSource.isPlaying)
        {
            magdurAvukatiAudioSource.Stop();
        }
        // --- SES ÝÇÝN EKLENEN KISIM SONU ---

        devamButonu.gameObject.SetActive(true); // Metin tamamlandýktan sonra devam et butonu aktif
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
        if (magdurAvukatiAudioSource != null && magdurAvukatiAudioSource.isPlaying)
        {
            magdurAvukatiAudioSource.Stop();
        }
        // --- SES ÝÇÝN EKLENEN KISIM SONU ---

        magdurAvukatiKonusmaPanel.SetActive(false); // Paneli kapat
        magdurAvukatiDevamEtButon.gameObject.SetActive(false); // Devam et butonunu kapat

        ButonlariAc(); // Tüm butonlarý aç
        mevcutMetinIndex = 0; // Ýndeksi sýfýrla
    }

    void ButonlariKapat()
    {
        // Diðer diyalog butonlarýný kapat
        sanikveHakimKonusma.mikrofonButon.interactable = false;
        savciKonusma.savciButon.interactable = false;
        sanýkAvukatýKonusma.sanikAvukatiButon.interactable = false;
        magdurKonusma.magdurButon.interactable = false;
        tokmakSistemiYoneticisi.anaTokmakButonu.interactable = false;
    }

    void ButonlariAc()
    {
        // Kendi butonunu aç (eðer ButonlariKapat içinde kapatýlmadýysa bu satýr gereksiz olabilir ama güvenli býrakýldý)
        magdurAvukatiButon.interactable = true;

        // Diðer diyalog butonlarýný aç
        sanikveHakimKonusma.mikrofonButon.interactable = true;
        savciKonusma.savciButon.interactable = true;
        sanýkAvukatýKonusma.sanikAvukatiButon.interactable = true;
        magdurKonusma.magdurButon.interactable = true;
        tokmakSistemiYoneticisi.anaTokmakButonu.interactable = true;
    }
}