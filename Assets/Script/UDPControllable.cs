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

public class UDPControllable : MonoBehaviour
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
    public UnityEvent<Vector2> onMove, onLook;
    public UnityEvent<int> onUIMove;
    public UnityEvent<bool> onUIAccept;
    public bool isActive, isCameraActive;

    /// <summary>
    ///  NOT REALLY NEEDED AS OF NOW, except for selectPanel, we need it for OnGestureInput_
    /// </summary>
    // Player GameObj
    public GameObject player;
    // SelectPanel GameObj (and probably future UI Elements)
    public GameObject selectPanel;

    // Variables to allow user to use keyboard when UDP is running
    /* EXPLANATIONS HERE
     Khi UDP chạy, nó sẽ nhận DL liên tục. Hàm Update có hàm OnGestureInput_ nhận DL của UDP về liên tục VÀ liên tục Invoke OnMove và OnLook event.
    - OnMove và OnLook sẽ gọi các actions (hàm) SetMoveVector và SetLookVector (Movable.cs) liên tục, cả 2 hàm sẽ đc nhận gtri liên tục từ UDP và OnGestureInput_
    - Khi đó, hàm Move() và Rotate() (Movable.cs) sẽ nhận gtri LIÊN TỤC - NON-STOP
    - Ta vẫn có thể sử dụng phím để di chuyển, OnMoveInput() (Controllable.cs) vẫn sẽ được nhận, OnMove event vẫn sẽ Invoke hàm Move() nhưng vì UDP truyền DL nhanh và
    liên tục nên DL từ phím chưa kịp đực xử lý thì cái của UDP được ưu tiên xử lý trước
    --> Cần có cách tạm thời ngưng hàm OnGestureInput_ khi ta nhập phím.

    Cụ thể hơn: Flow của công việc
    + Mỗi lần ta nhấn phím liên quan tới Move (WASD) thì Player Input (component của Player GameObj) sẽ gọi event Move
    + Move sẽ gọi/Invoke hàm SetKBInput(), cho biến isKBInput = true và KBInputTime = Tgian hiện tại lúc nhập phím (Time.time - ko sd Time.realTimeSinceStartup vì nó sẽ cộng dồn) + Tgian chờ thêm input phím (TimeToWait)
    + Khi isKBInput == true thì hàm OnGestureInput_ sẽ dừng và return. ==> DL sẽ ko đc nhận liên tục, OnMove và OnLook sẽ ko gọi SetMoveVector và SetLookVector liên tục, ta sd đc phím để
    di chuyển nhân vật
    + TA XÉT THEO THỜI ĐIỂM ĐỂ CẬP NHẬT isKBInput (ta không để nó = true hoài đc vì nó sẽ chặn OnGestureInput luôn):
        - Trong Update(), xét nếu Time.time (tgian hiện tại) > KBInputTime (thời điểm nhấn phím + TimeToWait(seconds)) thì isKBInput = false
        - Nếu trong khoảng thời gian đó mà ta nhấn thêm 1 phím thì BƯỚC 1 đc lặp lại, isKBInput vẫn = true, KBInputTime hiện giờ = tgian hiện tại (mới) + TimeToWait
    
    ==> Ta giải quyết được 1 phần của vấn đề, timer chặn OnGestureInput_ sẽ reset mỗi khi ta NHẤN PHÍM; Vậy khi ta GIỮ PHÍM thì sao...? timer nó vẫn chỉ tính ở mốc khi ta NHẤN PHÍM, nếu ta giữ
    phím hơn TimeToWait thì nó sẽ quay về OnGestureInput_ (UDP) xong nó quay lại Keyboard Input cái nữa.

    Cách giải quyết đơn giản:
    (Line 125) Xét nếu ifKeyboardInput == true VÀ moveVector (trong Player Obj - Movable.cs) != Vector2.zero THÌ:
    + Cho KBInputTime = Tgian hiện tại lúc nhập phím (Time.time) + Tgian chờ thêm input phím (TimeToWait)
    + Cho elapsedKBTime = TimeToWait - reset lại UI timer

    Vì sao xét moveVector != V2.zero: 
    - Khi ta di chuyển, hàm ReadValue<Vector2> (của Input System) sẽ đọc alue của người chơi. Nếu người chơi không nhấn phím để di chuyển thì 
    ReadValue<Vector2> sẽ return giá trị mặc định của Vector2, là V2.zero.
    - Sau đó, OnMoveInput vẫn sẽ lụm context (có V2.zero) để chạy 1 lần, và event OnMove sẽ Invoke các hàm actions bằng cái V2.zero đúng 1 lần
     */
    // Bool for KBInput
    [HideInInspector]
    public bool isKeyboardInput;

    // Current point of time + Wait time
    private float KBInputWaitTime;

    // Wait time (to get more keyboard inputs)
    public float TimeToWait;

    // Is used for displaying in the UI
    [HideInInspector]
    public float elapsedKBTime;

    //--------------------------
    // Rewriting data converted from label (to make it work with the UI input)
    [HideInInspector]
    public ConvertedLabel clInput;
    private int oldPanelNum;

    private void Awake()
    {
        player = GameObject.Find("Player");
        selectPanel = GameObject.Find("SelectItemPanel");
    }

    // Start is called before the first frame update
    void Start()
    {
        //isCameraActive = GetComponent<Controllable>().isCameraActive;
        InitUDPSocket();
        isKeyboardInput = false;
        clInput = new ConvertedLabel();
    }

    // Update is called once per frame
    void Update()
    {
        oldPanelNum = -(int)clInput.DirectionInput.y;
        clInput.LabelConverter(dataReceived);
        //OnGestureInput(LabelConverter(dataReceived));
        
        // Nếu thời điểm hiện tại > Thời điểm dùng để chờ thêm KB Input ==> isKBInput = false, sd lại đc UDP
        //if (Time.time > KBInputWaitTime)
        //{
        //    isKeyboardInput = false;
        //}

        OnGestureInput_(clInput);
    }

    private void FixedUpdate()
    {
        //oldPanelNum = -(int)clInput.DirectionInput.y;
        //clInput.LabelConverter(dataReceived);
        //OnGestureInput(LabelConverter(dataReceived));

        // Sử dụng hiển thị cho UI
        if (elapsedKBTime > 0)
            elapsedKBTime -= Time.deltaTime;

        // Thêm trường hợp cho việc người chơi NHẤN GIỮ nút di chuyển thay vì nhấn (Khi NHẤN GIỮ, KBInputTime sẽ ko đc cập nhật --> fix cho nó cập nhật lại)
        if (isKeyboardInput && player.GetComponent<Movable>().moveVector != Vector2.zero)
        {
            KBInputWaitTime = Time.time + TimeToWait;
            print(KBInputWaitTime);
            elapsedKBTime = TimeToWait;
        }

        // Nếu thời điểm hiện tại > Thời điểm dùng để chờ thêm KB Input ==> isKBInput = false, sd lại đc UDP
        if (Time.time > KBInputWaitTime)
        {
            isKeyboardInput = false;
            InputSystem.DisableDevice(Mouse.current); // Time-out then disable mouse input
        }

        //OnGestureInput_(clInput);
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
                //print("Data Received: " + LabelConverter(dataReceived));
                print("Data Received: " + dataReceived);

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

    /// ----- REDUNDANT FUNC START ----- ///
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
        if (isActive == false)
        {
            return;
        }

        if (selectPanel.activeSelf)
        {
            // Create funcs like that of SetMoveVector and SetLookVector
            // Vector2 value = context.ReadValue<Vector2>();
            //Debug.Log("Value: " + value);
            //ChangeIndex(-(int)value.y);
            // Change index takes in a "step", which is either int 1 or -1, with each number allowing the UI (currentIndex)
            // to increment/decrement - move down/up
           // Maybe we can use Vector2.y to our advantage

            // For ConfirmSelect, we might need to write a new similar func that takes in a...string??? or bool, idk
        }
        else
        {
            onMove.Invoke(inputVector);
            // Find out why whenever V2.zero, character always face forward
            if (inputVector != Vector2.zero)
                onLook.Invoke(inputVector);
        }
    }
    /// ----- REDUNDANT FUNCS END ----- ///

    public void SetKeyboardInput()
    {
        if (!isActive)
        {
            return;     // If UDP Controller is not active then there's not really any point of, well, calling this function to begin with.
        }
        isKeyboardInput = true;
        InputSystem.EnableDevice(Mouse.current); // You forgot to enable the mouse

        // Thời điểm để xét có thêm input từ Keyboard hay không
        //KBInputWaitTime = Time.realtimeSinceStartup + TimeToWait;
        KBInputWaitTime = Time.time + TimeToWait;
        print(KBInputWaitTime);
        
        // Giá trị time để hiển thị trên UI
        elapsedKBTime = TimeToWait;
    }

    public void OnGestureInput_(ConvertedLabel input)
    {
        if (isActive == false || isKeyboardInput == true)   // Added isKeyboardInput
        {
            return;
        }

        print("OnGestureInput_ running");
        if (selectPanel.activeSelf == true)
        {
            if (oldPanelNum != -(int)input.DirectionInput.y)
            {
                onUIMove.Invoke(-(int)input.DirectionInput.y);
            }
            if (input.UIConfirmSelect == true)
            {
                onUIAccept.Invoke(input.UIConfirmSelect);
            }
        }
        
        if (selectPanel.activeSelf == false && player.activeSelf == true)
        {
            onMove.Invoke(input.DirectionInput);
            // Find out why whenever V2.zero, character always face forward
            if (input.DirectionInput != Vector2.zero)
                onLook.Invoke(input.DirectionInput);
        }
    }

    private void OnEnable()
    {
        if (isActive)
            InputSystem.DisableDevice(Mouse.current);
    }

    private void OnDisable()
    {
        InputSystem.EnableDevice(Mouse.current);
    }

    //public void Active()
    //{
    //    InputSystem.DisableDevice(Mouse.current);
    //}
    //public void Disable()
    //{
    //    InputSystem.EnableDevice(Mouse.current);
    //}

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

[System.Serializable]
public class ConvertedLabel
{
    public Vector2 DirectionInput;
    public bool UIConfirmSelect;

    public ConvertedLabel()
    {
        DirectionInput = Vector2.zero;
        UIConfirmSelect = false;
    }

    public ConvertedLabel(Vector2 v2, bool confirm)
    {
        DirectionInput = new Vector2(v2.x, v2.y);
        UIConfirmSelect = confirm;
    }

    public void LabelConverter(string label)
    {
        Vector2 defaultV2 = Vector2.zero;
        bool defaultBool = false;
        switch (label)
        {
            case "UP":
                DirectionInput = Vector2.up;
                UIConfirmSelect = false;
                break;
            case "DOWN":
                DirectionInput = Vector2.down;
                UIConfirmSelect = false;
                break;
            case "LEFT":
                DirectionInput = Vector2.left;
                UIConfirmSelect = false;
                break;
            case "RIGHT":
                DirectionInput = Vector2.right;
                UIConfirmSelect = false;
                break;
            case "IDLE":
                DirectionInput = Vector2.zero;       // This is unnecessary, I'm adding this to make the logic look uniform
                UIConfirmSelect = false;
                break;
            case "CONFIRM":
                DirectionInput = Vector2.zero;
                UIConfirmSelect = true;
                break;
            // v2convert =  new Vector2(Mathf.Sin(facingAngle * Mathf.Deg2Rad), Mathf.Cos(facingAngle * Mathf.Deg2Rad));
            default:
                DirectionInput = defaultV2;
                UIConfirmSelect = defaultBool;
                break;
        }

    }
}