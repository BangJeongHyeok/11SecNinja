using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class CharacterControl : MonoBehaviour
{
    //Component
    [SerializeField] Camera mainCamera;
    Rigidbody2D rigid;
    LineRenderer lineRenderer;

    //Data
    Vector2 CurDir = Vector2.zero;
    [SerializeField] float MoveSpeed = 0;
    [SerializeField] float ReflectPower = 0;

    //더블탭 딜레이
    [SerializeField] float TouchDelay = 0;
    float CurDelay = float.NegativeInfinity;
    bool SlashTime = false;//썰어버리기
    bool Slashed = false;//썰어버린 직후
    Vector3 SlashOriginPos;//Slash 시작지점



    void ComponenetSetting()
    {
        rigid = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        ComponenetSetting();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ScreenColliderCheck();
    }
    private void Update()
    {
        CharacterInput();
    }

    void MouseDownAction()
    {
        if (Slashed)
            Slashed = false;
        else if (Time.time - CurDelay <= TouchDelay)
        {
            SlashTime = true;
            SlashOriginPos = Input.mousePosition;
            Vector3 FixedPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.SetPosition(0, new Vector3(FixedPos.x, FixedPos.y, 0));
        }
    }

    void MouseAction()
    {
        if (SlashTime)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale,0.2f,12f * Time.deltaTime);
            Vector3 FixedPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.SetPosition(1, new Vector3(FixedPos.x, FixedPos.y, 0));
        }
        else
        {
            CurDir = (mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            rigid.AddForce(CurDir * MoveSpeed * Time.deltaTime);
        }
    }

    void MouseUpAction()
    {
        if (SlashTime)//제드타임이면
        {
            Time.timeScale = 1f;
            rigid.AddForce((Input.mousePosition - SlashOriginPos).normalized * 50, ForceMode2D.Impulse);

            Vector3 FixedPos = mainCamera.ScreenToWorldPoint(SlashOriginPos);
            lineRenderer.SetPosition(1, new Vector3(FixedPos.x, FixedPos.y, 0));
            SlashTime = false;
            Slashed = true;
        }

        CurDelay = Time.time;
    }

    void CharacterInput()
    {

        if (Input.GetMouseButtonDown(0))
            MouseDownAction();

        if (Input.GetMouseButton(0))//터치하고 있다면
            MouseAction();
        else
            CurDir = Vector2.zero;

        if (Input.GetMouseButtonUp(0))
            MouseUpAction();

        transform.Rotate(rigid.velocity);
        Debug.Log(SlashTime);
    }


    bool ScreenColliderCheck()
    {
        Vector3 ViewportTransform = mainCamera.WorldToViewportPoint(transform.position);
        if (ViewportTransform.x <= 0 || ViewportTransform.x >= 1 || ViewportTransform.y <= 0 || ViewportTransform.y >= 1)//화면밖으로 넘어가려고 하면
        {
            //화면 중심으로 튕겨낸다
            ViewportTransform = new Vector2(Mathf.Clamp(ViewportTransform.x,0,1), Mathf.Clamp(ViewportTransform.y, 0, 1));
            Vector3 ConvertedPosition = mainCamera.ViewportToWorldPoint(ViewportTransform);
            ConvertedPosition = new Vector3(ConvertedPosition.x, ConvertedPosition.y,0);
            transform.position = ConvertedPosition;
            rigid.velocity = Vector3.zero;
            rigid.AddForce((Vector3.zero - transform.position).normalized * ReflectPower, ForceMode2D.Impulse);
            return true;
        }
        return false;
    }
}
