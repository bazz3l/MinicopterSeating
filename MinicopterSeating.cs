using UnityEngine;
using System;

namespace Oxide.Plugins
{
    [Info("Minicopter Seating", "Bazz3l", "1.1.0")]
    [Description("Spawns an extra seat each side of the minicopter.")]
    class MinicopterSeating : RustPlugin
    {
        void OnEntitySpawned(MiniCopter mini)
        {
            if (mini.ShortPrefabName == "minicopter.entity" && mini.mountPoints.Length < 4)
            {
                new SeatingManager((BaseVehicle) mini);
            }
        }

        class SeatingManager
        {
            string chairPrefab = "assets/prefabs/vehicle/seats/passengerchair.prefab";
            BaseVehicle mini;

            public SeatingManager(BaseVehicle mini)
            {
                this.mini = mini;

                SetupSeating();
            }

            void SetupSeating()
            {
                BaseVehicle.MountPointInfo pilot     = mini.mountPoints[0];
                BaseVehicle.MountPointInfo passenger = mini.mountPoints[1];

                Array.Resize(ref mini.mountPoints, 4);

                mini.mountPoints[0] = pilot;
                mini.mountPoints[1] = passenger;
                mini.mountPoints[2] = MakeMount(new Vector3(0.6f, 0.2f, -0.2f));
                mini.mountPoints[3] = MakeMount(new Vector3(-0.6f, 0.2f, -0.2f));

                MakeSeat(new Vector3(0.6f, 0.2f, -0.5f));
                MakeSeat(new Vector3(-0.6f, 0.2f, -0.5f));
            }

            void MakeSeat(Vector3 position)
            {
                BaseEntity seat = GameManager.server.CreateEntity(chairPrefab, mini.transform.position);
                if (seat == null)
                {
                    return;
                }

                seat.SetParent(mini);
                seat.Spawn();
                seat.transform.localPosition = position;
                seat.SendNetworkUpdateImmediate(true);
            }

            BaseVehicle.MountPointInfo MakeMount(Vector3 position)
            {
                return new BaseVehicle.MountPointInfo
                {
                    pos       = position,
                    rot       = mini.mountPoints[1].rot,
                    prefab    = mini.mountPoints[1].prefab,
                    mountable = mini.mountPoints[1].mountable,
                };
            }
        }
    }
}