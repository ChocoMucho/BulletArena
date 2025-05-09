using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //=====Status=====
    //public bool IsRunning => Controller.IsRunning;
    public bool IsAiming { get; set; } = false;
    public bool IsReloading { get; set; } = false;

    //=====Refernces=====
    public Animator Animator { get; private set; }
    public PlayerInputs Input { get; private set; }
    public PlayerController Controller { get; private set; }

    //=====AnimationRigging=====
    public GameObject FireTargetObject;
    [SerializeField] private Transform Muzzle;
    private RigController _rigController;

    //=====Camera=====
    public Camera MainCamera;

    [SerializeField] private LayerMask layerMask;

    //=====UI=====임시
    public Image CameraAim;
    public Image FireAim;

    //=====Shake/Recoil=====
    [Header("Aim Shake/Recoil")]
    [Range(0, 40)]
    [SerializeField] private float shakeAmountX = 0.05f;
    [Range(0, 40)]
    [SerializeField] private float shakeAmountY = 0.05f;
    [Range(1, 10)]
    [SerializeField] private float shakeSpeed = 1f;
    private float _noiseX;
    private float _noiseY;
    [Range(0,30)]
    [SerializeField] private float _recoilStrength = 10f; 
    [SerializeField] private float _recoilCurrentAmountY; 
    [Range(0, 10)]
    [SerializeField] private float _recoilRecoverySpeed = 2f;

    //=====Attack=====
    public GameObject BulletPrefab;
    public float _fireTimeout = 0.2f;
    public float _fireTimeoutDelta;
    public GameObject MuzzleEffect;

    //=====FSM=====
    //TODO

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Input = GetComponent<PlayerInputs>();
        Controller = GetComponent<PlayerController>();
        MainCamera = Camera.main;
        _rigController = GetComponent<RigController>();
    }

    void Start()
    {
        _rigController.Init();
        _rigController.SetMultiAimConstraint("RigUpperBodyLayer", FireTargetObject);
    }

    void Update()
    {
        _fireTimeoutDelta += Time.deltaTime;
        if (Input.Aim)
        {
            AimPositioning();
            SetAnimatorLayerWeight(1, 1f);
            _rigController.SetRigWeight("RigUpperBodyLayer");

            if (Input.Fire)
            {
                Shoot();
            }
        }
        else
        {
            SetAnimatorLayerWeight(1, 0f);
            _rigController.SetRigWeight("RigUpperBodyLayer", 0f);
        }
    }

    public void AimPositioning()
    {
        Transform camera = MainCamera.transform;
        Vector3 baseDirection = camera.forward;

        Vector3 shakeDirection = AimNoise(camera) * AimRecoil(camera) * baseDirection;

        // 카메라 중앙에서 흔들리는 방향으로 레이 발사
        if (Physics.Raycast(camera.position, shakeDirection, out RaycastHit hit, float.MaxValue, layerMask))
        {
            FireTargetObject.transform.position =
                Vector3.Lerp(FireTargetObject.transform.position, hit.point, Time.deltaTime * 5f);
        }

        Vector3 screenPoint = MainCamera.WorldToScreenPoint(FireTargetObject.transform.position);
        RectTransform rectTransform = FireAim.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = screenPoint;

        BodyRotation(hit.point);
    }

    public Quaternion AimNoise(Transform camera) // calculate aim point noise
    {
        _noiseX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * shakeAmountX;
        _noiseY = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * shakeAmountY;
        Quaternion shakeRotation = Quaternion.AngleAxis(_noiseX, camera.up) * Quaternion.AngleAxis(_noiseY, camera.right);

        return shakeRotation;
    }

    public Quaternion AimRecoil(Transform camera) // calculate aim recoil (y, up)
    {
        Quaternion recoilRotation = Quaternion.AngleAxis(_recoilCurrentAmountY, camera.right);
        _recoilCurrentAmountY = Mathf.Lerp(_recoilCurrentAmountY, 0f, Time.deltaTime * _recoilRecoverySpeed);

        return recoilRotation;
    }

    public void ApplyRecoil()
    {
        _recoilCurrentAmountY -= _recoilStrength + Random.Range(-0.5f, 0.5f);
    }

    public void SetAnimatorLayerWeight(int index, float weight)
    {
        float layerWeight = Animator.GetLayerWeight(index);
        Animator.SetLayerWeight(index, Mathf.Lerp(layerWeight, weight, Time.deltaTime * 10f));
    }

    public void BodyRotation(Vector3 lookPoint)
    {
        Vector3 tempVector = lookPoint;
        tempVector.y = transform.position.y; // y 값은 캐릭터 트랜스폼과 같게
        Vector3 playerDirection = (tempVector - transform.position).normalized;

        // 캐릭터 트랜스폼의 포워드를 위에 구한 값으로
        transform.forward = Vector3.Lerp(transform.forward, playerDirection, Time.deltaTime * 10f);
    }

    public void Shoot()
    {
        if (_fireTimeoutDelta < _fireTimeout)
            return;
        
        _fireTimeoutDelta = 0;
        GameObject bulletObject = Instantiate(BulletPrefab, Muzzle.position, Muzzle.rotation);
        bulletObject.GetComponent<Projectile>().SetDirection(FireTargetObject.transform);
        Instantiate(MuzzleEffect, Muzzle.position, Quaternion.identity);

        ApplyRecoil();
    }
}
