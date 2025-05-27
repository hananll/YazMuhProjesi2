using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SanıkAvukatıKonusma : MonoBehaviour
{
    public KarakterKonusmaAnimasyonu sanikAvukatiAnimationControl;

    public GameObject sanikAvukatiKonusmaPanel;
    public TextMeshProUGUI sanikAvukatiMetinText;
    public TextMeshProUGUI sanikAvukatiAdiText;
    public Button sanikAvukatiKapatButon;
    public Button sanikAvukatiButon; 
    public Button sanikAvukatiDevamEtButon;

    public AudioSource sanikAvukatiAudioSource; 

    public float harfHiz = 0.05f;

    public List<KonusmaMetniData> sanikAvukatiDiyalogMetinleri;

    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;
    private KarakterKonusmaAnimasyonu currentSpeakerAnimationControl = null;

    public SanikveHakimKonusma sanikveHakimKonusma;
    public SavciKonusma savciKonusma;
    public MagdurveHakimKonusma magdurKonusma;
    public MagdurAvukatiKonusma magdurAvukatiKonusma;


    void Start()
    {
        sanikAvukatiKonusmaPanel.SetActive(false); 

        sanikAvukatiButon.onClick.AddListener(KonusmayiBaslat);
        sanikAvukatiKapatButon.onClick.AddListener(KonusmayiBitir);
        sanikAvukatiDevamEtButon.onClick.AddListener(SonrakiMetniGoster);

        sanikAvukatiDevamEtButon.gameObject.SetActive(false); 

        
    }

    void KonusmayiBaslat()
    {
        if (sanikAvukatiDiyalogMetinleri == null || sanikAvukatiDiyalogMetinleri.Count == 0)
        {
            return;
        }

        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        if (sanikAvukatiAudioSource != null && sanikAvukatiAudioSource.isPlaying)
        {
            sanikAvukatiAudioSource.Stop();
        }

        sanikAvukatiKonusmaPanel.SetActive(true); 

        ButonlariKapat(); 
        mevcutMetinIndex = 0;
        MevcutMetniGoster();
    }

    void SonrakiMetniGoster()
    {
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;

            KonusmaMetniData currentData = sanikAvukatiDiyalogMetinleri[mevcutMetinIndex];
            sanikAvukatiMetinText.text = currentData.metin; 

            if (currentSpeakerAnimationControl != null)
            {
                sanikAvukatiAnimationControl.KonusmayiBitir();
            }

            if (sanikAvukatiAudioSource != null && sanikAvukatiAudioSource.isPlaying)
            {
                sanikAvukatiAudioSource.Stop();
            }

            sanikAvukatiDevamEtButon.gameObject.SetActive(true);
            sanikAvukatiDevamEtButon.interactable = true;
            return;
        }

        mevcutMetinIndex++;
        if (mevcutMetinIndex < sanikAvukatiDiyalogMetinleri.Count)
        {
            MevcutMetniGoster();
        }
        else
        {
            KonusmayiBitir();
        }
    }

    void MevcutMetniGoster()
    {
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;
        }
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        if (sanikAvukatiAudioSource != null && sanikAvukatiAudioSource.isPlaying)
        {
            sanikAvukatiAudioSource.Stop();
        }

        sanikAvukatiDevamEtButon.gameObject.SetActive(false);

        if (mevcutMetinIndex < sanikAvukatiDiyalogMetinleri.Count)
        {
            KonusmaMetniData mevcutKonusma = sanikAvukatiDiyalogMetinleri[mevcutMetinIndex];

            sanikAvukatiMetinText.text = ""; 
            sanikAvukatiAdiText.text = mevcutKonusma.konusmaciAdi;

            if (sanikAvukatiAnimationControl != null)
            {
                sanikAvukatiAnimationControl.KonusmayaBasla();
                currentSpeakerAnimationControl = sanikAvukatiAnimationControl; 
            }

            if (sanikAvukatiAudioSource != null && mevcutKonusma.sesDosyasi != null)
            {
                sanikAvukatiAudioSource.clip = mevcutKonusma.sesDosyasi;
                sanikAvukatiAudioSource.Play();
            }
           
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(sanikAvukatiMetinText, mevcutKonusma.metin, sanikAvukatiDevamEtButon));
        }
        else
        {
            KonusmayiBitir();
        }
    }

    IEnumerator MetniHarfHarfGoster(TextMeshProUGUI metinAlani, string metin, Button devamButonu) 
    {
        int harfSayisi = 0;
        metinAlani.text = "";
        while (harfSayisi < metin.Length)
        {
            harfSayisi++;
            metinAlani.text = metin.Substring(0, harfSayisi);
            yield return new WaitForSeconds(harfHiz);
        }

        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
        }

        
        if (sanikAvukatiAudioSource != null && sanikAvukatiAudioSource.isPlaying)
        {
            sanikAvukatiAudioSource.Stop();
        }

        devamButonu.gameObject.SetActive(true); 
        devamButonu.interactable = true;
    }

    void KonusmayiBitir()
    {
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        if (sanikAvukatiAudioSource != null && sanikAvukatiAudioSource.isPlaying)
        {
            sanikAvukatiAudioSource.Stop();
        }

        sanikAvukatiKonusmaPanel.SetActive(false);
        sanikAvukatiDevamEtButon.gameObject.SetActive(false);
        ButonlariAc();
        mevcutMetinIndex = 0;
    }

    void ButonlariKapat()
    {
        
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
        sanikAvukatiButon.interactable = true;

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