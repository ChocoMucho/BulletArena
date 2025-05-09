using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //=====Player Move Values=====
    [field: SerializeField] public float WalkSpeed { get; private set; } = 2.5f;
    [field: SerializeField] public float RotationSmoothTime { get; private set; } = 0.1f;
    [field: SerializeField] public float SpeedChangeRate { get; private set; } = 10.0f;


    //=====player move value=====
    private Vector3 _inputDirection;
    private float _playerTargetYaw;
    private float _speed; // 현재 속도
    private float _playerRotationVelocity;
    private Vector3 _targetDirection;
    private float _animationBlend;
    private Vector2 _animationBlend2;

    //=====Camera=====
    [field: SerializeField] public Transform CameraRoot { get; private set; }
    [field: SerializeField] public float TopClamp { get; private set; } = 70.0f;
    [field: SerializeField] public float BottomClamp { get; private set; } = -30.0f;
    [field: SerializeField] public float Sensitivity { get; private set; } = 10f;
    private float _cameraTargetYaw;    // horizontal move
    private float _cameraTargetPitch;  // vertical move

    //=====References=====
    private Player _player;
    private GameObject _mainCamera;
    private CharacterController _controller;
    private PlayerInputs _input;
    private Animator _animator;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputs>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _cameraTargetYaw = CameraRoot.rotation.eulerAngles.y;
    }

    void Update()
    {
        CapturePlayerDirection();
        Move();
    }

    private void LateUpdate()
    { 
        // ESC 키를 누르면 커서 표시 및 잠금 해제
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // 마우스가 다시 클릭되면 커서 숨기고 잠금
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            CameraRotation();
        }
        
    }

    #region Camera
    private void CameraRotation()
    {
        // 입력 값을 요우, 피치 값에 적용
        if (_input.look.magnitude >= 0.1f)
        {
            _cameraTargetYaw += _input.look.x * Time.deltaTime * Sensitivity;
            _cameraTargetPitch -= _input.look.y * Time.deltaTime * Sensitivity;
        }
        _cameraTargetYaw = ClampAngle(_cameraTargetYaw, -360f, 360f);
        _cameraTargetPitch = ClampAngle(_cameraTargetPitch, BottomClamp, TopClamp);

        CameraRoot.rotation = Quaternion.Euler(_cameraTargetPitch, _cameraTargetYaw, 0f);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        // -360~360 조정
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;

        return Mathf.Clamp(angle, min, max);
    }
    #endregion

    private void CapturePlayerDirection()
    {
        _inputDirection = new Vector3(_input.move.x, 0, _input.move.y).normalized;
        if (_input.move != Vector2.zero)
        {
            // 메인 카메라 yaw 값 + 입력의 yaw 값 -> Atan2 * Rad머시기 그거
            // 카메라 방향, 입력 방향으로 목표 방향의 yaw값 계산
            _playerTargetYaw = _mainCamera.transform.eulerAngles.y + (Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg);
        }

        // 오일러 * forward -> 원하는 벡터. 아무튼 됨
        _targetDirection = Quaternion.Euler(0f, _playerTargetYaw, 0f) * Vector3.forward;
    }

    private void Move()
    {
        float targetSpeed;
        float inputMagnitude = _input.move.magnitude;

        if (_input.move == Vector2.zero) // 방향키 입력 없음
        {
            targetSpeed = 0;
        }
        else
        {
            // 속도 변화 및 상태 변화
            targetSpeed = WalkSpeed;

            // 회전
            float yaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, _playerTargetYaw, ref _playerRotationVelocity, RotationSmoothTime);

            if (!_input.Aim)
                transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        _speed = targetSpeed;

        _controller.Move(_targetDirection.normalized * _speed * Time.deltaTime);
        // 수직 움직임은 아직

        // 애니메이션 처리
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend <= 0.1f) _animationBlend = 0f;

        _animationBlend2 = Vector2.Lerp(_animationBlend2, _input.move.normalized, Time.deltaTime * SpeedChangeRate);
        if (_input.Aim)
        {
            _animator.SetFloat(PlayerAnimatorHashes.BlendX, _animationBlend2.x);
            _animator.SetFloat(PlayerAnimatorHashes.BlendY, _animationBlend2.y);
            if (!_animator.GetBool(PlayerAnimatorHashes.Aim))
                _animator.SetBool(PlayerAnimatorHashes.Aim, true);
        }
        else
        {
            _animator.SetFloat(PlayerAnimatorHashes.Blend, _animationBlend);
            if (_animator.GetBool(PlayerAnimatorHashes.Aim))
                _animator.SetBool(PlayerAnimatorHashes.Aim, false);
        }

        _animator.SetFloat(PlayerAnimatorHashes.MotionSpeed, inputMagnitude);
    }
}
