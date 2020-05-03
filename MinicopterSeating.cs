using UnityEngine;
using System;

namespace Oxide.Plugins
{
    [Info("Minicopter Seating", "Bazz3l", "1.1.0")]
    [Description("Spawns an extra seat each side of the minicopter.")]
    class MinicopterSeating : RustPlugin
    {
        #region Fields
        SeatingManager _manager = new SeatingManager();
        #endregion

        #region Oxide
        void OnEntitySpawned(MiniCopter mini)
        {
            if (mini.mountPoints.Length < 4 && mini.ShortPrefabName == "minicopter.entity")
            {
                _manager.Setup((BaseVehicle) mini);
            }
        }
        #endregion

        #region SeatingManger
        class SeatingManager
        {
            const string _chairPrefab = "assets/prefabs/vehicle/seats/passengerchair.prefab";

            public void Setup(BaseVehicle mini)
            {
                BaseVehicle.MountPointInfo pilot = mini.mountPoints[0];
                BaseVehicle.MountPointInfo passenger = mini.mountPoints[1];

                Array.Resize(ref mini.mountPoints, 4);

                mini.mountPoints[0] = pilot;
                mini.mountPoints[1] = passenger;
                mini.mountPoints[2] = MakeMount(mini, new Vector3(0.6f, 0.2f, -0.2f));
                mini.mountPoints[3] = MakeMount(mini, new Vector3(-0.6f, 0.2f, -0.2f));

                MakeSeat(mini, new Vector3(0.6f, 0.2f, -0.5f));
                MakeSeat(mini, new Vector3(-0.6f, 0.2f, -0.5f));
            }

            void MakeSeat(BaseVehicle mini, Vector3 position)
            {
                BaseEntity seat = GameManager.server.CreateEntity(_chairPrefab, mini.transform.position);
                if (seat == null)
                {
                    return;
                }

                seat.SetParent(mini);
                seat.Spawn();
                seat.transform.localPosition = position;
                seat.SendNetworkUpdateImmediate(true);
            }

            BaseVehicle.MountPointInfo MakeMount(BaseVehicle mini, Vector3 position)
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
        #endregion
    }
}