using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class IlkKonusmaYoneticisi : MonoBehaviour
{
    public GameObject ilkKonusmaBalonuPanel; // Inspector'dan atanacak ilk konu�ma balonu paneli
    public TextMeshProUGUI ilkKonusmaMetniTMP; // Inspector'dan atanacak ilk konu�ma metni alan�
    public float harfHiz = 0.05f;             // Harflerin gelme h�z�
    public Button ilkDevamEtButon;           // Inspector'dan atanacak "Devam Et" butonu
    public Button mikrofonButon;             // Inspector'dan atanacak mikrofon butonu
    public KonusmaMetniData ilkKonusmaData;   // Inspector'dan atanacak ilk konu�ma metni verisi (Scriptable Object)

    private int mevcutHarfSayisi = 0;
    private Coroutine ilkMetinAnimasyonu;

    void Start()
    {
        if (mikrofonButon != null)
        {
            mikrofonButon.onClick.AddListener(MikrofonButonunaTiklandi);
        }
        else
        {
            Debug.LogError("Mikrofon Butonu Inspector'da atanmam��!");
        }

        if (ilkDevamEtButon != null)
        {
            ilkDevamEtButon.gameObject.SetActive(false); // Ba�lang��ta "Devam Et" butonu kapal� olsun
            ilkDevamEtButon.onClick.AddListener(IlkDevamEtButonunaTiklandi);
        }
        else
        {
            Debug.LogError("�lk Devam Et Butonu Inspector'da atanmam��!");
        }

        if (ilkKonusmaBalonuPanel != null)
        {
            ilkKonusmaBalonuPanel.SetActive(false); // Ba�lang��ta konu�ma balonu kapal� olsun
        }
        else
        {
            Debug.LogError("�lk Konu�ma Balonu Paneli Inspector'da atanmam��!");
        }

        if (ilkKonusmaMetniTMP == null)
        {
            Debug.LogError("�lk Konu�ma Metni TMP Inspector'da atanmam��!");
        }
    }

    public void MikrofonButonunaTiklandi()
    {
        Debug.Log("Mikrofon butonuna t�kland�!");
        if (ilkKonusmaBalonuPanel != null && ilkKonusmaData != null)
        {
            ilkKonusmaBalonuPanel.SetActive(true);
            mevcutHarfSayisi = 0;
            ilkKonusmaMetniTMP.text = "";
            ilkMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(ilkKonusmaData.metin));

            // Mikrofon butonunu devre d��� b�rakabiliriz (iste�e ba�l�)
            if (mikrofonButon != null)
            {
                mikrofonButon.interactable = false;
            }
        }
        else
        {
            if (ilkKonusmaBalonuPanel == null)
            {
                Debug.LogError("�lk Konu�ma Balonu Paneli Inspector'da atanmam��!");
            }
            if (ilkKonusmaData == null)
            {
                Debug.LogError("�lk Konu�ma Verisi (Scriptable Object) Inspector'da atanmam��!");
            }
        }
    }

    private IEnumerator MetniHarfHarfGoster(string metin)
    {
        while (mevcutHarfSayisi < metin.Length)
        {
            mevcutHarfSayisi++;
            ilkKonusmaMetniTMP.text = metin.Substring(0, mevcutHarfSayisi);
            yield return new WaitForSeconds(harfHiz);
        }

        // �lk metin tamamland�ktan sonra "Devam Et" butonunu aktif et
        if (ilkDevamEtButon != null)
        {
            ilkDevamEtButon.gameObject.SetActive(true);
        }
    }

    public void IlkDevamEtButonunaTiklandi()
    {
        Debug.Log("�lk Devam Et butonuna t�kland�!");
        // Burada sonraki metinleri veya olaylar� tetikleyebilirsiniz.
        // �imdilik sadece ilk konu�ma balonunu kapat�p bir sonraki a�amaya ge�elim.
        if (ilkKonusmaBalonuPanel != null)
        {
            ilkKonusmaBalonuPanel.SetActive(false);
        }
    }
}