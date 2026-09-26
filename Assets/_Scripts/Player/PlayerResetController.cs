using System.Collections.Generic;
using UnityEngine;
using BaskgayBall.Core;

namespace BaskgayBall.Player
{
    public class PlayerResetController : MonoBehaviour, IResettable
    {
        [SerializeField] private MonoBehaviour[] logicResettables;

        [SerializeField] private Rigidbody2D bodyRigidbody;
        [SerializeField] private Rigidbody2D headRigidbody;
        [SerializeField] private Joint2D[] jointsToRecreate;

        private readonly List<IResettable> validatedLogicResettables = new List<IResettable>();

        private Vector2 initialBodyPosition;
        private float initialBodyRotation;
        private Vector2 initialHeadPosition;
        private float initialHeadRotation;

        private void Awake()
        {
            initialBodyPosition = bodyRigidbody.position;
            initialBodyRotation = bodyRigidbody.rotation;
            initialHeadPosition = headRigidbody.position;
            initialHeadRotation = headRigidbody.rotation;

            CacheLogicResettables();
        }

        private void CacheLogicResettables()
        {
            validatedLogicResettables.Clear();

            foreach (MonoBehaviour candidate in logicResettables)
            {
                if (candidate is IResettable resettable)
                {
                    validatedLogicResettables.Add(resettable);
                }
                else if (candidate != null)
                {
                    Debug.LogWarning(
                        candidate.name + " is assigned to PlayerResetController but does not implement IResettable",
                        candidate);
                }
            }
        }

        public void ResetState()
        {
            StopLogicComponents();
            ResetRigidbodies();
            RecreateJoints();
            Physics2D.SyncTransforms();
        }

        private void StopLogicComponents()
        {
            for (int i = 0; i < validatedLogicResettables.Count; i++)
            {
                validatedLogicResettables[i].ResetState();
            }
        }

        private void ResetRigidbodies()
        {
            ResetRigidbody(bodyRigidbody, initialBodyPosition, initialBodyRotation);
            ResetRigidbody(headRigidbody, initialHeadPosition, initialHeadRotation);
        }

        private static void ResetRigidbody(Rigidbody2D body, Vector2 position, float rotation)
        {
            if (body == null)
            {
                return;
            }

            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
            body.position = position;
            body.rotation = rotation;
            body.WakeUp();
        }

        private void RecreateJoints()
        {
            for (int i = 0; i < jointsToRecreate.Length; i++)
            {
                Joint2D joint = jointsToRecreate[i];

                if (joint == null)
                {
                    continue;
                }

                joint.enabled = false;
                joint.enabled = true;
            }
        }
    }
}