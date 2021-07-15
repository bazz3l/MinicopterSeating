using Newtonsoft.Json;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Minicopter Seating", "Bazz3l", "1.1.8")]
    [Description("Spawns an extra seat each side of the minicopter.")]
    class MinicopterSeating : RustPlugin
    {
        #region Fields

        private const string CHAIR_PREFAB = "assets/prefabs/vehicle/seats/passengerchair.prefab";
        private readonly SeatingManager _manager = new SeatingManager();
        private static PluginConfig _config;
        
        #endregion
        
        #region Config

        protected override void LoadDefaultConfig() => _config = PluginConfig.DefaultConfig();

        protected override void LoadConfig()
        {
            base.LoadConfig();

            try
            {
                _config = Config.ReadObject<PluginConfig>();

                if (_config == null)
                {
                    throw new JsonException();
                }
            }
            catch
            {
                PrintWarning("Default config loaded.");

                LoadDefaultConfig();
            }
        }

        protected override void SaveConfig() => Config.WriteObject(_config, true);
        
        private class PluginConfig
        {
            #region Fields

            public bool EnableTail;
            public bool EnableSide;
            public Vector3 TailPosition;
            public Vector3 LeftPosition;
            public Vector3 RightPosition;

            #endregion
            
            public static PluginConfig DefaultConfig()
            {
                return new PluginConfig
                {
                    EnableTail = false,
                    EnableSide = true,
                    TailPosition = new Vector3(0f, 0.4f, -1.5f),
                    LeftPosition = new Vector3(-0.6f, 0.2f, -0.3f),
                    RightPosition = new Vector3(0.6f, 0.2f, -0.3f)
                };
            }
        }

        #endregion

        #region Oxide

        private void Unload()
        {
            _config = null;
        }

        private void OnEntitySpawned(BaseVehicle mini)
        {
            if (mini.mountPoints.Count == 2 && mini.ShortPrefabName == "minicopter.entity")
            {
                _manager.Setup(mini);
            }
        }
        
        #endregion

        #region Seating

        private class SeatingManager
        {
            public void Setup(BaseVehicle vehicle)
            {
                CreateSideSeating(vehicle);
                CreateTailSeating(vehicle);
            }

            private void CreateSideSeating(BaseVehicle vehicle)
            {
                if (!_config.EnableSide) return;

                CreateSeat(vehicle, _config.LeftPosition);
                CreateSeat(vehicle, _config.RightPosition);
            }
            
            private void CreateTailSeating(BaseVehicle vehicle)
            {
                if (!_config.EnableTail) return;
                
                CreateSeat(vehicle, _config.TailPosition);
            }

            private void CreateSeat(BaseVehicle vehicle, Vector3 position)
            {
                BaseEntity entity = GameManager.server.CreateEntity(CHAIR_PREFAB, vehicle.transform.position);
                entity.SetParent(vehicle);
                entity.transform.localPosition = position;
                entity.Spawn();
                
                vehicle.mountPoints.Add(CreateMount(vehicle, position));
            }

            private BaseVehicle.MountPointInfo CreateMount(BaseVehicle vehicle, Vector3 position)
            {
                return new BaseVehicle.MountPointInfo
                {
                    pos = position,
                    rot = vehicle.mountPoints[1].rot,
                    prefab = vehicle.mountPoints[1].prefab,
                    mountable = vehicle.mountPoints[1].mountable,
                };
            }
        }
        
        #endregion
    }
}
