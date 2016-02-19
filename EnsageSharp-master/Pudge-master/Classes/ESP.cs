using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pudge_Plus.Classes
{
    class ESP
    {
        public static class Draw
        {
            public static class Interface
            {
                private static int MenuIndex = 0;
                private static int Width = 150;
                private static string Title = "Pudge+ By NadeHouse";
                private static string ToolTip = "Tool tip";
                private static List<Item> MenuItems = new List<Item>();
                private class Item
                {
                    public string Text { get; set; }
                    public int Index { get; set; }
                    public Variables.CustomInteger targetVariable { get; set; }
                    public string[] CustomText { get; set; }
                    public int Max { get; set; }
                    public int Min { get; set; }
                    public string ToolTip { get; set; }
                }
                public static void Add(string text, ref Variables.CustomInteger targetVariable, string ToolTip,  int Min = 0, int Max = 1, string[] CustomOverride = null)
                {
                    Item item = new Item();
                    item.Text = text;
                    item.Index = MenuIndex;
                    item.targetVariable = targetVariable;
                    item.CustomText = CustomOverride;
                    item.Min = Min;
                    item.Max = Max;
                    item.ToolTip = ToolTip;
                    MenuItems.Add(item);
                    MenuIndex++;
                }
                public static void Render()
                {
                    Vector2 StartingCoords = Variables.ESP_Notifier_StartingCoords;
                    StartingCoords.Y += 53 + 8;
                    Vector2 TitleCoords = StartingCoords;
                    TitleCoords.X += Width / 2 - ((Title.ToCharArray().Length * 6)/2);
                    
                    StartingCoords.X -= 5;
                    
                    Vector2 backdropUntil = new Vector2(Width, ((MenuIndex + 1) * 12) + 5 + 12 + 5 +3);
                    Drawing.DrawRect(StartingCoords, backdropUntil, new Color(0, 0, 0, 255));//Background (backdrop)
                    
                    Vector2 tooltipBanner = new Vector2(StartingCoords.X, backdropUntil.Y + StartingCoords.Y - 12 - 5 -3); //
                    Drawing.DrawRect(tooltipBanner, new Vector2(Width, 12 + 5), Color.RoyalBlue); //Tooltip background
                    Drawing.DrawText(MenuItems[Variables.Settings.SelectedIndex].ToolTip, new Vector2(tooltipBanner.X + Width/2 - ((MenuItems[Variables.Settings.SelectedIndex].ToolTip.ToCharArray().Length/2 * 5)), tooltipBanner.Y), Color.DarkGray, FontFlags.AntiAlias | FontFlags.Outline); //Tooltip text
                    Drawing.DrawRect(StartingCoords, new Vector2(Width, (12 * MenuIndex) + 10 + 12 + 3 + 12), Color.DarkBlue, true); //Borderline
                    Drawing.DrawText(Title, TitleCoords, Color.LightSkyBlue, FontFlags.AntiAlias | FontFlags.Outline); // Title
                    Vector2 underLineStart = new Vector2(StartingCoords.X, StartingCoords.Y + 12 + 1);
                    Vector2 underLineEnd = new Vector2(underLineStart.X + Width, underLineStart.Y);
                    Drawing.DrawLine(underLineStart, underLineEnd, Color.DarkBlue);
                    StartingCoords.X += 5;
                    StartingCoords.Y += 2;
                    foreach (var option in MenuItems)
                    {
                        Color color = Color.White;
                        if (option.Index == Variables.Settings.SelectedIndex)
                            color = Color.Cyan;
                        StartingCoords.Y += (12);
                        Drawing.DrawText(option.Text, StartingCoords, color, FontFlags.AntiAlias | FontFlags.Outline);
                        Vector2 valueCoords = StartingCoords;
                        valueCoords.X = (Width + 10) - 12 - 5;
                        string OptionText = "";
                        Color optionColor = Color.Lime;
                        if (option.CustomText != null)
                            OptionText = option.CustomText[option.targetVariable.val];
                        else
                            OptionText = option.targetVariable.val.ToString();
                        if (OptionText == option.Max.ToString())
                            OptionText = "Off";
                        if (OptionText == "Off")
                            optionColor = Color.Red;

                        Drawing.DrawText(OptionText, valueCoords, optionColor, FontFlags.AntiAlias | FontFlags.Outline);
                    }
                }
                public static class MenuControls
                {
                    public static void Left()
                    {
                        if (MenuItems[Variables.Settings.SelectedIndex].targetVariable.val > MenuItems[Variables.Settings.SelectedIndex].Min)
                        {
                            MenuItems[Variables.Settings.SelectedIndex].targetVariable.val--;
                        }
                        else
                            MenuItems[Variables.Settings.SelectedIndex].targetVariable.val = MenuItems[Variables.Settings.SelectedIndex].Max;
                    }
                    public static void Right()
                    {
                        if (MenuItems[Variables.Settings.SelectedIndex].targetVariable.val < MenuItems[Variables.Settings.SelectedIndex].Max)
                            MenuItems[Variables.Settings.SelectedIndex].targetVariable.val++;
                        else
                            MenuItems[Variables.Settings.SelectedIndex].targetVariable.val = MenuItems[Variables.Settings.SelectedIndex].Min;
                    }
                    public static void Down()
                    {
                        if (Variables.Settings.SelectedIndex >= 0 && Variables.Settings.SelectedIndex < ESP.Draw.Interface.MenuIndex -1)
                            Variables.Settings.SelectedIndex++;
                        else
                            Variables.Settings.SelectedIndex = 0;
                    }
                    public static void Up()
                    {
                        if (Variables.Settings.SelectedIndex <= ESP.Draw.Interface.MenuIndex-1 && Variables.Settings.SelectedIndex > 0)
                            Variables.Settings.SelectedIndex--;
                        else
                            Variables.Settings.SelectedIndex = ESP.Draw.Interface.MenuIndex -1;
                    }
                }
            }
            public static class Notifier
            {
                public static void Backdrop(int StartingX, int StartingY, int ClosingX, int ClosingY, Color color)
                {
                    Drawing.DrawRect(new Vector2(StartingX, StartingY), new Vector2(ClosingX, ClosingY), color);//Background (backdrop)
                }
                public static void Info(string Content, Color color, int Index, int x = 0, FontFlags flags = FontFlags.Outline | FontFlags.AntiAlias)
                {
                    Vector2 coords = Variables.ESP_Notifier_StartingCoords;
                    coords.Y += 12 * Index;
                    coords.X += x;
                    Drawing.DrawText(Content, coords, color, flags);
                     /*public static void Info(Hero target, string text, int Index, Color color, FontFlags flags = FontFlags.Outline | FontFlags.AntiAlias, int x = 0)
                {
                    Vector2 coords = Drawing.WorldToScreen(target.Position);
                    coords.Y -= 80;
                    coords.Y += 12 * Index;
                    coords.X += 75 + x;
                    Drawing.DrawText(text, coords, color, flags);
                }*/
            }
                private static void SelectedHeroTopEnemy(int Index, float Base, int Offset = 0)
                {
                    if (Variables.EnemyTracker[Index - Offset].EnemyTracker != null)
                    {
                        int BaseX = (int)Base + ((int)Variables.HeroIconWidth * (Index - Offset));
                        int BaseY = (int)Variables.ToolTipActivationY + 10;
                        int counter = 1;
                        Color itemColor = Color.Green;
                        var Player = ObjectMgr.GetPlayerById((uint)Index);
                        Drawing.DrawText(GlobalClasses.GetHeroNameFromLongHeroName(Player.Hero.Name), new Vector2(BaseX, BaseY), Color.Red, FontFlags.AntiAlias | FontFlags.Outline);
                        foreach (var p in Variables.EnemyTracker)
                        {
                            if (p != null)
                            {
                                if (p.EnemyTracker.Player.Name == Player.Name)
                                    foreach (var item in p.EnemyTracker.Inventory.Items)
                                    {
                                        itemColor = GlobalClasses.GetCostColor(item);
                                        Drawing.DrawText(item.Name.Remove(0, 5), new Vector2(BaseX, BaseY + (counter * 12)), itemColor, FontFlags.AntiAlias | FontFlags.Outline);
                                        counter++;
                                        itemColor = Color.Green;
                                    }
                            }
                        }
                    }
                }
                private static void SelectedHeroTopFriendly(int Index, float Base, int Offset = 0)
                {
                    string PlayerName = GlobalClasses.GetHeroNameFromLongHeroName(ObjectMgr.GetPlayerById((uint)Index).Hero.Name);
                    int counter = 1;
                    int BaseX = (int)Base + ((int)Variables.HeroIconWidth * (Index));
                    int BaseY = (int)Variables.ToolTipActivationY + 10;
                    Color itemColor = Color.Green;
                    Drawing.DrawText(PlayerName, new Vector2(BaseX, BaseY), Color.Red, FontFlags.AntiAlias | FontFlags.Outline);
                    var p = ObjectMgr.GetPlayerById((uint)Index);
                    foreach (var item in p.Hero.Inventory.Items)
                    {
                        itemColor = GlobalClasses.GetCostColor(item);
                        Drawing.DrawText(item.Name.Remove(0, 5), new Vector2(BaseX, BaseY + (counter * 12)), itemColor, FontFlags.AntiAlias | FontFlags.Outline);
                        counter++;
                        itemColor = Color.Green;
                    }
                }
                public static void SelectedHeroTop(int Index)
                {
                    try
                    {
                        var team = Variables.me.Team;
                        if (team == Team.Radiant) //enable only if player's team is radiant - BUG if on DIRE (needs fix)
                        {
                            if (Index >= 5) //Dire
                            {
                                if (team == Team.Dire) //if my team is the dire team
                                    SelectedHeroTopFriendly(Index - 5, Variables.ToolTipDireStart);
                                else // if my team is the radiant team but selected hero is dire - thus enemy selected in the dire region
                                    SelectedHeroTopEnemy(Index, Variables.ToolTipDireStart, 5);
                            }
                            else if (Index < 5) //Radiant
                            {
                                if (team == Team.Radiant) //if my friendly team is radiant
                                    SelectedHeroTopFriendly(Index, Variables.ToolTipRadiantStart);
                                else //enemy are radiant
                                    SelectedHeroTopEnemy(Index, Variables.ToolTipRadiantStart);
                            }
                        }
                    
                    }
                    catch (Exception ex)
                    {
                        //Print.Error("error caught in Top Tool Tip\n" + ex.Message);
                    }
                }
                public static void HeroVisible()
                {
                    if (Variables.me.IsVisibleToEnemies)
                    {
                        // Variables.DrawNotification = true;
                        ESP.Draw.Notifier.Info("Enemies can see you", Color.Red, 2);
                        if (Utils.SleepCheck("particleEffect"))
                        {
                            if (Variables.visibleGlow.IsDestroyed)
                                Variables.visibleGlow = new ParticleEffect(Variables.visibleParticleEffect, Variables.me);
                            Variables.visibleGlow.Restart();
                            if (Variables.DeveloperMode)
                                Print.Info("Particle effect added");
                            Utils.Sleep(500, "particleEffect");
                        }
                    }
                    else
                    {
                        Variables.DrawNotification = false;
                        if (!Variables.visibleGlow.IsDestroyed)
                            Variables.visibleGlow.Dispose();
                        ESP.Draw.Notifier.Info("Hidden from enemies", Color.Orange, 2);
                    }
                }
                public static void FriendlyVisible(Hero target)
                {
                    if (target.IsVisibleToEnemies)
                    {
                        if (Utils.SleepCheck("particleEffect" + target.Player.Name))
                        {
                            target.AddParticleEffect(Variables.visibleParticleEffect);
                            Utils.Sleep(500,"particleEffect" + target.Player.Name);
                        }
                    }
                }
                public static void SpiritBreakerCharge(Hero target)
                {
                    foreach (var mod in target.Modifiers)
                    {
                        if (mod.Name == "modifier_spirit_breaker_charge_of_darkness_vision")
                            ESP.Draw.Enemy.Info(target,"Charged by Spirit Breaker",0, Color.DarkOrange);
                    }
                }

            }
            public static class LastHit
            {
                public static void Marker(List<Creep> creeps, Hero me)
                {
                    try
                    {
                        foreach (var creep in creeps)
                        {
                            if (creep.Team != me.Team && creep.IsAlive && creep.IsVisible)
                            {
                                if (creep.Health <= (me.DamageAverage + me.BonusDamage) * 1 - creep.DamageResist)
                                    Drawing.DrawText("Killable", Drawing.WorldToScreen(creep.Position), Color.Gold, FontFlags.Outline | FontFlags.AntiAlias);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Print.Error(ex.Message);
                    }
                }
            }
            public static void TeleportCancel(Hero friendly)//(float dist, Modifier mod, Hero friendly)
            {
                var dist = friendly.Distance2D(ObjectMgr.LocalHero); //Distance from team mate to 'me'
                if (dist <= Variables.me.Spellbook.Spell1.CastRange && friendly.Name != Variables.me.Name) //Within hook range
                    foreach (var mod in friendly.Modifiers)
                        if (mod.Name.Contains("teleporting")) //Affected by teleport
                        {
                            var distance = dist;
                            var speed = 1600;
                            var time = distance / speed;
                            var hookAirTime = time * 2;

                            var remainingTime = mod.RemainingTime;
                            if (!(remainingTime - hookAirTime >= 0))
                                Drawing.DrawText("HOOK NOW", Drawing.WorldToScreen(friendly.Position), Color.Red, FontFlags.AntiAlias | FontFlags.Outline);
                            else
                                Drawing.DrawText("WAIT " + Math.Round((remainingTime - hookAirTime), 1, MidpointRounding.AwayFromZero), Drawing.WorldToScreen(friendly.Position), Color.Cyan, FontFlags.AntiAlias | FontFlags.Outline);
                        }
            }
            public static void HookEuls(Hero enemy)
            {
                var dist = enemy.Distance2D(ObjectMgr.LocalHero); //Distance from team mate to 'me'
                if (dist <= Variables.me.Spellbook.Spell1.CastRange && enemy.Name != Variables.me.Name) //Within hook range
                    foreach (var mod in enemy.Modifiers)
                        if (mod.Name == "modifier_eul_cyclone") //Affected by teleport
                        {
                            var distance = dist;
                            var speed = 1600;
                            var time = (distance - 100) / speed;
                            var hookAirTime = time;
                            var remainingTime = mod.RemainingTime;
                            var vec = enemy.Position;
                            vec.Z = 0;
                            var vec2D = Drawing.WorldToScreen(vec);
                            vec2D.Y -= 100;
                            if (!(remainingTime - hookAirTime >= 0))
                                Drawing.DrawText("HOOK NOW", vec2D, Color.Red, FontFlags.AntiAlias | FontFlags.Outline);
                            else
                                Drawing.DrawText("WAIT " + Math.Round((remainingTime - hookAirTime), 1, MidpointRounding.AwayFromZero), vec2D, Color.Cyan, FontFlags.AntiAlias | FontFlags.Outline);
                        }
            }
            public static class Enemy
            {
                public static void basic(Hero enemy)
                {
                    if (Variables.Settings.Basic_ESP_Value.val == 0)
                    {
                        ESP.Draw.Enemy.Info(enemy, enemy.Player.Name, 0, Color.White, FontFlags.Outline | FontFlags.AntiAlias);
                        ESP.Draw.Enemy.Info(enemy, int.Parse(enemy.Health.ToString()).ToString(), 1, Color.Red);
                        ESP.Draw.Enemy.Info(enemy, " ," + Math.Round((Decimal)enemy.Mana, 0, MidpointRounding.AwayFromZero).ToString(), 1, Color.Blue, FontFlags.AntiAlias | FontFlags.Outline, enemy.Health.ToString().ToCharArray().Length * 6);
                    }
                    int disCounter = 0;
                    if (Variables.Settings.Enemy_Skills_Value.val == 2) //Draw basic cool downs
                        foreach (var skill in enemy.Spellbook.Spells)
                        {
                            if (skill.AbilityState == AbilityState.OnCooldown)
                            {
                                Vector2 location = Drawing.WorldToScreen(enemy.Position);
                                location.Y += disCounter * 12;
                                Drawing.DrawText(skill.Name + " (" + (int)skill.Cooldown + ")", location, Color.Green, FontFlags.Outline | FontFlags.AntiAlias);
                                disCounter++;
                            }
                        }

                }
                public static void SkillShotText(string Text, Vector3 Location, float Duration, GlobalClasses.SkillShotClass item)
                {
                    if (Utils.SleepCheck(Text + "one")) // if not asleep
                    {
                        Utils.Sleep((double)Duration + 1000, Text + "one"); //make it sleep
                        Utils.Sleep(Duration, Text); //set delay
                    }
                    else //if in original sleep
                    {
                        Vector2 location2D = Drawing.WorldToScreen(Location);
                        location2D.X -= (6 * Text.ToCharArray().Length) / 2;
                        Drawing.DrawText(Text, location2D, Color.Cyan, FontFlags.AntiAlias | FontFlags.Outline);
                    }
                    if (Utils.SleepCheck(Text)) //delay complete
                        Variables.DrawTheseSkillshots.Remove(item);


                }
                public static void LastKnownPosition(Hero enemy, int enemyIndex)
                {
                    try
                    {if (enemy.IsAlive)
                        {
                            var Angle = enemy.FindAngleR();
                            Vector2 StraightDis = Drawing.WorldToScreen(enemy.Position); //Facing position line
                            StraightDis.X += (float)Math.Cos(Angle) * 500;
                            StraightDis.Y += (float)Math.Sin(Angle) * 500;
                            if (Drawing.WorldToScreen(Variables.EnemyTracker[enemyIndex].EnemyTracker.Position).Y > 15)
                            {
                                Drawing.DrawLine(Drawing.WorldToScreen(Variables.EnemyTracker[enemyIndex].EnemyTracker.Position), StraightDis, Color.Red);
                                Drawing.DrawText(string.Format("{0} {1}", GlobalClasses.GetHeroNameFromLongHeroName(enemy.Name), GlobalClasses.GetTimeDifference(Variables.EnemyTracker[enemyIndex].RelativeGameTime)), Drawing.WorldToScreen(Variables.EnemyTracker[enemyIndex].EnemyTracker.Position), Color.Cyan, FontFlags.AntiAlias | FontFlags.Outline);
                            }
                        }
                    }
                    catch (Exception ex)
                    { }
                }
                public static void SkillShotDisplay()
                {
                    var ents = ObjectMgr.GetEntities<Unit>().Where(x => x.ClassID == ClassID.CDOTA_BaseNPC).ToList();
                    foreach (var ent in ents)
                        for (var n = 0; n <= 4; n++)
                        {
                            try
                            {
                                var mod = ent.Modifiers.FirstOrDefault(x => x.Name == Variables.SkillShots[n].ModName);
                                if (mod == null) continue;
                                ParticleEffect effect;
                                if (!Variables.SkillShotEffect.TryGetValue(ent, out effect))
                                {
                                    effect = ent.AddParticleEffect(@"particles\ui_mouseactions\range_display.vpcf");
                                    effect.SetControlPoint(1, new Vector3(Variables.SkillShots[n].Range, 0, 0));
                                    Variables.SkillShotEffect.Add(ent, effect);
                                    var newSkillshot = Variables.SkillShots[n];
                                    //
                                    newSkillshot.Location = ent.Position;
                                    Variables.DrawTheseSkillshots.Add(newSkillshot);
                                    Drawing.DrawText(Variables.SkillShots[n].ModName, Drawing.WorldToScreen(ent.Position), Color.Cyan, FontFlags.AntiAlias | FontFlags.Outline);
                                    new ParticleEffect(@"particles/units/heroes/" + Variables.SkillShots[n].EffectName + ".vpcf", ent.Position);
                                }
                                break;
                            }
                            catch (Exception ex)
                            {
                                //Print.Info(ex.Message);
                            }

                        }
                    if (Variables.DrawTheseSkillshots.Count > 0) //Draw global skill shots
                        try
                        {
                            foreach (var skillshotToDraw in Variables.DrawTheseSkillshots)
                                ESP.Draw.Enemy.SkillShotText(skillshotToDraw.FriendlyName, skillshotToDraw.Location, skillshotToDraw.Duration, skillshotToDraw);
                        }
                        catch { }
                }
                public static void Skills(Hero enemy)
                {
                    if (enemy != null)
                    {
                        try
                        {
                            int counter = 0;
                            foreach (var spell in enemy.Spellbook.Spells)
                            {
                                int Height = 20;
                                if (Variables.Settings.Enemy_Skills_Value.val == 1)
                                    Height = 0;
                                if (spell == null || spell.Name == "attribute_bonus") continue;
                                int Cooldown = (int)spell.Cooldown;
                                //Print.Info(enemy.Name + " " + enemy.Spellbook.Spells.ToList().Count);
                                Vector2 heroBase = Drawing.WorldToScreen(enemy.Position) + new Vector2(- ((20 * (enemy.Spellbook.Spells.ToList().Count -1)/2)) , 40); //Base drawing point
                                if (Variables.Settings.Enemy_Skills_Value.val == 0)
                                    Drawing.DrawRect(heroBase + new Vector2(counter * 20 - 5, 0), new Vector2(20, 20), Drawing.GetTexture(string.Format("materials/ensage_ui/spellicons/{0}.vmat", spell.Name ))); //Skill icons
                                Drawing.DrawRect(heroBase + new Vector2(counter * 20 - 5, Height), new Vector2(20, Cooldown == 0 ? 6 : 22), new ColorBGRA(0, 0, 0, 100), true); //Skill box outlines
                                if (spell.ManaCost > enemy.Mana) //Out of mana - Draw background Blue
                                    Drawing.DrawRect(heroBase + new Vector2(counter * 20 - 5, Height), new Vector2(20, Cooldown == 0 ? 6 : 22), new ColorBGRA(0, 0, 150, 150));
                                if (Cooldown > 0) //Draw cool down
                                {
                                    var text = Cooldown.ToString();
                                    var textSize = Drawing.MeasureText(text, "Arial", new Vector2(10, 200), FontFlags.Outline | FontFlags.AntiAlias); //Measure text
                                    var textPos = (heroBase + new Vector2(counter * 20 - 5, Height) - 1 + new Vector2(10 - textSize.X / 2, -textSize.Y / 2 + 12));
                                    Drawing.DrawText(text, textPos, Color.White, FontFlags.AntiAlias | FontFlags.Outline);
                                }
                                if (spell.Level > 0)
                                    for (int lvl = 1; lvl <= spell.Level; lvl++)

                                        Drawing.DrawRect(heroBase + new Vector2(counter * 20 - 5 + 3 * lvl, Height + 2), new Vector2(2, 2), new ColorBGRA(255, 255, 0, 255), true); //Draw skill level
                                counter++; //Skill index
                            }
                            if (Variables.Settings.Enemy_Skills_Value.val == 0)
                            {
                                Item[] specialItems = { enemy.FindItem("item_blink"), enemy.FindItem("item_force_staff"), enemy.GetDagon() };
                                foreach (var item in specialItems)
                                {
                                    if (item != null)
                                    {
                                        int Cooldown = (int)item.Cooldown;
                                        Vector2 heroBase = Drawing.WorldToScreen(enemy.Position) + new Vector2(-((20 * (enemy.Spellbook.Spells.ToList().Count - 1) / 2)), 40); //Base drawing point
                                        Drawing.DrawRect(heroBase + new Vector2(counter * 20 - 5, 0), new Vector2(28, 20), Drawing.GetTexture(string.Format("materials/ensage_ui/items/{0}.vmat", item.Name.Remove(0, 5)))); //Skill box outlines
                                        Drawing.DrawRect(heroBase + new Vector2(counter * 20 - 5, 20), new Vector2(20, Cooldown == 0 ? 6 : 22), new ColorBGRA(0, 0, 0, 100), true); //Skill box outlines
                                        if (item.ManaCost > enemy.Mana) //Out of mana - Draw background Blue
                                            Drawing.DrawRect(heroBase + new Vector2(counter * 20 - 5, 20), new Vector2(20, Cooldown == 0 ? 6 : 22), new ColorBGRA(0, 0, 150, 150));
                                        if (Cooldown > 0) //Draw cool down
                                        {
                                            var text = Cooldown.ToString();
                                            var textSize = Drawing.MeasureText(text, "Arial", new Vector2(10, 200), FontFlags.Outline | FontFlags.AntiAlias); //Measure text
                                            var textPos = (heroBase + new Vector2(counter * 20 - 5, 20) - 1 + new Vector2(10 - textSize.X / 2, -textSize.Y / 2 + 12));
                                            Drawing.DrawText(text, textPos, Color.White, FontFlags.AntiAlias | FontFlags.Outline);
                                        }
                                        counter++; //Skill index
                                    }
                                }
                            }
                        }
                        catch
                        { }
                    }
                }
                public static void pudge(Hero enemy)
                {
                    if (Variables.Settings.Hook_Lines_value.val == 0)
                    {
                        var opponent = enemy.Position;
                        var distance = Math.Sqrt(Math.Pow((opponent.X - Variables.me.Position.X), 2) + Math.Pow((opponent.Y - Variables.me.Position.Y), 2)) - (enemy.HullRadius * 2);
                        Color color;
                        if (distance <= Variables.me.Spellbook.Spell1.CastRange)
                            color = Color.Green;
                        else if (distance <= Variables.me.Spellbook.Spell1.CastRange + 80)
                            color = Color.Orange;
                        else
                            color = Color.Red;
                        Drawing.DrawLine(Drawing.WorldToScreen(Variables.me.Position), Drawing.WorldToScreen(enemy.Position), color); //draw line between player and enemy
                                                                                                                                      // PrintInfo(me.Distance2D(enemy.Position).ToString());
                                                                                                                                      //PrintInfo("Drawing Line");
                    }
                    int maxDmg = -99;
                    if (Variables.Settings.Combo_Status_Value.val == 0)
                    {
                         maxDmg= HookHandler.CalculateMaximumDamageOutput(Variables.me, enemy);
                        var manaReq = HookHandler.CalculateManaRequired(Variables.me);
                        string comboMessage = "null";
                        if (enemy.Health - maxDmg <= 0)
                            comboMessage = "Instant Death";
                        else if (Variables.me.Mana - manaReq < 0)
                            comboMessage = "Need " + (int)(manaReq - Variables.me.Mana) + " more mana";
                        else if (maxDmg == 0)
                            comboMessage = "No combo available";
                        else
                            comboMessage = "Wounded with " + (enemy.Health - maxDmg) + " hp left";
                        ESP.Draw.Enemy.Info(enemy, comboMessage, 2, Color.Lime); //combo predict
                    }
                    if (Variables.Settings.Maximum_Damage_Output_Value.val == 0)
                    {
                        if (maxDmg == -99)
                            maxDmg = HookHandler.CalculateMaximumDamageOutput(Variables.me, enemy);
                        ESP.Draw.Enemy.Info(enemy, maxDmg.ToString(), 3, Color.Orange); //dmg calc
                    }
                    if (Variables.Settings.Mana_Required_Value.val == 0)
                         ESP.Draw.Enemy.Info(enemy, HookHandler.CalculateManaRequired(Variables.me).ToString(), 4, Color.Cyan); //mana calc
                }

                public static void zeus(Hero enemy)
                {
                    var ultSpell = Variables.me.Spellbook.SpellR;
                    var ultdmg = 0;
                    if (ultSpell.Level > 0)
                    {
                        if (Variables.me.AghanimState())
                            ultdmg = (int)ultSpell.AbilityData.FirstOrDefault(x => x.Name == "damage_scepter").GetValue(ultSpell.Level - 1);
                        else
                            ultdmg = (int)ultSpell.AbilityData.FirstOrDefault(x => x.Name == "damage").GetValue(ultSpell.Level - 1);
                        var healthAFterUlt = enemy.Health - (ultdmg * (1 - enemy.MagicDamageResist));
                        int healthAfter = (int)healthAFterUlt;
                        if (healthAfter < 0)
                            ESP.Draw.Notifier.Info("Ult is Lethal", Color.Red, 3);
                    }
                }
                public static void Info(Hero target, string text, int Index, Color color, FontFlags flags = FontFlags.Outline | FontFlags.AntiAlias, int x = 0)
                {
                    Vector2 coords = Drawing.WorldToScreen(target.Position);
                    coords.Y -= 80;
                    coords.Y += 12 * Index;
                    coords.X += 75 + x;
                    Drawing.DrawText(text, coords, color, flags);
                }
                public static void PredictionBox(HookHandler.PredictClass predict, Color color)
                {
                    var x = predict.PredictedLocation.X;
                    var y = predict.PredictedLocation.Y;
                    Color textC = Color.Green;
                    string displayText = "Prediction";
                    if (Variables.Settings.Auto_Hook_Value.val == 0)
                    if (predict.closest)
                    {
                        textC = Color.Red;
                        displayText = "Prediction [e]";
                    }
                    for (int i = 1; i != 3; i++)
                    {
                        Drawing.DrawLine(new Vector2(x - 50 + i, y - 50 + i), new Vector2(x + 50 + i, y - 50 + i), color); //Top Left to Top Right
                        Drawing.DrawLine(new Vector2(x + 50 + i, y - 50 + i), new Vector2(x + 50 + i, y + 50 + i), color); //Top Right to Bottom Right
                        Drawing.DrawLine(new Vector2(x + 50 + i, y + 50 + i), new Vector2(x - 50 + i, y + 50 + i), color); //Bottom Right to Bottom Left
                        Drawing.DrawLine(new Vector2(x - 50 + i, y + 50 + i), new Vector2(x - 50 + i, y - 50 + i), color); //Bottom Left to Top Left
                    }
                    Drawing.DrawText(displayText, new Vector2(x + 5, y + 55), textC, FontFlags.Outline | FontFlags.AntiAlias);
                }
            }
        }
        public static class Calculate
        {
            public static class Enemy
            {
                public static bool isMoving(Vector3 pos, int Index)
                {
                    if (pos != Variables.EnemiesPos[Index])
                        return true;
                    else
                        return false;
                }
                public static Hero ClosestToMouse(Hero source, float range = 1000)
                {
                    var mousePosition = Game.MousePosition;
                    var enemyHeroes =
                        ObjectMgr.GetEntities<Hero>()
                            .Where(
                                x =>
                                    x.Team == source.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                                    && x.Distance2D(mousePosition) <= range);
                    Hero[] closestHero = { null };
                    foreach (
                        var enemyHero in
                            enemyHeroes.Where(
                                enemyHero =>
                                    closestHero[0] == null ||
                                    closestHero[0].Distance2D(mousePosition) > enemyHero.Distance2D(mousePosition)))
                    {
                        closestHero[0] = enemyHero;
                    }
                    return closestHero[0];
                }
            }
            public static class Mouse
            {
                private static float RadiantMinX = Variables.ToolTipRadiantStart;
                private static int RadiantMaxX = 860;
                private static float DireMinX = Variables.ToolTipDireStart;
                private static int DireMaxX = 1395;
                private static float HeroIconWidth = Variables.HeroIconWidth;

                public static int SelectedHero(int MouseX)
                {
                    //1065 = min
                    //1395 = max
                    //330 = dif
                    //66 = width
                    for (int i = 0; i < 5; i++)
                        if (MouseX >= RadiantMinX + (HeroIconWidth * i) && MouseX <= RadiantMinX + (HeroIconWidth * (i + 1)))
                            return i;
                    for (int i = 0; i < 5; i++)
                        if (MouseX >= DireMinX + (HeroIconWidth * i) && MouseX <= DireMinX + (HeroIconWidth * (i + 1)))
                            return i + 5;
                    Print.Error("Error finding hero");
                       return -1;
                }
            }
            public static class Creeps
            {
                public static List<Creep> GetCreeps()
                {
                    try
                    {
                        return ObjectMgr.GetEntities<Creep>().Where(creep => (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                    || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep) && (
                        creep.IsAlive && creep.IsVisible && creep.IsSpawned &&
                         creep.Team == Variables.me.GetEnemyTeam() && creep.Distance2D(Variables.me) < 1500)).ToList(); //Get Creeps
                    }
                    catch
                    { return new List<Creep>(); }
                }
            }
            public static class SpecificLists
            {
                public static List<Player> GetPlayersNoSpecsNoIllusionsNoNull()
                {
                    try
                    {
                        return ObjectMgr.GetEntities<Player>().Where(player => player != null && player.Team != Team.Observer && player.Hero != null && !player.Hero.IsIllusion).ToList();
                    }
                    catch { return new List<Player>(); }
                }
                public static List<Hero> EnemyHeroNotIllusion(List<Player> baseList)
                {try
                    {
                        return baseList.Where(player => player.Hero.Team != Variables.me.Team && !player.Hero.IsIllusion).Select(player => player.Hero).ToList();
                    }
                    catch { return new List<Hero>(); }
                }
                public static List<Hero> TeamMates(List<Player> baseList)
                {try
                    {
                        return baseList.Where(player => (player.Hero.Team == Variables.me.Team)).Select(player => player.Hero).ToList();
                    }
                    catch { return new List<Hero>(); }
                }
            }
        }
    }
}

