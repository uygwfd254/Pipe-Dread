using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        detect_key_press();
    }

    void detect_key_press() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            if (Time.timeScale == 2)
                Time.timeScale = 1;
            else
                Time.timeScale = 2;
        } else if(Input.GetKeyDown("escape")) {
            Audio_Manager.Instance.Stop("Water Flowing");
            Level_Manager.Instance.FadeToMainMenu();
        }
    }
}
