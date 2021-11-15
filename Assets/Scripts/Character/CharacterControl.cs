using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class CharacterControl : MonoBehaviour
{
    //Component
    [SerializeField] Camera mainCamera;
    Rigidbody2D rigid;

    //Data
    Vector2 CurDir = Vector2.zero;
    [SerializeField] float MoveSpeed = 0;
    [SerializeField] float ReflectPower = 0;



    void ComponenetSetting()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        ComponenetSetting();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CharacterInput();
        ScreenColliderCheck();
    }

    void CharacterInput()
    {
        if (Input.GetMouseButton(0))//터치하고 있다면
        {
            CurDir = (mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            rigid.AddForce(CurDir * MoveSpeed);
        }
        else
            CurDir = Vector2.zero;

        transform.Rotate(rigid.velocity);
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
