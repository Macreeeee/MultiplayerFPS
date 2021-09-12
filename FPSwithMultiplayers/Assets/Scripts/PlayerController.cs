



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    private PlayerMotor motor;

    private ConfigurableJoint cj;

    private Animator animator;

    [Header("Spring settings:-)")]

    [SerializeField]
    private float jointSpring = 20.0f;
    [SerializeField]
    private float jointMaxForce = 1000f;

    [SerializeField]
    private float LookSensitivity = 20.0f;

    [SerializeField]
    private float thrusterForce = 1000f;

    private void Start() {
        motor = GetComponent<PlayerMotor>();
        cj = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    private void Update() {
        //Get x & y move direction and apply to motor move
        float _xMove = Input.GetAxis("Horizontal");
        float _zMove = Input.GetAxis("Vertical");

        Vector3 _moveHorizontal = transform.right * _xMove;
        Vector3 _moveVertical = transform.forward * _zMove;

        Vector3 _velocity = (_moveHorizontal + _moveVertical) * speed;

        //Apply animator veriable with movement velocity
        animator.SetFloat("ForwardVelocity", _zMove);

        motor.Move(_velocity);

        //camera move horizontally
        float _yRot = Input.GetAxisRaw("Mouse X");
        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * LookSensitivity;
        motor.Rotate(_rotation);

        //camera rotate vertically
        float _xRot = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRot * LookSensitivity;
        motor.RotateCamera(_cameraRotationX);

        //Apply thruster force when "jump"
        Vector3 _thrusterForce = Vector3.zero;
        if (Input.GetButton ("Jump")){
            _thrusterForce = Vector3.up * thrusterForce;
            SetJointSettings(0f);
        }else{
            SetJointSettings(jointSpring);
        }

        motor.ApplyThruster(_thrusterForce);
    }

    private void SetJointSettings(float _jointSpring)
    {
        cj.yDrive = new JointDrive { 
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }
}
