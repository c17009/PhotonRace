using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Photon.MonoBehaviour {
    public float speed;
    PhysicMaterial PhysicMate;
    float currentspeed;
    public GameObject gamemanager;
    Transform cameraObj;
    Vector3 Camforword;
    Vector3 ido;
    Vector3 startpos;
    Rigidbody rb;
    private bool isLocal;
    private PhotonView photonview;

    void Start()
    {
        startpos = transform.position;
        rb = GetComponent<Rigidbody>();
        cameraObj = Camera.main.transform;
        PhysicMate = GetComponent<Collider>().material;
        photonview = GetComponent<PhotonView>();
        isLocal = photonview.isMine;
        if (isLocal)
        {
            cameraObj.position = new Vector3(startpos.x - 8, startpos.y + 5, startpos.z);
            cameraObj.Rotate(0, 90, 0);
            cameraObj.parent = transform;
        }
    }


    void Update() {
        if (!isLocal) { return; }
        if (transform.position.y <= -50)
        {
            CmdReSpawn();
        }
    }

    private void FixedUpdate()
    {
        if (!isLocal) { return; }
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        bool Down = Input.GetButton("Space");
        bool Up = Input.GetButtonUp("Space");
        currentspeed = rb.velocity.magnitude;
        //Camforword = Vector3.Scale(cameraObj.forward, new Vector3(1, 0, 1)).normalized;
        //ido = y * Camforword + x * cameraObj.right;
        //print(y + ":" + ido);
        //print(rb.velocity.magnitude);
        if (transform.position.y > 20) CmdDsn();//Playerの高さ制限
        CmdMovePlayer(x,y,Down,Up,currentspeed);
    }

    public void CmdMovePlayer(float x,float y,bool spaceDown,bool spaceUp,float csp)//左右、spaceの入力、現在の速度
    {
        if (x != 0)
        {
            transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime * x, Space.Self);//旋回
            //rb.AddRelativeForce(0, 0, speed / 2 * x);
        }
        if(rb.velocity.magnitude > 50) { return; }
        rb.AddRelativeForce(speed * y, 0, 0);//オートアクセル

        if (spaceDown && y > 0)
        {
            rb.AddRelativeForce(-csp , -csp, 0);//ブレーキ
        }

        if (spaceUp)
        {
            rb.AddRelativeForce(speed * 1.5f, 0, 0, ForceMode.Impulse);//加速
        }
    }

    private void CmdDsn()
    {
        Vector3 v = transform.position;
        v.y = 20;
        transform.position = v;
    }

    private void OnTriggerEnter(Collider collision)//ホッパーに触れたとき
    {
        if (!isLocal) { return; }

        if (collision.gameObject.tag == "Hooper")
        {
            CmdHop();
        }
        if (collision.gameObject.tag == "Upper")
        {
            CmdSpeedUp();
        }
    }


    private void CmdHop()
    {
        GetComponent<Rigidbody>().AddForce(0, 1000, 0);
    }

    private void CmdReSpawn()
    {
        transform.position = startpos;
    }

    private void CmdSpeedUp()
    {
        rb.AddRelativeForce(speed * 500, 0, 0);
        print("Upper!!!");
    }
}
