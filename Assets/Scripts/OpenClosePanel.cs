using UnityEngine;

public class OpenClosePanel : MonoBehaviour
{
    [SerializeField] GameObject _panelToOpenClose;


    private void Awake()
    {
        if (_panelToOpenClose != null)
            _panelToOpenClose.SetActive(false);
    }

    public void OpenPanel()
    {
        if(_panelToOpenClose != null)
        {
            // Define o estado como o CONTRÁRIO (!) do estado atual (.activeSelf)
            _panelToOpenClose.SetActive(!_panelToOpenClose.activeSelf);
        }
    }
    
}
