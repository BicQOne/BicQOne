using Ensage;
using Ensage.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pudge_Plus.Classes
{
    public static class RuneHandler
    {
        public static CustomReturnRune GetRuneType(CustomRune rune)
        {
            CustomReturnRune cus = new CustomReturnRune();
            cus.customRune = rune;
            cus.color = Color.Green;
            cus.RuneType = "unknown";
            try
            {
                
                if (rune.current)
                    cus.RuneType = rune.rune.RuneType.ToString();
                else
                    return cus;
                switch (cus.RuneType)
                {
                    case "unknown": cus.color = Color.Green; break;
                    case "DoubleDamage": cus.RuneType = "Double Damage"; cus.color = Color.Cyan; break;
                    case "Invisibility": cus.color = Color.Purple; break;
                    case "Illusion": cus.color = Color.Yellow; break;
                    case "Haste": cus.color = Color.Red; break;
                    case "Bounty": cus.color = Color.Orange; break;
                    case "Regeneration": cus.color = Color.Lime; break;
                    default: cus.color = Color.Green; cus.RuneType = "UNHANDELED RUNE"; break;
                }
                return cus;
            }
            catch
            {
                cus.RuneType = "gone";
                return cus;
            }
        }
        public static void GetRunes()
        {
            bool isRunes = false;
            if (Variables.DeveloperMode)
                Print.Info("Checking for runes");
            foreach (Rune r in ObjectMgr.GetEntities<Rune>().Where(rune => rune.IsVisibleForTeam(Variables.me.Team)).ToList())
            {
                isRunes = true;
                if (Variables.DeveloperMode)
                    Print.Info("Rune found");
                switch (r.Position.X.ToString())
                {
                    case "2988": //Bot Rune
                        if (!Variables.BottomRune.current) //if rune should be updated
                        {
                            if (Variables.DeveloperMode)
                                Print.Info(r.RuneType.ToString());
                            Variables.BottomRune.rune = r;
                            Variables.BottomRune.current = true;
                        }
                        break;
                    case "-2271.531": //Top Rune
                        if (!Variables.TopRune.current) //if rune should be updated
                        {
                            if (Variables.DeveloperMode)
                                Print.Info(r.RuneType.ToString());
                            Variables.TopRune.rune = r;
                            Variables.TopRune.current = true;
                        }
                        break;
                }
                if (Variables.TopRune.current && Variables.BottomRune.current)
                {
                    Utils.Sleep(Variables.TimeTillNextRune * 1000, "runeCheck");
                    if (Variables.DeveloperMode)
                        Print.Info(string.Format("runeCheck sleeping for {0} seconds", Variables.TimeTillNextRune));
                }
                else
                    Utils.Sleep(250, "runeCheck");
            }
            if (!isRunes)
                Utils.Sleep(250, "runeCheck");
        }
        public static void ResetRunes()
        {
            if (Variables.TimeTillNextRune == 120) //Every Two Minutes
            {
                if (Variables.DeveloperMode)
                    Print.Encolored("Runes reset", ConsoleColor.Green);
                Variables.TopRune.current = false; //Declare runes as 'out dated'
                Variables.BottomRune.current = false;
                Utils.Sleep(115000, "runeResetAntiSpam");
            }
        }
    }
    public class CustomReturnRune
    {
        public CustomRune customRune { get; set; }
        public Color color { get; set; }
        public string RuneType { get; set; }
    }
    public class CustomRune
    {
        public Rune rune { get; set; }
        public bool current { get; set; }
    }
}
