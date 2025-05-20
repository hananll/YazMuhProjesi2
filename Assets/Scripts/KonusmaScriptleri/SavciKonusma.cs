using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SavciKonusma : MonoBehaviour
{
    public GameObject savciKonusmaPanel;
    public TextMeshProUGUI savciMetinText;
    public TextMeshProUGUI savciAdiText;
    public Button savciKapatButon;
    public Button savciButon;
    public Button savciDevamEtButon; 

    public float harfHiz = 0.05f;

    public List<KonusmaMetniData> savciDiyalogMetinleri; // Birden fazla diyalog i�in liste
    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;

    public San�kAvukat�Konusma san�kAvukat�Konusma;
    public SanikveHakimKonusma sanikveHakimKonusma;
    public MagdurveHakimKonusma magdurKonusma;
    public MagdurAvukatiKonusma magdurAvukatiKonusma;

    void Start()
    {
        // Savc� Paneli Ba�lang��ta Kapal� Olacak
        if (savciKonusmaPanel != null) savciKonusmaPanel.SetActive(false);

        // Savc� Butonuna T�klama Olay�n� Ba�la
        if (savciButon != null)
        {
            savciButon.onClick.AddListener(KonusmayiBaslat);
        }
        else
        {
            Debug.LogError("Savc� Butonu Inspector'da atanmam��!");
        }

        // Kapat Butonuna T�klama Olay�n� Ba�la
        if (savciKapatButon != null)
        {
            savciKapatButon.onClick.AddListener(KonusmayiBitir);
        }
        else
        {
            Debug.LogError("Savc� Kapat Butonu Inspector'da atanmam��!");
        }

        // Devam Et Butonuna T�klama Olay�n� Ba�la
        if (savciDevamEtButon != null)
        {
            savciDevamEtButon.onClick.AddListener(SonrakiMetniGoster);
            savciDevamEtButon.gameObject.SetActive(false); // Ba�lang��ta kapal�
        }
        else
        {
            Debug.LogError("Savc� Devam Et Butonu Inspector'da atanmam��!");
        }

        // Diyalog metinleri kontrol�
        if (savciDiyalogMetinleri == null || savciDiyalogMetinleri.Count == 0)
        {
            Debug.LogError("Savc� Diyalog Metinleri (Scriptable Object Listesi) Inspector'da atanmam�� veya bo�!");
        }

        sanikveHakimKonusma.mikrofonButon.interactable = true;
        san�kAvukat�Konusma.sanikAvukatiButon.interactable = true;
        magdurKonusma.magdurButon.interactable = true;
        magdurAvukatiKonusma.magdurAvukatiButon.interactable = true;
    }

    void KonusmayiBaslat()
    {
        Debug.Log("Savc� konu�mas� ba�lat�l�yor.");
       /*
        if (savciButon != null)
        {
            savciButon.interactable = false; // Ba�lang�� butonunu devre d��� b�rak
        }
       */
        if (savciKonusmaPanel != null && savciDiyalogMetinleri != null && savciDiyalogMetinleri.Count > 0)
        {
            savciKonusmaPanel.SetActive(true);
            mevcutMetinIndex = 0;
            MevcutMetniGoster();
        }
        else
        {
            Debug.LogError("Savc� konu�mas� ba�lat�lamad�!");
        }

        sanikveHakimKonusma.mikrofonButon.interactable = false;
        san�kAvukat�Konusma.sanikAvukatiButon.interactable = false;
        magdurKonusma.magdurButon.interactable = false;
        magdurAvukatiKonusma.magdurAvukatiButon.interactable = false;
    }

    void SonrakiMetniGoster()
    {
        mevcutMetinIndex++;
        MevcutMetniGoster();
    }

    void MevcutMetniGoster()
    {
        if (mevcutMetinIndex < savciDiyalogMetinleri.Count)
        {
            KonusmaMetniData mevcutKonusma = savciDiyalogMetinleri[mevcutMetinIndex];

            // �nceki animasyonu durdur
            if (mevcutMetinAnimasyonu != null)
            {
                StopCoroutine(mevcutMetinAnimasyonu);
            }

            savciMetinText.text = ""; // Metin alan�n� temizle
            savciAdiText.text = mevcutKonusma.konusmaciAdi;
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(savciMetinText, mevcutKonusma.metin));
            savciDevamEtButon.gameObject.SetActive(false); // Yeni metin ba�lad���nda devam et butonu kapal�
        }
        else
        {
            KonusmayiBitir(); // T�m metinler g�sterildiyse konu�may� bitir
        }
    }

    IEnumerator MetniHarfHarfGoster(TextMeshProUGUI metinAlani, string metin)
    {
        int harfSayisi = 0;
        metinAlani.text = "";
        while (harfSayisi < metin.Length)
        {
            harfSayisi++;
            metinAlani.text = metin.Substring(0, harfSayisi);
            yield return new WaitForSeconds(harfHiz);
        }
        savciDevamEtButon.gameObject.SetActive(true); // Metin tamamland�ktan sonra devam et butonu aktif
    }

    void KonusmayiBitir()
    {
        Debug.Log("Savc� konu�mas� bitti.");
        if (savciKonusmaPanel != null)
        {
            savciKonusmaPanel.SetActive(false);
        }
        if (savciButon != null)
        {
            savciButon.interactable = true; // Ba�lang�� butonunu tekrar aktif et (iste�e ba�l�)
        }

        sanikveHakimKonusma.mikrofonButon.interactable = true;
        san�kAvukat�Konusma.sanikAvukatiButon.interactable = true;
        magdurKonusma.magdurButon.interactable = true;
        magdurAvukatiKonusma.magdurAvukatiButon.interactable = true;

        mevcutMetinIndex = 0; // Konu�ma tekrar ba�lat�l�rsa indeksi s�f�rla
    }
}