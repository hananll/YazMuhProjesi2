using UnityEngine;
using UnityEngine.EventSystems; 

public class IpucuTetikleyici : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(3, 5)]
    public string ipucuMesaji;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(ipucuMesaji) && IpucuYoneticisi.Ornek != null)
        {
            IpucuYoneticisi.Ornek.IpucunuGoster(ipucuMesaji, Input.mousePosition);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IpucuYoneticisi.Ornek != null)
        {
            IpucuYoneticisi.Ornek.IpucunuGizle();
        }
    }
}