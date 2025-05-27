using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SanikveHakimKonusma : MonoBehaviour
{
    public KarakterKonusmaAnimasyonu sanikAnimasyonKontrolu;

    public GameObject hakimKonusmaPanel;
    public TextMeshProUGUI hakimMetinText;
    public TextMeshProUGUI hakimAdiText;
    public Button hakimDevamEtButon;

    public GameObject sanikKonusmaPanel;
    public TextMeshProUGUI sanikMetinText;
    public TextMeshProUGUI sanikAdiText;
    public Button sanikDevamEtButon;

    public Image sanikGorselBaslangic; 
    public Button mikrofonButon;

    public float harfHiz = 0.05f;

    public List<KonusmaMetniData> diyalogMetinleri;
    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;
    private KarakterKonusmaAnimasyonu sonKonusanAnimasyonKontrolu = null;

    public SavciKonusma savciKonusma;
    public SanıkAvukatıKonusma sanıkAvukatıKonusma;

    public AudioSource diyalogAudioSource;

    void Start()
    {

       hakimKonusmaPanel.SetActive(false);
       sanikKonusmaPanel.SetActive(false);
       sanikGorselBaslangic.gameObject.SetActive(true);
       

       

       mikrofonButon.onClick.AddListener(IlkKonusmayiBaslat);
        
        
        if (hakimDevamEtButon != null) hakimDevamEtButon.onClick.AddListener(SonrakiMetniGoster);
        if (sanikDevamEtButon != null) sanikDevamEtButon.onClick.AddListener(SonrakiMetniGoster);

       

        savciKonusma.savciButon.interactable = true;
        sanıkAvukatıKonusma.sanikAvukatiButon.interactable = true;  
        
    }

    void IlkKonusmayiBaslat()
    {
     
        mikrofonButon.interactable = false;
        savciKonusma.savciButon.interactable = false;
        sanıkAvukatıKonusma.sanikAvukatiButon.interactable = false;

        mevcutMetinIndex = 0;
        MevcutMetniGoster();

    }

    void SonrakiMetniGoster()
    {
        mevcutMetinIndex++;
        if (mevcutMetinIndex < diyalogMetinleri.Count)
        {
            MevcutMetniGoster();
        }
        else
        {
            DiyaloguBitir();
        }
    }

    void MevcutMetniGoster()
    {
        if (mevcutMetinIndex >= diyalogMetinleri.Count)
        {
            DiyaloguBitir();
            return;
        }

        if (sonKonusanAnimasyonKontrolu != null)
        {
            sonKonusanAnimasyonKontrolu.KonusmayiBitir();
            sonKonusanAnimasyonKontrolu = null;
        }

        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }

        KonusmaMetniData mevcutKonusma = diyalogMetinleri[mevcutMetinIndex];


        hakimKonusmaPanel.SetActive(false);
        sanikKonusmaPanel.SetActive(false);

        if (mevcutKonusma.konusmaciAdi == "Hakim")
        {


            if (hakimKonusmaPanel != null) hakimKonusmaPanel.SetActive(true);
            if (hakimAdiText != null) hakimAdiText.text = mevcutKonusma.konusmaciAdi;
            if (hakimMetinText != null) mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(hakimMetinText, mevcutKonusma.metin));

            if (diyalogAudioSource != null && mevcutKonusma.sesDosyasi != null)
            {
                diyalogAudioSource.clip = mevcutKonusma.sesDosyasi;
                diyalogAudioSource.Play();
            }
        }
        else if (mevcutKonusma.konusmaciAdi == "Sanık" || mevcutKonusma.konusmaciAdi=="Tanık")
        {

            if (sanikAnimasyonKontrolu != null)
            {
                sanikAnimasyonKontrolu.KonusmayaBasla();
                sonKonusanAnimasyonKontrolu = sanikAnimasyonKontrolu; 
            }

            if (sanikKonusmaPanel != null) sanikKonusmaPanel.SetActive(true);
            if (sanikAdiText != null) sanikAdiText.text = mevcutKonusma.konusmaciAdi;
            if (sanikMetinText != null) mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(sanikMetinText, mevcutKonusma.metin));

            if (diyalogAudioSource != null && mevcutKonusma.sesDosyasi != null)
            {
                diyalogAudioSource.clip = mevcutKonusma.sesDosyasi;
                diyalogAudioSource.Play();
            }

        }
        

        if (hakimDevamEtButon != null) hakimDevamEtButon.interactable = false;
        if (sanikDevamEtButon != null) sanikDevamEtButon.interactable = false;
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

       
        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }


        if (metinAlani == hakimMetinText && hakimDevamEtButon != null && hakimKonusmaPanel.activeSelf)
        {
            hakimDevamEtButon.interactable = true;
        }
        else if (metinAlani == sanikMetinText && sanikDevamEtButon != null && sanikKonusmaPanel.activeSelf)
        {
            sanikDevamEtButon.interactable = true;
        }
    }

    void DiyaloguBitir()
    {

        if (sonKonusanAnimasyonKontrolu != null)
        {
            sonKonusanAnimasyonKontrolu.KonusmayiBitir();
            sonKonusanAnimasyonKontrolu = null;
        }


        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }

        hakimKonusmaPanel.SetActive(false);
        sanikKonusmaPanel.SetActive(false); 
        sanikGorselBaslangic.gameObject.SetActive(true);

        mikrofonButon.interactable = true;

        savciKonusma.savciButon.interactable = true;
        sanıkAvukatıKonusma.sanikAvukatiButon.interactable = true;


       
        mevcutMetinIndex = 0;

    }
}