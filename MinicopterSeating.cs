using System;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Minicopter Seating", "Bazz3l", "1.0.1")]
    [Description("Allows 4 to be seated on the mini copter")]
    class MinicopterSeating : RustPlugin
    {
        void OnEntitySpawned(MiniCopter mini)
        {
            if (mini == null)
            {
                return;
            }

            mini.gameObject.AddComponent<CopterSeating>();
        }

        class CopterSeating : MonoBehaviour
        {
            const string ChairPrefab = "assets/prefabs/vehicle/seats/passengerchair.prefab";
            public BaseVehicle mini;
            public BaseEntity seat1;
            public BaseEntity seat2;

            void Awake()
            {
                mini = GetComponent<BaseVehicle>();
                if (mini == null)
                {
                    return;
                }

                Array.Resize(ref mini.mountPoints, 4);

                BaseVehicle.MountPointInfo mountPoint1 = new BaseVehicle.MountPointInfo
                {
                    pos       = new Vector3(0.6f, 0f, -0.2f),
                    rot       = mini.mountPoints[0].rot,
                    prefab    = mini.mountPoints[1].prefab,
                    mountable = mini.mountPoints[0].mountable,
                };

                BaseVehicle.MountPointInfo mountPoint2 = new BaseVehicle.MountPointInfo
                {
                    pos       = new Vector3(-0.6f, 0f, -0.2f),
                    rot       = mini.mountPoints[0].rot,
                    prefab    = mini.mountPoints[1].prefab,
                    mountable = mini.mountPoints[0].mountable,
                };

                mini.mountPoints[2] = mountPoint1;
                mini.mountPoints[3] = mountPoint2;

                seat1 = GameManager.server?.CreateEntity(ChairPrefab, mini.transform.position) as BaseEntity;
                if (seat1 == null)
                {
                    return;
                }

                seat1.SetParent(mini);
                seat1.Spawn();
                seat1.transform.localPosition = new Vector3(0.6f, 0f, -0.4f);
                seat1.SendNetworkUpdateImmediate(true);

                seat2 = GameManager.server?.CreateEntity(ChairPrefab, mini.transform.position) as BaseEntity;
                if (seat2 == null)
                {
                    return;
                }

                seat2.SetParent(mini);
                seat2.Spawn();
                seat2.transform.localPosition = new Vector3(-0.6f, 0f, -0.4f);
                seat2.SendNetworkUpdateImmediate(true);
            }

            void OnDestroy()
            {
                Destroy(this);
            }
        }
    }
}
