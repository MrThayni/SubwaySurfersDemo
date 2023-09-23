using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollisionX { None, Left, Middle, Right};
public enum CollisionY { None,Up, Middle, Down, LowDown };
public enum CollisionZ { None, Forwad, Middle, Backward };

public class PlayerCollision : MonoBehaviour
{
    private PlayerController playerController;
    private CollisionX _collisionX;
    private CollisionY _collisionY;
    private CollisionZ _collisionZ;

    public CollisionX CollisionX { get => _collisionX; set => _collisionX = value; }
    public CollisionY CollisionY { get => _collisionY; set => _collisionY = value; }
    public CollisionZ CollisionZ { get => _collisionZ; set => _collisionZ = value; }

    private Vector3 positon;

    void Awake()
    {
        playerController = gameObject.GetComponent<PlayerController>();
    }

    private void Start()
    {
        positon = transform.position;
    }

    public void OnCharacterCollision(Collider collider)
    {
        CollisionX = GetCollisionX(collider);
        CollisionY = GetCollisionY(collider);
        CollisionZ = GetCollisionZ(collider);
        setAnimationByCollision(collider);
    }

    private CollisionX GetCollisionX(Collider collider)
    {
        Bounds characterControllerBounds = playerController.MyCharacterController.bounds;
        Bounds colliderBounds = collider.bounds;
        float minX = Mathf.Max(colliderBounds.min.x, characterControllerBounds.min.x);
        float maxX = Mathf.Min(colliderBounds.max.x, characterControllerBounds.max.x);
        float average = (minX + maxX)/2 - colliderBounds.min.x;
        CollisionX colx;
        if (average > colliderBounds.size.x - 0.33f)
        {
            colx = CollisionX.Right;
        }
        else if(average < 0.33f)
        {
            colx = CollisionX.Left;
        }
        else
        {
            colx = CollisionX.Middle;
        }
        return colx;
    }

    private CollisionY GetCollisionY(Collider collider)
    {
        Bounds characterControllerBounds = playerController.MyCharacterController.bounds;
        Bounds colliderBounds = collider.bounds;
        float minY = Mathf.Max(colliderBounds.min.y, characterControllerBounds.min.y);
        float maxY = Mathf.Min(colliderBounds.max.y, characterControllerBounds.max.y);
        float average = (minY + maxY) / 2 - colliderBounds.min.y;
        CollisionY coly;
        if (average > colliderBounds.size.y - 0.33f)
        {
            coly = CollisionY.Up;
        }
        else if (average < .17f)
        {
            coly = CollisionY.LowDown;
        }
        else if (average < 0.33f)
        {
            coly = CollisionY.Down;
        }
        else
        {
            coly = CollisionY.Middle;
        }
        return coly;
    }

    private CollisionZ GetCollisionZ(Collider collider)
    {
        Bounds characterControllerBounds = playerController.MyCharacterController.bounds;
        Bounds colliderBounds = collider.bounds;
        float minZ = Mathf.Max(colliderBounds.min.z, characterControllerBounds.min.z);
        float maxZ = Mathf.Min(colliderBounds.max.z, characterControllerBounds.max.z);
        float average = (minZ + maxZ) / 2 - colliderBounds.min.z;
        CollisionZ colz;
        if (average > colliderBounds.size.z - 0.33f)
        {
            colz = CollisionZ.Forwad;
        }
        else if (average < 0.33f)
        {
            colz = CollisionZ.Backward;
        }
        else
        {
            colz = CollisionZ.Middle;
        }
        return colz;
    }

    private void setAnimationByCollision(Collider collider)
    {
        if (CollisionZ == CollisionZ.Backward && CollisionX == CollisionX.Middle)
        {
            if (CollisionY == CollisionY.LowDown)
            {
                collider.enabled = false;
                playerController.SetPlayerAnimator(playerController.IdStumbleLow, false);
            }
            else if (CollisionY == CollisionY.Down)
            {
                playerController.SetPlayerAnimator(playerController.IdDeathLower, false);
                GameManager.Instance.GameOver();
            }
            else if (CollisionY == CollisionY.Middle)
            {
                if (collider.CompareTag("TrainOn"))
                {
                    playerController.SetPlayerAnimator(playerController.IdDeathMovingTrain, false);
                    GameManager.Instance.GameOver();
                }
                else if(!collider.CompareTag("Ramp"))
                {
                    playerController.SetPlayerAnimator(playerController.IdDeathBounce, false);
                    GameManager.Instance.GameOver();
                }
            }
            else if (CollisionY == CollisionY.Up && !playerController.IsRolling)
            {
                playerController.SetPlayerAnimator(playerController.IdDeathUpper, false);
                GameManager.Instance.GameOver();
            }
        }
        else if (CollisionZ == CollisionZ.Middle || CollisionZ == CollisionZ.Backward || CollisionZ == CollisionZ.Forwad)//
        {
            if (CollisionX == CollisionX.Right)
            {
                //reset player position
                playerController.StumbleLeft = true;
                playerController.SetPlayerAnimator(playerController.IdStumbleSideRight, false);
            }
            else if (CollisionX == CollisionX.Left)
            {
                //reset player position
                playerController.StumbleRight = true;
                playerController.SetPlayerAnimator(playerController.IdStumbleSideLeft, false);
            }
        }
        else
        {
            if (CollisionX == CollisionX.Right)
            {
                playerController.SetPlayerAnimatorWithLayer(playerController.IdStumbleCornerRight);
            }
            else if (CollisionX == CollisionX.Left)
            {
                playerController.SetPlayerAnimatorWithLayer(playerController.IdStumbleCornerLeft);
            }
        }
    }
}
