using HarmonyLib;
using UnityEngine;
using XiaWorld;

namespace iguana_acs_functions
{
    public class InfiniteScroll
    {
        public static bool enabled = true;

        [HarmonyPatch(typeof(UILogicMode_Global), "OnScroll")]
        public class RemoveMouseScrollLimiter
        {
            static bool Prefix(Vector2 offset)
            {
                if (!InfiniteScroll.enabled) { return true; }
                if (!TeachExMgr.Instance.IsLockInput && !InputMgr.Instance.LockInput && !UILogicModeBase.SelectLock && !MapCamera.Instance.IsFreeze)
                {
                    MapCamera.Instance.SetOSize(Mathf.Clamp(MapCamera.Instance.MainCamera.orthographicSize - offset.y * 0.5f, 6f, 100f));
                }
                return false;
            }
        }
    }
}
