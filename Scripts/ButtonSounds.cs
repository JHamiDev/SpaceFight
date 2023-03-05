using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    AudioManager audioManager;

    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        audioManager = AudioManager.instance;
    }

    void OnClick()
    {
        audioManager.sounds[6].source.Play();
    }

    public void HoverOver()
    {
        audioManager.sounds[5].source.Play();
    }
}
