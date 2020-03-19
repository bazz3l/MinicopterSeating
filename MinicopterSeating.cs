using UnityEngine;
using System;

namespace Oxide.Plugins
{
    [Info("Minicopter Seating", "Bazz3l", "1.0.9")]
    [Description("Spawns minicopters with extra seats at the sides.")]
    class MinicopterSeating : RustPlugin
    {
        #region Oxide
        void OnEntitySpawned(MiniCopter mini)
        {
            if (mini?.ShortPrefabName != "minicopter.entity")
            {
                return;
            }

            if (mini.mountPoints.Length < 4)
            {
                mini.gameObject.AddComponent<CopterSeating>();
            }
        }
        #endregion

        #region Component
        class CopterSeating : MonoBehaviour
        {
            const string _chairPrefab = "assets/prefabs/vehicle/seats/passengerchair.prefab";
            BaseVehicle _mini;

            void Awake()
            {
                _mini = GetComponent<BaseVehicle>();
                if (_mini == null)
                {
                    Destroy(this);
                    return;
                }

                BaseVehicle.MountPointInfo pilot     = _mini.mountPoints[0];
                BaseVehicle.MountPointInfo passenger = _mini.mountPoints[1];

                Array.Resize(ref _mini.mountPoints, 4);

                _mini.mountPoints[0] = pilot;
                _mini.mountPoints[1] = passenger;
                _mini.mountPoints[2] = MountPoint(new Vector3(0.6f, 0.2f, -0.3f));
                _mini.mountPoints[3] = MountPoint(new Vector3(-0.6f, 0.2f, -0.3f));

                MakeSeat(new Vector3(0.6f, 0.2f, -0.5f));
                MakeSeat(new Vector3(-0.6f, 0.2f, -0.5f));
            }

            BaseVehicle.MountPointInfo MountPoint(Vector3 position)
            {
                return new BaseVehicle.MountPointInfo
                {
                    pos       = position,
                    rot       = _mini.mountPoints[1].rot,
                    prefab    = _mini.mountPoints[1].prefab,
                    mountable = _mini.mountPoints[1].mountable,
                };
            }

            void MakeSeat(Vector3 localPosition)
            {
                BaseEntity seat = GameManager.server.CreateEntity(_chairPrefab, transform.position) as BaseEntity;
                if (seat == null)
                {
                    return;
                }

                seat.SetParent(_mini);
                seat.Spawn();
                seat.transform.localPosition = localPosition;
                seat.SendNetworkUpdateImmediate(true);
            }
        }
        #endregion
    }
}