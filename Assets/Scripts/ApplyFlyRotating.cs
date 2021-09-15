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
    /// �Է°��� ���� ������ ���� ���͸� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="horizontal"></param>
    /// <param name="vertical"></param>
    /// <returns></returns>
    public Vector3 Rotating(float horizontal, float vertical, bool isMoving)
    {
        Vector3 forward = playerCamera.TransformDirection(Vector3.forward);
        // ī�޶� ���⺤�� normalized
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        // Ÿ�� ���� ���
        Vector3 targetDirection = forward * vertical + right * horizontal;

        ApplayRigidBodyRotation(isMoving, targetDirection);

        return targetDirection;
    }

    /// <summary>
    /// RigidBody flyMode Rotation���� ����
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
    /// ���� �� ��� ĳ������ ��Ʈ�ѷ� ȸ��
    /// </summary>
    /// <param name="playerController"></param>
    /// <param name="horizontal"></param>
    /// <param name="vertical"></param>
    public void ApplyColliderDirection(CapsuleCollider playerCapsuleCollider, CharacterController playerController, float horizontal, float vertical)
    {
		// �÷��̾ �������̰�, idle ������ ���
		if (!(Mathf.Abs(horizontal) > 0.2 || Mathf.Abs(vertical) > 0.2))
        {
            // �÷��̾� �⺻ 
            Repositioning();
            // Set collider direction to vertical.
            playerController.detectCollisions = false;
            SetFlyModeCollider(playerCapsuleCollider, true);
        }
        else
		{
			// Collider ��ġ �ʱ�ȭ
			playerController.detectCollisions = true;
			SetFlyModeCollider(playerCapsuleCollider, true);
		}
	}

	/// <summary>
	/// ĸ�� �ݶ��̴��� direction ��ȭ �� Ȱ��ȭ / ��Ȱ��ȭ
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
    /// Rigidbody ���� �������� ȸ��
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
