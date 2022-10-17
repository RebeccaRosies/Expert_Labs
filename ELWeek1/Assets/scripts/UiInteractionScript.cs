using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UiInteractionScript : MonoBehaviour
{

    public GameObject ui_canvas;
    GraphicRaycaster ui_raycaster; // initialize a graphic raycaster -> 17: grab graphicraycaster from ui_canvas 
    PointerEventData click_data;
    List<RaycastResult> click_results;
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
         if(Mouse.current.leftButton.isPressed){
            GetUIElementsClicked();
        }
    }
    void GetUIElementsClicked(){
        //** get all UI elements clicked, in order of depth (first detected is first) **//
        //grab mosue position & update click-data
        click_data.position = Mouse.current.position.ReadValue();
        //clear out previous results 
        click_results.Clear();
        //perform Raycast itself -> will return UI elements it comes into contacct with
        ui_raycaster.Raycast(click_data, click_results);

        foreach(RaycastResult result in click_results){
            GameObject ui_element = result.gameObject;
            Debug.Log(ui_element.name);
        }
    }

}

