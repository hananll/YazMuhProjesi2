using UnityEngine;
using TMPro; 

public class IpucuYoneticisi : MonoBehaviour
{
    public static IpucuYoneticisi Ornek { get; private set; }

    public GameObject ipucuPaneliNesnesi;   
    public TextMeshProUGUI ipucuMetinAlani; 
    public RectTransform tuvalRectTransform;
    public Vector2 kaydirmaMiktari = new Vector2(20f, -20f); 

    void Awake()
    {
        if (Ornek == null)
        {
            Ornek = this;
            if (ipucuPaneliNesnesi != null)
                ipucuPaneliNesnesi.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IpucunuGoster(string mesaj, Vector2 fareKonumu)
    {
        if (ipucuPaneliNesnesi == null || ipucuMetinAlani == null) return;

        ipucuMetinAlani.text = mesaj;
        ipucuPaneliNesnesi.SetActive(true);

        RectTransform ipucuRectTransform = ipucuPaneliNesnesi.GetComponent<RectTransform>();

        ipucuRectTransform.position = fareKonumu + kaydirmaMiktari;


        IpucunuEkranaSigdir(ipucuRectTransform);
    }

    public void IpucunuGizle()
    {
        if (ipucuPaneliNesnesi != null)
        {
            ipucuPaneliNesnesi.SetActive(false);
        }
    }

    private void IpucunuEkranaSigdir(RectTransform ipucuRect)
    {
        
        Vector3[] ipucuKoseNoktalari = new Vector3[4];
        ipucuRect.GetWorldCorners(ipucuKoseNoktalari); 

        float ekranGenisligi = Screen.width;
        float ekranYuksekligi = Screen.height;

        Vector3 duzeltilmisPozisyon = ipucuRect.position;

        float ipucuGenisligi = ipucuRect.rect.width * ipucuRect.lossyScale.x;
        float ipucuYuksekligi = ipucuRect.rect.height * ipucuRect.lossyScale.y;

        if (ipucuKoseNoktalari[0].x < 0) 
            duzeltilmisPozisyon.x += Mathf.Abs(ipucuKoseNoktalari[0].x);
        if (ipucuKoseNoktalari[2].x > ekranGenisligi)
            duzeltilmisPozisyon.x -= (ipucuKoseNoktalari[2].x - ekranGenisligi);
        if (ipucuKoseNoktalari[0].y < 0) 
            duzeltilmisPozisyon.y += Mathf.Abs(ipucuKoseNoktalari[0].y);
        if (ipucuKoseNoktalari[1].y > ekranYuksekligi) 
            duzeltilmisPozisyon.y -= (ipucuKoseNoktalari[1].y - ekranYuksekligi);

        ipucuRect.position = duzeltilmisPozisyon;
    }
}