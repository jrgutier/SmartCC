using HREngine.API;
using HREngine.API.Utilities;
using System;
using System.Collections.Generic;

namespace HREngine.Bots
{
    public class SmartCC : HREngine.Basics.BasicBot
    {
        private Simulation SmartCc = null;
        public SmartCC()
        {
            try
            {
                OnBattleStateUpdate = HandleOnBattleStateUpdate;
                OnMulliganStateUpdate = HandleBattleMulliganPhase;
            }

            catch (Exception Exception)
            {
                HRLog.Write(Exception.Message);
                HRLog.Write(Environment.StackTrace);
            }
        }

        private HREngine.API.Actions.ActionBase HandleBattleMulliganPhase()
        {
            SmartCc = new Simulation();
            string path = (HRSettings.Get.CustomRuleFilePath).Split(new string[] { "Common" }, StringSplitOptions.RemoveEmptyEntries)[0];
            CardTemplate.DatabasePath = path;
            CardTemplate.LoadAll();

            SmartCc.CreateLogFolder();
            SmartCc.TurnCount = 0;

            if (HRMulligan.IsMulliganActive())
            {
                List<HRCard> Choices = HRCard.GetCards(HRPlayer.GetLocalPlayer(), HRCardZone.HAND);

                foreach (HRCard card in Choices)
                {
                    if (card.GetEntity().GetCost() >= 4)
                    {
                        HRMulligan.ToggleCard(card);
                    }
                }
                return null;
            }
            return null;

        }

        private void SeedSmartCc()
        {


            SmartCc.SimuCount++;
            Board root = new Board();

            HREntity HeroEnnemy = HRPlayer.GetEnemyPlayer().GetHero();

            Card ce = Card.Create(HeroEnnemy.GetCardId(), false, HeroEnnemy.GetEntityId());
            root.HeroEnnemy = ce;

            HREntity HeroFriend = HRPlayer.GetLocalPlayer().GetHero();
            root.HeroFriend = Card.Create(HeroFriend.GetCardId(), true, HeroFriend.GetEntityId());

            root.HeroEnnemy.CurrentHealth = HeroEnnemy.GetHealth() - HeroEnnemy.GetDamage();
            root.HeroEnnemy.MaxHealth = 30;

            root.HeroEnnemy.CurrentArmor = HeroEnnemy.GetArmor();
            root.HeroFriend.CurrentHealth = HeroFriend.GetHealth() - HeroFriend.GetDamage();
            root.HeroFriend.MaxHealth = 30;

            root.HeroFriend.CurrentArmor = HeroFriend.GetArmor();
            root.HeroFriend.CurrentAtk = HeroFriend.GetATK();

            if(HRCard.GetCards(HRPlayer.GetEnemyPlayer(), HRCardZone.SECRET).Count > 0)
            {
                root.SecretEnnemy = true;
            }
            
            if (HRPlayer.GetEnemyPlayer().HasWeapon())
            {
                HRCard weaponEnnemyCard = HRPlayer.GetEnemyPlayer().GetWeaponCard();
                if (weaponEnnemyCard != null)
                {
                    root.WeaponEnnemy = Card.Create(weaponEnnemyCard.GetEntity().GetCardId(), false, weaponEnnemyCard.GetEntity().GetEntityId());
                }
            }

            if (HRPlayer.GetLocalPlayer().HasWeapon())
            {
                HRCard weaponFriendCard = HRPlayer.GetLocalPlayer().GetWeaponCard();
                if (weaponFriendCard != null)
                {
                    root.WeaponFriend = Card.Create(weaponFriendCard.GetEntity().GetCardId(), true, weaponFriendCard.GetEntity().GetEntityId());
                }
            }

            root.ManaAvailable = HRPlayer.GetLocalPlayer().GetNumAvailableResources();

            foreach (HREntity e in GetEnnemyEntitiesOnBoard())
            {
                Card newc = Card.Create(e.GetCardId(), false, e.GetEntityId());
                newc.CurrentAtk = e.GetATK();
                newc.CurrentHealth = e.GetHealth() - e.GetDamage();
                newc.MaxHealth = e.GetHealth();
                foreach (HREntity az in e.GetEnchantments())
                {
                    Buff b = Buff.GetBuffById(az.GetCardId());
                    if (b != null)
                    {
                        b.OwnerId = az.GetCreatorId();
                        newc.AddBuff(b);
                        newc.currentAtk -= b.Atk;
                        newc.MaxHealth -= b.Hp;
                    }
                }

                newc.CurrentCost = e.GetCost();
                newc.IsCharge = e.HasCharge();
                newc.IsDivineShield = e.HasDivineShield();
                newc.IsEnraged = e.IsEnraged();
                newc.IsFrozen = e.IsFrozen();
                newc.HasFreeze = e.IsFreeze();
                newc.IsStealth = e.IsStealthed();
                newc.IsSilenced = e.IsSilenced();
                newc.HasPoison = e.IsPoisonous();
                newc.IsWindfury = e.HasWindfury();
                newc.IsTaunt = e.HasTaunt();
                newc.Index = e.GetTag(HRGameTag.ZONE_POSITION) - 1;
                // HRLog.Write(e.GetName() + " at " + newc.Index.ToString());
                root.MinionEnnemy.Add(newc);
            }
            foreach (HREntity e in GetFriendEntitiesOnBoard())
            {
                Card newc = Card.Create(e.GetCardId(), true, e.GetEntityId());

                newc.CurrentAtk = e.GetATK();
                newc.CurrentHealth = e.GetHealth() - e.GetDamage();
                newc.MaxHealth = e.GetHealth();
                foreach (HREntity az in e.GetEnchantments())
                {
                    Buff b = Buff.GetBuffById(az.GetCardId());
                    if (b != null)
                    {
                        b.OwnerId = az.GetCreatorId();
                        newc.AddBuff(b);
                        newc.currentAtk -= b.Atk;
                        newc.MaxHealth -= b.Hp;
                    }
                }
                newc.CurrentCost = e.GetCost();
                newc.IsCharge = e.HasCharge();
                newc.IsDivineShield = e.HasDivineShield();
                newc.IsEnraged = e.IsEnraged();
                newc.IsFrozen = e.IsFrozen();
                newc.HasFreeze = e.IsFreeze();
                newc.IsStealth = e.IsStealthed();
                newc.IsSilenced = e.IsSilenced();
                newc.HasPoison = e.IsPoisonous();
                newc.IsWindfury = e.HasWindfury();
                newc.IsTaunt = e.HasTaunt();
                newc.IsTired = e.IsExhausted();
                newc.IsImmune = e.IsImmune();
                newc.Index = e.GetTag(HRGameTag.ZONE_POSITION) - 1;
                newc.CountAttack = e.GetTag(HRGameTag.NUM_ATTACKS_THIS_TURN);

                root.MinionFriend.Add(newc);
            }

            foreach (HRCard c in GetAllCardsInHand())
            {
                Card newc = Card.Create(c.GetEntity().GetCardId(), true, c.GetEntity().GetEntityId());
                root.Hand.Add(newc);
            }
            if (!HRPlayer.GetLocalPlayer().GetHeroPower().IsExhausted())
            {
                root.Ability = Card.Create(HRPlayer.GetLocalPlayer().GetHeroPower().GetCardId(), true, HRPlayer.GetLocalPlayer().GetHeroPower().GetEntityId());
            }

            SmartCc.root = root;
        }

        private HREngine.API.Actions.ActionBase HandleOnBattleStateUpdate()
        {
            if(SmartCc == null)
            {
                SmartCc = new Simulation();
                string path = (HRSettings.Get.CustomRuleFilePath).Split(new string[] { "Common" }, StringSplitOptions.RemoveEmptyEntries)[0];
                CardTemplate.DatabasePath = path;
                CardTemplate.LoadAll();

                SmartCc.CreateLogFolder();
                SmartCc.TurnCount = 0;
            }
            while (true)
            {
                if (SmartCc.NeedCalculation)
                {

                    HRLog.Write("Seed");
                    SeedSmartCc();
                    HRLog.Write("Simulation");

                    SmartCc.Simulate();
                    HRLog.Write("Simulation Done");

                }

                if (SmartCc.ActionStack.Count <= 0)
                {
                    HRLog.Write("Simulation didnt found any action");
                }
                else
                {
                    HRLog.Write("Actions :");

                    foreach (Action a in SmartCc.ActionStack)
                    {
                        HRLog.Write(a.ToString());

                    }
                }


                Action ActionToDo = SmartCc.GetNextAction();
                try
                {
                    switch (ActionToDo.Type)
                    {
                        case Action.ActionType.CAST_ABILITY:
                            HRCard cardAbility = HRPlayer.GetLocalPlayer().GetHeroPower().GetCard();
                            if (ActionToDo.Target != null)
                            {
                                HREntity target = GetEntityById(ActionToDo.Target.Id);
                                SmartCc.ActionStack.Remove(ActionToDo);
                                return new HREngine.API.Actions.PlayCardAction(cardAbility, target, ActionToDo.Index + 1);
                            }
                            else
                            {
                                SmartCc.ActionStack.Remove(ActionToDo);
                                return new HREngine.API.Actions.PlayCardAction(cardAbility, null, ActionToDo.Index + 1);
                            }
                        case Action.ActionType.CAST_WEAPON:
                        case Action.ActionType.CAST_MINION:
                        case Action.ActionType.CAST_SPELL:

                            HRCard card = GetCardById(ActionToDo.Actor.Id);
                            if (ActionToDo.Target != null)
                            {
                                HREntity target = GetEntityById(ActionToDo.Target.Id);
                                SmartCc.ActionStack.Remove(ActionToDo);
                                return new HREngine.API.Actions.PlayCardAction(card, target, ActionToDo.Index + 1);
                            }
                            else
                            {
                                SmartCc.ActionStack.Remove(ActionToDo);
                                return new HREngine.API.Actions.PlayCardAction(card, null, ActionToDo.Index + 1);
                            }

                        case Action.ActionType.HERO_ATTACK:

                            HREntity attackerAttack = GetEntityById(ActionToDo.Actor.Id);
                            HREntity targetAttack = GetEntityById(ActionToDo.Target.Id);
                            if (HRPlayer.GetLocalPlayer().HasWeapon())
                            {
                                return new HREngine.API.Actions.AttackAction(HRPlayer.GetLocalPlayer().GetWeaponCard().GetEntity(), targetAttack);
                            }
                            return new HREngine.API.Actions.AttackAction(HRPlayer.GetLocalPlayer().GetHero(), targetAttack);

                        case Action.ActionType.MINION_ATTACK:

                            HREntity attackerAttackM = GetEntityById(ActionToDo.Actor.Id);
                            HREntity targetAttackM = GetEntityById(ActionToDo.Target.Id);
                            return new HREngine.API.Actions.AttackAction(attackerAttackM, targetAttackM);

                        case Action.ActionType.END_TURN:
                            HRLog.Write("EndTurn");
                            SmartCc.TurnCount++;
                            SmartCc.SimuCount = 0;
                            HRBattle.FinishRound();
                            return null;

                        case Action.ActionType.RESIMULATE:
                            HRLog.Write("Resimulate");
                            SmartCc.NeedCalculation = true;
                            continue;

                    }
                }
                catch (Exception Exception)
                {
                    HRLog.Write(Exception.Message);
                    HRLog.Write(Environment.StackTrace);
                    return null;

                }

                HRLog.Write("Action not handled");
                return null;

            }

            SmartCc.NeedCalculation = true;
            return null;
        }

        private HREntity GetEntityById(int id)
        {
            foreach (HREntity e in GetAllEntitiesOnBoard())
            {
                if (id == e.GetEntityId()) return e;
            }
            return null;
        }

        private HRCard GetCardById(int id)
        {
            foreach (HRCard e in GetAllCardsInHand())
            {
                if (id == e.GetEntity().GetEntityId()) return e;
            }
            return null;
        }

        private List<HRCard> GetAllCardsInHand()
        {
            return HRCard.GetCards(HRPlayer.GetLocalPlayer(), HRCardZone.HAND);
        }

        private List<HREntity> GetAllEntitiesOnBoard()
        {
            List<HREntity> ret = new List<HREntity>();

            HREntity HeroFriend = HRPlayer.GetLocalPlayer().GetHero();
            ret.Add(HeroFriend);

            HREntity HeroEnnemy = HRPlayer.GetEnemyPlayer().GetHero();
            ret.Add(HeroEnnemy);

            HREntity HeroAbility = HRPlayer.GetLocalPlayer().GetHeroPower();
            ret.Add(HeroAbility);

            foreach (HRCard c in HRCard.GetCards(HRPlayer.GetLocalPlayer(), HRCardZone.PLAY))
            {
                ret.Add(c.GetEntity());
            }
            foreach (HRCard c in HRCard.GetCards(HRPlayer.GetEnemyPlayer(), HRCardZone.PLAY))
            {
                ret.Add(c.GetEntity());
            }
            return ret;
        }
        private List<HREntity> GetEnnemyEntitiesOnBoard()
        {
            List<HREntity> ret = new List<HREntity>();
            foreach (HRCard c in HRCard.GetCards(HRPlayer.GetEnemyPlayer(), HRCardZone.PLAY))
            {
                ret.Add(c.GetEntity());
            }

            return ret;
        }
        private List<HREntity> GetFriendEntitiesOnBoard()
        {
            List<HREntity> ret = new List<HREntity>();
            foreach (HRCard c in HRCard.GetCards(HRPlayer.GetLocalPlayer(), HRCardZone.PLAY))
            {
                ret.Add(c.GetEntity());
            }

            return ret;
        }
    }
}