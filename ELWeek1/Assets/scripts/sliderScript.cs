using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class sliderScript : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private TextMeshProUGUI slidertext;
   
    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener((v)=>{
            Debug.Log(v);
            slidertext.text = v.ToString("0.00");
        });
        
    }

    // Update is called once per frame
    void Update()
    {

        //move based on how many rotations/minute 
        // -> slider with amount of rotations/minute
        // -> on buttonclick stop (grabbing the wheels)
        // -> else slow down over time 

    }
    
}

        
