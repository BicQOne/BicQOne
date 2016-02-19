using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pudge_Plus.Classes
{
    class HookHandler
    {
        public static void main(Hero enemy)
        {
            #region Combo
            foreach (var mod in enemy.Modifiers)
            {
                if (Variables.Settings.Euls_Timer_Value.val == 0)
                    if (mod.Name == "modifier_eul_cyclone")
                        ESP.Draw.HookEuls(enemy);
                if (Variables.Settings.Auto_Combo_Value.val == 0)
                    if (mod.Name.ToUpper().Contains("PUDGE"))
                    {
                        //enemy.AddParticleEffect("particles/" + "items_fx/aura_shivas" + ".vpcf");

                        if (Utils.SleepCheck("pragmaOnce"))
                        {
                            switch (mod.Name)
                            {
                                case "modifier_pudge_meat_hook":
                                    QueueCombo(enemy);
                                    Utils.Sleep(3000, "pragmaOnce");
                                    break;
                            }
                        }
                    }
            }
            #endregion
        }
        
        private static void QueueCombo(Hero enemy)
        {
            var rot = Variables.me.Spellbook.SpellW;
            var dismember = Variables.me.Spellbook.SpellR;
            var urn = Variables.me.FindItem("item_urn_of_shadows");
            var eblade = Variables.me.FindItem("item_ethereal_blade");
            var dagon = Variables.me.GetDagon();
            Print.Error(string.Format("[{0}]",GlobalClasses.GetHeroNameFromLongHeroName(enemy.Name)));
            Variables.HookCounter++;
            if (!rot.IsToggled && rot.Level > 0) //rot
            {
                rot.ToggleAbility();
                Print.Info("Queued Rot");
            }
            if (urn != null) //urn
            {
                if (urn.CurrentCharges > 0)
                {
                    urn.UseAbility(enemy, true);
                    Print.Info("Queued Urn");
                }
            }
            if (eblade != null) //eblade
            {
                eblade.UseAbility(enemy, true);
                Print.Info("Queued Eblade");
            }
            if (dagon != null) //dagon
            {
                dagon.UseAbility(enemy, true);
                Print.Info("Queued Dagon");
            }
            if (dismember.Level > 0 && dismember.AbilityState == AbilityState.Ready) //dismember (ulti)
            {
                dismember.UseAbility(enemy, true);
                Print.Info("Queued Dismember");
            }
            Print.Error(string.Format("[{0}]", GlobalClasses.GetHeroNameFromLongHeroName(enemy.Name)));
        }
        public static int CalculateManaRequired(Hero me)
        {
            try
            {
                if (Utils.SleepCheck("manaCalcErrorTimeout"))
                {
                    var hook = me.Spellbook.Spell1;
                    var dismember = me.Spellbook.SpellR;
                    var urn = me.FindItem("item_urn_of_shadows");
                    var eblade = me.FindItem("item_ethereal_blade");
                    var dagon = me.GetDagon();
                    var ManaRequired = 0;

                    if (hook != null)
                        ManaRequired += (int)hook.ManaCost;
                    if (dismember != null)
                        ManaRequired += (int)dismember.ManaCost;
                    if (eblade != null)
                        ManaRequired += (int)eblade.ManaCost;
                    if (dagon != null)
                        ManaRequired += (int)dagon.ManaCost;
                    return (int)ManaRequired;
                }
                return 0;
            }
            catch (Exception ex)
            {
                // Print.Error("MANACALC: " + ex.Message);
                Utils.Sleep(500, "manaCalcErrorTimeout");
                return 0;
            }
        }
        public static int CalculateMaximumDamageOutput(Hero me, Hero enemy)
        {
            try
            {
                //NEED TO ADD EBLADE!!!!
                var hook = me.Spellbook.Spell1;
                var rot = me.Spellbook.SpellW;
                var dismember = me.Spellbook.SpellR;
                var urn = me.FindItem("item_urn_of_shadows");
                var dagon = me.GetDagon();
                var eblade = me.FindItem("item_ethereal_blade");
                var Damage = 0;
                var theoreticalMana = me.Mana;
                var resis = 1 - enemy.MagicDamageResist;

                if (hook != null)
                    if (hook.Level > 0 && theoreticalMana - hook.ManaCost >= 0 && hook.AbilityState == AbilityState.Ready)
                    {
                        Damage += (int)hook.AbilityData.FirstOrDefault(x => x.Name == "#AbilityDamage").GetValue(hook.Level - 1);
                        theoreticalMana -= hook.ManaCost;
                    }
                if (eblade != null)
                {
                    int dmg = (int)(((me.TotalStrength  + 75) * 2) * resis);
                    theoreticalMana -= eblade.ManaCost;
                    Damage += dmg;
                   // Print.Info(dmg.ToString());
                }
                if (dismember != null && rot != null)
                {

                    if (dismember.Level > 0 && theoreticalMana - dismember.ManaCost >= 0 && dismember.AbilityState == AbilityState.Ready)
                    {
                        if (rot.Level > 0)
                        {
                            var rotDPS = (int)rot.AbilityData.FirstOrDefault(x => x.Name == "#AbilityDamage").GetValue(rot.Level - 1);
                            var totalDmgAfterResis = (rotDPS * 3) * resis;
                            Damage += (int)totalDmgAfterResis;

                        }
                        var ultiDamage = dismember.AbilityData.FirstOrDefault(x => x.Name == "dismember_damage").GetValue(dismember.Level - 1);
                        if (me.AghanimState())
                        {
                            var scepterDamage = ultiDamage + (me.TotalStrength * dismember.AbilityData.FirstOrDefault(x => x.Name == "strength_damage_scepter").GetValue(dismember.Level - 1));
                            var totalDamageAfterResis = scepterDamage * resis;
                            ultiDamage = totalDamageAfterResis * 3;
                        }
                        Damage += (int)ultiDamage;
                        theoreticalMana -= dismember.ManaCost;
                    }



                    if (urn != null)
                        if (urn.CurrentCharges > 0)
                            Damage += 100;
                    if (dagon != null)
                        if (dagon != null && theoreticalMana - dagon.ManaCost >= 0 && dagon.AbilityState == AbilityState.Ready)
                        {
                            var dagonDamageAfterResis = (int)dagon.AbilityData.FirstOrDefault(x => x.Name == "damage").GetValue(dagon.Level - 1) * resis;
                            Damage += (int)dagonDamageAfterResis;
                            theoreticalMana -= dagon.ManaCost;
                        }
                }
                if (eblade != null)
                    return (int)(Damage * 1.4);
                else
                    return Damage;

            }
            catch (Exception ex)
            {
            //    Print.Error("DAMAGE: " + ex.Message);
                return 0;
            }
        }
        public static PredictClass getPrediction(Hero me, Hero enemy, string Method)
        {
            var Angle = enemy.FindAngleR();
            PredictClass pre = new PredictClass();
            pre.PredictedLocation = Vector2.Zero;
            pre.closest = false;
            switch (Method)
            {
                case "one":
                    Vector2 StraightDis = /*enemy.Position;*/Drawing.WorldToScreen(enemy.Position); //Facing position line
                    StraightDis.X += (float)Math.Cos(Angle) * 100;
                    StraightDis.Y += (float)Math.Sin(Angle) * 100;
                    //
                    Vector2 me2D = Drawing.WorldToScreen(Variables.me.Position);
                    Vector2 enemy2D = Drawing.WorldToScreen(enemy.Position);
                    Vector2 enemyDirection2D = StraightDis;
                    var alpha1 = Math.Atan((me2D.Y - enemy2D.Y) / (me2D.X - enemy2D.X));
                    var alpha2 = Math.Atan((enemyDirection2D.Y - enemy2D.Y) / (enemyDirection2D.X - enemy2D.X));
                    var alpha = Math.Abs(alpha2 - alpha1);
                    var beta = Math.Asin((enemy.MovementSpeed / Variables.HookSpeed) * Math.Sin(alpha));
                    var distanceBetweenMeandEnemy = Variables.me.Distance2D(enemy.Position);
                    var time = (distanceBetweenMeandEnemy) / ((enemy.MovementSpeed * Math.Cos(alpha)) + (Variables.HookSpeed * Math.Cos(beta)));

                    // Print.Info(time.ToString());
                    var predictedDistance = enemy.MovementSpeed * time;
                    Vector2 StraightDis2D = Drawing.WorldToScreen(enemy.Position); //Facing position line
                    StraightDis2D.X += (float)Math.Cos(Angle) * (float)predictedDistance + (float)(enemy.MovementSpeed * Variables.me.Spellbook.Spell1.GetCastDelay(Variables.me, enemy));
                    StraightDis2D.Y += (float)Math.Sin(Angle) * ((float)predictedDistance + (float)(enemy.MovementSpeed * Variables.me.Spellbook.Spell1.GetCastDelay(Variables.me, enemy)));
                   // ESP.Draw.Enemy.PredictionBox(StraightDis2D, Color.Black);
                    pre.PredictedLocation = StraightDis2D;
                    return pre;

                    break;
                case "two":

                    //Previous prediction method
                    var tBase = 0.1;
                    var tIncrement = 0.1;
                    var enemyMovementSpeed = enemy.MovementSpeed;
                    if (enemy.Name == "npc_dota_hero_spirit_breaker")
                        foreach (var mod in enemy.Modifiers)
                            if (mod.Name.Contains("charge"))
                            {
                                tBase = 0.05;
                                tIncrement = 0.05;
                                enemyMovementSpeed = 550 + ((int)enemy.Spellbook.Spell1.Level * 50);
                            }
                    for (double t = tBase; t <= 1; t += tIncrement)
                    {
                        var dis = enemyMovementSpeed * t;
                        Vector2 StraightDis1 = /*enemy.Position;*/Drawing.WorldToScreen(enemy.Position); //Facing position line
                        StraightDis1.X += (float)Math.Cos(Angle) * (float)dis;
                        StraightDis1.Y += (float)Math.Sin(Angle) * (float)dis;
                        //
                        Vector3 StraightDis3D1 = enemy.Position; //Facing position line
                        StraightDis3D1.X += (float)Math.Cos(enemy.RotationRad) * ((float)dis + 120);
                        StraightDis3D1.Y += (float)Math.Sin(enemy.RotationRad) * ((float)dis + 120);
                        if (Variables.DeveloperMode)
                             Drawing.DrawText("***", Drawing.WorldToScreen(StraightDis3D1), Color.Red, FontFlags.None);
                        var DistanceFromMeToPredict = Variables.me.Distance2D(StraightDis3D1);

                        if (Variables.me.Spellbook.Spell1.AbilityState == AbilityState.Ready)
                        {
                            var foobar1 = DistanceFromMeToPredict / Variables.HookSpeed;
                            if (foobar1 >= t * .8 && foobar1 <= t * 1.2)
                            {
                                //  Print.Info(Drawing.WorldToScreen(StraightDis3D1).ToString() + " | " + StraightDis1.ToString());
                                if (Variables.Settings.Auto_Hook_Value.val == 0)
                                {
                                    if (Utils.SleepCheck("hook") && (Variables.HookForMe))
                                    {
                                        Print.Info("casting hook");
                                        Variables.HookLocationDrawer = true;
                                        Variables.EnemyLocation = Drawing.WorldToScreen(enemy.Position);
                                        Variables.AutoHookLocation = Drawing.WorldToScreen(StraightDis3D1);
                                        Variables.PredictionLocation = StraightDis1;
                                        //Variables.me.Spellbook.Spell1.UseAbility(StraightDis3D1); //Hook based on prediction location (buggy)
                                        Variables.me.Spellbook.Spell1.CastSkillShot(enemy); //Ensage skill shot caster (temp solution untill fix)
                                        Utils.Sleep(1000, "hook");
                                        Variables.HookForMe = false;
                                    }
                                    if (ESP.Calculate.Enemy.ClosestToMouse(me).Player.Name == enemy.Player.Name)
                                        pre.closest = true;
                                }
                                pre.PredictedLocation = StraightDis1;
                                return pre;
                                break;
                            }

                        }
                    }
                    return pre;
                    break;
            }
            return pre;
        }
        public class PredictClass
        {
            public Vector2 PredictedLocation { get; set; }
            public bool closest { get; set; }
        }

    }
}
