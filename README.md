# Iguana's Overhaul, functional changes
A mod, consisting of functional changes for Amazing Cultivation Simulator, not just fixes.

## Currently implemented changes

* Fix Trimerous Essence Price - The Trimerous Essence Pill has a buy price and item quantity equal to equivalent items (SR pool).
* Change Talisman of Foreseer from "Advanced Talismans" and "Luck with Talisman Room" to 96% quality, so it actually has an adventure exploration effect
* The golden core tier 3 has a 90% chance for breakthrough at PS instead of the current 10%, more in line with the effect at T2 (100%) and T4 (25%) and its effect on breakthroughs at golden core (150% like T2).
* Skill Level Everywhere (optional with files deletion) - Displays Skill Level instead of Ability Rating ([Standalone mod](https://www.nexusmods.com/amazingcultivationsimulator/mods/20/))
* Story Button Interaction Change - Changes the interaction with Ancient Caskets and Festive Goods, making it select an opener randomly.
* Sort Manuals by Attainment (optional) - Sorts the manuals by ascending attainment in the Manual Library. Also displays a small element icon next to manuals. Takes into account the element of the cultivator if any ([Standalone mod](https://www.nexusmods.com/amazingcultivationsimulator/mods/21/))
* Sync Perfect Alignment moodlet - the perfect alignment moodlet is changed to trigger at the same element strength (1.85) as optimal cultivation speed and golden core
* Heavenly Reforge Limit increased (optional) - Increases the Heavenly Reforge Cap from 36 (Vanilla) to 2048. ([Standalone mod](https://mega.nz/file/O10BVYwR#N09NIqmaX3KZ_M72BKY-XJ6wlhlhTjH4DEvsj5qtElI))
* Agency Population Limit increased (optional) - Increases the Agency Population limit from 400 000 (Vanilla) to 999 999. ([Standalone mod](https://mega.nz/file/vwNA3RzY#ft7q0U-nhHHuCv7QHXRuJ1z0BCtpWrY6SFRkQlo-5Y8))
* Farmable Strange Mushroom - Adds Strange Mushroom as a rare drop to the Mushroom plant, with similar rates to Red Ginseng.
* Wudang DLC Translation" - Translates the Duck and related untranslated content added with the Wudang DLC
* Fix Camping Head UI (optional) - Various improvements to the Head UI when camping ([Standalone Mod](https://www.nexusmods.com/amazingcultivationsimulator/mods/22/))
* Add Sect Rules (optional) -  better control over the popups when playing, both through additional Sect Rules and Concern/Neglect Sect Event rules. See detailed changes in the [[Standalone Mod description]](https://www.nexusmods.com/amazingcultivationsimulator/mods/24)
* Infinite Mouse Scrolling (optional) - Removes the limit to zooming out with the scroll wheel, matching prior behaviour with PageUp. ([Standalone mod](https://www.nexusmods.com/amazingcultivationsimulator/mods/23)).
* Instant Story Hotkey (optional) - Instantly choose the story option associated with an hotkey when pressing the key.
* Display Base Mental State (optional) - Displays the base mental state superimposed over the current mental statein the profile info for Xiandao cultivators ([Standalone mod](https://www.nexusmods.com/amazingcultivationsimulator/mods/18)).
* Display max Mood at 200 (optional) - Whenever the game uses mood for calculations, it uses 200 as maximum value and not 100 as it is displayed in-game. This corrects the in-game indicators to treat 200 mood as the actual maximum to avoid confusion.
Everything that isn't marked as optional can still be manually disabled by removing the modified entities from the mod files.
* Display Adventure Results Notifications - Adds a small notification on adventure return if some harvest has been obtained.
* Chance Estimation Precision - Increases the precision of estimates from GetRateString, anything below 1% is listed now as insignificant.
* Formation Sharing - Reveals the Formation Diagram Sharing/Import buttons, in both normal gameplay as well as RPG Map Maker.
* Body Cultivation+ Less Grindy Remolding - Improves and Makes the remolding process simpler, faster, and cleaner. Color filters work as intended, by removing unwanted labels (unless required by the body part).

## Install instructions

Download the latest release, extract the iguana_acs_functions into the Mods folder. If the release is behind the Main version and you want to update to the preview version, download the repository directly, and extract the contents of the archive into the iguana_acs_functions folder, located in the Mods folder.
(Due to the inclusion of Harmony Patches, it's best to stick with the Release.)

Activate the Mod in Mod Manager, make sure to load it after any of the prerequisite Mods.

For ensuring that Patches are loaded, restart the game.

## Configuration

Some of the changes are configurable on a per-save basis. To do so, in the menu item System's dropdown select "MLL设置".

![screenshot of the system menu dropdown with the cursor on the "MLL设置" option](https://i.imgur.com/E2HtqCW.png)

You can then disable the changes you might not want to apply to your save. Some of these changes require you to save and relaunch the game then load to apply.

![screenshot of the MLL Configuration menu](https://i.imgur.com/nWPUddG.png)

In case the mod modifies XML files, to disable it entirely you'll need to remove the XML files related to the mod from its subfolders.

## Compatibility with other mods

The full list of changed entities can be seen at the bottom. This mod does not guarantee compatibility with any of the mods that also change those entities.

### Suggested Load Order

* ModLoaderLite
* **[iguana_acs_fix](https://github.com/iguanacore/iguana_acs_fix)**
* **[iguana_acs_functions](https://github.com/iguanacore/iguana_acs_functions)**
* **[iguana_acs_features](https://github.com/iguanacore/iguana_acs_features)**
* Anything else
* Alternative Translations

## List of changed entities

Below is a list of changed entities. To revert a particular change from the mod, either comment out the relevant lines and entities, or delete the files in question. Changed files in the Language\OfficialEnglish directory are related to translation and can be reverted by removing them.

For example, removing the Trimerous Essence Price change requires the removal of iguana_acs_functions\Settings\ThingDef\Item\Item_DLC_WuDang.xml, as well as the corresponding file under Language\OfficialEnglish.

### Items

* `Item_Dan_SanHuaHuangJing` - Fix Trimerous Essence Price
* `Item_StoneBox2 and Item_SpringBox` - Ancient Caskets and Festive Goods, related to Story Button Interaction Change

### Plants

* `Mushroom` - Farmable Strange Mushroom

### JianghuNpc

* `Npc_WuDang_1` - Wudang DLC Translation

### MapStories

* (`MapStory_Item`) `Story_Item_E_FuBox` - Change Talisman of Foreseer in starting experiences
* `Settings/MapStories/...` - Add Sect Rules:
    * `MapStory_FillingLv1.xml` : City_Base, Business_Base
    * `MapStory_FillingLv2.xml`: Story_LingZhiGet, Story_LingZhiBeg
    * `MapStory_Special.xml`: Secrets_Esoterica1, Secrets_Esoterica2, Secrets_FabaoAppear1, Secrets_FabaoAppear2, Secrets_FabaoAppear3, Secrets_FabaoAppear3_Cold, Secrets_FabaoAppear3_Hot, Secrets_DongFuAppear, Secrets_DongFuAppear_Cold, Secrets_DongFuAppear_Hot, Secrets_DongFuAppear_Manual, Secrets_DongFuAppear_Medicine, Secrets_Magic, Secrets_Magic_Cold, Secrets_Magic_Hot
    * `MapStory_SpecialGong.xml`: Secrets_Gong2, Secrets_Gong3, Secrets_Gong4, Secrets_Gong5, Secrets_Gong6,Secrets_Gong7

### Properties

* `SkillProperty` - Skill Level Everywhere

### Message displays / UI Events
* `Settings/MsgShow/Event/EventShow.xml` - Event display changes for Add Sect Rules
* `Settings/MsgShow/Rule/Rule,xml` - Rule changes for Add Sect Rules
* `Settings/Ui/NpcUIEvent/NewNpcUIEvent.txt` - Add Sect Rules

### Functions

Modifications applied by iguana_acs_functions.dll.

* `FabaoData.AddGodCount`, `FabaoData.LoadInit` - Heavenly Reforge Limit increased, Transpiler changing ldc.i4.s 36 to ldc.i4 2048
* `OutspreadMgr.Region.AddPopulation`, `OutspreadMgr.Region.RawAddPopulation`, `OutspreadMgr.GetRegionPopulationAddPerday` - Agency Population Limit increased, Transpiler changing ldc.i4 400000 to ldc.i4 999999
* `Wnd_StorySelect.SelectClicking` - Instant Story Hotkey
* `MapStoryMgr.TriggerStorySelection`, `MapStoryMgr.DidStorySelect`, `MessageMgr.AddMessage`, `MsgShowMgr.GetStorySelect`- Add Sect Rules
* `Panel_ThingInfo.ShowNpc` - Display Base Mental State
* `PlacesMgr.Step`, `Wnd_GameMain.AddNpcHead`, `Wnd_GameMain.AddNpcHeadSimple` - Fix Camping Head UI
* `UILogicMode_Global.OnScroll` - Infinite Mouse Scroll
* `Wnd_StorySelect.SelectClicking` - Instant Story Hotkey
* `Panel_NpcInfoPanel.UpdateSkill`, `Panel_NpcPropertyPanel.UpdateSkill`, `Wnd_NpcWork.ItemRoolOver` - Skill Level Everywhere
* `Wnd_CangJingGeWindow.CheckEsoLise`, `Wnd_CangJingGeWindow.RenderEsoListItem` - Sort Manuals By Attainment
* `Wnd_NpcInfo.OnInit`, `Wnd_GameMain.OnInit` - Display max Mood at 200
* `GameUlt.GetRateString` - Chance Estimation Precision
* `Wnd_CreateZhen.OnShown` - Formation Sharing
* `Wnd_BodyRollShow.Begin`, `Wnd_BodyRollShow.OnHide`, `XiaWorld.PracticeMgr.GetRandomQuenchingLabelList` - Body Cultivation+ Less Grindy Remolding

### Other files

* `Scripts\main.lua` - main LUA mod loading utility
* `Scripts\fix-gc-tier3-breakthrough-multiplier.lua` - GC T3 breakthrough multiplier fix
* `Scripts\sync-perfect-alignment-moodlet.lua` - Sync Perfect Alignment moodlet
* `Scripts\MapStory\MapStory.lua` - Agency Population Limit increased, MapStoryHelper.SlightlyIncreaseAllPop and MapStoryHelper.IncreaseAllPop
* `Scripts\add-sect-rules.lua` - Add Sect Rules
* `iguana_acs_functions.dll` - Configuration and Functions

## How to Contribute

Any Issues/Pull Requests are welcome. To ensure a similar level of quality between all parts of the mod, here's a few guidelines.

When changing vanilla aspects, keep in line with the original naming logic.
* If the original was MapStory_Item.xml, the PR should use the same filename with MapStory_Item.xml.

If the fixes exist as a standalone mod, include a link to it.

## Credits/Contributions

* ucddj - Trimerous Essence Price, FuBox change, Skill Level Everywhere, Sort Manuals by Attainment, Sync Perfect Alignment moodlet, Fix Camping Head UI, Add Sect Rules, Infinite Mouse Scrolling, Instant Story Hotkey, Display Base Mental State, Display max Mood at 200, Display Adventure Results Notifications
* Gothmos - Add Sect Rules
* NecrCode - Skill Level Everywhere additions, Body Cultivation+ Less Grindy Remolding
