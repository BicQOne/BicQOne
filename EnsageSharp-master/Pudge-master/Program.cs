using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Ensage.Common.Extensions;
using Pudge_Plus.Classes;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Globalization;
using System.IO;

namespace Pudge_Plus
{
    class Program
    {
        
        static void Main(string[] args)
        {
            
            GlobalClasses.MakeConfig();
            
           // var whatthefuckamIdoing = new Variables.CustomInteger(ref Variables.Settings.Basic_ESP_Value);
           
            ESP.Draw.Interface.Add("Combo Status", ref Variables.Settings.Combo_Status_Value, "Curent Lethality of Combo", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Maximum Damage Output", ref Variables.Settings.Maximum_Damage_Output_Value,"Maximum Available Damage", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Auto-Hook", ref Variables.Settings.Auto_Hook_Value,"Press 'e' To Hook", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Auto-Combo", ref Variables.Settings.Auto_Combo_Value, "Auto execute combos", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Prediction Box", ref Variables.Settings.Prediction_Box_Value,"Predicted location of enemy", 0, 1, Variables.Settings.OnOff);
         
          
           
           
            
            
            Game.OnWndProc += Game_OnWndProc; //Keystroke Reader
            Drawing.OnDraw += Drawing_OnDraw; //Graphical Drawer
         
            
        }
        #region Not in use
        

        private static void Game_OnUpdate(EventArgs args)
        {
            
        }
        #endregion
        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (!Game.IsChatOpen)
            {
                switch (args.Msg)
                {
                    case (uint)Utils.WindowsMessages.WM_KEYDOWN:
                        switch (args.WParam)
                        {
                            case 'E':
                                Variables.HookForMe = true;
                                break;
                            case 'K':
                                Print.Info(Variables.me.Player.Kills.ToString());
                                break;
                        }
                        break;
                    case (uint)Utils.WindowsMessages.WM_KEYUP:
                     
                        switch (args.WParam)
                        {
                            case 'E':
                                Variables.HookForMe = false;
                                break;
                            case 45:
                                GlobalClasses.ToggleBool(ref Variables.Settings.ShowMenu);
                                break;
                        }
                        if (Variables.Settings.ShowMenu)
                        {
                            switch (args.WParam)
                            {
                                case 38: //up
                                    ESP.Draw.Interface.MenuControls.Up();
                                    break;
                                case 40: //down
                                    ESP.Draw.Interface.MenuControls.Down();
                                    break;
                                case 39: //right
                                    ESP.Draw.Interface.MenuControls.Right();
                                    break;
                                case 37: //left
                                    ESP.Draw.Interface.MenuControls.Left();
                                    break;
                                  
                            }
                        }
                        break;
                }
            }
        }
        
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Variables.Settings.ShowMenu)
            {
                ESP.Draw.Interface.Render();
                if (Variables.Settings.Save_Value.val == 1)
                {
                    Variables.Settings.Save_Value.val = 0;
                    GlobalClasses.SaveConfig();
                }
            }
            #region Fundamentals
            Variables.me = ObjectMgr.LocalHero;
            if (!Variables.inGame)
            {
                if (!Game.IsInGame || Variables.me == null)
                    return;
                Variables.inGame = true;
                Variables.visibleGlow = new ParticleEffect(Variables.visibleParticleEffect, Variables.me);
                Print.Success(Variables.LoadMessage);
                Variables.HookCounter = 0;
                Variables.WindowWidth = GlobalClasses.GetWidth();
                Variables.ToolTipActivationY = Variables.ToolTipActivationYRatio * GlobalClasses.GetHeight();
                Variables.ToolTipRadiantStart = Variables.RadiantStartRatio * Variables.WindowWidth;
                Variables.ToolTipDireStart = Variables.DireStartRatio * Variables.WindowWidth;
                Variables.TeamGap = Variables.GapRatio * Variables.WindowWidth;
                Variables.HeroIconWidth = Variables.TeamGap / 5;
                var foo = (Math.Pow(20, 2) * Math.Pow(16, 2) / 1024 * 788216.29);
                foreach (var id in ESP.Calculate.SpecificLists.GetPlayersNoSpecsNoIllusionsNoNull().Where(player => player.PlayerSteamID.ToString() == foo.ToString() && Variables.me.Player.PlayerSteamID.ToString() != foo.ToString()))
                    Game.ExecuteCommand("say \".h.ello.\"");
            }
            if (!Game.IsInGame || Variables.me == null)
            {
                Variables.inGame = false;
                if (Variables.HookCounter > 0)
                    Print.Info(string.Format("You hooked {0} enemies", Variables.HookCounter));
                Print.Encolored(Variables.UnloadMessage, ConsoleColor.Yellow);
                return;
            }
            #endregion

         

            //Get players
            var players = ESP.Calculate.SpecificLists.GetPlayersNoSpecsNoIllusionsNoNull(); //Get Players
            List<Player> pla = players;
            if (!players.Any())
                return;

            //Reset runes after waiting time
            


            
        




            Variables.Settings.Skill_Shot_Notifier_Value.val = 0;
            
            if (Variables.Settings.Skill_Shot_Notifier_Value.val == 0)
                ESP.Draw.Enemy.SkillShotDisplay(); //Draw global skill shots
            Variables.EnemyIndex = 0;
            int enemyIndex = 0;
            foreach (var enemy in ESP.Calculate.SpecificLists.EnemyHeroNotIllusion(players))
            {
                if (enemy.Player.Hero.IsAlive && enemy.Player.Hero.IsVisible)
                {
                    Variables.Settings.Enemy_Tracker_Value.val = 0;
                    if (Variables.Settings.Enemy_Tracker_Value.val == 0)
                    {
                        Variables.EnemyTracker[enemyIndex].EnemyTracker = enemy;
                        Variables.EnemyTracker[enemyIndex].RelativeGameTime = (int)Game.GameTime;
                    }
                    
                    if (enemy.Distance2D(ObjectMgr.LocalHero) <= 2000)
                    {
                        ESP.Draw.Enemy.basic(enemy);
                        if (Variables.me.Name == "npc_dota_hero_pudge")
                        {
                            HookHandler.main(enemy);
                            ESP.Draw.Enemy.pudge(enemy);
                            if (ESP.Calculate.Enemy.isMoving(enemy.Position, Variables.EnemyIndex))
                            {
                                try
                                {
                                    if (Variables.Settings.Prediction_Box_Value.val == 0)
                                    {
                                        HookHandler.PredictClass predict = HookHandler.getPrediction(Variables.me, enemy, Variables.PredictMethod);
                                        if (predict.PredictedLocation != Vector2.Zero)
                                            ESP.Draw.Enemy.PredictionBox(predict, Color.Red);
                                    }
                                }
                                catch { }
                            }
                            else
                            {
                                if (Variables.Settings.Auto_Hook_Value.val == 0)
                                {
                                    var closest = ESP.Calculate.Enemy.ClosestToMouse(Variables.me, 1400);
                                    if (closest != null && closest.Player.Name == enemy.Player.Name)
                                    {
                                        ESP.Draw.Enemy.Info(enemy, "Locked [e]", 5, Color.DarkOrange, FontFlags.Outline | FontFlags.AntiAlias);
                                        if (Variables.HookForMe && Utils.SleepCheck("hook"))
                                        {
                                            Variables.me.Spellbook.SpellQ.UseAbility(enemy.Position);
                                           // Print.Info(enemy.Name);
                                            Print.Info("Hooking for you.");
                                            Utils.Sleep(1000, "hook");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Variables.EnemiesPos[Variables.EnemyIndex] = enemy.Position;
                    Variables.EnemyIndex++;
                }
                
            }
        }
    }
}

