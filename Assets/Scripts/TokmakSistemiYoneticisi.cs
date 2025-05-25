using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Eğer KararGirisPaneliYoneticisi içinde TextMeshPro kullanılıyorsa
using System.Collections; // Coroutine'ler için gerekli

public class TokmakSistemiYoneticisi : MonoBehaviour
{
    [Header("Ana Açılış Butonu")]
    [Tooltip("Oyun sahnesindeki ana 'Tokmak Vur' veya 'Karar Aşamasına Geç' butonu.")]
    public Button anaTokmakButonu;

    [Header("Paneller")]
    [Tooltip("Tokmağa tıklandığında açılacak olan ana seçenekler paneli")]
    public GameObject tokmakKararPanel;
    [Tooltip("Erteleme seçeneklerini veya mesajını gösteren panel")]
    public GameObject erteleMesajPanel;

    [Header("Tokmak Karar Paneli İçindeki Butonlar")]
    [Tooltip("'tokmakKararPanel' içindeki 'Davayı Ertele' butonu")]
    public Button secenekErteleButonu;
    [Tooltip("'tokmakKararPanel' içindeki 'Davayı Düşür' butonu")]
    public Button secenekDusurButonu;
    [Tooltip("'tokmakKararPanel' içindeki 'Karar Ver' (giriş panelini açacak) butonu")]
    public Button secenekKararVerButonu;

    [Header("Erteleme Paneli Butonları")]
    public Button ertelemeKapatButton;
    public Button ertelemeTutukluDevamKararButonu;
    public Button ertelemeTutuksuzYargilamaKararButonu;

    // --- SES AYARLARI ---
    [Header("Ses Ayarları")]
    public AudioSource tokmakAudioSource;     // Tokmak sesini çalacak AudioSource
    public AudioClip tokmakVurmaSoundClip;    // Tokmak vurma ses dosyanız
    public AudioClip gecisTokmakSoundClip;    // Sahne geçişlerinde çalacak tokmak sesi

    [Header("Karar Giriş Paneli Entegrasyonu")]
    public KararGirisPaneliYoneticisi kararGirisPaneli; // KararGirisPaneliYoneticisi'ne referans

    [Header("Bağlantılı Sistemler")]
    public BarYoneticisi barYoneticisi;

    [Header("Aktif Dava Verisi")]
    public DavaVerisiSO aktifDava;

    // --- SAHNE GEÇİŞ AYARLARI ---
    [Header("Sahne Geçiş Ayarları")]
    [Tooltip("Sahne geçişi öncesi beklenecek süre.")]
    public float sahneGecisGecikmesi = 1.5f;

    void Awake()
    {
        // Tokmak sesleri için AudioSource'u kontrol et veya ekle
        if (tokmakAudioSource == null)
        {
            tokmakAudioSource = GetComponent<AudioSource>();
            if (tokmakAudioSource == null)
            {
                tokmakAudioSource = gameObject.AddComponent<AudioSource>();
            }
            tokmakAudioSource.playOnAwake = false;
            tokmakAudioSource.loop = false;
        }

        // Panelleri başlangıçta gizle
        if (tokmakKararPanel != null) tokmakKararPanel.SetActive(false);
        if (erteleMesajPanel != null) erteleMesajPanel.SetActive(false);
    }

    void OnEnable()
    {
        // Ana tokmak butonunun tıklama olayını bağla
        if (anaTokmakButonu != null)
        {
            anaTokmakButonu.onClick.AddListener(TokmakSecenekPaneliAc);
        }

        // --- BUTON TIKLAMA OLAYLARINI BURADA BAĞLIYORUZ ---
        // "Davayı Ertele" butonu
        if (secenekErteleButonu != null)
        {
            secenekErteleButonu.onClick.AddListener(ErteleButonunaTiklandi);
        }

        // "Davayı Düşür" butonu (Sahne geçişi içeren)
        if (secenekDusurButonu != null)
        {
            secenekDusurButonu.onClick.AddListener(() =>
            {
                DusurButonunaTiklandi(); // Davayı düşürme mantığını çağır
                SahneyiGecikmeliYukle("HakiminOdasi", gecisTokmakSoundClip, sahneGecisGecikmesi); // Sahne yüklemeyi başlat
            });
        }

        // "Karar Ver" butonu
        if (secenekKararVerButonu != null)
        {
            secenekKararVerButonu.onClick.AddListener(KararVerSecenegiTiklandi);
        }

        // Erteleme Paneli Butonları (Sahne geçişi içeren)
        if (ertelemeKapatButton != null)
        {
            ertelemeKapatButton.onClick.AddListener(ErtelemeMesajPaneliKapat);
        }

        if (ertelemeTutukluDevamKararButonu != null)
        {
            ertelemeTutukluDevamKararButonu.onClick.AddListener(() =>
            {
                ErtelemeKarari_TutukluDevam(); // Tutuklu devam mantığını çağır
                SahneyiGecikmeliYukle("HakiminOdasi", gecisTokmakSoundClip, sahneGecisGecikmesi); // Sahne yüklemeyi başlat
            });
        }

        if (ertelemeTutuksuzYargilamaKararButonu != null)
        {
            ertelemeTutuksuzYargilamaKararButonu.onClick.AddListener(() =>
            {
                ErtelemeKarari_TutuksuzYargilama(); // Tutuksuz yargılama mantığını çağır
                SahneyiGecikmeliYukle("HakiminOdasi", gecisTokmakSoundClip, sahneGecisGecikmesi); // Sahne yüklemeyi başlat
            });
        }

        // Hükmü Ver ve Beraat Ver butonlarının listener'ları artık KararGirisPaneliYoneticisi içinde yönetiliyor.
        // TokmakSistemiYoneticisi burada listener eklemeyecek.
    }

    void OnDisable()
    {
        // Butonların tıklama olaylarını çöz (Bellek sızıntısını önlemek için önemli!)
        // Eğer bu scriptin butonlara eklediği tek listener'lar bunlarsa, RemoveAllListeners() kullanılabilir.
        // Aksi takdirde, her lambda fonksiyonu için ayrı bir Action referansı tutup RemoveListener ile kaldırmak daha güvenli olur.
        if (anaTokmakButonu != null) anaTokmakButonu.onClick.RemoveListener(TokmakSecenekPaneliAc);
        if (secenekErteleButonu != null) secenekErteleButonu.onClick.RemoveListener(ErteleButonunaTiklandi);
        if (secenekKararVerButonu != null) secenekKararVerButonu.onClick.RemoveListener(KararVerSecenegiTiklandi);
        if (ertelemeKapatButton != null) ertelemeKapatButton.onClick.RemoveListener(ErtelemeMesajPaneliKapat);

        // Sahne geçişi yapan butonlar için:
        if (secenekDusurButonu != null) secenekDusurButonu.onClick.RemoveAllListeners();
        if (ertelemeTutukluDevamKararButonu != null) ertelemeTutukluDevamKararButonu.onClick.RemoveAllListeners();
        if (ertelemeTutuksuzYargilamaKararButonu != null) ertelemeTutuksuzYargilamaKararButonu.onClick.RemoveAllListeners();

        // Hükmü Ver ve Beraat Ver butonlarının listener kaldırma KararGirisPaneliYoneticisi içinde yönetiliyor.
    }

    // Aktif dava verisini ayarlar
    public void AktifDavayiAyarla(DavaVerisiSO yeniDava)
    {
        aktifDava = yeniDava;
    }

    // Tokmak seçenek panelini açar ve arka plan UI etkileşimini ayarlar
    public void TokmakSecenekPaneliAc()
    {
        if (aktifDava == null || tokmakKararPanel == null)
        {
            Debug.LogWarning("Aktif Dava verisi veya Tokmak Karar Paneli atanmamış!");
            return;
        }
        tokmakKararPanel.SetActive(true);
        PlayTokmakSound(); // Panel açıldığında tokmak sesi çalabilir
        ArkaPlanVeDigerUIAyarla(true);
    }

    // "Davayı Ertele" butonuna tıklandığında çağrılır, erteleme mesaj panelini açar
    public void ErteleButonunaTiklandi()
    {
        if (erteleMesajPanel == null || tokmakKararPanel == null) return;
        erteleMesajPanel.SetActive(true);
        tokmakKararPanel.SetActive(false);
    }

    // Erteleme kararı: Tutuklu devam etme
    public void ErtelemeKarari_TutukluDevam()
    {
        if (barYoneticisi == null || aktifDava == null) return;
        barYoneticisi.DegistirKamuoyuGuven(aktifDava.detaylar.ertelemeTutuklu_KamuoyuEtkisi);
        barYoneticisi.DegistirHukukGuven(aktifDava.detaylar.ertelemeTutuklu_HukukEtkisi);
        KapatVeGenelDurumuNormaleDonder(erteleMesajPanel);
        // Sahne yükleme SahneyiGecikmeliYukle metodu tarafından zaten yapılıyor.
    }

    // Erteleme kararı: Tutuksuz yargılama
    public void ErtelemeKarari_TutuksuzYargilama()
    {
        if (barYoneticisi == null || aktifDava == null) return;
        barYoneticisi.DegistirKamuoyuGuven(aktifDava.detaylar.ertelemeTutuksuz_KamuoyuEtkisi);
        barYoneticisi.DegistirHukukGuven(aktifDava.detaylar.ertelemeTutuksuz_HukukEtkisi);
        KapatVeGenelDurumuNormaleDonder(erteleMesajPanel);
        // Sahne yükleme SahneyiGecikmeliYukle metodu tarafından zaten yapılıyor.
    }

    // "Davayı Düşür" butonuna tıklandığında çağrılır
    public void DusurButonunaTiklandi()
    {
        if (barYoneticisi == null || aktifDava == null) return;
        if (aktifDava.detaylar.davayiDusurmekGecerliMi)
        {
            barYoneticisi.DegistirKamuoyuGuven(aktifDava.detaylar.davayiDusurGecerli_KamuoyuEtkisi);
            barYoneticisi.DegistirHukukGuven(aktifDava.detaylar.davayiDusurGecerli_HukukEtkisi);
        }
        else
        {
            barYoneticisi.DegistirKamuoyuGuven(aktifDava.detaylar.davayiDusurGecersiz_KamuoyuEtkisi);
            barYoneticisi.DegistirHukukGuven(aktifDava.detaylar.davayiDusurGecersiz_HukukEtkisi);
        }
        KapatVeGenelDurumuNormaleDonder(tokmakKararPanel);
        // Sahne yükleme SahneyiGecikmeliYukle metodu tarafından zaten yapılıyor.
    }

    // "Karar Ver" seçeneği tıklandığında çağrılır, karar giriş panelini gösterir
    public void KararVerSecenegiTiklandi()
    {
        if (kararGirisPaneli == null || aktifDava == null)
        {
            Debug.LogWarning("Karar Giriş Paneli veya Aktif Dava verisi atanmamış!");
            return;
        }
        kararGirisPaneli.KararPaneliniGoster(
            aktifDava.detaylar.idealHapisGun,
            aktifDava.detaylar.idealParaCezasi,
            aktifDava.detaylar.idealBeraatOlmalydi
        );
        if (tokmakKararPanel != null)
            tokmakKararPanel.SetActive(false); // Tokmak karar panelini kapat
    }

    // Erteleme mesaj panelini kapatır ve ana tokmak karar panelini geri açar
    public void ErtelemeMesajPaneliKapat()
    {
        if (erteleMesajPanel != null) erteleMesajPanel.SetActive(false);
        if (tokmakKararPanel != null) tokmakKararPanel.SetActive(true);
    }

    // Belirtilen paneli kapatır ve genel durumu normale döndürür (arka plan UI etkileşimini geri açar)
    void KapatVeGenelDurumuNormaleDonder(GameObject kapatilacakPanel)
    {
        if (kapatilacakPanel != null) kapatilacakPanel.SetActive(false);
        ArkaPlanVeDigerUIAyarla(false);
    }

    // Arka planı ve diğer UI etkileşimlerini ayarlar (bulanıklık, etkileşim vb.)
    void ArkaPlanVeDigerUIAyarla(bool modalAcikMi)
    {
        // Arka planı bulanıklaştırma veya diğer UI'ı devre dışı bırakma işlevi
    }

    // Tokmak karar panelini doğrudan kapatma (eğer dışarıdan çağrılacaksa)
    public void PaneliKapat()
    {
        tokmakKararPanel.SetActive(false);
    }

    // Tokmak sesini çalan fonksiyon (dışarıdan KararGirisPaneliYoneticisi tarafından çağrılacak)
    public void PlayTokmakSound()
    {
        if (tokmakAudioSource != null && tokmakVurmaSoundClip != null)
        {
            tokmakAudioSource.PlayOneShot(tokmakVurmaSoundClip);
        }
        else
        {
            Debug.LogWarning("Tokmak AudioSource veya Tokmak Vurma Sound Clip atanmamış!");
        }
    }

    // --- SAHNE GEÇİŞİ İÇİN YENİ EKLENEN METOT ---
    // Gecikmeli sahne yüklemeyi başlatan Coroutine'i çağırır (dışarıdan KararGirisPaneliYoneticisi tarafından çağrılacak)
    public void SahneyiGecikmeliYukle(string sahneAdi, AudioClip gecisSesi, float gecikme)
    {
        // Coroutine'i bu GameObject üzerinde başlat (bu GameObject her zaman aktif olmalı)
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("SahneyiGecikmeliYukle çağrıldı ama GameObject '" + gameObject.name + "' aktif değil! Coroutine başlatılamadı. Lütfen 'TokmakSistemiYoneticisi'nin her zaman aktif bir GameObject üzerinde olduğundan emin olun.", this);
            // Aktif değilse işlemi durdur ve sahneyi direkt yükle (fallback)
            if (!string.IsNullOrEmpty(sahneAdi))
            {
                SceneManager.LoadScene(sahneAdi);
            }
            return; // Metodu burada sonlandır
        }

        StopAllCoroutines(); // Önceki Coroutine'leri durdur (bir butona art arda tıklanırsa)
        StartCoroutine(SahneYukleBekle(sahneAdi, gecisSesi, gecikme));
    }

    // Sahneyi gecikmeli olarak yükleyen Coroutine
    private IEnumerator SahneYukleBekle(string sahneAdi, AudioClip sesKlibi, float gecikme)
    {
        if (sesKlibi != null && tokmakAudioSource != null)
        {
            tokmakAudioSource.clip = sesKlibi;
            tokmakAudioSource.Play();
        }

        yield return new WaitForSeconds(gecikme);

        if (!string.IsNullOrEmpty(sahneAdi))
        {
            SceneManager.LoadScene(sahneAdi);
        }
        else
        {
            Debug.LogWarning("Yüklenecek sahne adı belirtilmedi!");
        }
    }
    // --- SAHNE GEÇİŞİ İÇİN YENİ EKLENEN METOT SONU ---
}