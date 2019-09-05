using System;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Horse Seating", "Bazz3l", "1.0.1")]
    [Description("Adds 2 seats to horse")]
    class HorseSeating : RustPlugin
    {
        void OnEntitySpawned(RidableHorse horse)
        {
            if (horse == null)
            {
                return;
            }

            if(horse.transform.GetComponentsInChildren<Seating>().Length > 0)
            {
                return;
            }

            horse.gameObject.AddComponent<Seating>();
        }

        class Seating : MonoBehaviour
        {
            public BaseVehicle horse;

            void Awake()
            {
                horse = GetComponent<BaseVehicle>();
                if (mini == null)
                {
                    Destroy(this);
                    return;
                }

                Array.Resize(ref horse.mountPoints, 2);

                BaseVehicle.MountPointInfo mountPoint2 = new BaseVehicle.MountPointInfo
                {
                    pos       = new Vector3(0f, 0f, -0.2f),
                    rot       = mini.mountPoints[0].rot,
                    prefab    = mini.mountPoints[1].prefab,
                    mountable = mini.mountPoints[0].mountable,
                };

                horse.mountPoints[2] = mountPoint1;
            }
        }
    }
}
