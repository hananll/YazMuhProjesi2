using UnityEngine;
using UnityEngine.UI;

public class KanunKitabiYoneticisi : MonoBehaviour
{
    [Header("Ana Panel ve Butonlar")]
    public Button kanunKitabiButon;
    public GameObject kanunKitabiPanel;
    public Button kanunKitabiKapatButon;

    [Header("Sayfa Yönetimi")]
    public GameObject[] kanunKitabiSayfalari;
    public Button kanunSonrakiSayfaButon;
    public Button kanunOncekiSayfaButon;
    private int aktifSayfaIndex = 0;

    [Header("Diğer Sistemlerle Entegrasyon")]
    public ArkaPlanYonetimi arkaPlanYoneticisi;
    public CanvasGroup arkaPlanCanvasGroup;

    void Start()
    {
      

        if (kanunKitabiButon != null)
            kanunKitabiButon.onClick.AddListener(KanunKitabiPaneliAc);

        if (kanunKitabiKapatButon != null)
            kanunKitabiKapatButon.onClick.AddListener(KanunKitabiPaneliKapat);

        if (kanunSonrakiSayfaButon != null)
            kanunSonrakiSayfaButon.onClick.AddListener(SonrakiSayfa);
       
        if (kanunOncekiSayfaButon != null)
            kanunOncekiSayfaButon.onClick.AddListener(OncekiSayfa);
       

        if (kanunKitabiPanel != null)
            kanunKitabiPanel.SetActive(false);

        if (kanunKitabiSayfalari == null || kanunKitabiSayfalari.Length == 0)
        {
            if (kanunSonrakiSayfaButon != null) kanunSonrakiSayfaButon.interactable = false;
            if (kanunOncekiSayfaButon != null) kanunOncekiSayfaButon.interactable = false;
        }
        else
        {
            SayfayiGoster(0);
            if (kanunOncekiSayfaButon != null)
                kanunOncekiSayfaButon.interactable = false;
            if (kanunSonrakiSayfaButon != null)
                kanunSonrakiSayfaButon.interactable = kanunKitabiSayfalari.Length > 1;
        }
    }

    void KanunKitabiPaneliAc()
    {
        if (kanunKitabiPanel == null)
        {
          
            return;
        }

        kanunKitabiPanel.SetActive(true);
        
        if (arkaPlanYoneticisi != null)
        {
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(true);
        }
        

        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = false;
            arkaPlanCanvasGroup.blocksRaycasts = false; 
           
        }
    }

    void KanunKitabiPaneliKapat()
    {
        if (kanunKitabiPanel == null)
        {
           
            return;
        }

        kanunKitabiPanel.SetActive(false);

       
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
        if (kanunKitabiSayfalari == null || kanunKitabiSayfalari.Length == 0) return;

        if (aktifSayfaIndex < kanunKitabiSayfalari.Length - 1)
        {
            aktifSayfaIndex++;
            SayfayiGoster(aktifSayfaIndex);

            if (kanunOncekiSayfaButon != null)
                kanunOncekiSayfaButon.interactable = true;

            if (kanunSonrakiSayfaButon != null && aktifSayfaIndex == kanunKitabiSayfalari.Length - 1)
                kanunSonrakiSayfaButon.interactable = false;
        }
    }

    public void OncekiSayfa()
    {
        if (kanunKitabiSayfalari == null || kanunKitabiSayfalari.Length == 0) return;

        if (aktifSayfaIndex > 0)
        {
            aktifSayfaIndex--;
            SayfayiGoster(aktifSayfaIndex);

            if (kanunSonrakiSayfaButon != null)
                kanunSonrakiSayfaButon.interactable = true;

            if (kanunOncekiSayfaButon != null && aktifSayfaIndex == 0)
                kanunOncekiSayfaButon.interactable = false;
        }
    }

    void SayfayiGoster(int sayfaIndex)
    {
        if (kanunKitabiSayfalari == null || kanunKitabiSayfalari.Length == 0)
        {
           
            return;
        }

        for (int i = 0; i < kanunKitabiSayfalari.Length; i++)
        {
            if (kanunKitabiSayfalari[i] != null)
            {
                kanunKitabiSayfalari[i].SetActive(false);
            }
        }

        if (sayfaIndex >= 0 && sayfaIndex < kanunKitabiSayfalari.Length && kanunKitabiSayfalari[sayfaIndex] != null)
        {
            kanunKitabiSayfalari[sayfaIndex].SetActive(true);
        }
       
    }
}