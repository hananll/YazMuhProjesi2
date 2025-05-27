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

    [Header("Ses Ayarlar�")]
    public AudioSource sayfaCevirmeAudioSource; 
    public AudioClip sayfaCevirmeSoundClip;    

    [Header("Di�er Sistemlerle Entegrasyon")]
    public ArkaPlanYonetimi arkaPlanYoneticisi;
    public CanvasGroup arkaPlanCanvasGroup;

    void Start()
    {
        // Butonlar�n t�klama olaylar�n� ba�lad�m
        if (davaDosyasiButon != null)
            davaDosyasiButon.onClick.AddListener(DavaDosyasiPaneliAc);

        if (davaDosyasiKapatButon != null)
            davaDosyasiKapatButon.onClick.AddListener(DavaDosyasiPaneliKapat);

        // Sonraki sayfa butonuna hem sayfa �evirme hem de ses �alma olaylar�n� ba�lad�m.
        if (sonrakiSayfaButon != null)
        {
            sonrakiSayfaButon.onClick.AddListener(SonrakiSayfa);
            sonrakiSayfaButon.onClick.AddListener(PlayPageTurnSound); 
        }

        // �nceki sayfa butonuna hem sayfa �evirme hem de ses �alma olaylar�n� ba�lad�m.
        if (oncekiSayfaButon != null)
        {
            oncekiSayfaButon.onClick.AddListener(OncekiSayfa);
            oncekiSayfaButon.onClick.AddListener(PlayPageTurnSound);
        }

        // E�er tasar�m ekran�nda panellerden birini a��k unutursam kapal� olmas� i�in burda i�imi garantiye ald�m.
        if (davaDosyasiAnaPanel != null)
            davaDosyasiAnaPanel.SetActive(false);

        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0)
        {
            if (sonrakiSayfaButon != null) sonrakiSayfaButon.interactable = false;
            if (oncekiSayfaButon != null) oncekiSayfaButon.interactable = false;
        }
        else
        {
            SayfayiGoster(0); // �lk sayfay� g�ster
            if (oncekiSayfaButon != null)
                oncekiSayfaButon.interactable = false; // �lk sayfadayken �nceki buton devre d��� ��nk� ilk sayfaday�z.
            if (sonrakiSayfaButon != null)
                sonrakiSayfaButon.interactable = davaDosyasiSayfalari.Length > 1; // Tek sayfa varsa sonraki de devre d��� �eklinde yapt�m.
        }
    }

    // Dava dosyas� panelini a�ar ve arka plan� ayarlar
    void DavaDosyasiPaneliAc()
    {
        if (davaDosyasiAnaPanel == null)
        {
            Debug.LogWarning("Dava Dosyas� Ana Paneli atanmam��!"); // Kontrol i�in debug ekledim.
            return;
        }

        davaDosyasiAnaPanel.SetActive(true);

        if (arkaPlanYoneticisi != null)
        {
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(true);
        }

        // Panel a��ld���nda arka plandaki olan avukat button vs bunlar�n hepsine t�klanamaz hale getirdim.
        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = false;
            arkaPlanCanvasGroup.blocksRaycasts = false;
        }
    }

    void DavaDosyasiPaneliKapat()
    {
        if (davaDosyasiAnaPanel == null)
        {
            Debug.LogWarning("Dava Dosyas� Ana Paneli atanmam��!");  // Kontrol i�in yine debug sat�r� ekledim.
            return;
        }

        davaDosyasiAnaPanel.SetActive(false);

        if (arkaPlanYoneticisi != null)
        {
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(false);
        }

        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = true;
            arkaPlanCanvasGroup.blocksRaycasts = true;
        }
    }

    public void SonrakiSayfa()
    {
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0) return;

        if (aktifSayfaIndex < davaDosyasiSayfalari.Length - 1)
        {
            aktifSayfaIndex++;
            SayfayiGoster(aktifSayfaIndex);

            if (oncekiSayfaButon != null)
                oncekiSayfaButon.interactable = true;

            if (sonrakiSayfaButon != null && aktifSayfaIndex == davaDosyasiSayfalari.Length - 1)
                sonrakiSayfaButon.interactable = false;
        }
    }

    public void OncekiSayfa()
    {
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0) return;

        if (aktifSayfaIndex > 0)
        {
            aktifSayfaIndex--;
            SayfayiGoster(aktifSayfaIndex);

            if (sonrakiSayfaButon != null)
                sonrakiSayfaButon.interactable = true;

            if (oncekiSayfaButon != null && aktifSayfaIndex == 0)
                oncekiSayfaButon.interactable = false; 
        }
    }

    void SayfayiGoster(int sayfaIndex)
    {
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0)
        {
            Debug.LogWarning("Dava Dosyas� sayfalar� atanmam�� veya bo�!");
            return;
        }

        for (int i = 0; i < davaDosyasiSayfalari.Length; i++)
        {
            if (davaDosyasiSayfalari[i] != null)
            {
                davaDosyasiSayfalari[i].SetActive(false);
            }
        }

        if (sayfaIndex >= 0 && sayfaIndex < davaDosyasiSayfalari.Length && davaDosyasiSayfalari[sayfaIndex] != null)
        {
            davaDosyasiSayfalari[sayfaIndex].SetActive(true);
        }
        
    }

    
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
}