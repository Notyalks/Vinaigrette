using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSFX : MonoBehaviour
{
    [Header("Som do Clique")]
    public AudioClip clickSfx;

    void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(PlayClick);
    }

    private void PlayClick()
    {
        if (clickSfx != null)
        {
            AudioManager.Instance?.PlaySFX(clickSfx);
        }
    }
}
