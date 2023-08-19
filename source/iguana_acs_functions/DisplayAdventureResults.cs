using XiaWorld;
using HarmonyLib;
using FairyGUI;
using XiaWorld.UI.InGame;
using System.Collections.Generic;
using UnityEngine;

namespace iguana_acs_functions
{
    public class UIAdventureResultListItem : GComponent
    {
        public GImage npcBackground;
        public UI_NpcRolepaint npcRolePaint;
        public NpcRolepaintUI rolepaint;
        public GTextField charName;
        public UIAdventureResultListItem(Npc npc, List<int> AwardItems)
        {
            npcBackground = UIPackage.CreateObjectFromURL("ui://0xrxw6g7y0oqd5") as GImage;
            npcBackground.height = 40;
            npcBackground.width = 120;
            npcBackground.x = 0;
            npcBackground.y = 39;
            this.AddChild(npcBackground);
            npcRolePaint = UIPackage.CreateObjectFromURL("ui://0xrxw6g7v1ia2ovt9") as UI_NpcRolepaint;
            rolepaint = new NpcRolepaintUI(npcRolePaint);
            rolepaint.SetNpc(npc);
            npcRolePaint.SetScale(-0.13f, 0.13f);
            npcRolePaint.x = -npcRolePaint.actualWidth;
            npcRolePaint.y = 6;
            this.AddChild(npcRolePaint);
            charName = new GTextField
            {
                x = 2,
                y = 60,
                color = Color.white,
                stroke = 1,
                strokeColor = Color.black,
                text = npc.Name
            };
            charName.textFormat.bold = true;
            this.AddChild(charName);
            for (int i = 0; i < AwardItems.Count; i++)
            {
                Thing item = ThingMgr.Instance.FindThingByID(AwardItems[i]);
                UI_ctresicon resIcon = UIPackage.CreateObjectFromURL("ui://0xrxw6g7gdpn2ovts") as UI_ctresicon;
                resIcon.title = $"{item.Count}";
                resIcon.x = 65 + i * resIcon.width;
                resIcon.y = 44;
                resIcon.icon = item.def.GetTexPath(0);
                resIcon.m_icon.color = item.def.Color;
                resIcon.tooltips = item.def.ThingName;
                this.AddChild(resIcon);
            }
            npcBackground.width = -npcRolePaint.actualWidth + 30 * AwardItems.Count;
        }
    }
    public class DisplayAdventureResults
    {
        public static bool enabled = true;
        internal class AdventureResultDisplay
        {
            public GComponent UIContainer = null;
            public Queue<UIAdventureResultListItem> UIAdventureResultList = new Queue<UIAdventureResultListItem>();
            public UIAdventureResultListItem current = null;
            public float adventureResultTimeTracker = 0;
        }
        static AdventureResultDisplay display = new AdventureResultDisplay();

        [HarmonyPatch(typeof(Npc), "DropAllAwardItem")]
        class addAdventureResult
        {
            static void Prefix(Npc __instance, out List<int> __state)
            {
                if (__instance.AwardItems == null)
                {
                    __state = null;
                }
                else
                {
                    __state = new List<int>(__instance.AwardItems);
                }
            }

            static void Postfix(bool __result, List<int> __state, Npc __instance)
            {
                if (!enabled) { return; }
                if (!__result) { return; }
                if (__state == null || __state.Count == 0) { return; }
                UIAdventureResultListItem component = new UIAdventureResultListItem(__instance, __state);
                display.UIAdventureResultList.Enqueue(component);
            }
        }

        [HarmonyPatch(typeof(Wnd_GameMain), "OnInit")]
        class initializeAdventureResultList
        {
            static void Postfix(Wnd_GameMain __instance)
            {
                if (!enabled) { return; }
                UI_WindowGameMain UIInfo = __instance.UIInfo;
                if (display.UIContainer != null)
                {
                    display.UIContainer.Dispose();
                }
                display.UIContainer = new GComponent();
                display.UIContainer.x = UIInfo.m_guanxing.x + UIInfo.m_guanxing.width;
                display.UIContainer.y = UIInfo.m_guanxing.y - 10;
                UIInfo.AddChildAt(display.UIContainer, UIInfo.numChildren);
            }
        }

        [HarmonyPatch(typeof(Wnd_GameMain), "OnUpdate")]
        class handleNotifications
        {
            static void Postfix()
            {
                if (!enabled) { return;  }
                if (display.UIAdventureResultList.Count == 0 && display.current == null) { return; }
                if (display.current != null)
                {
                    display.adventureResultTimeTracker += Time.smoothDeltaTime;
                    if (display.adventureResultTimeTracker > 10)
                    {
                        display.UIContainer.RemoveChild(display.current);
                        for (int i = 0; i < display.current.numChildren; i++)
                        {
                            display.current.GetChildAt(i).Dispose();
                        }
                        display.current.Dispose();
                        display.current = null;
                        display.adventureResultTimeTracker = 0;
                    }
                }
                if (display.current == null && display.UIAdventureResultList.Count > 0)
                {
                    display.current = display.UIAdventureResultList.Dequeue();
                    display.UIContainer.AddChild(display.current);
                }
            }
        }
    }
}
