using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public Vector2 move;
    public Vector2 look;
    public bool Aim { get; set; }
    public bool Fire { get; set; }

    #region receipt value
    public void OnMove(InputValue value) => MoveInput(value.Get<Vector2>());
    public void OnLook(InputValue value) => LookInput(value.Get<Vector2>());
    public void OnAim(InputValue value) => AimInput(value.isPressed);
    public void OnFire(InputValue value) => FireInput(value.isPressed);
    #endregion

    #region restore value
    private void MoveInput(Vector2 moveDirection) => move = moveDirection;
    private void LookInput(Vector2 lookDirection) => look = lookDirection;
    private void AimInput(bool isPressed) => Aim = isPressed;
    private void FireInput(bool isPressed) => Fire = isPressed;
    #endregion

}
