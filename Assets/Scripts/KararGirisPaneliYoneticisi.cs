// Script Adı: KararGirisPaneliYoneticisi.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using UnityEngine.SceneManagement; // SceneManager hala burada, ama direkt kullanılmayacak

public class KararGirisPaneliYoneticisi : MonoBehaviour
{
    [Header("Panel Referansı")]
    public GameObject kararGirisPaneliObject;

    [Header("Giriş Alanları")]
    public TMP_InputField hapisYilInputField;
    public TMP_InputField hapisAyInputField;
    public TMP_InputField paraCezasiInputField;

    [Header("Butonlar")]
    public Button hukumVerButonu;
    public Button beraatButonu;
    public Button panelKapatButonu;

    [Header("Bağlantılı Yöneticiler")]
    public ArkaPlanYonetimi arkaPlanYoneticisi;
    public CanvasGroup digerAnaUIArayuzuCanvasGroup;
    [Tooltip("Sahnedeki TokmakSistemiYoneticisi scriptine sahip GameObject (örn: GameManager)")]
    public TokmakSistemiYoneticisi tokmakSistemiYoneticisi; // Tokmak sistemi yöneticisine referans

    private int _aktifDava_IdealHapisGun;
    private float _aktifDava_IdealParaCezasi;
    private bool _aktifDava_IdealBeraatOlmalydi;

    // Yıl ve ay için gün sabitleri (oyun mantığınıza göre değiştirebilirsiniz)
    private const int BIR_YIL_GUN_SAYISI = 365;
    private const int BIR_AY_GUN_SAYISI = 30; // Ortalama olarak

    void Awake()
    {
        if (kararGirisPaneliObject == null) kararGirisPaneliObject = this.gameObject;

        // Referans kontrolleri
        if (hapisYilInputField == null) Debug.LogError("HATA: KararGirisPaneliYoneticisi - Hapis Yıl InputField ATANMAMIŞ!");
        if (hapisAyInputField == null) Debug.LogError("HATA: KararGirisPaneliYoneticisi - Hapis Ay InputField ATANMAMIŞ!");
        if (paraCezasiInputField == null) Debug.LogError("HATA: KararGirisPaneliYoneticisi - Para Cezası InputField ATANMAMIŞ!");
        if (hukumVerButonu == null) Debug.LogError("HATA: KararGirisPaneliYoneticisi - Hüküm Ver Butonu ATANMAMIŞ!");
        if (beraatButonu == null) Debug.LogError("HATA: KararGirisPaneliYoneticisi - Beraat Butonu ATANMAMIŞ!");
        if (panelKapatButonu == null) Debug.LogError("HATA: KararGirisPaneliYoneticisi - Panel Kapat Butonu ATANMAMIŞ!");

        if (arkaPlanYoneticisi == null && ArkaPlanYonetimi.Ornek == null) Debug.LogWarning("UYARI: KararGirisPaneliYoneticisi - Arka Plan Yöneticisi ATANMAMIŞ ve Singleton bulunamadı.");
        else if (arkaPlanYoneticisi == null) arkaPlanYoneticisi = ArkaPlanYonetimi.Ornek;

        if (digerAnaUIArayuzuCanvasGroup == null) Debug.LogWarning("UYARI: KararGirisPaneliYoneticisi - Diğer Ana UI CanvasGroup ATANMAMIŞ.");

        // Tokmak Sistemi Yöneticisi'ni bul (eğer Inspector'dan atanmadıysa)
        if (tokmakSistemiYoneticisi == null)
        {
            tokmakSistemiYoneticisi = FindObjectOfType<TokmakSistemiYoneticisi>();
            if (tokmakSistemiYoneticisi == null)
            {
                Debug.LogError("Sahne'de 'TokmakSistemiYoneticisi' bulunamadı! Lütfen KararGirisPaneliYoneticisi'ne atayın veya doğru nesneye ekleyin.");
            }
        }

        if (kararGirisPaneliObject != null) kararGirisPaneliObject.SetActive(false);
    }

    void OnEnable()
    {
        // Listener'lar OnEnable'da eklenir
        if (hukumVerButonu != null) hukumVerButonu.onClick.AddListener(HukmuOnayla);
        if (beraatButonu != null) beraatButonu.onClick.AddListener(BeraatKarariVerildi);
        if (panelKapatButonu != null) panelKapatButonu.onClick.AddListener(KararPaneliniKapat);
    }

    void OnDisable()
    {
        // Listener'lar OnDisable'da kaldırılır
        if (hukumVerButonu != null) hukumVerButonu.onClick.RemoveListener(HukmuOnayla);
        if (beraatButonu != null) beraatButonu.onClick.RemoveListener(BeraatKarariVerildi);
        if (panelKapatButonu != null) panelKapatButonu.onClick.RemoveListener(KararPaneliniKapat);
    }

    public void KararPaneliniGoster(int idealHapisGun, float idealPara, bool idealBeraat)
    {
        _aktifDava_IdealHapisGun = idealHapisGun;
        _aktifDava_IdealParaCezasi = idealPara;
        _aktifDava_IdealBeraatOlmalydi = idealBeraat;

        if (kararGirisPaneliObject == null) return;
        kararGirisPaneliObject.SetActive(true);

        // Yeni input field'ları temizle
        if (hapisYilInputField != null) hapisYilInputField.text = "";
        if (hapisAyInputField != null) hapisAyInputField.text = "";
        if (paraCezasiInputField != null) paraCezasiInputField.text = "";

        ArkaPlanVeDigerUIAyarla(true);
    }

    public void KararPaneliniKapat()
    {
        if (kararGirisPaneliObject == null) return;
        kararGirisPaneliObject.SetActive(false);
        ArkaPlanVeDigerUIAyarla(false);
    }

    void HukmuOnayla()
    {
        if (hapisYilInputField == null || hapisAyInputField == null || paraCezasiInputField == null)
        {
            Debug.LogError("HATA: KararGirisPaneliYoneticisi - InputField referansları eksik, hüküm onaylanamıyor.");
            return;
        }

        string hapisYilYazisi = hapisYilInputField.text;
        string hapisAyYazisi = hapisAyInputField.text;
        string paraCezasiYazisi = paraCezasiInputField.text;

        int girilenYil = 0;
        int girilenAy = 0;
        int toplamHapisGunu = 0;
        float paraCezasiMiktari = 0f;

        if (!string.IsNullOrEmpty(hapisYilYazisi))
        {
            if (int.TryParse(hapisYilYazisi, out girilenYil))
            {
                if (girilenYil < 0) girilenYil = 0;
            }
            else
            {
                Debug.LogWarning("Geçersiz hapis yılı formatı: " + hapisYilYazisi + ". Yıl 0 olarak kabul edilecek.");
                girilenYil = 0;
            }
        }

        if (!string.IsNullOrEmpty(hapisAyYazisi))
        {
            if (int.TryParse(hapisAyYazisi, out girilenAy))
            {
                if (girilenAy < 0) girilenAy = 0;
            }
            else
            {
                Debug.LogWarning("Geçersiz hapis ayı formatı: " + hapisAyYazisi + ". Ay 0 olarak kabul edilecek.");
                girilenAy = 0;
            }
        }

        toplamHapisGunu = (girilenYil * BIR_YIL_GUN_SAYISI) + (girilenAy * BIR_AY_GUN_SAYISI);

        if (!string.IsNullOrEmpty(paraCezasiYazisi))
        {
            string duzeltilmisParaYazisi = paraCezasiYazisi.Replace(',', '.');
            if (!float.TryParse(duzeltilmisParaYazisi, NumberStyles.Any, CultureInfo.InvariantCulture, out paraCezasiMiktari))
            {
                paraCezasiMiktari = 0f;
                Debug.LogWarning("Geçersiz para cezası formatı: " + paraCezasiYazisi);
            }
            if (paraCezasiMiktari < 0) paraCezasiMiktari = 0f;
        }

        Debug.Log("===== HÜKÜM (CEZALI) ONAYLANDI =====");
        Debug.Log($"Girilen Hapis: {girilenYil} Yıl, {girilenAy} Ay (Toplam {toplamHapisGunu} gün)");
        Debug.Log("Verilen Para Cezası: " + paraCezasiMiktari.ToString("F2") + " ₺");
        Debug.Log("===================================");

        if (BarYoneticisi.Ornek != null)
        {
            BarYoneticisi.Ornek.NihaiKararaGoreBarlariGuncelle(
                toplamHapisGunu,
                paraCezasiMiktari,
                false, // beraat değil
                _aktifDava_IdealHapisGun,
                _aktifDava_IdealParaCezasi,
                _aktifDava_IdealBeraatOlmalydi
            );
        }
        else { Debug.LogError("HATA: BarYoneticisi.Ornek bulunamadı, barlar güncellenemedi! (Hüküm Onayla)"); }

        KararPaneliniKapat();

        // --- SAHNE GEÇİŞİ VE SES ÇALMA BURADA BAŞLIYOR ---
        if (tokmakSistemiYoneticisi != null)
        {
            tokmakSistemiYoneticisi.PlayTokmakSound(); // Tokmak sesi
            tokmakSistemiYoneticisi.SahneyiGecikmeliYukle(
                "HakiminOdasi", // Hedef sahne
                tokmakSistemiYoneticisi.gecisTokmakSoundClip, // Geçiş sesi
                tokmakSistemiYoneticisi.sahneGecisGecikmesi // Gecikme süresi
            );
        }
        else
        {
            Debug.LogWarning("TokmakSistemiYoneticisi atanmamış, sahne direkt yükleniyor.");
            SceneManager.LoadScene("HakiminOdasi"); // Fallback
        }
    }

    void BeraatKarariVerildi()
    {
        Debug.Log("===== BERAAT KARARI VERİLDİ =====");

        if (BarYoneticisi.Ornek != null)
        {
            BarYoneticisi.Ornek.NihaiKararaGoreBarlariGuncelle(
                0, 0, true, // beraat kararı
                _aktifDava_IdealHapisGun,
                _aktifDava_IdealParaCezasi,
                _aktifDava_IdealBeraatOlmalydi
            );
        }
        else { Debug.LogError("HATA: BarYoneticisi.Ornek bulunamadı, barlar güncellenemedi! (Beraat)"); }

        KararPaneliniKapat();

        // --- SAHNE GEÇİŞİ VE SES ÇALMA BURADA BAŞLIYOR ---
        if (tokmakSistemiYoneticisi != null)
        {
            tokmakSistemiYoneticisi.PlayTokmakSound(); // Tokmak sesi
            tokmakSistemiYoneticisi.SahneyiGecikmeliYukle(
                "HakiminOdasi", // Hedef sahne
                tokmakSistemiYoneticisi.gecisTokmakSoundClip, // Geçiş sesi
                tokmakSistemiYoneticisi.sahneGecisGecikmesi // Gecikme süresi
            );
        }
        else
        {
            Debug.LogWarning("TokmakSistemiYoneticisi atanmamış, sahne direkt yükleniyor.");
            SceneManager.LoadScene("HakiminOdasi"); // Fallback
        }
    }

    void ArkaPlanVeDigerUIAyarla(bool panelAcikMi)
    {
        if (arkaPlanYoneticisi != null)
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(panelAcikMi);

        if (digerAnaUIArayuzuCanvasGroup != null)
        {
            digerAnaUIArayuzuCanvasGroup.interactable = !panelAcikMi;
            digerAnaUIArayuzuCanvasGroup.blocksRaycasts = !panelAcikMi;
        }
    }
}