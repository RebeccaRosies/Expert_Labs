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
/*     [SerializeField] private UnityEngine.UI.Slider leftSlider;
    [SerializeField] private TextMeshProUGUI leftSlidertext;
    [SerializeField] private UnityEngine.UI.Slider rightSlider;
    [SerializeField] private TextMeshProUGUI rightSlidertext; */
  //---------------------------------------------------------------//
    public GameObject ui_canvas;
    GameObject ui_element;
    GraphicRaycaster ui_raycaster; // initialize a graphic raycaster -> 17: grab graphicraycaster from ui_canvas 
    PointerEventData click_data;
    List<RaycastResult> click_results;
    WheelchairControls wheelchairControls;

    public float speed;
    public float radius;

    double leftDir;
    double rightDir;

    //ARDITY VARIABLES
     string ticksString;
    float ticks;
    //float previousTicks = 0;
    string msg;

    double valueLeft;
    double valueRight;

    double drag = 0.005;
    //public double inputMulti = 0.01;

    //ARDUINO 2 MESSAGES
    bool reset;
    wheel previousMsg;
    wheel currentMsg;

    float timer = 0;

    public GameObject RightWheel;
    public GameObject LeftWheel;

    double moveForward;
    double rotatebydegrees;
    float rotateAroundByDegrees;
    bool timerStarted;
    private void Awake(){
        //new input action system -> keyboard input 
        wheelchairControls = new WheelchairControls();  
        GameObject RightWheel = GameObject.Find("RightWheel");
        GameObject LeftWheel = GameObject.Find("LeftWheel");
    }
    public class wheel
    {
        public double value;
        public bool isLeftWheel;
        public wheel(double value, bool isLeftWheel)
        {
            this.value = value;
            this.isLeftWheel = isLeftWheel;
        }
    }

    void Start()
    {      
        if(speed <=0){
            speed = 1;
            // can't be 0 because you can't divide by 0 
        } 

        if (radius<=0){
            radius=1;
        }
        
     /*    leftSlider.minValue = -1;
        leftSlider.maxValue = 1;
        rightSlider.minValue = -1;
        rightSlider.maxValue = 1; */

        characterController = GetComponent<CharacterController>();
        //listenToChanges();
        
        //----------GRAPHICRAYCASTER---------------------------------------------------//
         ui_raycaster = ui_canvas.GetComponent<GraphicRaycaster>(); //Add Raycaster so you don't have to get it from canvas every single frame
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>(); 
    }
     void OnMessageArrived(string msg)
    {
        messageDecoder(msg);
        Debug.Log("Message arrived in movementScript: " + msg);

    }

     void OnConnectionEvent(bool success)
    {
     if (success)
            Debug.Log("Connection established");
        else
            Debug.Log("Connection attempt failed or disconnection detected");
    }

     void messageDecoder(string msg){
        
        // if (timer <= 100){
        previousMsg = currentMsg; // currentmsg schuift op en word de previousmsg
          //} 
        timer = 0;
        timerStarted = true;
      
        if (reset == true){
        previousMsg = null;
        timerStarted = false;
        }

        if(msg != null ){
            if(msg[0] == 'O'){
                if(msg[4] == 'L'){
                    ticksString = msg.Remove(0,6); //messages are in the form: OneCCW-1| TwoCLW336 -> the first 2 items are always 6 chars
                    ticks = float.Parse(ticksString);
                    /* if (ticks > 0){
                         valueRight = -0.01;
                    } else if (ticks < 0){
                         valueRight = 0.01;
                    } */
                    valueRight = -0.1;
                    //valueRight = ticks/10;
                    rightDir = 1;
                    Debug.Log(valueRight);
                } 
                if(msg[4] == 'C'){
                    ticksString = msg.Remove(0,6); //messages are in the form: OneCCW1| TwoCLW336 -> the first 2 items are always 6 chars
                    ticks = float.Parse(ticksString);
                    valueRight = 0.1;
                    //valueRight =  ticks/10;
                    rightDir = 1;
                    Debug.Log(valueRight);
                } 

                currentMsg = new wheel(valueRight, false);
            }
            if(msg[0] == 'T'){
                if(msg[4] == 'L'){
                    ticksString = msg.Remove(0,6); //messages are in the form: OneCCW34 | TwoCLW336 -> the first 2 items are always 6 chars
                    ticks = float.Parse(ticksString);
                    valueLeft = 0.1;
                    //valueLeft = ticks/10;
                    leftDir = 1;
                    Debug.Log(valueLeft);
                } 
                if(msg[4] == 'C'){
                    ticksString = msg.Remove(0,6); //messages are in the form: OneCCW34 | TwoCLW336 -> the first 2 items are always 6 chars
                    ticks = float.Parse(ticksString);
                    valueLeft = -0.1;
                    //valueLeft = ticks/10;
                    leftDir = 1;
                    Debug.Log(valueLeft);
                } 
                currentMsg = new wheel(valueLeft, true);
            }
           // previousTicks = ticks;
            Debug.Log("valueLeft =" + valueLeft + "valueRight =" + valueRight + "leftDir =" + leftDir + "RightDir =" + rightDir);
        movement();
        
        }
    }
   
    void Update()
    {
        //double rotatebydegrees = 0;  
        //double moveForward = 0;
        if (timerStarted == true){
            timer += Time.deltaTime;
        }
        reset = false;
        if (timer >= 10){
            reset = true;
        }

        //moveForward = (valueRight + valueLeft) * speed ;
        //rotatebydegrees = ((valueRight - valueLeft) * speed)  *radius; 
        //Vector3 rotationToAdd = new Vector3(0, (float)(rotatebydegrees), 0);

        //characterController.transform.Translate(new Vector3(0,0, (float)(moveForward)), Space.Self);
        //transform.Rotate(rotationToAdd);
        

        if(valueRight > 1) {
            valueRight -= drag;
        } else if(valueRight < -1) {
            valueRight+= drag;
        } else {
            valueRight *= 0;
        }
        if(valueLeft > 1) {
            valueLeft -=drag;
        } else if(valueLeft < -1) {
            valueLeft+= drag;
        } else {
            valueLeft *= 0;
        }
    

        slowDown();
        //Debug.Log("rotateAroundByDegrees =" + rotateAroundByDegrees + "rotatebydegrees =" + rotatebydegrees + "moveForward =" + moveForward);
        //Debug.Log("valueLeft =" + valueLeft + "valueRight =" + valueRight);   
    }

    void movement(){
        if(previousMsg == null && currentMsg != null){
                    rotateAround(currentMsg.isLeftWheel, currentMsg.value);     // there havent been 2 messages  -> always rotationaround (0,1), (0,-1), (1,0) or (-1,0)
                }
        if(previousMsg != null){                                        // there've been 2 messages 
            if(previousMsg.isLeftWheel != currentMsg.isLeftWheel){          //there's a message from the left and right
                if(previousMsg.value == currentMsg.value){                      //they match -> move forward in direction of value
                    move(previousMsg.value, currentMsg.value);
                    Debug.Log("previousMsg.value "+ previousMsg.value + "currentMsg.value "+  currentMsg.value);
                    slowDown();
                };
                if(previousMsg.value != currentMsg.value){                      //they don't match -> rotation around self in direction of positive wheel 
                    rotate(previousMsg.isLeftWheel, previousMsg.value, currentMsg.isLeftWheel, currentMsg.value);
                    slowDown();
                }
            }
            if(previousMsg.isLeftWheel == currentMsg.isLeftWheel){          //there's 2 left messages or 2 right
                if(previousMsg.value == currentMsg.value){                      //they match -> rotate twice around left or right in direction of value
                    rotateAround(currentMsg.isLeftWheel, currentMsg.value*2);
                    slowDown();
                };
                if(previousMsg.value != currentMsg.value){                      //they don't match -> first rotate around direction of previous value then current value
                    rotateAround(currentMsg.isLeftWheel, previousMsg.value);
                    rotateAround(currentMsg.isLeftWheel, currentMsg.value);
                    slowDown();
                }
            }
        }
    }

    void slowDown(){

        Debug.Log("moveForward begin slowdown=" + moveForward);
        if (moveForward > 0){
            // Debug.Log("moveForward is bigger than 0");
            moveForward -= Time.deltaTime;
            // Debug.Log("moveForward " + moveForward);
            if( moveForward<0.01 || moveForward>-0.01){
                // Debug.Log("moveForward is being set to 0");
                moveForward = 0;
            }
         }
         if (moveForward < 0){
            // Debug.Log("moveForward is smaller than 0");
            moveForward += Time.deltaTime;
            if( moveForward<0.01 || moveForward>-0.01){
                //  Debug.Log("moveForward is being set to 0");
                moveForward = 0;
            }
         } 
         if (rotatebydegrees > 0){
        rotatebydegrees -= Time.deltaTime;
            if( rotatebydegrees<0.01 || rotatebydegrees>-0.01){
                rotatebydegrees = 0;
            }
         }
    
         if (rotatebydegrees < 0){
        rotatebydegrees += Time.deltaTime;
            if( rotatebydegrees<0.01 || rotatebydegrees>-0.01){
                rotatebydegrees =0;
            }
         } 

          if (rotateAroundByDegrees > 0){
        rotateAroundByDegrees -= Time.deltaTime;
            if( rotateAroundByDegrees<0.01 || rotateAroundByDegrees>-0.01){
                rotateAroundByDegrees = 0;
            }
         }
    
         if (rotateAroundByDegrees < 0){
        rotateAroundByDegrees += Time.deltaTime;
            if( rotateAroundByDegrees<0.01 || rotateAroundByDegrees>-0.01){
                rotateAroundByDegrees =0;
            }
         } 
         Debug.Log("moveForward end slowdown =" + moveForward);
    }
    void rotateAround(bool left, double value){
        //rotate character around 
        // 1st variable = target around which its gonna spin (depends on direction (left = true dan rightwheel))
            //= public gameobject(Rightwheel) -> gameobject.transform.position 
            //= public gameobject(leftwheel) -> gameobject.transformposition
        //2nd variable = Vector3.(0,0,1).space(self)? 
        //3rd variable = degrees per second -> value * Time.deltaTime
     
        rotateAroundByDegrees = (float)(value*10000) * Time.deltaTime;
        //Debug.Log("rotateAroundByDegrees = " + rotateAroundByDegrees );
        if (left){
            transform.RotateAround(RightWheel.transform.position, Vector3.down, (-rotateAroundByDegrees/1));
        } else if (!left){
            transform.RotateAround(LeftWheel.transform.position, Vector3.down, rotateAroundByDegrees/1);
        }
        
    }
    void rotate(bool left, double valuePrevious, bool right, double valueCurrent){
        //double rotatebydegrees = 0;
        rotatebydegrees = ((valueRight - valueLeft) * (speed))  *radius;
        Vector3 rotationToAdd = new Vector3(0, (float)(rotatebydegrees), 0);
        transform.Rotate(rotationToAdd);
        
    }

    void move(double previousValue , double currentValue){
        //double moveForward = 0;
        moveForward = (previousValue + currentValue) * (speed) ;
        Debug.Log("previousValue =" + previousValue + "currentValue =" + currentValue + "moveForward = " + moveForward);
        characterController.transform.Translate(new Vector3(0,0, (float)(moveForward)), Space.Self);
        
    }

}
