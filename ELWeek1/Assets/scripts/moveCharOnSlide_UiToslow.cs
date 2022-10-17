using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class moveCharOnSlide_UiToslow : MonoBehaviour
{

    CharacterController characterController;
    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private TextMeshProUGUI slidertext;
  //---------------------------------------------------------------//
    public GameObject ui_canvas;
    GraphicRaycaster ui_raycaster; // initialize a graphic raycaster -> 17: grab graphicraycaster from ui_canvas 
    PointerEventData click_data;
    List<RaycastResult> click_results;

    float sliderStartValue;
    float currentSliderValue;
    float slowDownStart;
    bool slowDownActive = false;


    void Start()
    {
        slider.minValue = -1;
        slider.maxValue = 1;

        characterController = GetComponent<CharacterController>();

        slider.onValueChanged.AddListener((v)=>{
           /*  Debug.Log(v); */
            slidertext.text = v.ToString("0.00"); //value of the string =  #of rotations/min
            moveCharOnSlide(v);
        });
        //
        //----------GRAPHICRAYCASTER---------------------------------------------------//
         ui_raycaster = ui_canvas.GetComponent<GraphicRaycaster>(); //Add Raycaster so you don't have to get it from canvas every single frame
        // Raycast cause onpointerclick / onpointerenter = unreliable
        //+pointer detection can fail when framejumps/ lagspikes occur
        // use graphics raycaster for UI elements (element of 'canvas')
        //for raycast we need:  1) pointerEventData object to set current mouse position into

        //                      2) List to store the data into
        //    -> put them in start so they are accesible for the whole script
        //      now we can clear out the results each time end populate them with a new set of results
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>(); 
    }

  
    void Update()
    {
         if(Mouse.current.leftButton.isPressed){
            GetUIElementsClicked();
        }
        if(Mouse.current.leftButton.wasReleasedThisFrame){
            slowDownActive = true;
        }

        if( currentSliderValue == 0.00f){
             slowDownActive = false;
        }
         if( currentSliderValue != 0.00f){
             moveCharOnSlide(currentSliderValue);
        }
        if (slowDownActive && currentSliderValue >= 0){
            resetPositiveSlider();
        }
        if (slowDownActive && currentSliderValue <= 0){
            resetNegativeSlider();
        }
        
    }
      void moveCharOnSlide(float v){
        Vector3 movingVector = new Vector3(0,0,v);
       /*  Debug.Log(movingVector); */
        characterController.Move(new Vector3(0,0,v*2) * Time.deltaTime);
        currentSliderValue = v;
    }


    void GetUIElementsClicked(){
        //** get all UI elements clicked, in order of depth (first detected is first) **//
        //grab mouse position & update click-data
        click_data.position = Mouse.current.position.ReadValue();
        //clear out previous results 
        click_results.Clear();
        //perform Raycast itself -> will return UI elements it comes into contacct with
        ui_raycaster.Raycast(click_data, click_results);

        foreach(RaycastResult result in click_results){
            GameObject ui_element = result.gameObject;
            Debug.Log(ui_element);
            if (ui_element.name == "Handle"){
                Debug.Log("Handle is being pressed");
                Debug.Log(ui_element.transform.position);
            }
        }
        
    }

    void resetPositiveSlider(){
        /* Debug.Log(currentSliderValue); */
         if (slider.value != 0){
        slider.value -= Time.deltaTime;
         }
    }
    void resetNegativeSlider(){
        /* Debug.Log(currentSliderValue); */
         if (slider.value != 0){
        slider.value += Time.deltaTime;
         }
    }
}
