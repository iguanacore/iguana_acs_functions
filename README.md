# Iguana's Overhaul, functional changes
A mod, consisting of functional changes for Amazing Cultivation Simulator, not just fixes.

## Currently implemented changes

* Fix Trimerous Essence Price - The Trimerous Essence Pill has a buy price and item quantity equal to equivalent items (SR pool).
* Change Talisman of Foreseer from "Advanced Talismans" and "Luck with Talisman Room" to 96% quality, so it actually has an adventure exploration effect
* The golden core tier 3 has a 90% chance for breakthrough at PS instead of the current 10%, more in line with the effect at T2 (100%) and T4 (25%) and its effect on breakthroughs at golden core (150% like T2).
* Skill Level Everywhere - Displays Skill Level instead of Ability Rating ([Standalone mod](https://www.nexusmods.com/amazingcultivationsimulator/mods/20/))
* Story Button Interaction Change - Changes the interaction with Ancient Caskets and Festive Goods, making it select an opener randomly.

## Install instructions

Download the latest release, extract the iguana_acs_functions into the Mods folder. If the release is behind the Main version and you want to update to the preview version, download the repository directly, and extract the contents of the archive into the iguana_acs_functions folder, located in the Mods folder.
(Due to the inclusion of Harmony Patches, it's best to stick with the Release.)

Activate the Mod in Mod Manager, make sure to load it after any of the prerequisite Mods.

For ensuring that Patches are loaded, restart the game.

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

For example, removing the Trimerous Essence Price change requires the removal of iguana_acs_fix\Settings\ThingDef\Item\Item_DLC_WuDang.xml, as well as the corresponding file under Language\OfficialEnglish.

### Items

* Item_Dan_SanHuaHuangJing - Fix Trimerous Essence Price
* Item_StoneBox2 and Item_SpringBox - Ancient Caskets and Festive Goods, related to Story Button Interaction Change

### MapStories

* (MapStory_Item) Story_Item_E_FuBox - Change Talisman of Foreseer in starting experiences

### Other files

* Scripts\main.lua - main LUA mod loading utility
* Scripts\fix-gc-tier3-breakthrough-multiplier.lua - GC T3 breakthrough multiplier fix
* iguana_acs_functions.dll - Skill Level Everywhere

## How to Contribute

Any Issues/Pull Requests are welcome. To ensure a similar level of quality between all parts of the mod, here's a few guidelines.

When changing vanilla aspects, keep in line with the original naming logic.
* If the original was MapStory_Item.xml, the PR should use the same filename with MapStory_Item.xml.

If the fixes exist as a standalone mod, include a link to it.

## Credits/Contributions

* ucddj - Trimerous Essence Price, FuBox change, Skill Level Everywhere