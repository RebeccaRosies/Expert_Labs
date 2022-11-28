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
    WheelchairControls wheelchairControls;

    //public float drag;
    //public float rotationDrag;

    public float speed;
    public float radius;

    float leftSliderStartValue;
    float currentLeftSliderValue;
    float currentRightSliderValue;
    float slowDownStart;
    bool slowDownLeftActive = false;
    bool slowDownRightActive = false;
    float leftwheel = 0;
    float rightwheel = 0;
    float listenValueleft = 0;
    float listenValueright= 0;

    double leftDir;
    double rightDir;
    private InputAction leftWheelPush;
    private InputAction rightWheelPush;

    //ARDITY VARIABLES
     string ticksString;
    float ticks;
    float previousTicks = 0;
    string msg;

    double valueLeft;
    double valueRight;

    double drag = 0.005;
    public double inputMulti = 1;
    
    private void Awake(){
        //new input action system -> keyboard input 
        wheelchairControls = new WheelchairControls();  
    }

    private void OnEnable(){
        //new input action system -> keyboard input 
       /*  leftWheelPush = wheelchairControls.LeftSliderInput.LeftWheelPush; 
        leftWheelPush.Enable();
        rightWheelPush = wheelchairControls.RightSliderInput.RightWheelPush; 
        rightWheelPush.Enable(); */
       
   }

    private void OnDisable(){
        //new input action system -> keyboard input 
       /*  leftWheelPush.Disable();
        rightWheelPush.Disable(); */
    }


    void Start()
    {      
        if(speed <=0){
            speed = 1;
            //drag can't be 0 because you can't divide by 0 
        } 

        if (radius<=0){
            radius=1;
        }
        
        leftSlider.minValue = -1;
        leftSlider.maxValue = 1;
        rightSlider.minValue = -1;
        rightSlider.maxValue = 1;

        characterController = GetComponent<CharacterController>();
        //listenToChanges();
        
        //
        //----------GRAPHICRAYCASTER---------------------------------------------------//
         ui_raycaster = ui_canvas.GetComponent<GraphicRaycaster>(); //Add Raycaster so you don't have to get it from canvas every single frame
        // Raycast cause onpointerclick / onpointerenter = unreliable
        // + pointer detection can fail when framejumps/ lagspikes occur
        // use graphics raycaster for UI elements (element of 'canvas')
        // for raycast we need:  1) pointerEventData object to set current mouse position into
        //                      2) List to store the data into
        //    -> put them in start -> accesible for whole script
        //      clear results each time & populate them with new set of results
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>(); 
    }
     void OnMessageArrived(string msg)
    {
        messageDecoder(msg);
        //Debug.Log("Message arrived in movementScript: " + msg);
    }

     void OnConnectionEvent(bool success)
    {
     if (success)
            Debug.Log("Connection established");
        else
            Debug.Log("Connection attempt failed or disconnection detected");
    }

     void messageDecoder(string msg){
        if(msg != null ){
            if(msg[0] == 'O'){
                if(msg[4] == 'L'){
                    ticksString = msg.Remove(0,6); //messages are in the form: OneCCW34 | TwoCLW336 -> the first 2 items are always 6 chars
                    ticks = float.Parse(ticksString);
                    valueRight = ticks/10;
                    rightDir = 1;
                   //  Debug.Log(valueRight);
                } 
                if(msg[4] == 'C'){
                    ticksString = msg.Remove(0,6); //messages are in the form: OneCCW34 | TwoCLW336 -> the first 2 items are always 6 chars
                    ticks = float.Parse(ticksString);
                    valueRight =  ticks/10;
                    rightDir = 1;
                    // Debug.Log(valueRight);
                } 
            }
            if(msg[0] == 'T'){
                if(msg[4] == 'L'){
                    ticksString = msg.Remove(0,6); //messages are in the form: OneCCW34 | TwoCLW336 -> the first 2 items are always 6 chars
                    ticks = float.Parse(ticksString);
                    valueLeft = ticks/10;
                    leftDir = 1;
                   // Debug.Log(valueLeft);
                } 
                if(msg[4] == 'C'){
                    ticksString = msg.Remove(0,6); //messages are in the form: OneCCW34 | TwoCLW336 -> the first 2 items are always 6 chars
                    ticks = float.Parse(ticksString);
                    valueLeft = ticks/10;
                    leftDir = 1;
                    //Debug.Log(valueLeft);
                } 
            }
            //moveCharacter();
            previousTicks = ticks;
            Debug.Log("valueLeft =" + valueLeft + "valueRight =" + valueRight + "leftDir =" + leftDir + "RightDir =" + rightDir);
            // moveCharOnSlide(leftDir, rightDir);
        }
    }
    /* void listenToChanges(){
        float rotatebydegrees = 0;
        leftSlider.onValueChanged.AddListener((valueLeft)=>{
            leftSlidertext.text = valueLeft.ToString("0.00"); //value of the string =  #of rotations/min
            listenValueleft = valueLeft;
            if (valueLeft>0.01){
                leftDir = 1;
            } else if (valueLeft<-0.01){
                leftDir = -1;
            } else if (valueLeft<0.01||valueLeft>-0.01){
                leftDir = 0;
                listenValueleft = 0;
            } 
            moveCharOnSlide(listenValueleft, listenValueright, leftDir, rightDir);
         
            //listen to the slowing down of right rotation and keep it rotating while sliders go to zero
            if (listenValueright>listenValueleft){
                    rotatebydegrees = listenValueright - listenValueleft; //same time, different direction, most power -> 1 - - 1 =  2, least power = 0 - 0 = 0 => 0 - 2
                    Vector3 rotationToAdd = new Vector3(0, rotatebydegrees, 0);
                    transform.Rotate(rotationToAdd);
                } else if (listenValueright<listenValueleft){
                    rotatebydegrees = listenValueleft - listenValueright;
                    Vector3 rotationToAdd = new Vector3(0, rotatebydegrees, 0);
                    transform.Rotate(rotationToAdd);
                }

        });
        rightSlider.onValueChanged.AddListener((valueRight)=>{
            rightSlidertext.text = valueRight.ToString("0.00"); //value of the string =  #of rotations/min
            listenValueright = valueRight;
            if (valueRight>0.01){
                rightDir =1;
            } else if (valueRight<-0.01){
                rightDir = -1;
            } else if (valueRight<0.01||valueRight>-0.01){
                rightDir = 0;
                listenValueright = 0;
            }
            moveCharOnSlide(listenValueleft, listenValueright, leftDir, rightDir);
        });
    } */
    void Update()
    {
        
        double rotatebydegrees = 0;  
        double moveForward = 0;
        moveForward = (valueRight + valueLeft) * speed ;
        rotatebydegrees = ((valueRight - valueLeft) * speed)  *radius; 
        
        Vector3 rotationToAdd = new Vector3(0, (float)(rotatebydegrees), 0);

        characterController.transform.Translate(new Vector3(0,0, (float)(moveForward)), Space.Self);
        transform.Rotate(rotationToAdd);

        if(valueRight > 0.05) {
            valueRight -= drag;
        } else if(valueRight < -0.05) {
            valueRight+= drag;
        } else {
            valueRight *= 0;
        }
        if(valueLeft > 0.05) {
            valueLeft -=drag;
        } else if(valueLeft < -0.05) {
            valueLeft+= drag;
        } else {
            valueLeft *= 0;
        }
            
        Debug.Log("valueLeft =" + valueLeft + "valueRight =" + valueRight);


        /*  if(Mouse.current.leftButton.isPressed){
            GetUIElementsClicked();
        }  */
     
        //--------------------------------------------------------------------------------------------------
        //new input action system -> keyboard input -> pass through - any || 1D axis: positive & negative  
        // actionValue = inputActionClass.ActionMap.Action.ReadValue<type>();
        /* leftwheel = wheelchairControls.LeftSliderInput.LeftWheelPush.ReadValue<float>();

        if (leftwheel > 0){
            slowDownLeftActive = false;
            Debug.Log("its registrating that W is being held");
            leftSlider.value += (Time.deltaTime);
            currentLeftSliderValue = leftSlider.value;
        }
         if (leftwheel == 0){
            slowDownLeftActive = true;
        }
        if (leftwheel < 0){
            slowDownLeftActive = false;
            Debug.Log("its registrating that S is being held");
            leftSlider.value -= (Time.deltaTime);
            currentLeftSliderValue = leftSlider.value;
        }

        rightwheel = wheelchairControls.RightSliderInput.RightWheelPush.ReadValue<float>();
       
        if (rightwheel > 0){
            slowDownRightActive = false;
            Debug.Log("its registrating that X is being held");
            rightSlider.value += (Time.deltaTime);
            currentRightSliderValue = rightSlider.value;
        }
         if (rightwheel == 0){
            slowDownRightActive = true;
        }
        if (rightwheel < 0){
            slowDownRightActive = false;
            Debug.Log("its registrating that D is being held");
            rightSlider.value -= (Time.deltaTime);
            currentRightSliderValue = rightSlider.value;
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
    // if the sliders are currently not zero then keep moving it at the # of rotations/minute it is set at
        if( currentLeftSliderValue != 0.00f || currentRightSliderValue != 0.00f){
             moveCharOnSlide(currentLeftSliderValue, currentRightSliderValue, leftwheel, rightwheel);
             Debug.Log("THE move function is being invoked");
             Debug.Log(currentLeftSliderValue + " = leftslider en rightslider = " + currentRightSliderValue);
        }
        //----------------------------------------------------------------------------------------------------------------
   
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
        } */
    }
    
    void moveCharOnSlide(float leftDir, float rightDir){
        // float rotatebydegrees = 0;  
        // float moveForward = 0;
        // moveForward = (valueRight + valueLeft) * speed ;
        // rotatebydegrees = (valueRight - valueLeft)*speed *radius; 
        
        // Vector3 rotationToAdd = new Vector3(0, rotatebydegrees, 0);

        // characterController.transform.Translate(new Vector3(0,0,moveForward), Space.Self);
        // transform.Rotate(rotationToAdd);


        /* if ((leftDir == 1 && rightDir == 1)||(leftDir == -1 && rightDir == -1)){
            // when both wheels turn in the same direction -> move forwards or backwards depending on direction
            if (valueRight == valueLeft){
                moveForward = valueRight + valueLeft;
                characterController.transform.Translate(new Vector3(0,0,moveForward), Space.Self);
            } else
            if(valueRight>valueLeft){
                moveForward = valueRight + valueLeft;
                characterController.transform.Translate(new Vector3(0,0,moveForward), Space.Self);
                Vector3 rotation = new Vector3(0, valueRight - valueLeft, 0);
                transform.Rotate(rotation);    
            } else
            if(valueRight<valueLeft){
                moveForward = valueRight + valueLeft;
                Vector3 rotation = new Vector3(0, valueLeft - valueRight, 0);
                transform.Rotate(rotation);     
            }   
        } else if ((leftDir == 1 && rightDir == -1)||(leftDir == -1 && rightDir == 1)){
            // when both wheels turn in a different direction -> move forwards or backwards depending on direction
            // if leftDir = +1 -> leftvalue = positive (cus otherwise message = CCL/CLW -> other leftDir )
            // if leftDir = -1 -> leftvalue = negative (cus otherwise message = CCL/CLW -> other leftDir )
            // if rightDir = +1 -> rightvalue = positive (cus otherwise message = CCL/CLW -> other leftDir )
            // if rightDir = -1 -> rightvalue = negative (cus otherwise message = CCL/CLW -> other leftDir )
            if (valueRight>valueLeft){ //valueright = always positive , valueLeft = always negative
                moveForward = (valueRight + valueLeft);
                characterController.transform.Translate(new Vector3(0,0,moveForward), Space.Self);
                rotatebydegrees = (valueRight + valueLeft); //same time, different direction, most power -> 1 - - 1 =  2, least power = 0 - 0 = 0 => 0 - 2
                Vector3 rotationToAdd = new Vector3(0, rotatebydegrees, 0);
                transform.Rotate(rotationToAdd);
                // Debug.Log(valueRight + "+" + valueLeft + "=" + moveForward); 
    
            } else if (valueRight<valueLeft){ //valueright = always negative , valueLeft = always positive
                moveForward = (valueLeft + valueRight);
                characterController.transform.Translate(new Vector3(0,0,moveForward), Space.Self);
                rotatebydegrees = (valueLeft - valueRight);
                Vector3 rotationToAdd = new Vector3(0, -rotatebydegrees, 0);
                transform.Rotate(rotationToAdd);    
                // Debug.Log(valueRight + "+" + valueLeft + "=" + moveForward);   
            }
        } else if ((leftDir == 0 || rightDir == 0)&&(leftDir != 0 || rightDir != 0)) {
            if(leftDir==0){
                moveForward = valueRight/2;
                characterController.transform.Translate(new Vector3(0,0,moveForward), Space.Self);
                rotatebydegrees = (valueRight - valueLeft);
                Vector3 rotationToAdd = new Vector3(0, rotatebydegrees, 0);
                transform.Rotate(rotationToAdd);        
            }
            if(rightDir==0){
                moveForward = valueLeft/2;
                characterController.transform.Translate(new Vector3(0,0,moveForward), Space.Self);
                rotatebydegrees = (valueLeft-valueRight);
                Vector3 rotationToAdd = new Vector3(0, -rotatebydegrees, 0);
                transform.Rotate(rotationToAdd);        
            }}
             */
     //    currentRightSliderValue = valueRight;
      //  currentLeftSliderValue = valueLeft;
        

    }


    /* void GetUIElementsClicked(){
        // get all UI elements clicked, in order of depth (1st detected is 1st) //
        click_data.position = Mouse.current.position.ReadValue();//grab mouse position & update click-data
        click_results.Clear(); //clear out previous results
        ui_raycaster.Raycast(click_data, click_results); //perform Raycast itself -> will return UI elements it comes into contact with

        foreach(RaycastResult result in click_results){
            GameObject ui_element = result.gameObject;
          //   if (ui_element.name == "Handle"){
           //     Debug.Log(ui_element.transform.position);
           // }
        }
    }
    void resetLeftPositiveSlider(){
         if (leftSlider.value != 0){
        leftSlider.value -= Time.deltaTime;
         if( leftSlider.value<0.01 || leftSlider.value>-0.01){
            leftSlider.value =0;
         }
         }
    }
    void resetLeftNegativeSlider(){
         if (leftSlider.value != 0){
        leftSlider.value += Time.deltaTime;
         if( leftSlider.value<0.01 || leftSlider.value>-0.01){
            leftSlider.value =0;
         }
         } 
    }
     void resetRightPositiveSlider(){
        if (rightSlider.value != 0){
        rightSlider.value -= Time.deltaTime;
        if( rightSlider.value<0.01 || rightSlider.value>-0.01){
            rightSlider.value =0;
         }
         }
    }
    void resetRightNegativeSlider(){
         if (rightSlider.value != 0){
        rightSlider.value += Time.deltaTime;
        if( rightSlider.value<0.01 || rightSlider.value>-0.01){
            rightSlider.value =0;
         }
         } 
    } */

}
