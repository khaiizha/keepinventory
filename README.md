# KeepInventory

An Unturned Plugin that saves the player inventory before they dies and then give back all the items back to the player.
This plugins also cater to drop item by a pre-defined percentage. Note that this is not my plugin, I did not create it. 
It just an old plugin i found and modified it for personal use.

## How to use

Require this permission to enable players to use the plugin

keepinventory.keep

To make the player drop based on percentage, you must put this permission:

* 50% drop: keepinventory.vip
* 20% drop: keepinventory.vvip
* 10% drop: keepinventory.arch

So in the permission file you'll going to have both the keepinventory.keep and keepinventory.vip

## Side Note

* Note that you need both .keep and .vip/.vvip/.arch if you want them to drop items based on certain percentage. 

* Using only the .keep will allow the user to keep 100% of the item.

* Editted some code for custom use. Sorry that I'm not using the configuration files for adjustable percentage.

* Item that is dropped on death is chosed randomly.

Ex: To maintain 100% item, 
<permission>keepinventory.keep</permission> 

Ex 2: to maintain % of item based on ranks
<permission>keepinventory.keep</permisson>
<permission>keepinventory.vip</permission>
