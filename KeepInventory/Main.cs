using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.API;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.Unturned.Chat;



namespace KeepInventory
{
    public class Main : RocketPlugin
    {

        public const string LOGGER_PREFIX = "[KeepInventory]: Modified by Miw0 ";
        private System.Random rand = new System.Random();

        private List<UnturnedPlayer> deadAdmins = new List<UnturnedPlayer> ();
        private Dictionary<UnturnedPlayer, List<Item>> adminItems = new Dictionary<UnturnedPlayer, List<Item>> ();

        protected override void Load()
        {
            UnturnedPlayerEvents.OnPlayerRevive += Give;
            UnturnedPlayerEvents.OnPlayerDeath += GetAndDrop;

            Logger.Log (LOGGER_PREFIX + "Loaded KeepInventory - Modified by Miw0!");
        }
        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerRevive -= Give;
            UnturnedPlayerEvents.OnPlayerDeath -= GetAndDrop;

            Logger.Log (LOGGER_PREFIX + "Unloaded KeepInventory!");
        }

        private void Give(UnturnedPlayer player, UnityEngine.Vector3 position, byte angle)
        {
            double count = 0;
            for (int i = 0; i < deadAdmins.Count; i++)
            {
                if (deadAdmins[i].CSteamID.ToString () == player.CSteamID.ToString () && deadAdmins[i].CharacterName == player.CharacterName)
                {
                    count = adminItems[deadAdmins[i]].Count; 

                    for (int j = 0; j < count; j++)
                    {
                        if (adminItems[deadAdmins[i]][j] == null)
                            continue;

                        player.Inventory.forceAddItem (adminItems[deadAdmins[i]][j], true);
                    }
                    
                    adminItems.Remove (player);
                    deadAdmins.RemoveAt (i);
                    break;
                }
            }
        }

        private void GetAndDrop(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (!player.HasPermission("keepinventory.keep"))
            {
                return;
            }

            dropInventory(player);
            ClearInventory(player,cause,limb,murderer);
        }

        private void dropInventory(UnturnedPlayer player)
        {
            //drop items
            int totalItems = 0;
            int amountItemToDrop = 0;
            for (byte page = 0; page < 8; page++)
            {
                totalItems += player.Player.inventory.getItemCount(page); // set allItemsAmount
            }

            amountItemToDrop = calculateRandomItemsToDrop(player, totalItems);

            if (amountItemToDrop == totalItems && totalItems != 0)
            {
                dropRandomItems(player, amountItemToDrop);
            }
            else
            {
                dropRandomItems(player, amountItemToDrop);
            }
        }

        private void ClearInventory(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
           
            var playerInventory = player.Inventory;
            List<Item> ids = new List<Item> ();
            List<Item> clothes = new List<Item> ();

            // "Remove "models" of items from player "body""
            player.Player.channel.send ("tellSlot", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, ( byte ) 0, ( byte ) 0, new byte[0]);
            player.Player.channel.send ("tellSlot", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, ( byte ) 1, ( byte ) 0, new byte[0]);
            // Remove items
            for (byte page = 0; page < 8; page++)
            {
                var count = playerInventory.getItemCount (page);
                   
                for (byte index = 0; index < count; index++)
                {
                    ids.Add(playerInventory.getItem(page, 0).item);
                    playerInventory.removeItem(page, 0);
                }
                
            }

            // Unequip & remove from inventory
            player.Player.clothing.askWearBackpack (0, 0, new byte[0], true);
            clothes.Add (removeUnequipped (playerInventory));

            player.Player.clothing.askWearGlasses (0, 0, new byte[0], true);
            clothes.Add (removeUnequipped (playerInventory));

            player.Player.clothing.askWearHat (0, 0, new byte[0], true);
            clothes.Add (removeUnequipped (playerInventory));

            player.Player.clothing.askWearPants (0, 0, new byte[0], true);
            clothes.Add (removeUnequipped (playerInventory));

            player.Player.clothing.askWearMask (0, 0, new byte[0], true);
            clothes.Add (removeUnequipped (playerInventory));

            player.Player.clothing.askWearShirt (0, 0, new byte[0], true);
            clothes.Add (removeUnequipped (playerInventory));

            player.Player.clothing.askWearVest (0, 0, new byte[0], true);
            clothes.Add (removeUnequipped (playerInventory));
            clothes.AddRange (ids);
            deadAdmins.Add (player);
            adminItems.Add (player, clothes);
        }

        private Item removeUnequipped(PlayerInventory playerInventory)
        {
            for (byte i = 0; i < playerInventory.getItemCount (2); i++)
            {
                Item item = playerInventory.getItem (2, 0).item;
                playerInventory.removeItem (2, 0);
                return item;
            }

            return null;
        }

        private int calculateRandomItemsToDrop(UnturnedPlayer player, int allItemamount)
        {
            int finalPercentage = 0;
            if (player.HasPermission("keepinventory.arch"))
            {
                finalPercentage = 90;
            }
            else if (player.HasPermission("keepinventory.vvip"))
            {
                finalPercentage = 80;
            }
            else if (player.HasPermission("keepinventory.vip"))
            {
                finalPercentage = 50;
            }
            else
            {
                finalPercentage = 100;
            }
            return Convert.ToInt32(allItemamount - Math.Floor(allItemamount * (finalPercentage / (double)100)));
        }

        private void dropRandomItems(UnturnedPlayer player, int amount)
        {
            while (amount > 0)
            {
                byte page = Convert.ToByte(rand.Next(0, 8));
                byte itemsCountonPage = player.Player.inventory.getItemCount(page);

                if (itemsCountonPage > 0)
                {
                    byte index = Convert.ToByte(rand.Next(0, itemsCountonPage));
                    try
                    {
                        byte posX = player.Player.inventory.getItem(page, index).x;
                        byte posY = player.Player.inventory.getItem(page, index).y;
                        player.Player.inventory.askDropItem(player.CSteamID,page,posX,posY);
                        amount--;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }
    }
}
