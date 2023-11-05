using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UDPControllable_old : MonoBehaviour
{
    // This script will be put in Game Controller or sth

    // UDP Variables
    Thread receiveThread;
    UdpClient udpClient;
    public readonly int port = 27001;

    private string dataReceived = "EMPTY";
    public Vector2 moveVector, lookVector;

    // Unity Event vars
    // With these events, we probably don't even need to use the 2 below variables to refer to 
    public UnityEvent<Vector2> onMoveUDP, onLookUDP;
    public bool isActive, isCameraActive;

    // Player GameObj
    public GameObject player;
    // SelectPanel GameObj (and probably future UI Elements)
    public GameObject selectPanel;

    // Boolean to see if there's any keyboard input
    [HideInInspector]
    public bool isKBInput;

    private void Awake()
    {
        player = GameObject.Find("Player");
        selectPanel = GameObject.Find("SelectItemPanel");
    }

    // Start is called before the first frame update
    void Start()
    {
        //isCameraActive = GetComponent<Controllable>().isCameraActive;
        isKBInput = false;
        InitUDPSocket();
    }

    // Update is called once per frame
    void Update()
    {
        //isKBInput = false;
        OnGestureInput(LabelConverter(dataReceived));
    }

    private void InitUDPSocket()
    {
        print("UDP Initialized");

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        /*
         Background threads are identical to foreground threads, except that background threads do not prevent a process from terminating.
         Once all foreground threads belonging to a process have terminated, the common language runtime ends the process. 
         Any remaining background threads are stopped and do not complete.
         */
        receiveThread.Start();

        // isCameraActive = true;
    }

    private void ReceiveData()
    {
        udpClient = new UdpClient(port);
        // Change while true to while client != null
        // Turn off client first before terminating thread in OnDestroy
        while (udpClient != null)
        {
            try
            {
                // IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port); //0.0.0.0 is the equivalent of ANY IP
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);
                byte[] buffer = udpClient.Receive(ref anyIP);          // Getting UDP Data in Bytes from Python

                // When connection is closed, buffer will receive ""
                if (buffer == null)
                    return;

                dataReceived = Encoding.UTF8.GetString(buffer);     // Convert Bytes into String
                print("Data Received: " + LabelConverter(dataReceived));

                // You can't call it here since this func belongs to the background thread.
                // Unity's Invoke can only be called from the MAIN thread.
                // Moving this to the Update() solves the issue, since this is similar to the Main Thread Dispatcher pattern
                //OnGestureInput(LabelConverter(dataReceived));

                //isCameraActive = true;
            }
            catch (Exception err)
            {
                print(err);
                if (udpClient != null)
                    print("UDP Socket Exception Error: " + err);
                else
                    print("Thread is about to terminate . . .");
            }
        }
    }

    private Vector2 LabelConverter(string label)
    {
        Vector2 v2convert = new Vector2(0f, 0f);
        switch(label)
        {
            case "UP":
                v2convert = Vector2.up;
                break;
            case "DOWN":
                v2convert = Vector2.down;
                break;
            case "LEFT":
                v2convert = Vector2.left;
                break;
            case "RIGHT":
                v2convert = Vector2.right;
                break;
            case "IDLE":
                v2convert = Vector2.zero;       // This is unnecessary, I'm adding this to make the logic look uniform
                break;

            // v2convert =  new Vector2(Mathf.Sin(facingAngle * Mathf.Deg2Rad), Mathf.Cos(facingAngle * Mathf.Deg2Rad));
            default:
                break;
        }

        return v2convert.normalized;
    }

    public void OnGestureInput(Vector2 inputVector)
    {
        if ((isActive == false) || (isKBInput == true))
        {
            
            return;
        }

        //if (selectPanel.activeSelf)
        //{
        //    // Create funcs like that of SetMoveVector and SetLookVector
        //    // Vector2 value = context.ReadValue<Vector2>();
        //    //Debug.Log("Value: " + value);
        //    //ChangeIndex(-(int)value.y);
        //    // Change index takes in a "step", which is either int 1 or -1, with each number allowing the UI (currentIndex)
        //    // to increment/decrement - move down/up
        //   // Maybe we can use Vector2.y to our advantage

        //    // For ConfirmSelect, we might need to write a new similar func that takes in a...string??? or bool, idk
        //}
        
        onMoveUDP.Invoke(inputVector);
        // Find out why whenever V2.zero, character always face forward
        if (inputVector != Vector2.zero)
            onLookUDP.Invoke(inputVector);
        //if (selectPanel.activeSelf == false && player.activeSelf == true)
        //{
        //}
    }

    public void SetKeyboardInput()
    {
        isKBInput = true;
    }

    private void OnEnable()
    {
        InputSystem.EnableDevice(Keyboard.current);
        if (isActive)
            InputSystem.DisableDevice(Mouse.current);
    }

    private void OnDisable()
    {
        InputSystem.EnableDevice(Keyboard.current);
        InputSystem.EnableDevice(Mouse.current);
    }

    public void Active()
    {
        InputSystem.DisableDevice(Mouse.current);
    }
    public void Disable()
    {
        InputSystem.EnableDevice(Mouse.current);
    }

    private void OnDestroy()
    {
        //isCameraActive = false;
        if (udpClient != null)
        {
            udpClient.Close();

            udpClient = null;
        }
        // Client must be TURNED OFF before CLOSING THREAD
        if (receiveThread.IsAlive || receiveThread != null)
        {
            if (receiveThread.Join(100))
            {
                print("UDP Thread has closed successfully - OnDestroy");
            }
            else
            {
                print("UDP Thread did not close in 100ms, abort - OnDestroy");
                receiveThread.Abort();
            }
            //receiveThread.Abort();
            ////receiveThread.Join();

            receiveThread = null;

        }

    }
}
