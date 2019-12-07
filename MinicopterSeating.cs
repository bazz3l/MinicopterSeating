using System;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Minicopter Seating", "Bazz3l", "1.0.8")]
    [Description("Allows 2 extra seats on the mini copter")]
    class MinicopterSeating : RustPlugin
    {
        void OnEntitySpawned(MiniCopter mini)
        {
            if (mini == null || mini.ShortPrefabName != "minicopter.entity") return;
            if (mini.mountPoints.Length < 4)
                mini?.gameObject.AddComponent<CopterSeating>();
        }

        class CopterSeating : MonoBehaviour
        {
            public string ChairPrefab = "assets/prefabs/vehicle/seats/passengerchair.prefab";
            public BaseVehicle mini;

            void Awake()
            {
                mini = GetComponent<BaseVehicle>();
                if (mini == null)
                {
                    Destroy(this);
                    return;
                }

                BaseVehicle.MountPointInfo pilot      = mini.mountPoints[0];
                BaseVehicle.MountPointInfo passenger1 = mini.mountPoints[1];

                Array.Resize(ref mini.mountPoints, 4);

                BaseVehicle.MountPointInfo passenger2 = new BaseVehicle.MountPointInfo
                {
                    pos       = new Vector3(0.6f, 0.2f, -0.2f),
                    rot       = mini.mountPoints[1].rot,
                    prefab    = mini.mountPoints[1].prefab,
                    mountable = mini.mountPoints[1].mountable,
                };

                BaseVehicle.MountPointInfo passenger3 = new BaseVehicle.MountPointInfo
                {
                    pos       = new Vector3(-0.6f, 0.2f, -0.2f),
                    rot       = mini.mountPoints[1].rot,
                    prefab    = mini.mountPoints[1].prefab,
                    mountable = mini.mountPoints[1].mountable,
                };

                mini.mountPoints[0] = pilot;
                mini.mountPoints[1] = passenger1;
                mini.mountPoints[2] = passenger2;
                mini.mountPoints[3] = passenger3;

                MakeSeat(mini, new Vector3(0.6f, 0.2f, -0.5f));
                MakeSeat(mini, new Vector3(-0.6f, 0.2f, -0.5f));
            }

            void MakeSeat(BaseVehicle mini, Vector3 locPos)
            {
                BaseEntity seat = GameManager.server.CreateEntity(ChairPrefab, mini.transform.position) as BaseEntity;
                if (seat == null) return;

                seat.SetParent(mini);
                seat.Spawn();
                seat.transform.localPosition = locPos;
                seat.SendNetworkUpdateImmediate(true);
            }
        }
    }
}