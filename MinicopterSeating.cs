using System;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Minicopter Seating", "Bazz3l", "1.0.8")]
    [Description("Allows 2 extra seats on the mini copter")]
    class MinicopterSeating : RustPlugin
    {
        private const string Perm = "minicopterseating.mymini";

        #region Config
        private PluginConfig config;

        protected override void LoadDefaultConfig()
        {
            Config.WriteObject(GetDefaultConfig(), true);
        }

        private PluginConfig GetDefaultConfig()
        {
            return new PluginConfig
            {
                MyMiniOnly = false
            };
        }

        private class PluginConfig
        {
            public bool MyMiniOnly;
        }
        #endregion

        #region Oxide
        void Init()
        {
            permission.RegisterPermission(Perm, this);

            config = Config.ReadObject<PluginConfig>();
        }

        void OnEntitySpawned(MiniCopter mini)
        {
            if (mini == null || mini.ShortPrefabName != "minicopter.entity") return;

            if (config.MyMiniOnly && mini.OwnerID == 0) return;
            if (config.MyMiniOnly && !permission.UserHasPermission(mini.OwnerID.ToString(), Perm)) return;

            if (mini.mountPoints.Length < 4)
                mini?.gameObject.AddComponent<CopterSeating>();
        }
        #endregion

        #region Scripts
        class CopterSeating : MonoBehaviour
        {
            public BaseVehicle mini;

            void Awake()
            {
                mini = GetComponent<BaseVehicle>();
                if (mini == null)
                {
                    Destroy(this);
                    return;
                }

                Array.Resize(ref mini.mountPoints, 4);

                BaseVehicle.MountPointInfo mountPoint1 = new BaseVehicle.MountPointInfo
                {
                    pos       = new Vector3(0.6f, 0.2f, -0.2f),
                    rot       = mini.mountPoints[0].rot,
                    prefab    = mini.mountPoints[1].prefab,
                    mountable = mini.mountPoints[0].mountable,
                };

                BaseVehicle.MountPointInfo mountPoint2 = new BaseVehicle.MountPointInfo
                {
                    pos       = new Vector3(-0.6f, 0.2f, -0.2f),
                    rot       = mini.mountPoints[0].rot,
                    prefab    = mini.mountPoints[1].prefab,
                    mountable = mini.mountPoints[0].mountable,
                };

                mini.mountPoints[2] = mountPoint1;
                mini.mountPoints[3] = mountPoint2;

                MakeSeat(mini, new Vector3(0.6f, 0.2f, -0.5f));
                MakeSeat(mini, new Vector3(-0.6f, 0.2f, -0.5f));
            }

            void MakeSeat(BaseVehicle mini, Vector3 locPos)
            {
                BaseEntity seat = GameManager.server.CreateEntity("assets/prefabs/vehicle/seats/passengerchair.prefab", mini.transform.position) as BaseEntity;
                if (seat == null)
                {
                    return;
                }

                seat.SetParent(mini);
                seat.transform.localPosition = locPos;
                seat.Spawn();
            }
        }
        #endregion
    }
}
