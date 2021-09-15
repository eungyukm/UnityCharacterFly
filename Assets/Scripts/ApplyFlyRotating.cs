using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyFlyRotating
{
    public float turnSmoothing = 0.06f;
    public Vector3 lastDirection;
    public Rigidbody playerRigidbody;
    private Transform playerCamera;

    public ApplyFlyRotating(Rigidbody rigidbody, Transform camera)
    {
        playerRigidbody = rigidbody;
        playerCamera = camera;
    }

    /// <summary>
    /// 입력값에 따라 움직일 방향 벡터를 반환하는 함수
    /// </summary>
    /// <param name="horizontal"></param>
    /// <param name="vertical"></param>
    /// <returns></returns>
    public Vector3 Rotating(float horizontal, float vertical, bool isMoving)
    {
        Vector3 forward = playerCamera.TransformDirection(Vector3.forward);
        // 카메라 방향벡터 normalized
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        // 타겟 방향 계산
        Vector3 targetDirection = forward * vertical + right * horizontal;

        ApplayRigidBodyRotation(isMoving, targetDirection);

        return targetDirection;
    }

    /// <summary>
    /// RigidBody flyMode Rotation으로 변경
    /// </summary>
    /// <param name="isMoving"></param>
    /// <param name="targetDirection"></param>
    private void ApplayRigidBodyRotation(bool isMoving, Vector3 targetDirection)
    {
        if ((isMoving && targetDirection != Vector3.zero))
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            Quaternion newRotation = Quaternion.Slerp(playerRigidbody.rotation, targetRotation, turnSmoothing);

            playerRigidbody.MoveRotation(newRotation);
            lastDirection = targetDirection;
        }
    }

    /// <summary>
    /// 날게 될 경우 캐릭터의 컨트롤러 회전
    /// </summary>
    /// <param name="playerController"></param>
    /// <param name="horizontal"></param>
    /// <param name="vertical"></param>
    public void ApplyColliderDirection(CapsuleCollider playerCapsuleCollider, CharacterController playerController, float horizontal, float vertical)
    {
		// 플레이어가 나는중이고, idle 상태일 경우
		if (!(Mathf.Abs(horizontal) > 0.2 || Mathf.Abs(vertical) > 0.2))
        {
            // 플레이어 기본 
            Repositioning();
            // Set collider direction to vertical.
            playerController.detectCollisions = false;
            SetFlyModeCollider(playerCapsuleCollider, true);
        }
        else
		{
			// Collider 위치 초기화
			playerController.detectCollisions = true;
			SetFlyModeCollider(playerCapsuleCollider, true);
		}
	}

	/// <summary>
	/// 캡슐 콜라이더의 direction 변화 및 활성화 / 비활성화
	/// </summary>
	/// <param name="playerCapsuleCollider"></param>
    private void SetFlyModeCollider(CapsuleCollider playerCapsuleCollider, bool isFlyAndMoving)
    {
		if (isFlyAndMoving)
		{
			playerCapsuleCollider.direction = 1;
			playerCapsuleCollider.enabled = true;
		}
		else
        {
			playerCapsuleCollider.direction = 2;
			playerCapsuleCollider.enabled = false;
		}
    }

    /// <summary>
    /// Rigidbody 이전 방향으로 회전
    /// </summary>
    public void Repositioning()
    {
        if (lastDirection != Vector3.zero)
        {
            lastDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(playerRigidbody.rotation, targetRotation, turnSmoothing);
            playerRigidbody.MoveRotation(newRotation);
        }
    }
}
