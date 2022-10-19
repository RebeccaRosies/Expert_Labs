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
    [SerializeField] private UnityEngine.UI.Slider leftSlider;
    [SerializeField] private TextMeshProUGUI leftSlidertext;
    [SerializeField] private UnityEngine.UI.Slider rightSlider;
    [SerializeField] private TextMeshProUGUI rightSlidertext;
  //---------------------------------------------------------------//
    public GameObject ui_canvas;
    GameObject ui_element;
    GraphicRaycaster ui_raycaster; // initialize a graphic raycaster -> 17: grab graphicraycaster from ui_canvas 
    PointerEventData click_data;
    List<RaycastResult> click_results;

    float leftSliderStartValue;
    float currentLeftSliderValue;
    float currentRightSliderValue;
    float slowDownStart;
    bool slowDownLeftActive = false;
    bool slowDownRightActive = false;


    void Start()
    {
        leftSlider.minValue = -1;
        leftSlider.maxValue = 1;
        rightSlider.minValue = -1;
        rightSlider.maxValue = 1;

        characterController = GetComponent<CharacterController>();

        leftSlider.onValueChanged.AddListener((v)=>{
           /*  Debug.Log(v); */
            leftSlidertext.text = v.ToString("0.00"); //value of the string =  #of rotations/min
            moveCharLeftOnSlide(v);
        });
        rightSlider.onValueChanged.AddListener((v)=>{
           /*  Debug.Log(v); */
            rightSlidertext.text = v.ToString("0.00"); //value of the string =  #of rotations/min
            moveCharRightOnSlide(v);
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

    // if the left slider was released then make it start to slow down , if the right slider was released then make it start to slow down
        if(Mouse.current.leftButton.wasReleasedThisFrame && click_results[0].gameObject.transform.parent.parent.name == "SliderLeft"){
            slowDownLeftActive = true;
        }
        if(Mouse.current.leftButton.wasReleasedThisFrame && click_results[0].gameObject.transform.parent.parent.name == "SliderRight"){
            slowDownRightActive = true;
        }
    // if the left or right slider gets back to zero then stop decreasing speed in the respective slider
        if(currentLeftSliderValue == 0.00f){
             slowDownLeftActive = false;
        }
        if(currentRightSliderValue == 0.00f){
             slowDownRightActive = false;
        }

    // if the slider is currently not zero then keep movig it at the # of rotations/minute it is set at
         if( currentLeftSliderValue != 0.00f){
             moveCharLeftOnSlide(currentLeftSliderValue);
        }
        if( currentRightSliderValue != 0.00f){
             moveCharRightOnSlide(currentRightSliderValue);
        }
    // if the LEFT slider is currently above / below zero and should be slowed down -> decrease speed to 0
        if (slowDownLeftActive && currentLeftSliderValue >= 0){
            resetLeftPositiveSlider();
        }
        if (slowDownLeftActive && currentLeftSliderValue <= 0){
            resetLeftNegativeSlider();
        }
    // if the RIGHT slider is currently above zero and should be slowed down -> decrease speed to 0
        if (slowDownRightActive && currentRightSliderValue >= 0){
            resetRightPositiveSlider();
        }
        if (slowDownRightActive && currentRightSliderValue <= 0){
            resetRightNegativeSlider();
        }
        
    }
      void moveCharLeftOnSlide(float v){
        Vector3 movingVector = new Vector3(0,0,v);
       /*  Debug.Log(movingVector); */
        characterController.Move(new Vector3(0,0,v*2) * Time.deltaTime);
        currentLeftSliderValue = v;
    }
       void moveCharRightOnSlide(float v){
        Vector3 movingVector = new Vector3(0,0,v);
       /*  Debug.Log(movingVector); */
        characterController.Move(new Vector3(0,0,v*2) * Time.deltaTime);
        currentRightSliderValue = v;
    }


    void GetUIElementsClicked(){
        //** get all UI elements clicked, in order of depth (first detected is first) **//
        //grab mouse position & update click-data
        click_data.position = Mouse.current.position.ReadValue();
        click_results.Clear(); //clear out previous results
        ui_raycaster.Raycast(click_data, click_results); //perform Raycast itself -> will return UI elements it comes into contact with

        foreach(RaycastResult result in click_results){
            GameObject ui_element = result.gameObject;
            Debug.Log(ui_element);
            if (ui_element.name == "Handle"){
                Debug.Log("Handle is being pressed");
                Debug.Log(ui_element.transform.position);
            }
        }
        
    }

    void resetLeftPositiveSlider(){
         if (leftSlider.value != 0){
        leftSlider.value -= Time.deltaTime;
         }
    }
    void resetLeftNegativeSlider(){
         if (leftSlider.value != 0){
        leftSlider.value += Time.deltaTime;
         } 
    }
     void resetRightPositiveSlider(){
        if (rightSlider.value != 0){
        rightSlider.value -= Time.deltaTime;
         }
    }
    void resetRightNegativeSlider(){
         if (rightSlider.value != 0){
        rightSlider.value += Time.deltaTime;
         } 
    }
}
