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

    public List<KonusmaMetniData> savciDiyalogMetinleri; 
    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;

    public SanýkAvukatýKonusma sanýkAvukatýKonusma;
    public SanikveHakimKonusma sanikveHakimKonusma;
    public MagdurveHakimKonusma magdurKonusma;
    public MagdurAvukatiKonusma magdurAvukatiKonusma;

    void Start()
    {
        if (savciKonusmaPanel != null) savciKonusmaPanel.SetActive(false);

        if (savciButon != null)
        {
            savciButon.onClick.AddListener(KonusmayiBaslat);
        }
       

        if (savciKapatButon != null)
        {
            savciKapatButon.onClick.AddListener(KonusmayiBitir);
        }
       

        if (savciDevamEtButon != null)
        {
            savciDevamEtButon.onClick.AddListener(SonrakiMetniGoster);
            savciDevamEtButon.gameObject.SetActive(false); // Baþlangýçta kapalý
        }
        
        

        sanikveHakimKonusma.mikrofonButon.interactable = true;
        sanýkAvukatýKonusma.sanikAvukatiButon.interactable = true;
        magdurKonusma.magdurButon.interactable = true;
        magdurAvukatiKonusma.magdurAvukatiButon.interactable = true;
    }

    void KonusmayiBaslat()
    {
       
        if (savciKonusmaPanel != null && savciDiyalogMetinleri != null && savciDiyalogMetinleri.Count > 0)
        {
            savciKonusmaPanel.SetActive(true);
            mevcutMetinIndex = 0;
            MevcutMetniGoster();
        }
        

        sanikveHakimKonusma.mikrofonButon.interactable = false;
        sanýkAvukatýKonusma.sanikAvukatiButon.interactable = false;
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

            if (mevcutMetinAnimasyonu != null)
            {
                StopCoroutine(mevcutMetinAnimasyonu);
            }

            savciMetinText.text = "";
            savciAdiText.text = mevcutKonusma.konusmaciAdi;
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(savciMetinText, mevcutKonusma.metin));
            savciDevamEtButon.gameObject.SetActive(false); 
        }
        else
        {
            KonusmayiBitir();
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
        savciDevamEtButon.gameObject.SetActive(true); 
    }

    void KonusmayiBitir()
    {
        if (savciKonusmaPanel != null)
        {
            savciKonusmaPanel.SetActive(false);
        }
        if (savciButon != null)
        {
            savciButon.interactable = true;
        }

        sanikveHakimKonusma.mikrofonButon.interactable = true;
        sanýkAvukatýKonusma.sanikAvukatiButon.interactable = true;
        magdurKonusma.magdurButon.interactable = true;
        magdurAvukatiKonusma.magdurAvukatiButon.interactable = true;

        mevcutMetinIndex = 0;
    }
}