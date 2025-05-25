using UnityEngine;
using UnityEngine.UI;

public class DavaDosyasiYoneticisi : MonoBehaviour
{
    [Header("Ana Panel ve Butonlar")]
    public Button davaDosyasiButon;
    public GameObject davaDosyasiAnaPanel;
    public Button davaDosyasiKapatButon;

    [Header("Sayfa Y�netimi")]
    public GameObject[] davaDosyasiSayfalari;
    public Button sonrakiSayfaButon;
    public Button oncekiSayfaButon;
    private int aktifSayfaIndex = 0;

    // --- SES ���N EKLENEN KISIM ---
    [Header("Ses Ayarlar�")]
    public AudioSource sayfaCevirmeAudioSource; // Sayfa �evirme sesini �alacak AudioSource
    public AudioClip sayfaCevirmeSoundClip;     // Sayfa �evirme ses dosyan�z
    // --- SES ���N EKLENEN KISIM SONU ---

    [Header("Di�er Sistemlerle Entegrasyon")]
    public ArkaPlanYonetimi arkaPlanYoneticisi;
    public CanvasGroup arkaPlanCanvasGroup;

    void Start()
    {
        // Butonlar�n t�klama olaylar�n� ba�la
        if (davaDosyasiButon != null)
            davaDosyasiButon.onClick.AddListener(DavaDosyasiPaneliAc);

        if (davaDosyasiKapatButon != null)
            davaDosyasiKapatButon.onClick.AddListener(DavaDosyasiPaneliKapat);

        // Sonraki sayfa butonuna hem sayfa �evirme hem de ses �alma olaylar�n� ba�la
        if (sonrakiSayfaButon != null)
        {
            sonrakiSayfaButon.onClick.AddListener(SonrakiSayfa);
            sonrakiSayfaButon.onClick.AddListener(PlayPageTurnSound); // SES ���N EKLEND�
        }

        // �nceki sayfa butonuna hem sayfa �evirme hem de ses �alma olaylar�n� ba�la
        if (oncekiSayfaButon != null)
        {
            oncekiSayfaButon.onClick.AddListener(OncekiSayfa);
            oncekiSayfaButon.onClick.AddListener(PlayPageTurnSound); // SES ���N EKLEND�
        }

        // Paneli ba�lang��ta gizle
        if (davaDosyasiAnaPanel != null)
            davaDosyasiAnaPanel.SetActive(false);

        // Sayfa navigasyon butonlar�n�n ba�lang�� durumunu ayarla
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0)
        {
            if (sonrakiSayfaButon != null) sonrakiSayfaButon.interactable = false;
            if (oncekiSayfaButon != null) oncekiSayfaButon.interactable = false;
        }
        else
        {
            SayfayiGoster(0); // �lk sayfay� g�ster
            if (oncekiSayfaButon != null)
                oncekiSayfaButon.interactable = false; // �lk sayfadayken �nceki buton devre d���
            if (sonrakiSayfaButon != null)
                sonrakiSayfaButon.interactable = davaDosyasiSayfalari.Length > 1; // Tek sayfa varsa sonraki de devre d���
        }
    }

    // Dava dosyas� panelini a�ar ve arka plan� ayarlar
    void DavaDosyasiPaneliAc()
    {
        if (davaDosyasiAnaPanel == null)
        {
            Debug.LogWarning("Dava Dosyas� Ana Paneli atanmam��!");
            return;
        }

        davaDosyasiAnaPanel.SetActive(true);

        // Arka plan y�netimi entegrasyonu
        if (arkaPlanYoneticisi != null)
        {
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(true);
        }

        // Arka plandaki UI etkile�imini devre d��� b�rak
        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = false;
            arkaPlanCanvasGroup.blocksRaycasts = false;
        }
    }

    // Dava dosyas� panelini kapat�r ve arka plan� eski haline getirir
    void DavaDosyasiPaneliKapat()
    {
        if (davaDosyasiAnaPanel == null)
        {
            Debug.LogWarning("Dava Dosyas� Ana Paneli atanmam��!");
            return;
        }

        davaDosyasiAnaPanel.SetActive(false);

        // Arka plan y�netimi entegrasyonu
        if (arkaPlanYoneticisi != null)
        {
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(false);
        }

        // Arka plandaki UI etkile�imini tekrar aktif et
        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = true;
            arkaPlanCanvasGroup.blocksRaycasts = true;
        }
    }

    // Sonraki sayfaya ge�i�i sa�lar
    public void SonrakiSayfa()
    {
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0) return;

        if (aktifSayfaIndex < davaDosyasiSayfalari.Length - 1)
        {
            aktifSayfaIndex++;
            SayfayiGoster(aktifSayfaIndex);

            // Butonlar�n etkile�im durumunu g�ncelle
            if (oncekiSayfaButon != null)
                oncekiSayfaButon.interactable = true;

            if (sonrakiSayfaButon != null && aktifSayfaIndex == davaDosyasiSayfalari.Length - 1)
                sonrakiSayfaButon.interactable = false; // Son sayfadaysa sonraki buton devre d���
        }
    }

    // �nceki sayfaya ge�i�i sa�lar
    public void OncekiSayfa()
    {
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0) return;

        if (aktifSayfaIndex > 0)
        {
            aktifSayfaIndex--;
            SayfayiGoster(aktifSayfaIndex);

            // Butonlar�n etkile�im durumunu g�ncelle
            if (sonrakiSayfaButon != null)
                sonrakiSayfaButon.interactable = true;

            if (oncekiSayfaButon != null && aktifSayfaIndex == 0)
                oncekiSayfaButon.interactable = false; // �lk sayfadaysa �nceki buton devre d���
        }
    }

    // Belirtilen indeksteki sayfay� g�sterir ve di�erlerini gizler
    void SayfayiGoster(int sayfaIndex)
    {
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0)
        {
            Debug.LogWarning("Dava Dosyas� sayfalar� atanmam�� veya bo�!");
            return;
        }

        // T�m sayfalar� gizle
        for (int i = 0; i < davaDosyasiSayfalari.Length; i++)
        {
            if (davaDosyasiSayfalari[i] != null)
            {
                davaDosyasiSayfalari[i].SetActive(false);
            }
        }

        // Sadece istenen sayfay� aktif et
        if (sayfaIndex >= 0 && sayfaIndex < davaDosyasiSayfalari.Length && davaDosyasiSayfalari[sayfaIndex] != null)
        {
            davaDosyasiSayfalari[sayfaIndex].SetActive(true);
        }
        else
        {
            Debug.LogError($"Ge�ersiz sayfa indeksi: {sayfaIndex}. Toplam sayfa say�s�: {davaDosyasiSayfalari.Length}");
        }
    }

    // --- SES ���N EKLENEN KISIM ---
    // Sayfa �evirme sesini �alan fonksiyon
    private void PlayPageTurnSound()
    {
        if (sayfaCevirmeAudioSource != null && sayfaCevirmeSoundClip != null)
        {
            sayfaCevirmeAudioSource.PlayOneShot(sayfaCevirmeSoundClip);
        }
        else
        {
            Debug.LogWarning("Sayfa �evirme AudioSource veya AudioClip atanmam��! L�tfen Inspector'dan atay�n.");
        }
    }
    // --- SES ���N EKLENEN KISIM SONU ---
}