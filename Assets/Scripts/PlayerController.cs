using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum Side {  Left = -2, Middle = 0, Right = 2 };

public class PlayerController : MonoBehaviour
{
    private Transform myTransform;
    private Animator myAnimator;
    private CharacterController _myCharacterController;
    public CharacterController MyCharacterController { get => _myCharacterController; set => _myCharacterController = value; }
    private PlayerCollision playerCollision;
    public bool CanMove { get => _canMove; set => _canMove = value; }
    [SerializeField]private bool _canMove;
    private Side position;
    private Vector3 motionVector;
    [SerializeField] private float jumpPower;
    [SerializeField] private float dodgeSpeed;
    [SerializeField] private float forwardSpeed;
    private float newXPosition;
    private float xPosition;
    private float yPosition;
    private int IdDodgeLeft = Animator.StringToHash("DodgeLeft");
    private int IdDodgeRight = Animator.StringToHash("DodgeRight");
    private int IdJump = Animator.StringToHash("Jump");
    private int IdFall = Animator.StringToHash("Fall");
    private int IdLanding = Animator.StringToHash("Landing");
    private int IdRoll = Animator.StringToHash("Roll");
    private int _IdDeathBounce = Animator.StringToHash("DeathBounce");
    public int IdDeathBounce { get => _IdDeathBounce; set => _IdDeathBounce = value; }
    private int _IdDeathLower = Animator.StringToHash("DeathLower");
    public int IdDeathLower { get => _IdDeathLower; set => _IdDeathLower = value; }
    private int _IdDeathMovingTrain = Animator.StringToHash("DeathMovingTrain");
    public int IdDeathMovingTrain { get => _IdDeathMovingTrain; set => _IdDeathMovingTrain = value; }
    private int _IdDeathUpper = Animator.StringToHash("DeathUpper");
    public int IdDeathUpper { get => _IdDeathUpper; set => _IdDeathUpper = value; }
    private int _IdStumbleLow = Animator.StringToHash("StumbleLow");
    public int IdStumbleLow { get => _IdStumbleLow; set => _IdStumbleLow = value; }
    private int _IdStumbleCornerRight = Animator.StringToHash("StumbleCornerRight");
    public int IdStumbleCornerRight { get => _IdStumbleCornerRight; set => _IdStumbleCornerRight = value; }
    private int _IdStumbleCornerLeft = Animator.StringToHash("StumbleCornerLeft");
    public int IdStumbleCornerLeft { get => _IdStumbleCornerLeft; set => _IdStumbleCornerLeft = value; }
    private int _IdStumbleSideLeft = Animator.StringToHash("StumbleSideLeft");
    public int IdStumbleSideLeft { get => _IdStumbleSideLeft; set => _IdStumbleSideLeft = value; }
    private int _IdStumbleSideRight = Animator.StringToHash("StumbleSideRight");
    public int IdStumbleSideRight { get => _IdStumbleSideRight; set => _IdStumbleSideRight = value; }
    private int IdStumbleFall = Animator.StringToHash("StumbleFall");
    private int IdStumbleOffLeft = Animator.StringToHash("StumbleOffLeft");
    private int IdStumbleOffRight = Animator.StringToHash("StumbleOffRight");
    public static PlayerController Instance; 

    private bool _swipeLeft, _swipeRight, swipeUp, swipeDown;
    public bool SwipeLeft { get => _swipeLeft; set => _swipeLeft = value; }
    public bool SwipeRight { get => _swipeRight; set => _swipeRight = value; }
    [Header("Player States")]
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float rollTimer;
    [SerializeField] private bool _isRolling;
    private bool _stumbleRight;
    public bool StumbleRight { get => _stumbleRight; set => _stumbleRight = value; }
    private bool _stumbleLeft;
    public bool StumbleLeft { get => _stumbleLeft; set => _stumbleLeft = value; }
    public bool IsRolling { get => _isRolling; set => _isRolling = value; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        position = Side.Middle;
        myTransform = GetComponent<Transform>();
        myAnimator = GetComponent<Animator>();
        _myCharacterController = GetComponent<CharacterController>();
        playerCollision = GetComponent<PlayerCollision>();
        yPosition = -7;
    }

    void LateUpdate()
    {
        if (_canMove)
        {
            GetSwipe();
            SetPlayerPosition();
            MovePlayer();
            ReposPlayer();
            Jump();
            Roll();
            isGrounded = _myCharacterController.isGrounded;
            LoopLevel();
        }
    }

    private void GetSwipe()
    {
        _swipeLeft = Input.GetKeyDown(KeyCode.LeftArrow);
        _swipeRight = Input.GetKeyDown(KeyCode.RightArrow);
        swipeUp = Input.GetKeyDown(KeyCode.UpArrow);
        swipeDown = Input.GetKeyDown(KeyCode.DownArrow);
    }
    
    public void SetPlayerPosition()
    {
        if (_swipeLeft && !IsRolling)
        {
            if (position == Side.Middle)
            {
                UpdatePlayerXPosition(Side.Left);
                SetPlayerAnimator(IdDodgeLeft, false);
            }
            else if (position == Side.Right)
            {
                UpdatePlayerXPosition(Side.Middle);
                SetPlayerAnimator(IdDodgeLeft, false);
            }
        }
        else if (_swipeRight && !IsRolling)
        {
            if (position == Side.Middle)
            {
                UpdatePlayerXPosition(Side.Right);
                SetPlayerAnimator(IdDodgeRight, false);
            }
            else if (position == Side.Left)
            {
                UpdatePlayerXPosition(Side.Middle);
                SetPlayerAnimator(IdDodgeRight, false);
            }
        }
    }

    private void ReposPlayer()
    {
        if (_stumbleLeft)
        {
            if (position == Side.Left)
            {
                _stumbleLeft = false;
                UpdatePlayerXPosition(Side.Middle);
            }
            else if (position == Side.Middle)
            {
                _stumbleLeft = false;
                UpdatePlayerXPosition(Side.Right);
            }
        }
        else if (_stumbleRight)
        {
            if (position == Side.Right)
            {
                _stumbleRight = false;
                UpdatePlayerXPosition(Side.Middle);
            }
            else if (position == Side.Middle)
            {
                _stumbleRight = false;
                UpdatePlayerXPosition(Side.Left);
            }
        }
    }

    private void UpdatePlayerXPosition(Side plPosition)
    {
        newXPosition = (int)plPosition;
        position = plPosition;
    }

    public void SetPlayerAnimator(int id, bool isCrossFade, float fadeTime = 0.1f)
    {
        myAnimator.SetLayerWeight(0, 1);
        if (isCrossFade)
        {
            myAnimator.CrossFadeInFixedTime(id, fadeTime);
        }
        else
        {
            myAnimator.Play(id);
        }
        ResetCollision();
    }

    public void SetPlayerAnimatorWithLayer(int id)
    {
        myAnimator.SetLayerWeight(1, 1);
        myAnimator.Play(id);
        ResetCollision();
    }

    private void ResetCollision()
    {
        print(playerCollision.CollisionX + " " + playerCollision.CollisionY + " " + playerCollision.CollisionZ);
        playerCollision.CollisionX = CollisionX.None;
        playerCollision.CollisionY = CollisionY.None;
        playerCollision.CollisionZ = CollisionZ.None;
    }

    private void MovePlayer()
    {
        motionVector = new Vector3(xPosition - myTransform.position.x, yPosition * Time.deltaTime, forwardSpeed * Time.deltaTime);
        xPosition = Mathf.Lerp(xPosition, newXPosition, Time.deltaTime * dodgeSpeed);
        _myCharacterController.Move(motionVector);
    }

    private void Jump()
    {
        if (_myCharacterController.isGrounded)
        {
            isJumping = false;
            if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
            {
                SetPlayerAnimator(IdLanding, false);
            }
            if (swipeUp && !IsRolling)
            {
                isJumping = true;
                yPosition = jumpPower;
                SetPlayerAnimator(IdJump, true);
            }
        }
        else
        {
            yPosition -= jumpPower * 2 * Time.deltaTime;
            if (_myCharacterController.velocity.y <= 0)
            {
                SetPlayerAnimator(IdFall, false);
            }
        }
    }

    private void Roll()
    {
        rollTimer -= Time.deltaTime;
        if (rollTimer <= 0)
        {
            IsRolling = false;
            rollTimer = 0;
            //normal size
            _myCharacterController.center = new Vector3(0, .45f, 0);
            _myCharacterController.height = .9f;
        }
        if (swipeDown && !isJumping)
        {
            IsRolling = true;
            rollTimer = .5f;
            SetPlayerAnimator(IdRoll, true);
            //roll size
            _myCharacterController.center = new Vector3(0, .2f, 0);
            _myCharacterController.height = .4f;
        }
    }

    private void LoopLevel()
    {
        if (myTransform.position.z >= 2000f)
        {
            Vector3 newPosition = myTransform.position;
            newPosition.z = 0f;
            myTransform.position = newPosition;
        }
    }
}
