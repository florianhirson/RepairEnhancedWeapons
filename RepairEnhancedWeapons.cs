using System;
using System.Collections.Generic;
using Eleon.Modding;
using System.IO;
using Newtonsoft.Json;

public partial class RepairEnhancedWeapons : ModInterface
{
    static ModGameAPI GameAPI;
    ushort eventRepairId = 23989;
    ushort eventSetInventoryId = 25380;
    Dictionary<String, String> ItemsId;



    public void Game_Start(ModGameAPI dediAPI)
    {
        GameAPI = dediAPI;
        LogFile("chat.txt", "Mod Loaded");
        GameAPI.Console_Write("RepairWeapons! ");
        try
        {
            var text = File.ReadAllText("Content\\Mods\\RepairWeapons\\ItemsID.json");
            ItemsId = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
        }
        catch(Exception e)
        {
            LogFile("chat.txt", "Exception : " + e);
        }
    }

    private void LogFile(String FileName, String FileData)
    {
        if (!File.Exists("Content\\Mods\\RepairWeapons\\" + FileName))
        {
            File.Create("Content\\Mods\\RepairWeapons\\" + FileName);
        }
        string FileData2 = FileData + Environment.NewLine;
        File.AppendAllText("Content\\Mods\\RepairWeapons\\" + FileName, FileData2);
    }


    public void Game_Event(CmdId eventId, ushort seqNr, object data)
    {

        GameAPI.Console_Write($"ID:EVENT! {eventId} - {seqNr}");
        LogFile("chat.txt", $"ID:EVENT! {eventId} - {seqNr}");
        try
        {
            switch (eventId)
            {

                case CmdId.Event_Player_Connected:
                    int id = (int)data;
                    GameAPI.Console_Write("RepairEnhancedWeapons : player " + id + "connected! ");
                    LogFile("chat.txt", "Player " + id + "connected");
                    break;
                case CmdId.Event_ChatMessage:
                    ChatInfo ci = (ChatInfo)data;
                    if (ci.msg.StartsWith("s! "))
                    {
                        ci.msg = ci.msg.Remove(0, 3);
                    }
                    ci.msg = ci.msg.ToLower();
                    if (ci.msg.StartsWith("/repair"))
                    {
                        id = ci.playerId;
                        LogFile("chat.txt", "Player " + id + " fixed EnhancedWeapons");
                        GameAPI.Game_Request(CmdId.Request_Player_GetInventory, eventRepairId, new Eleon.Modding.Id(id));
                        
                    }
                    break;
                case CmdId.Event_Player_Inventory:
                    Inventory inventory = (Inventory) data;
                    if(seqNr == eventRepairId)
                    {
                        Repair(inventory);
                    }
                    
                    break;

                default:
                    GameAPI.Console_Write($"event: {eventId}");
                    var outmessage = "NO DATA";
                    if (data != null)
                    {
                        outmessage = "data: " + data.ToString();
                    }
                    GameAPI.Console_Write(outmessage);
                    
                    break;
            }
        }
        catch (Exception ex)
        {
            GameAPI.Console_Write(ex.Message);
            GameAPI.Console_Write(ex.ToString());
        }
    }

    public void Repair(Inventory inventory)
    {
        GameAPI.Console_Write("RepairEnhancedWeapons : beginning repairing ");
        LogFile("chat.txt", "RepairEnhancedWeapons : beginning repairing ");
        ItemStack[] bag = inventory.bag;

        for (int i = 0; i < bag.Length; i++)
        {
            if (bag[i].ammo != 0 && ItemsId.ContainsKey("" + bag[i].id)) //check if its a weapon and if the id matches
            {
                bag[i] = new ItemStack(bag[i].id, 1);
                GameAPI.Console_Write("RepairEnhancedWeapons : repair item " + ItemsId["" + bag[i].id] + ", id : " + bag[i].id);
                LogFile("chat.txt", "RepairEnhancedWeapons : repair item " + ItemsId["" + bag[i].id] + ", id : " + bag[i].id);
            } 
        }

        inventory.bag = bag;
        GameAPI.Console_Write("RepairEnhancedWeapons : repairing ended successfully ! ");
        LogFile("chat.txt", "RepairEnhancedWeapons : repairing ended successfully ! ");
        GameAPI.Game_Request(CmdId.Request_Player_SetInventory, eventSetInventoryId, inventory);              
    }

    public void Game_Update()
    {
    }

    public void Game_Exit()
    {
    }

}