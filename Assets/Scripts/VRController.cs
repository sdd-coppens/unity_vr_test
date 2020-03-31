using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRController : MonoBehaviour
{
    public float m_Gravity = 30.0f;
    public float m_RotateIncrement = 60;
    public float m_Sensitivity = 0.1f;
    public float m_MaxSpeed = 1.0f;

    public SteamVR_Action_Boolean m_SnapRotateButton = null;
    public SteamVR_Action_Boolean m_RotatePress = null;
    public SteamVR_Action_Boolean m_MovePress = null;
    public SteamVR_Action_Vector2 m_MoveValue = null;
    
    private float m_Speed = 0.0f;

    private CharacterController m_CharacterController = null;
    private Transform m_CameraRig = null;
    private Transform m_Head = null;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        m_CameraRig = SteamVR_Render.Top().origin;
        m_Head = SteamVR_Render.Top().head;
    }

    private void Update()
    {
        HandleHeight();
        CalculateMovement();
        SnapRotation();
    }

    private void HandleHeight()
    {
        // get the head in local space
        float headHeight = Mathf.Clamp(m_Head.localPosition.y, 1, 2);
        m_CharacterController.height = headHeight;

        // cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = m_CharacterController.height / 2;
        newCenter.y += m_CharacterController.skinWidth;

        // move capsule in local space
        newCenter.x = m_Head.localPosition.x;
        newCenter.y = m_Head.localPosition.y;

        // apply
        m_CharacterController.center = newCenter;
    }

    private void CalculateMovement()
    {
        
            // figure out movement orientation
            Quaternion orientation = CalculateOrientation();
            Vector3 movement = Vector3.zero;

        // if not moving
        //if (m_MoveValue.axis.magnitude == 0)
        if (m_MovePress.GetStateUp(SteamVR_Input_Sources.Any))
        {
                m_Speed = 0;
        }

        // if button pressed
        // if (m_MovePress.GetStateDown(SteamVR_Input_Sources.RightHand))
        if (m_MovePress.state)
        {
            // add, clamp
            m_Speed += m_MoveValue.axis.magnitude * m_Sensitivity;
            m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);

            // orientation
            movement += orientation * (m_Speed * Vector3.forward);
        }

        // grativty
        movement.y -= m_Gravity * Time.deltaTime;
        //}
        // apply
        m_CharacterController.Move(movement * Time.deltaTime);
    }

    private Quaternion CalculateOrientation()
    {
        float rotation = Mathf.Atan2(m_MoveValue.axis.x, m_MoveValue.axis.y);
        rotation *= Mathf.Rad2Deg;

        Vector3 orientationeuler = new Vector3(0, m_Head.eulerAngles.y + rotation, 0);
        return Quaternion.Euler(orientationeuler);
    }
    
    private void SnapRotation()
    {
        float snapValue = 0f;

        //if (m_RotatePress.GetStateDown(SteamVR_Input_Sources.LeftHand))
        //{
            //snapValue = -Mathf.Abs(m_RotateIncrement);
        //}

        //if (m_RotatePress.GetStateDown(SteamVR_Input_Sources.RightHand))
        //{
            //snapValue = Mathf.Abs(m_RotateIncrement);
        //}

        if (m_SnapRotateButton.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            Quaternion rotation = CalculateOrientation();
            if (m_MoveValue.axis.x <= 0)
            {
                snapValue = -Mathf.Abs(m_RotateIncrement);
            } else
            {
                snapValue = Mathf.Abs(m_RotateIncrement);

            }  
        }
        transform.RotateAround(m_Head.position, Vector3.up, snapValue);
    }
}
