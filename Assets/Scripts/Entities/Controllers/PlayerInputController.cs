using UnityEngine;
using UnityEngine.InputSystem;

// Player의 Input을 처리하는 부분, 몬스터는 입력이 없겠죠?
public class PlayerInputController : TopDownController
{
    private Camera _camera;
    // 부모 클래스에서 Awake가 virtual로 정의되어 있으니 그거에 얹어서! 추가적인 로직을 실행해야겠죠?
    protected override void Awake()
    {
        // 부모의 Awake도 빼먹지 말고 실행하라는 의미
        base.Awake();
        _camera = Camera.main;
    }

    public void OnMove(InputValue value)
    {
        // [우리가 하고 싶은 것]
        // 입력으로 들어온 값을 크기를 1로 만듬 (정규화, normalize)
        Vector2 moveInput = value.Get<Vector2>().normalized;

        // 부모인 TopDownController 클래스의 CallMoveEvent로 moveInput전달
        CallMoveEvent(moveInput);
    }

    public void OnLook(InputValue value)
    {
        // [우리가 하고 싶은것]
        // 마우스가 캐릭터 왼쪽에서 있으면 왼쪽보고, 오른쪽에 있으면 오른쪽 보기
        // 마우스 입력이 OnLook에서 value로 떨어지게 됨

        Vector2 newAim = value.Get<Vector2>();
        // 카메라가 찍고 있는 범위를 바탕으로 이뤄짐. 그래서 _camera에 대한 참조
        Vector2 worldPos = _camera.ScreenToWorldPoint(newAim);
        newAim = (worldPos - (Vector2)transform.position).normalized;

        if (newAim != Vector2.zero)
        {
            // 부모인 TopDownController 클래스의 CallLookEvent로 newAim전달
            CallLookEvent(newAim);
        }
    }

    public void OnFire(InputValue value)
    {
        isAttacking = value.isPressed;
    }
}