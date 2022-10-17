using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class MoveCharOnSlideScript : MonoBehaviour
{

    CharacterController characterController;
    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private TextMeshProUGUI slidertext;
   
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        slider.onValueChanged.AddListener((v)=>{
            Debug.Log(v);
            slidertext.text = v.ToString("0.00"); //value of the string =  #of rotations/min
            moveCharOnSlide(v);
        });
        
    }

    void moveCharOnSlide(float v){
        Vector3 movingVector = new Vector3(0,0,v);
        Debug.Log(movingVector);
        characterController.Move(new Vector3(0,0,v*100) * Time.deltaTime);
        
        //when the slider is released: reset the slider to 0
        /* slider.OnPointerUp.AddListener(()) */
    /*     StartCoroutine(autoResetValue(v)); */
    }
/*     IEnumerator autoResetValue(float v){
        slider.value = 0.00f;
        yield break;
    }
 */
    void Update()
    {

        //move based on how many rotations/minute 
        // -> slider with amount of rotations/minute
        // -> on buttonclick stop (grabbing the wheels)
        // -> else slow down over time = lower slider automatically
        // 
       
    }
}
