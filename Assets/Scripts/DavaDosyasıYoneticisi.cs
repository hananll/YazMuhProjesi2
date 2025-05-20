using UnityEngine;
using UnityEngine.UI;

public class DavaDosyasiYoneticisi : MonoBehaviour
{
    [Header("Ana Panel ve Butonlar")]
    public Button davaDosyasiButon;         // Dava dosyas�n� a�an buton
    public GameObject davaDosyasiAnaPanel;  // Dava dosyas�n�n ana paneli
    public Button davaDosyasiKapatButon;   // Dava dosyas�n� kapatan buton

    [Header("Sayfa Y�netimi")]
    public GameObject[] davaDosyasiSayfalari; // Sayfalar� tutan GameObject dizisi
    public Button sonrakiSayfaButon;
    public Button oncekiSayfaButon;
    private int aktifSayfaIndex = 0;

    [Header("Di�er Sistemlerle Entegrasyon")]
    public ArkaPlanYonetimi arkaPlanYoneticisi;  // ArkaPlanYonetimi script'ine referans
    public CanvasGroup arkaPlanCanvasGroup;    // Di�er UI elemanlar�n� i�eren CanvasGroup'a referans

    void Start()
    {
        // Temel referanslar�n atan�p atanmad���n� kontrol et
        if (davaDosyasiAnaPanel == null)
            Debug.LogError("HATA: DavaDosyasiYoneticisi - Dava Dosyas� Ana Paneli (davaDosyasiAnaPanel) Inspector'da ATANMAMI�!");
        if (davaDosyasiButon == null)
            Debug.LogError("HATA: DavaDosyasiYoneticisi - Dava Dosyas� Butonu (davaDosyasiButon) Inspector'da ATANMAMI�!");
        if (davaDosyasiKapatButon == null)
            Debug.LogError("HATA: DavaDosyasiYoneticisi - Dava Dosyas� Kapat Butonu (davaDosyasiKapatButon) Inspector'da ATANMAMI�!");

        if (arkaPlanYoneticisi == null)
            Debug.LogError("HATA: DavaDosyasiYoneticisi - Arka Plan Yoneticisi Inspector'da ATANMAMI�!");
        if (arkaPlanCanvasGroup == null)
            Debug.LogWarning("UYARI: DavaDosyasiYoneticisi - Arka Plan UI Group (arkaPlanCanvasGroup) Inspector'da ATANMAMI�! Di�er UI elemanlar� devre d��� b�rak�lamayacak/etkinle�tirilemeyecek.");

        // Listener'lar� ekle
        if (davaDosyasiButon != null)
            davaDosyasiButon.onClick.AddListener(DavaDosyasiPaneliAc);

        if (davaDosyasiKapatButon != null)
            davaDosyasiKapatButon.onClick.AddListener(DavaDosyasiPaneliKapat);

        if (sonrakiSayfaButon != null)
            sonrakiSayfaButon.onClick.AddListener(SonrakiSayfa);
        else if (davaDosyasiSayfalari != null && davaDosyasiSayfalari.Length > 0)
            Debug.LogError("HATA: DavaDosyasiYoneticisi - Sonraki Sayfa Butonu (Dava Dosyas�) Inspector'da atanmam��!");

        if (oncekiSayfaButon != null)
            oncekiSayfaButon.onClick.AddListener(OncekiSayfa);
        else if (davaDosyasiSayfalari != null && davaDosyasiSayfalari.Length > 0)
            Debug.LogError("HATA: DavaDosyasiYoneticisi - �nceki Sayfa Butonu (Dava Dosyas�) Inspector'da atanmam��!");

        // Dava Dosyas� Paneli ba�lang��ta kapal� olsun
        if (davaDosyasiAnaPanel != null)
            davaDosyasiAnaPanel.SetActive(false);

        // Sayfalar�n ve sayfa butonlar�n�n ba�lang�� durumunu ayarla
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0)
        {
            Debug.LogWarning("UYARI: DavaDosyasiYoneticisi - Dava Dosyas� Sayfalar dizisi Inspector'da atanmam�� veya bo�!");
            if (sonrakiSayfaButon != null) sonrakiSayfaButon.interactable = false;
            if (oncekiSayfaButon != null) oncekiSayfaButon.interactable = false;
        }
        else
        {
            SayfayiGoster(0); // �lk sayfay� g�ster
            if (oncekiSayfaButon != null)
                oncekiSayfaButon.interactable = false;
            if (sonrakiSayfaButon != null)
                sonrakiSayfaButon.interactable = davaDosyasiSayfalari.Length > 1;
        }
    }

    void DavaDosyasiPaneliAc()
    {
        // Debug.Log("Dava dosyas� butonu t�kland�."); // �ste�e ba�l� log
        if (davaDosyasiAnaPanel == null)
        {
            Debug.LogError("HATA: DavaDosyasiYoneticisi - DavaDosyasiPaneliAc �a�r�ld� ama davaDosyasiAnaPanel NULL!");
            return;
        }

        davaDosyasiAnaPanel.SetActive(true);
        // Panelin di�er her �eyin �zerinde �izilmesi i�in (ayn� Canvas ve sorting layer i�indeyse)
        // davaDosyasiAnaPanel.transform.SetAsLastSibling(); 

        // Arka plan� blur yap
        if (arkaPlanYoneticisi != null)
        {
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(true);
        }
        else
        {
            // Start i�inde zaten hata logu var.
        }

        // Di�er UI elemanlar�n� devre d��� b�rak
        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = false;
            arkaPlanCanvasGroup.blocksRaycasts = false;
            // arkaPlanCanvasGroup.alpha = 0.7f; // �ste�e ba�l�: Arka plan UI'�n� solukla�t�r
        }
    }

    void DavaDosyasiPaneliKapat()
    {
        // Debug.Log("Dava dosyas� kapat butonu t�kland�."); // �ste�e ba�l� log
        if (davaDosyasiAnaPanel == null)
        {
            Debug.LogError("HATA: DavaDosyasiYoneticisi - DavaDosyasiPaneliKapat �a�r�ld� ama davaDosyasiAnaPanel NULL!");
            return;
        }

        davaDosyasiAnaPanel.SetActive(false);

        // Arka plan� normale d�nd�r
        if (arkaPlanYoneticisi != null)
        {
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(false);
        }
        else
        {
            // Start i�inde zaten hata logu var.
        }

        // Di�er UI elemanlar�n� tekrar aktif et
        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = true;
            arkaPlanCanvasGroup.blocksRaycasts = true;
            // arkaPlanCanvasGroup.alpha = 1f; // �ste�e ba�l�: Arka plan UI'�n�n soluklu�unu geri al
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
            // Debug.LogWarning("UYARI: DavaDosyasiYoneticisi - SayfayiGoster �a�r�ld� ama sayfa dizisi bo� veya atanmam��.");
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
        else
        {
            Debug.LogError("HATA: DavaDosyasiYoneticisi - Ge�ersiz dava dosyas� sayfa indeksi (" + sayfaIndex + ") veya ilgili sayfa objesi NULL!");
        }
    }
}