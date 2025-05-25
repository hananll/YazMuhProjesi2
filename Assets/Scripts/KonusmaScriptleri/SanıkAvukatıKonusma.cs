using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// KonusmaMetniData ScriptableObject'i ayrı bir dosyada tanımlı olduğu için,
// bu script içinde tekrar TANIMLANMAMALI.

public class SanıkAvukatıKonusma : MonoBehaviour
{
    // Sanık Avukatı'nın ağız animasyon kontrol script'ine referans
    public KarakterKonusmaAnimasyonu sanikAvukatiAnimationControl;

    public GameObject sanikAvukatiKonusmaPanel;
    public TextMeshProUGUI sanikAvukatiMetinText;
    public TextMeshProUGUI sanikAvukatiAdiText;
    public Button sanikAvukatiKapatButon;
    public Button sanikAvukatiButon; // Kendi diyalogunu başlatan buton
    public Button sanikAvukatiDevamEtButon;

    // --- SES İÇİN EKLENEN KISIM ---
    public AudioSource sanikAvukatiAudioSource; // Sanık Avukatı'nın seslerini çalacak AudioSource
    // --- SES İÇİN EKLENEN KISIM SONU ---

    public float harfHiz = 0.05f;

    // Diyalog verileri (KonusmaMetniData ScriptableObject'ini kullanıyor)
    public List<KonusmaMetniData> sanikAvukatiDiyalogMetinleri;

    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;
    // Hangi karakterin animasyonunun o an aktif olduğunu tutmak için
    private KarakterKonusmaAnimasyonu currentSpeakerAnimationControl = null;

    public SanikveHakimKonusma sanikveHakimKonusma;
    public SavciKonusma savciKonusma;
    public MagdurveHakimKonusma magdurKonusma;
    public MagdurAvukatiKonusma magdurAvukatiKonusma;


    void Start()
    {
        sanikAvukatiKonusmaPanel.SetActive(false); // Paneli başlangıçta kapat

        // Buton olaylarını dinlemeye başla
        sanikAvukatiButon.onClick.AddListener(KonusmayiBaslat);
        sanikAvukatiKapatButon.onClick.AddListener(KonusmayiBitir);
        sanikAvukatiDevamEtButon.onClick.AddListener(SonrakiMetniGoster);

        sanikAvukatiDevamEtButon.gameObject.SetActive(false); // Devam et butonunu başlangıçta kapat

        // Diyalog metinleri kontrolü
        if (sanikAvukatiDiyalogMetinleri == null || sanikAvukatiDiyalogMetinleri.Count == 0)
        {
            Debug.LogError("Sanık Avukatı Diyalog Metinleri Inspector'da atanmamış veya boş!");
        }
    }

    void KonusmayiBaslat()
    {
        if (sanikAvukatiDiyalogMetinleri == null || sanikAvukatiDiyalogMetinleri.Count == 0)
        {
            Debug.LogWarning("Sanık Avukatı diyalog listesi boş veya atanmamış!");
            return;
        }

        // Önceki konuşanın animasyonunu durdur (eğer varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        // --- SES İÇİN EKLENEN KISIM: Konuşma başladığında önceki sesi durdur ---
        if (sanikAvukatiAudioSource != null && sanikAvukatiAudioSource.isPlaying)
        {
            sanikAvukatiAudioSource.Stop();
        }
        // --- SES İÇİN EKLENEN KISIM SONU ---

        sanikAvukatiKonusmaPanel.SetActive(true); // Paneli aktif et

        ButonlariKapat(); // Diğer butonları kapat (kendi butonunu kapatma hariç)

        mevcutMetinIndex = 0;
        MevcutMetniGoster();
    }

    void SonrakiMetniGoster()
    {
        // Eğer metin hala yazılıyorsa, yazmayı tamamla
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;

            KonusmaMetniData currentData = sanikAvukatiDiyalogMetinleri[mevcutMetinIndex];
            sanikAvukatiMetinText.text = currentData.metin; // Metni anında tamamla

            // Hızlı tıklamada metin yazımı tamamlandığında ağzı kapat
            if (currentSpeakerAnimationControl != null)
            {
                sanikAvukatiAnimationControl.KonusmayiBitir();
            }

            // --- SES İÇİN EKLENEN KISIM: Hızlı tıklamada ses hala çalıyorsa durdur ---
            if (sanikAvukatiAudioSource != null && sanikAvukatiAudioSource.isPlaying)
            {
                sanikAvukatiAudioSource.Stop();
            }
            // --- SES İÇİN EKLENEN KISIM SONU ---

            // Butonları tekrar aktif et (metin yazma tamamlandı)
            sanikAvukatiDevamEtButon.gameObject.SetActive(true);
            sanikAvukatiDevamEtButon.interactable = true;
            return; // Metin tamamlandığı için sonraki adıma geçmeden çık
        }

        mevcutMetinIndex++;
        if (mevcutMetinIndex < sanikAvukatiDiyalogMetinleri.Count)
        {
            MevcutMetniGoster();
        }
        else
        {
            KonusmayiBitir(); // Tüm metinler gösterildiyse konuşmayı bitir.
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
        // Önceki konuşanın animasyonunu durdur (eğer varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        // --- SES İÇİN EKLENEN KISIM: Yeni konuşma başladığında önceki sesi durdur ---
        if (sanikAvukatiAudioSource != null && sanikAvukatiAudioSource.isPlaying)
        {
            sanikAvukatiAudioSource.Stop();
        }
        // --- SES İÇİN EKLENEN KISIM SONU ---

        sanikAvukatiDevamEtButon.gameObject.SetActive(false); // Metine Başladığımızda devam et butonuna tıklayamamayı sağladık.

        if (mevcutMetinIndex < sanikAvukatiDiyalogMetinleri.Count)
        {
            KonusmaMetniData mevcutKonusma = sanikAvukatiDiyalogMetinleri[mevcutMetinIndex];

            sanikAvukatiMetinText.text = ""; // Metin alanını temizle
            sanikAvukatiAdiText.text = mevcutKonusma.konusmaciAdi;

            // Sanık Avukatı animasyonunu başlat
            if (sanikAvukatiAnimationControl != null)
            {
                sanikAvukatiAnimationControl.KonusmayaBasla();
                currentSpeakerAnimationControl = sanikAvukatiAnimationControl; // Aktif konuşan yap
            }

            // --- SES İÇİN EKLENEN KISIM: İlgili ses dosyasını çal ---
            if (sanikAvukatiAudioSource != null && mevcutKonusma.sesDosyasi != null)
            {
                sanikAvukatiAudioSource.clip = mevcutKonusma.sesDosyasi;
                sanikAvukatiAudioSource.Play();
            }
            else if (mevcutKonusma.sesDosyasi == null)
            {
                Debug.LogWarning($"Sanık Avukatı diyaloğu için ses dosyası atanmamış: Index {mevcutMetinIndex}");
            }
            // --- SES İÇİN EKLENEN KISIM SONU ---

            // MetniHarfHarfGoster'e devam butonu parametresini ekledik
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(sanikAvukatiMetinText, mevcutKonusma.metin, sanikAvukatiDevamEtButon));
        }
        else
        {
            KonusmayiBitir(); // Tüm metinler gösterildiyse konuşmayı bitir.
        }
    }

    IEnumerator MetniHarfHarfGoster(TextMeshProUGUI metinAlani, string metin, Button devamButonu) // Button parametresi eklendi
    {
        int harfSayisi = 0;
        metinAlani.text = "";
        while (harfSayisi < metin.Length)
        {
            harfSayisi++;
            metinAlani.text = metin.Substring(0, harfSayisi);
            yield return new WaitForSeconds(harfHiz);
        }

        // Metin yazımı tamamlandığında konuşma animasyonunu durdur
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
        }

        // --- SES İÇİN EKLENEN KISIM: Metin animasyonu bittiğinde sesi durdur ---
        // Eğer ses hala çalıyorsa (yani metinden daha uzunsa), durdur.
        if (sanikAvukatiAudioSource != null && sanikAvukatiAudioSource.isPlaying)
        {
            sanikAvukatiAudioSource.Stop();
        }
        // --- SES İÇİN EKLENEN KISIM SONU ---

        devamButonu.gameObject.SetActive(true); // Metin tamamlandıktan sonra devam et butonu aktif
        devamButonu.interactable = true;
    }

    void KonusmayiBitir()
    {
        // Aktif olan son konuşanın animasyonunu durdur
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        // --- SES İÇİN EKLENEN KISIM: Diyalog bittiğinde sesi durdur ---
        if (sanikAvukatiAudioSource != null && sanikAvukatiAudioSource.isPlaying)
        {
            sanikAvukatiAudioSource.Stop();
        }
        // --- SES İÇİN EKLENEN KISIM SONU ---

        sanikAvukatiKonusmaPanel.SetActive(false); // Paneli kapat
        sanikAvukatiDevamEtButon.gameObject.SetActive(false); // Devam et butonunu kapat

        ButonlariAc(); // Tüm butonları aç
        mevcutMetinIndex = 0; // İndeksi sıfırla
    }

    void ButonlariKapat()
    {
        // sanikAvukatiButon.interactable = false; // Bu satır yorum satırı yapıldı / silindi

        // Diğer diyalog butonlarını kapat
        if (sanikveHakimKonusma != null && sanikveHakimKonusma.mikrofonButon != null)
            sanikveHakimKonusma.mikrofonButon.interactable = false;
        if (savciKonusma != null && savciKonusma.savciButon != null)
            savciKonusma.savciButon.interactable = false;
        if (magdurKonusma != null && magdurKonusma.magdurButon != null)
            magdurKonusma.magdurButon.interactable = false;
        if (magdurAvukatiKonusma != null && magdurAvukatiKonusma.magdurAvukatiButon != null)
            magdurAvukatiKonusma.magdurAvukatiButon.interactable = false;
    }

    void ButonlariAc()
    {
        // Kendi butonunu aç (eğer ButonlariKapat içinde kapatılmadıysa bu satır gereksiz olabilir ama güvenli bırakıldı)
        sanikAvukatiButon.interactable = true;

        // Diğer diyalog butonlarını aç
        if (sanikveHakimKonusma != null && sanikveHakimKonusma.mikrofonButon != null)
            sanikveHakimKonusma.mikrofonButon.interactable = true;
        if (savciKonusma != null && savciKonusma.savciButon != null)
            savciKonusma.savciButon.interactable = true;
        if (magdurKonusma != null && magdurKonusma.magdurButon != null)
            magdurKonusma.magdurButon.interactable = true;
        if (magdurAvukatiKonusma != null && magdurAvukatiKonusma.magdurAvukatiButon != null)
            magdurAvukatiKonusma.magdurAvukatiButon.interactable = true;
    }
}