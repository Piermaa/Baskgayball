using System.Collections.Generic;
using BaskgayBall.Input;
using UnityEngine;

namespace BaskgayBall.PowerUps
{
    public class HoopShield : MonoBehaviour
    {
        private static readonly Dictionary<EPlayerSide, HoopShield> Registry =
            new Dictionary<EPlayerSide, HoopShield>();

        [SerializeField] private EPlayerSide ownerSide;
        [SerializeField] private GameObject shieldObject;

        private int _holders;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Registry.Clear();
        }

        public static bool TryGet(EPlayerSide side, out HoopShield shield)
        {
            return Registry.TryGetValue(side, out shield);
        }

        private void OnEnable()
        {
            Registry[ownerSide] = this;
            Refresh();
        }

        private void OnDisable()
        {
            if (Registry.TryGetValue(ownerSide, out HoopShield current) && current == this)
            {
                Registry.Remove(ownerSide);
            }
        }

        public void Acquire()
        {
            _holders++;
            Refresh();
        }

        public void Release()
        {
            _holders = Mathf.Max(0, _holders - 1);
            Refresh();
        }

        private void Refresh()
        {
            shieldObject.SetActive(_holders > 0);
        }
    }
}