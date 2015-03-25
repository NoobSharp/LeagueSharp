using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Sparta_SpaceBar
{
    class Program
    {
        public const string ChampionName = "Pantheon";

        //Orbwalker
        public static Orbwalking.Orbwalker Orbwalker;

        //Spells
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static bool PacketCast;

        public static List<Obj_SpellMissile> SkillShots = new List<Obj_SpellMissile>();

        public static SpellSlot IgniteSlot;

        public static Spell Spear, Aegis, Heartseeker, Skyfall;


        //Menu
        public static Menu Config;

        private static Obj_AI_Hero Player;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.BaseSkinName != ChampionName) return;
            Game.PrintChat("Sparta Spacebar Loaded XD");

            //Create Spells
            Q = new Spell(SpellSlot.Q, 620f);
            W = new Spell(SpellSlot.W, 620f);
            E = new Spell(SpellSlot.E, 640f);
            R = new Spell(SpellSlot.R, 2000f);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

          



            //Menu
            Config = new Menu(ChampionName, ChampionName, true);
            
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Orbwalker submenu
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            //Add orbwalker
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Info", "Info"));
            Config.SubMenu("Info").AddItem(new MenuItem("Author", "Created by Noobies"));
           

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushQCombo", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushWCombo", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushECombo", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("RushQHarass", "Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("HarassActive", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Farm", "Farm"));
            Config.SubMenu("Farm").AddItem(new MenuItem("RushQFarm", "Use Q").SetValue(true));
            Config.SubMenu("Farm").AddItem(new MenuItem("FreezeActive", "Freeze!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Farm").AddItem(new MenuItem("LaneClearActive", "LaneClear!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));


            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushQRange", "Q Range").SetValue(new Circle(false, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushERange", "E Range").SetValue(new Circle(false, System.Drawing.Color.FromArgb(255, 255, 255, 255))));     
                       
                           
          
                  
                     
                          


            Config.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            Orbwalker.SetAttack(true);
            if (Config.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else if (Config.Item("HarassActive").GetValue<KeyBind>().Active)
            {
                Harass();
            }
            else if (Config.Item("LaneClearActive").GetValue<KeyBind>().Active)
            {
                Farm();
            }
          
           
        }

        private static void Combo()
        {
            var targetQ = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            var targetW = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            var targetE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (Config.Item("RushWCombo").GetValue<bool>())
            {
            if (Geometry.Distance(Player, targetW) <= W.Range && W.IsReady())
            {
                W.Cast(targetW, true);
                
            }
            }


             if (Config.Item("RushECombo").GetValue<bool>())
            {
            if (Geometry.Distance(Player, targetE) <= E.Range && E.IsReady())
            {
                E.Cast(targetE, true);
                
            }
            }

             if (Config.Item("RushQCombo").GetValue<bool>())
            {
            if (Geometry.Distance(Player, targetQ) <= Q.Range && Q.IsReady())
            {
                Q.Cast(targetQ, true);
            }
            }
        }
        

        
        public  static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            
            if (Config.Item("RushQHarass").GetValue<bool>())
            {
                
                if (Geometry.Distance(Player, target) <= Q.Range && Q.IsReady())
                {
                    Q.Cast(target, true);
                }
                
            }
            

        }

        public static  void Farm()
        {
            if (Orbwalker.ActiveMode.ToString() == "LaneClear")
            {
                if (Config.Item("RushQFarm").GetValue<bool>())
                {

                    var minyon = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
                    foreach (Obj_AI_Base minions in minyon.Where(minions => minions.Health < ObjectManager.Player.GetSpellDamage(minions, SpellSlot.Q)))
  
                        Q.Cast(minions);
                }
            }
        }

      

        private static void Drawing_OnDraw(EventArgs args)
        {
            //Draw spells
            var menuItem = Config.Item("RushQRange").GetValue<Circle>();
            if (menuItem.Active) Utility.DrawCircle(Player.Position, Q.Range, menuItem.Color);

            var menuItem2 = Config.Item("RushERange").GetValue<Circle>();
            if (menuItem2.Active) Utility.DrawCircle(Player.Position, E.Range, menuItem2.Color);

           


        }
    }
}
