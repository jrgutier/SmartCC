using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    [Serializable]
    public class Board : IEquatable<Board>
    {
        public List<Action> ActionsStack { get; set; }
        public List<Card> Hand { get; set; }
        public List<Card> MinionFriend { get; set; }
        public List<Card> MinionEnemy { get; set; }
        public Card WeaponEnemy { get; set; }
        public Card WeaponFriend { get; set; }
        public Card HeroEnemy { get; set; }
        public Card HeroFriend { get; set; }
        public List<Card> Secret { get; set; }
        public bool SecretEnemy { get; set; }
        public int ManaAvailable { get; set; }
        public Card Ability { get; set; }
        public Card EnemyAbility { get; set; }

        public int EnemyHealFactor { get; set; }
        public int HealFactor { get; set; }
        public int FriendCardDraw { get; set; }
        public int EnemyCardDraw { get; set; }
        public int WastedATK { get; set; }
        public int TurnCount { get; set; }

        public Board EnemyTurnWorseBoard { get; set; }
        public bool EnemyTurnCalculated = false;

        static int lastIdGen = 10000;


        private int Value = 0;
        private int FriendValue = 0;
        private int EnemyValue = 0;

        public int GetValue()
        {
            int value = 0;

            foreach (Card c in MinionEnemy)
            {
                value -= c.GetValue(this) + (int)c.Behavior.GetKillPriority(this);
            }

            value += Secret.Count * ValuesInterface.ValueSecret;

            EnemyValue = ((HeroEnemy.CurrentHealth * ValuesInterface.ValueHealthEnemy) + HeroEnemy.CurrentArmor * ValuesInterface.ValueArmorEnemy);
            value -= EnemyValue;

            foreach (Card c in MinionFriend)
            {
                value += c.GetValue(this);
            }

            FriendValue = ((HeroFriend.CurrentHealth * ValuesInterface.ValueHealthFriend) + HeroFriend.CurrentArmor * ValuesInterface.ValueArmorFriend);
            value += FriendValue;

            value += FriendCardDraw * ValuesInterface.ValueFriendCardDraw;
            value -= EnemyCardDraw * ValuesInterface.ValueEnemyCardDraw;

            value -= MinionEnemy.Count * ValuesInterface.ValueEnemyMinionCount;
            value += MinionFriend.Count * ValuesInterface.ValueFriendMinionCount;


            if (WeaponFriend != null)
            {
                value += WeaponFriend.GetValue(this);
            }


            if (HeroEnemy.CurrentHealth < 1)
                value += 100000;

            if (HeroFriend.CurrentHealth < 1)
                value -= 100000;

            foreach(Card c in Hand)
            {
                value += c.Behavior.GetHandValue(this);
            }


            Value = value;
            return value;
        }

        public Board CalculateEnemyTurnValue()
        {
            List<Action> enemyActions = CalculateEnemyAvailableActions();

            List<Board> childss = new List<Board>();
            childss.Add(Board.Clone(this));
            Board worseBoard = null;

            int maxWide = 50;

            int wide = 0;
            while (childss.Count != 0)
            {
                wide = 0;
                List<Board> childs = new List<Board>();

                foreach (Board b in childss)
                {
                    List<Action> actions = b.CalculateEnemyAvailableActions();

                    foreach (Action a in actions)
                    {
                        if (wide >= maxWide)
                            break;
                        wide++;

                        Board bb = b.ExecuteAction(a);
                        childs.Add(bb);
                    }
                    if (wide >= maxWide)
                        break;
                }

                foreach (Board baa in childs)
                {
                    if (worseBoard == null)
                        worseBoard = baa;
                    if (worseBoard.GetValue() > baa.GetValue())
                    {
                        worseBoard = baa;
                    }
                }
                childss = childs;
            }


            return worseBoard;
        }


        public List<Action> CalculateEnemyAvailableActions()
        {
            List<Action> enemyActions = new List<Action>();

            if (EnemyAbility != null)
            {
                if (EnemyAbility.TargetTypeOnPlay == Card.TargetType.MINION_ENEMY || EnemyAbility.TargetTypeOnPlay == Card.TargetType.MINION_BOTH
                           || EnemyAbility.TargetTypeOnPlay == Card.TargetType.BOTH_ENEMY || EnemyAbility.TargetTypeOnPlay == Card.TargetType.ALL)
                {
                    foreach (Card Friend in MinionFriend)
                    {
                        if (!Friend.IsTargetable || Friend.IsStealth)
                            continue;
                        Action a = null;
                        a = new Action(Action.ActionType.CAST_ABILITY, EnemyAbility, Friend);
                        enemyActions.Add(a);
                    }
                }
                if (EnemyAbility.TargetTypeOnPlay == Card.TargetType.MINION_FRIEND || EnemyAbility.TargetTypeOnPlay == Card.TargetType.MINION_BOTH
                        || EnemyAbility.TargetTypeOnPlay == Card.TargetType.BOTH_FRIEND || EnemyAbility.TargetTypeOnPlay == Card.TargetType.ALL)
                {
                    foreach (Card Enemy in MinionEnemy)
                    {
                        if (!Enemy.IsTargetable || Enemy.IsStealth)
                            continue;
                        Action a = null;
                        a = new Action(Action.ActionType.CAST_ABILITY, EnemyAbility, Enemy);
                        enemyActions.Add(a);
                    }
                }
                if (EnemyAbility.TargetTypeOnPlay == Card.TargetType.HERO_ENEMY || EnemyAbility.TargetTypeOnPlay == Card.TargetType.HERO_BOTH
                        || EnemyAbility.TargetTypeOnPlay == Card.TargetType.BOTH_ENEMY || EnemyAbility.TargetTypeOnPlay == Card.TargetType.ALL)
                {
                    Action a = null;
                    a = new Action(Action.ActionType.CAST_ABILITY, EnemyAbility, HeroFriend);
                    enemyActions.Add(a);
                }
                if (EnemyAbility.TargetTypeOnPlay == Card.TargetType.HERO_FRIEND || EnemyAbility.TargetTypeOnPlay == Card.TargetType.HERO_BOTH
                        || EnemyAbility.TargetTypeOnPlay == Card.TargetType.BOTH_FRIEND || EnemyAbility.TargetTypeOnPlay == Card.TargetType.ALL)
                {
                    Action a = null;
                    a = new Action(Action.ActionType.CAST_ABILITY, EnemyAbility, HeroEnemy);
                    enemyActions.Add(a);
                }

                if (EnemyAbility.TargetTypeOnPlay == Card.TargetType.NONE)
                {
                    Action a = null;
                    a = new Action(Action.ActionType.CAST_ABILITY, EnemyAbility);
                    enemyActions.Add(a);
                }

            }

            List<Card> taunts = new List<Card>();

            foreach (Card Friend in MinionFriend)
            {
                if (Friend.IsTaunt && !Friend.IsStealth)
                    taunts.Add(Friend);
            }

            List<Card> attackers = new List<Card>();

            foreach (Card minion in MinionEnemy)
            {
                if (!minion.CanAttack)
                    continue;
                if (taunts.Count == 0)
                {
                    foreach (Card Friend in MinionFriend)
                    {
                        if (Friend.IsStealth)
                            continue;

                        Action a = new Action(Action.ActionType.MINION_ATTACK, minion, Friend);
                        enemyActions.Add(a);
                    }
                    Action ac = new Action(Action.ActionType.MINION_ATTACK, minion, HeroFriend);
                    enemyActions.Add(ac);
                }
                else
                {
                    foreach (Card taunt in taunts)
                    {
                        Action a = new Action(Action.ActionType.MINION_ATTACK, minion, taunt);
                        enemyActions.Add(a);
                    }
                }

            }

            if (WeaponEnemy != null)
            {
                if (WeaponEnemy.CurrentDurability > 0 && WeaponEnemy.CanAttack)
                {
                    if (taunts.Count == 0)
                    {
                        foreach (Card Friend in MinionFriend)
                        {
                            if (Friend.IsStealth)
                                continue;
                            Action a = new Action(Action.ActionType.HERO_ATTACK, WeaponEnemy, Friend);
                            enemyActions.Add(a);
                        }
                        Action ac = new Action(Action.ActionType.HERO_ATTACK, WeaponEnemy, HeroEnemy);
                        enemyActions.Add(ac);
                    }
                    else
                    {
                        foreach (Card taunt in taunts)
                        {
                            Action a = new Action(Action.ActionType.HERO_ATTACK, WeaponEnemy, taunt);
                            enemyActions.Add(a);
                        }
                    }
                }
            }

            return enemyActions;

        }

        public int GetSpellPower()
        {
            int ret = 0;
            foreach (Card c in MinionFriend)
            {
                ret += c.SpellPower;
            }
            return ret;
        }
        public Board()
        {
            Hand = new List<Card>();
            MinionFriend = new List<Card>();
            MinionEnemy = new List<Card>();
            WeaponEnemy = null;
            WeaponFriend = null;
            HeroEnemy = null;
            HeroFriend = null;
            Ability = null;
            EnemyAbility = null;
            Secret = new List<Card>();
            SecretEnemy = false;
            ManaAvailable = 0;
            ActionsStack = new List<Action>();
            HealFactor = 1;
            EnemyHealFactor = 1;
            TurnCount = 1;
        }

        public bool PlayCardFromHand(int id)
        {
            List<Card> tmp = new List<Card>();
            foreach (Card c in Hand)
            {
                tmp.Add(c);
            }
            for (int i = 0; i < tmp.Count; i++)
            {
                if (tmp[i].Id == id)
                {
                    if (SecretEnemy)
                    {
                        Resimulate();
                    }
                    //int idx = Hand.IndexOf(c);
                    Hand.RemoveAt(i);
                    if (tmp[i].Type != Card.CType.WEAPON)
                        FriendCardDraw--;
                    ManaAvailable -= tmp[i].CurrentCost;
                    if (tmp[i].Type == Card.CType.WEAPON)
                    {
                        WeaponFriend = tmp[i];
                    }

                    return true;
                }
            }

            return false;
        }

        public bool RemoveCardFromHand(int id)
        {
            List<Card> tmp = new List<Card>();
            foreach (Card c in Hand)
            {
                tmp.Add(c);
            }
            for (int i = 0; i < tmp.Count; i++)
            {
                if (tmp[i].Id == id)
                {
                    Hand.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public bool PlayMinion(int id)
        {
            List<Card> tmp = new List<Card>();
            foreach (Card c in Hand)
            {
                tmp.Add(c);
            }
            for (int i = 0; i < tmp.Count; i++)
            {
                if (tmp[i].Id == id)
                {
                    if (SecretEnemy)
                    {
                        Resimulate();
                    }
                    Hand.RemoveAt(i);
                    ManaAvailable -= tmp[i].CurrentCost;
                    if (tmp[i].Type == Card.CType.MINION)
                    {
                        if (tmp[i].IsCharge)
                        {
                            tmp[i].IsTired = false;
                        }
                        else
                        {
                            tmp[i].IsTired = true;
                        }

                        foreach (Card ca in MinionFriend)
                        {
                            if (ca.Index >= tmp[i].Index)
                                ca.Index++;
                        }

                        MinionFriend.Add(tmp[i]);

                    }

                    return true;
                }
            }

            return false;
        }

        public Card SpawnMinion(string id, int idx, int cost = 0)
        {
            Card c = Card.Create(id, true, GenId(), idx);
            ManaAvailable -= cost;

            foreach (Card ca in MinionFriend)
            {
                if (ca.Index >= idx)
                    ca.Index++;
            }
            c.IsTired = true;
            MinionFriend.Add(c);
            Resimulate();

            return c;
        }

        public void DoRandomDamage(int damage, bool FriendSide)
        {
            List<Card> KillableMinions = new List<Card>();
            bool IsHeroKillable = false;

            if (FriendSide)
            {
                foreach (Card c in MinionFriend)
                {
                    if (c.CurrentHealth <= damage)
                        KillableMinions.Add(c);
                }
                if (HeroFriend.CurrentHealth + HeroFriend.CurrentArmor <= damage)
                {
                    IsHeroKillable = true;
                }

                if (IsHeroKillable)
                {
                    HeroFriend.CurrentHealth = 0;
                    HeroFriend.CurrentArmor = 0;
                }
                else
                {
                    if (KillableMinions.Count > 0)
                    {
                        Card bestMinion = null;
                        foreach (Card c in MinionFriend)
                        {
                            if (bestMinion == null)
                            {
                                bestMinion = c;
                                continue;
                            }

                            if (bestMinion.GetValue(this) > c.GetValue(this))
                                bestMinion = c;
                        }
                        if (bestMinion != null)
                        {
                            RemoveCardFromBoard(bestMinion.Id);
                        }
                    }
                    else
                    {
                        if (MinionFriend.Count > 0)
                        {
                            Card bestMinion = null;
                            foreach (Card c in MinionFriend)
                            {
                                if (bestMinion == null)
                                {
                                    bestMinion = c;
                                    continue;
                                }

                                if (bestMinion.GetValue(this) < c.GetValue(this))
                                    bestMinion = c;
                            }
                            if (bestMinion != null)
                            {
                                bestMinion.CurrentHealth -= damage;
                            }
                        }
                        else
                        {
                            if (HeroFriend.CurrentArmor < 1)
                            {
                                HeroFriend.CurrentHealth -= damage;
                            }
                            else
                            {
                                int tmp = damage - HeroFriend.CurrentArmor;
                                HeroFriend.CurrentArmor -= (damage - tmp);
                                HeroFriend.CurrentHealth -= tmp;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Card c in MinionEnemy)
                {
                    if (c.CurrentHealth <= damage)
                        KillableMinions.Add(c);
                }
                if (HeroEnemy.CurrentHealth + HeroEnemy.CurrentArmor <= damage)
                {
                    IsHeroKillable = true;
                }

                if (!IsHeroKillable)
                {
                    if (MinionEnemy.Count - KillableMinions.Count > 0)
                    {
                        Card worstMinion = null;
                        foreach (Card c in MinionEnemy)
                        {
                            if (KillableMinions.Contains(c))
                                continue;
                            if (worstMinion == null)
                            {
                                worstMinion = c;
                                continue;
                            }

                            if (worstMinion.GetValue(this) > c.GetValue(this))
                                worstMinion = c;
                        }
                        if (worstMinion != null)
                        {
                            worstMinion.CurrentHealth -= damage;
                        }
                    }
                    else
                    {
                        if (HeroEnemy.CurrentArmor < 1)
                        {
                            HeroEnemy.CurrentHealth -= damage;
                        }
                        else
                        {
                            int tmp = damage - HeroEnemy.CurrentArmor;
                            HeroFriend.CurrentArmor -= (damage - tmp);
                            HeroFriend.CurrentHealth -= tmp;
                        }
                    }

                }
                else
                {
                    if (MinionEnemy.Count - KillableMinions.Count > 0)
                    {
                        Card worstMinion = null;
                        foreach (Card c in MinionEnemy)
                        {
                            if (KillableMinions.Contains(c))
                                continue;
                            if (worstMinion == null)
                            {
                                worstMinion = c;
                                continue;
                            }

                            if (worstMinion.GetValue(this) > c.GetValue(this))
                                worstMinion = c;
                        }
                        if (worstMinion != null)
                        {
                            worstMinion.CurrentHealth -= damage;
                        }
                    }
                    else if (KillableMinions.Count > 0)
                    {

                        Card bestMinion = null;
                        foreach (Card c in KillableMinions)
                        {
                            if (bestMinion == null)
                            {
                                bestMinion = c;
                                continue;
                            }

                            if (bestMinion.GetValue(this) < c.GetValue(this))
                                bestMinion = c;
                        }
                        if (bestMinion != null)
                        {
                            RemoveCardFromBoard(bestMinion.Id);
                        }
                    }
                    else
                    {
                        if (HeroEnemy.CurrentArmor < 1)
                        {
                            HeroEnemy.CurrentHealth -= damage;
                        }
                        else
                        {
                            int tmp = damage - HeroEnemy.CurrentArmor;
                            HeroFriend.CurrentArmor -= (damage - tmp);
                            HeroFriend.CurrentHealth -= tmp;
                        }
                    }
                }
            }
        }

        public void PlayAbility()
        {
            ManaAvailable -= Ability.CurrentCost;
            Ability = null;
            if (SecretEnemy)
            {
                Resimulate();
            }
        }

        public void PlayEnemyAbility()
        {
            EnemyAbility = null;
        }

        public void Resimulate()
        {
            if (ActionsStack.Count > 0)
            {
                if (ActionsStack[ActionsStack.Count - 1].Type != Action.ActionType.RESIMULATE)
                    ActionsStack.Add(new Action(Action.ActionType.RESIMULATE, null));
            }
            else
            {

            }

        }

        public static int GenId()
        {
            return lastIdGen++;
        }

        public void AddCardToBoard(string id, bool friend)
        {

            if (!friend)
            {
                Card c = Card.Create(id, false, GenId());
                if (!c.IsCharge)
                {
                    c.IsTired = true;
                }

                MinionEnemy.Add(c);
            }
            else
            {
                Card c = Card.Create(id, true, GenId());
                if (!c.IsCharge)
                {
                    c.IsTired = true;
                }

                MinionFriend.Add(c);
            }
            Resimulate();
        }

        public bool HasFriendBuffer()
        {
            foreach (Card c in MinionFriend)
            {
                if (c.TestAllIndexOnPlay)
                {
                    return true;
                }
            }

            return false;
        }

        public void ReplaceWeapon(string id)
        {
            Random random = new Random();
            int randomNumber = random.Next(88888, 99999);

            WeaponFriend = Card.Create(id, true, randomNumber);
            Resimulate();
        }

        public void ReplaceEnemyWeapon(string id)
        {
            Random random = new Random();
            int randomNumber = random.Next(88888, 99999);

            WeaponEnemy = Card.Create(id, false, randomNumber);
        }

        public Card GetMinionByIndex(int idx, bool friend)
        {
            if (friend)
            {
                foreach (Card c in MinionFriend)
                {
                    if (c.Index == idx)
                    {
                        return c;
                    }
                }
            }
            else
            {
                foreach (Card c in MinionEnemy)
                {
                    if (c.Index == idx)
                    {
                        return c;

                    }
                }
            }
            return null;
        }

        public bool RemoveCardFromBoard(int id)
        {
            List<Card> tmp = new List<Card>();
            foreach (Card c in MinionFriend)
            {
                tmp.Add(c);
            }
            for (int i = 0; i < tmp.Count; i++)
            {
                if (tmp[i].Id == id)
                {
                    foreach (Card ca in MinionFriend)
                    {
                        if (ca == tmp[i])
                            continue;

                        if (tmp[i].Index < ca.Index)
                            ca.Index--;
                    }

                    MinionFriend.RemoveAt(i);
                    return true;
                }
            }

            tmp.Clear();
            foreach (Card c in MinionEnemy)
            {
                tmp.Add(c);
            }
            for (int i = 0; i < tmp.Count; i++)
            {
                if (tmp[i].Id == id)
                {
                    foreach (Card ca in MinionEnemy)
                    {
                        if (ca == tmp[i])
                            continue;

                        if (tmp[i].Index < ca.Index)
                            ca.Index--;
                    }
                    MinionEnemy.RemoveAt(i);
                    return true;
                }
            }

            tmp.Clear();
            foreach (Card c in Secret)
            {
                tmp.Add(c);
            }
            for (int i = 0; i < tmp.Count; i++)
            {

                if (tmp[i].Id == id)
                {
                    Secret.RemoveAt(i);
                    return true;
                }
            }
            if (WeaponEnemy != null)
            {
                if (WeaponEnemy.Id == id)
                {
                    WeaponEnemy = null;
                    return true;
                }
            }
            if (WeaponFriend != null)
            {
                if (WeaponFriend.Id == id)
                {
                    WeaponFriend = null;
                    return true;
                }
            }
            return false;
        }
        public bool IsCombo()
        {
            return (ActionsStack.Count > 1);
        }


        public Board ExecuteAction(Action a)
        {
            Board child = Board.Clone(this);
            child.ActionsStack.Add(a);
            child.Update();

            switch (a.Type)
            {
                case Action.ActionType.CAST_WEAPON:
                    if (a.Target != null)
                        a.Actor.OnPlay(ref child, child.GetCard(a.Target.Id));
                    else
                        a.Actor.OnPlay(ref child, null);
                    break;

                case Action.ActionType.CAST_MINION:
                    if (a.Target != null)
                    {
                        a.Actor.OnPlay(ref child, child.GetCard(a.Target.Id), a.Index, a.Choice);
                    }
                    else
                    {
                        a.Actor.OnPlay(ref child, null, a.Index, a.Choice);
                    }
                    child.Update();
                    foreach (Card c in child.GetAllMinionsOnBoard())
                    {
                        if (a.Actor.Id == c.Id)
                            continue;
                        c.OnPlayOtherMinion(ref child, child.GetCard(a.Actor.Id));
                    }
                    if (child.WeaponFriend != null)
                    {
                        child.WeaponFriend.OnPlayOtherMinion(ref child, child.GetCard(a.Actor.Id));
                    }
                    break;

                case Action.ActionType.CAST_SPELL:
                    if (a.Target != null)
                        a.Actor.OnPlay(ref child, child.GetCard(a.Target.Id), 0, a.Choice);
                    else
                        a.Actor.OnPlay(ref child, null, 0, a.Choice);

                    child.Update();

                    foreach (Card c in child.GetAllMinionsOnBoard())
                    {
                        c.OnCastSpell(ref child, child.GetCard(a.Actor.Id));
                    }
                    break;

                case Action.ActionType.HERO_ATTACK:
                    if (a.Target != null)
                        a.Actor.OnAttack(ref child, child.GetCard(a.Target.Id));
                    else
                        a.Actor.OnAttack(ref child, null);

                    if (SecretEnemy)
                    {
                        child.Resimulate();
                    }
                    break;

                case Action.ActionType.MINION_ATTACK:
                    if (a.Target != null)
                        a.Actor.OnAttack(ref child, child.GetCard(a.Target.Id));
                    else
                        a.Actor.OnAttack(ref child, null);
                    if (SecretEnemy)
                    {
                        child.Resimulate();
                    }
                    break;

                case Action.ActionType.CAST_ABILITY:
                    if (a.Target != null)
                        a.Actor.OnPlay(ref child, child.GetCard(a.Target.Id));
                    else
                        a.Actor.OnPlay(ref child, null);
                    break;
            }

            child.Update();
            return child;
        }

        public List<Action> CalculateAvailableActions()
        {
            List<Action> availableActions = new List<Action>();
            bool HasLethal = false;

            List<Card> taunts = new List<Card>();

            foreach (Card Enemy in MinionEnemy)
            {
                if (Enemy.IsTaunt && !Enemy.IsStealth)
                    taunts.Add(Enemy);
            }


            if (taunts.Count == 0)
            {
                int totalDamage = 0;
                foreach (Card c in MinionFriend)
                {
                    totalDamage += c.CurrentAtk;
                }
                if (totalDamage >= HeroEnemy.CurrentHealth + HeroEnemy.CurrentArmor)
                    HasLethal = true;
            }


            List<Card> attackers = new List<Card>();
            foreach (Card minion in MinionFriend)
            {
                if (!minion.CanAttack || !minion.Behavior.ShouldAttack(this))
                    continue;

                bool containsSimilarMinion = false;
                foreach (Card cc in attackers)
                {
                    if (cc.IsSimilar(minion))
                        containsSimilarMinion = true;
                }
                if (containsSimilarMinion)
                    continue;

                attackers.Add(minion);

                if (taunts.Count == 0)
                {
                    foreach (Card Enemy in MinionEnemy)
                    {
                        if (Enemy.IsStealth)
                            continue;
                        if (HasLethal)
                            continue;
                        Action a = new Action(Action.ActionType.MINION_ATTACK, minion, Enemy);
                        availableActions.Add(a);
                    }
                    Action ac = new Action(Action.ActionType.MINION_ATTACK, minion, HeroEnemy);
                    availableActions.Add(ac);
                }
                else
                {
                    foreach (Card taunt in taunts)
                    {
                        Action a = new Action(Action.ActionType.MINION_ATTACK, minion, taunt);
                        availableActions.Add(a);
                    }
                }

            }

            if (WeaponFriend != null)
            {
                if (WeaponFriend.CurrentDurability > 0 && WeaponFriend.CanAttack && !HeroFriend.IsFrozen && ProfileInterface.Behavior.ShouldAttackWithWeapon(this))
                {
                    if (taunts.Count == 0)
                    {
                        foreach (Card Enemy in MinionEnemy)
                        {
                            if (Enemy.IsStealth)
                                continue;

                            if (!ProfileInterface.Behavior.ShouldAttackTargetWithWeapon(WeaponFriend, Enemy))
                                continue;
                            Action a = new Action(Action.ActionType.HERO_ATTACK, WeaponFriend, Enemy);
                            availableActions.Add(a);
                        }

                        if (ProfileInterface.Behavior.ShouldAttackTargetWithWeapon(WeaponFriend, HeroEnemy))
                        {
                            Action ac = new Action(Action.ActionType.HERO_ATTACK, WeaponFriend, HeroEnemy);
                            availableActions.Add(ac);
                        }

                    }
                    else
                    {
                        foreach (Card taunt in taunts)
                        {
                            Action a = new Action(Action.ActionType.HERO_ATTACK, WeaponFriend, taunt);
                            availableActions.Add(a);
                        }
                    }
                }
            }
            else if (HeroFriend.CurrentAtk > 0 && HeroFriend.CanAttack)
            {

                if (taunts.Count == 0)
                {
                    foreach (Card Enemy in MinionEnemy)
                    {
                        if (Enemy.IsStealth)
                            continue;
                        Action a = new Action(Action.ActionType.HERO_ATTACK, HeroFriend, Enemy);
                        availableActions.Add(a);
                    }
                    Action ac = new Action(Action.ActionType.HERO_ATTACK, HeroFriend, HeroEnemy);
                    availableActions.Add(ac);
                }
                else
                {
                    foreach (Card taunt in taunts)
                    {
                        Action a = new Action(Action.ActionType.HERO_ATTACK, HeroFriend, taunt);
                        availableActions.Add(a);
                    }

                }

            }

            if (Ability != null)
            {
                if (Ability.CurrentCost <= ManaAvailable && Ability.Behavior.ShouldBePlayed(this))
                {
                    if (Ability.TargetTypeOnPlay == Card.TargetType.MINION_ENEMY || Ability.TargetTypeOnPlay == Card.TargetType.MINION_BOTH
                           || Ability.TargetTypeOnPlay == Card.TargetType.BOTH_ENEMY || Ability.TargetTypeOnPlay == Card.TargetType.ALL)
                    {
                        foreach (Card Enemy in MinionEnemy)
                        {
                            if (!Enemy.IsTargetable || Enemy.IsStealth)
                                continue;
                            if (!Ability.Behavior.ShouldBePlayedOnTarget(Enemy))
                                continue;
                            Action a = null;
                            a = new Action(Action.ActionType.CAST_ABILITY, Ability, Enemy);
                            availableActions.Add(a);
                        }
                    }
                    if (Ability.TargetTypeOnPlay == Card.TargetType.MINION_FRIEND || Ability.TargetTypeOnPlay == Card.TargetType.MINION_BOTH
                        || Ability.TargetTypeOnPlay == Card.TargetType.BOTH_FRIEND || Ability.TargetTypeOnPlay == Card.TargetType.ALL)
                    {
                        foreach (Card friend in MinionFriend)
                        {
                            if (!friend.IsTargetable || friend.IsStealth)
                                continue;
                            if (!Ability.Behavior.ShouldBePlayedOnTarget(friend))
                                continue;
                            Action a = null;
                            a = new Action(Action.ActionType.CAST_ABILITY, Ability, friend);
                            availableActions.Add(a);
                        }
                    }
                    if (Ability.TargetTypeOnPlay == Card.TargetType.HERO_ENEMY || Ability.TargetTypeOnPlay == Card.TargetType.HERO_BOTH
                        || Ability.TargetTypeOnPlay == Card.TargetType.BOTH_ENEMY || Ability.TargetTypeOnPlay == Card.TargetType.ALL)
                    {

                        Action a = null;
                        a = new Action(Action.ActionType.CAST_ABILITY, Ability, HeroEnemy);
                        if (Ability.Behavior.ShouldBePlayedOnTarget(HeroEnemy))
                        {
                            availableActions.Add(a);

                        }
                    }
                    if (Ability.TargetTypeOnPlay == Card.TargetType.HERO_FRIEND || Ability.TargetTypeOnPlay == Card.TargetType.HERO_BOTH
                        || Ability.TargetTypeOnPlay == Card.TargetType.BOTH_FRIEND || Ability.TargetTypeOnPlay == Card.TargetType.ALL)
                    {
                        Action a = null;
                        a = new Action(Action.ActionType.CAST_ABILITY, Ability, HeroFriend);
                        if (Ability.Behavior.ShouldBePlayedOnTarget(HeroFriend))
                        {
                            availableActions.Add(a);

                        }
                    }

                    if (Ability.TargetTypeOnPlay == Card.TargetType.NONE)
                    {
                        Action a = null;
                        a = new Action(Action.ActionType.CAST_ABILITY, Ability);
                        availableActions.Add(a);
                    }
                }
            }
            List<Card> castedCards = new List<Card>();
            foreach (Card c in Hand)
            {
                bool alreadyCasted = false;
                foreach (Card cc in castedCards)
                {
                    if (cc.template.Id == c.template.Id)
                        alreadyCasted = true;
                }
                if (alreadyCasted)
                    continue;

                castedCards.Add(c);
                if (c.CurrentCost <= ManaAvailable && c.Behavior.ShouldBePlayed(this))
                {

                    if (c.TargetTypeOnPlay == Card.TargetType.MINION_BOTH)
                    {
                        Action a = null;
                        if (c.Type == Card.CType.MINION && MinionFriend.Count < 1 && MinionEnemy.Count < 1 && ProfileInterface.Behavior.ShouldPlayMoreMinions(this))
                        {
                            if (c.HasChoices)
                            {
                                a = new Action(Action.ActionType.CAST_MINION, c, null, 0, 1);
                                if (c.ChoiceOneTarget)
                                    availableActions.Add(a);
                                a = new Action(Action.ActionType.CAST_MINION, c, null, 0, 2);
                                if (c.ChoiceTwoTarget)
                                    availableActions.Add(a);
                            }
                            else
                            {
                                a = new Action(Action.ActionType.CAST_MINION, c);
                                availableActions.Add(a);
                            }
                        }
                    }

                    if (c.TargetTypeOnPlay == Card.TargetType.MINION_ENEMY || c.TargetTypeOnPlay == Card.TargetType.MINION_BOTH
                        || c.TargetTypeOnPlay == Card.TargetType.BOTH_ENEMY || c.TargetTypeOnPlay == Card.TargetType.ALL)
                    {
                        Action a = null;
                        if (c.Type == Card.CType.MINION && MinionEnemy.Count < 1 && c.TargetTypeOnPlay != Card.TargetType.MINION_BOTH && ProfileInterface.Behavior.ShouldPlayMoreMinions(this))
                        {
                            if (c.HasChoices)
                            {
                                a = new Action(Action.ActionType.CAST_MINION, c, null, 0, 1);
                                if (c.ChoiceOneTarget)
                                    availableActions.Add(a);
                                a = new Action(Action.ActionType.CAST_MINION, c, null, 0, 2);
                                if (c.ChoiceTwoTarget)
                                    availableActions.Add(a);
                            }
                            else
                            {
                                a = new Action(Action.ActionType.CAST_MINION, c);
                                availableActions.Add(a);
                            }
                        }

                        foreach (Card Enemy in MinionEnemy)
                        {
                            if (c.Type == Card.CType.SPELL)
                            {
                                if (!Enemy.IsTargetable)
                                    continue;
                            }

                            if (Enemy.IsStealth)
                                continue;
                            if (c.Type == Card.CType.MINION && MinionFriend.Count < 7 && ProfileInterface.Behavior.ShouldPlayMoreMinions(this))
                            {

                                if (c.TestAllIndexOnPlay || HasFriendBuffer())
                                {
                                    if (c.HasChoices)
                                    {
                                        for (int i = 0; i <= MinionFriend.Count; i++)
                                        {
                                            a = new Action(Action.ActionType.CAST_MINION, c, Enemy, i, 1);
                                            if (c.Behavior.ShouldBePlayedOnTarget(Enemy))
                                            {
                                                if (c.ChoiceOneTarget)
                                                    availableActions.Add(a);
                                            }
                                            a = new Action(Action.ActionType.CAST_MINION, c, Enemy, i, 2);
                                            if (c.Behavior.ShouldBePlayedOnTarget(Enemy))
                                            {
                                                if (c.ChoiceTwoTarget)
                                                    availableActions.Add(a);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i <= MinionFriend.Count; i++)
                                        {
                                            a = new Action(Action.ActionType.CAST_MINION, c, Enemy, i);
                                            if (c.Behavior.ShouldBePlayedOnTarget(Enemy))
                                            {
                                                availableActions.Add(a);
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if (c.HasChoices)
                                    {

                                        a = new Action(Action.ActionType.CAST_MINION, c, Enemy, 0, 1);
                                        if (c.Behavior.ShouldBePlayedOnTarget(Enemy))
                                        {
                                            if (c.ChoiceOneTarget)
                                                availableActions.Add(a);
                                        }
                                        a = new Action(Action.ActionType.CAST_MINION, c, Enemy, 0, 2);
                                        if (c.Behavior.ShouldBePlayedOnTarget(Enemy))
                                        {
                                            if (c.ChoiceTwoTarget)
                                                availableActions.Add(a);
                                        }
                                    }
                                    else
                                    {
                                        a = new Action(Action.ActionType.CAST_MINION, c, Enemy);
                                        if (c.Behavior.ShouldBePlayedOnTarget(Enemy))
                                        {
                                            availableActions.Add(a);
                                        }
                                    }
                                }
                            }
                            else if (c.Type == Card.CType.SPELL)
                            {
                                if (c.HasChoices)
                                {
                                    a = new Action(Action.ActionType.CAST_SPELL, c, Enemy, 0, 1);
                                    if (c.Behavior.ShouldBePlayedOnTarget(Enemy))
                                    {
                                        if (c.ChoiceOneTarget)
                                            availableActions.Add(a);
                                    }
                                    a = new Action(Action.ActionType.CAST_SPELL, c, Enemy, 0, 2);
                                    if (c.Behavior.ShouldBePlayedOnTarget(Enemy))
                                    {
                                        if (c.ChoiceTwoTarget)
                                            availableActions.Add(a);
                                    }
                                }
                                else
                                {
                                    a = new Action(Action.ActionType.CAST_SPELL, c, Enemy);
                                    if (c.Behavior.ShouldBePlayedOnTarget(Enemy))
                                    {
                                        availableActions.Add(a);
                                    }
                                }
                            }
                            else if (c.Type == Card.CType.WEAPON)
                            {
                                a = new Action(Action.ActionType.CAST_WEAPON, c, Enemy);
                                if (c.Behavior.ShouldBePlayedOnTarget(Enemy))
                                {
                                    availableActions.Add(a);
                                }

                            }
                        }
                    }
                    if (c.TargetTypeOnPlay == Card.TargetType.MINION_FRIEND || c.TargetTypeOnPlay == Card.TargetType.MINION_BOTH
                        || c.TargetTypeOnPlay == Card.TargetType.BOTH_FRIEND || c.TargetTypeOnPlay == Card.TargetType.ALL)
                    {
                        Action a = null;
                        if (c.Type == Card.CType.MINION && MinionFriend.Count < 1 && c.TargetTypeOnPlay != Card.TargetType.MINION_BOTH && ProfileInterface.Behavior.ShouldPlayMoreMinions(this))
                        {
                            if (c.HasChoices)
                            {
                                a = new Action(Action.ActionType.CAST_MINION, c, null, 0, 1);
                                if (c.ChoiceOneTarget)
                                    availableActions.Add(a);
                                a = new Action(Action.ActionType.CAST_MINION, c, null, 0, 2);
                                if (c.ChoiceTwoTarget)
                                    availableActions.Add(a);
                            }
                            else
                            {
                                a = new Action(Action.ActionType.CAST_MINION, c);
                                availableActions.Add(a);
                            }
                        }

                        foreach (Card friend in MinionFriend)
                        {
                            if (c.Type == Card.CType.SPELL)
                            {
                                if (!friend.IsTargetable)
                                    continue;
                            }

                            if (c.Type == Card.CType.MINION && MinionFriend.Count < 7 && ProfileInterface.Behavior.ShouldPlayMoreMinions(this))
                            {
                                if (c.TestAllIndexOnPlay || HasFriendBuffer())
                                {
                                    if (c.HasChoices)
                                    {
                                        for (int i = 0; i <= MinionFriend.Count; i++)
                                        {
                                            a = new Action(Action.ActionType.CAST_MINION, c, friend, i, 1);
                                            if (c.Behavior.ShouldBePlayedOnTarget(friend))
                                            {
                                                if (c.ChoiceOneTarget)
                                                    availableActions.Add(a);
                                            }
                                            a = new Action(Action.ActionType.CAST_MINION, c, friend, i, 2);
                                            if (c.Behavior.ShouldBePlayedOnTarget(friend))
                                            {
                                                if (c.ChoiceTwoTarget)
                                                    availableActions.Add(a);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i <= MinionFriend.Count; i++)
                                        {
                                            a = new Action(Action.ActionType.CAST_MINION, c, friend, i);
                                            if (c.Behavior.ShouldBePlayedOnTarget(friend))
                                            {
                                                availableActions.Add(a);
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if (c.HasChoices)
                                    {
                                        a = new Action(Action.ActionType.CAST_MINION, c, friend, 0, 1);
                                        if (c.Behavior.ShouldBePlayedOnTarget(friend))
                                        {
                                            if (c.ChoiceOneTarget)
                                                availableActions.Add(a);
                                        }
                                        a = new Action(Action.ActionType.CAST_MINION, c, friend, 0, 2);
                                        if (c.Behavior.ShouldBePlayedOnTarget(friend))
                                        {
                                            if (c.ChoiceTwoTarget)
                                                availableActions.Add(a);
                                        }
                                    }
                                    else
                                    {
                                        a = new Action(Action.ActionType.CAST_MINION, c, friend);
                                        if (c.Behavior.ShouldBePlayedOnTarget(friend))
                                        {
                                            availableActions.Add(a);
                                        }
                                    }
                                }
                            }
                            else if (c.Type == Card.CType.SPELL)
                            {
                                if (c.HasChoices)
                                {
                                    a = new Action(Action.ActionType.CAST_SPELL, c, friend, 0, 1);
                                    if (c.Behavior.ShouldBePlayedOnTarget(friend))
                                    {
                                        if (c.ChoiceOneTarget)
                                            availableActions.Add(a);
                                    }
                                    a = new Action(Action.ActionType.CAST_SPELL, c, friend, 0, 2);
                                    if (c.Behavior.ShouldBePlayedOnTarget(friend))
                                    {
                                        if (c.ChoiceTwoTarget)
                                            availableActions.Add(a);
                                    }
                                }
                                else
                                {
                                    a = new Action(Action.ActionType.CAST_SPELL, c, friend);
                                    if (c.Behavior.ShouldBePlayedOnTarget(friend))
                                    {
                                        availableActions.Add(a);
                                    }
                                }
                            }
                            else if (c.Type == Card.CType.WEAPON)
                            {
                                a = new Action(Action.ActionType.CAST_WEAPON, c, friend);
                                if (c.Behavior.ShouldBePlayedOnTarget(friend))
                                {
                                    availableActions.Add(a);
                                }
                            }
                        }
                    }
                    if (c.TargetTypeOnPlay == Card.TargetType.HERO_ENEMY || c.TargetTypeOnPlay == Card.TargetType.HERO_BOTH
                        || c.TargetTypeOnPlay == Card.TargetType.BOTH_ENEMY || c.TargetTypeOnPlay == Card.TargetType.ALL)
                    {

                        Action a = null;
                        if (c.Type == Card.CType.MINION && MinionFriend.Count < 7 && ProfileInterface.Behavior.ShouldPlayMoreMinions(this))
                        {
                            if (c.TestAllIndexOnPlay || HasFriendBuffer())
                            {
                                if (c.HasChoices)
                                {
                                    for (int i = 0; i <= MinionFriend.Count; i++)
                                    {
                                        a = new Action(Action.ActionType.CAST_MINION, c, HeroEnemy, i, 1);
                                        if (c.Behavior.ShouldBePlayedOnTarget(HeroEnemy))
                                        {
                                            if (c.ChoiceOneTarget)
                                                availableActions.Add(a);
                                        }
                                        a = new Action(Action.ActionType.CAST_MINION, c, HeroEnemy, i, 2);
                                        if (c.Behavior.ShouldBePlayedOnTarget(HeroEnemy))
                                        {
                                            if (c.ChoiceTwoTarget)
                                                availableActions.Add(a);
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i <= MinionFriend.Count; i++)
                                    {
                                        a = new Action(Action.ActionType.CAST_MINION, c, HeroEnemy, i);
                                        if (c.Behavior.ShouldBePlayedOnTarget(HeroEnemy))
                                        {
                                            availableActions.Add(a);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (c.HasChoices)
                                {
                                    a = new Action(Action.ActionType.CAST_MINION, c, HeroEnemy, 0, 1);

                                    if (c.Behavior.ShouldBePlayedOnTarget(HeroEnemy))
                                    {
                                        if (c.ChoiceOneTarget)
                                            availableActions.Add(a);
                                    }
                                    a = new Action(Action.ActionType.CAST_MINION, c, HeroEnemy, 0, 2);
                                    if (c.Behavior.ShouldBePlayedOnTarget(HeroEnemy))
                                    {
                                        if (c.ChoiceTwoTarget)
                                            availableActions.Add(a);
                                    }
                                }
                                else
                                {
                                    a = new Action(Action.ActionType.CAST_MINION, c, HeroEnemy);
                                    if (c.Behavior.ShouldBePlayedOnTarget(HeroEnemy))
                                    {
                                        availableActions.Add(a);
                                    }
                                }
                            }
                        }
                        else if (c.Type == Card.CType.SPELL)
                        {
                            if (c.HasChoices)
                            {
                                a = new Action(Action.ActionType.CAST_SPELL, c, HeroEnemy, 0, 1);
                                if (c.Behavior.ShouldBePlayedOnTarget(HeroEnemy))
                                {
                                    if (c.ChoiceOneTarget)
                                        availableActions.Add(a);
                                }
                                a = new Action(Action.ActionType.CAST_SPELL, c, HeroEnemy, 0, 2);
                                if (c.Behavior.ShouldBePlayedOnTarget(HeroEnemy))
                                {
                                    if (c.ChoiceTwoTarget)
                                        availableActions.Add(a);
                                }
                            }
                            else
                            {
                                a = new Action(Action.ActionType.CAST_SPELL, c, HeroEnemy);
                                if (c.Behavior.ShouldBePlayedOnTarget(HeroEnemy))
                                {
                                    availableActions.Add(a);
                                }
                            }
                        }
                        else if (c.Type == Card.CType.WEAPON)
                        {
                            a = new Action(Action.ActionType.CAST_WEAPON, c, HeroEnemy);
                            if (c.Behavior.ShouldBePlayedOnTarget(HeroEnemy))
                            {
                                availableActions.Add(a);
                            }
                        }
                    }
                    if (c.TargetTypeOnPlay == Card.TargetType.HERO_FRIEND || c.TargetTypeOnPlay == Card.TargetType.HERO_BOTH
                        || c.TargetTypeOnPlay == Card.TargetType.BOTH_FRIEND || c.TargetTypeOnPlay == Card.TargetType.ALL)
                    {
                        Action a = null;
                        if (c.Type == Card.CType.MINION && MinionFriend.Count < 7 && ProfileInterface.Behavior.ShouldPlayMoreMinions(this))
                        {
                            if (c.TestAllIndexOnPlay || HasFriendBuffer())
                            {
                                if (c.HasChoices)
                                {
                                    for (int i = 0; i <= MinionFriend.Count; i++)
                                    {
                                        a = new Action(Action.ActionType.CAST_MINION, c, HeroFriend, i, 1);
                                        if (c.Behavior.ShouldBePlayedOnTarget(HeroFriend))
                                        {
                                            if (c.ChoiceOneTarget)
                                                availableActions.Add(a);
                                        }
                                        a = new Action(Action.ActionType.CAST_MINION, c, HeroFriend, i, 2);
                                        if (c.Behavior.ShouldBePlayedOnTarget(HeroFriend))
                                        {
                                            if (c.ChoiceTwoTarget)
                                                availableActions.Add(a);
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i <= MinionFriend.Count; i++)
                                    {
                                        a = new Action(Action.ActionType.CAST_MINION, c, HeroFriend, i);
                                        if (c.Behavior.ShouldBePlayedOnTarget(HeroFriend))
                                        {
                                            availableActions.Add(a);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (c.HasChoices)
                                {
                                    a = new Action(Action.ActionType.CAST_MINION, c, HeroFriend, 0, 1);
                                    if (c.Behavior.ShouldBePlayedOnTarget(HeroFriend))
                                    {
                                        if (c.ChoiceOneTarget)
                                            availableActions.Add(a);
                                    }

                                    a = new Action(Action.ActionType.CAST_MINION, c, HeroFriend, 0, 2);
                                    if (c.Behavior.ShouldBePlayedOnTarget(HeroFriend))
                                    {
                                        if (c.ChoiceTwoTarget)
                                            availableActions.Add(a);
                                    }
                                }
                                else
                                {
                                    a = new Action(Action.ActionType.CAST_MINION, c, HeroFriend);
                                    if (c.Behavior.ShouldBePlayedOnTarget(HeroFriend))
                                    {
                                        availableActions.Add(a);
                                    }
                                }
                            }
                        }
                        else if (c.Type == Card.CType.SPELL)
                        {
                            if (c.HasChoices)
                            {
                                a = new Action(Action.ActionType.CAST_SPELL, c, HeroFriend, 0, 1);
                                if (c.Behavior.ShouldBePlayedOnTarget(HeroFriend))
                                {
                                    if (c.ChoiceOneTarget)
                                        availableActions.Add(a);
                                }
                                a = new Action(Action.ActionType.CAST_SPELL, c, HeroFriend, 0, 2);
                                if (c.Behavior.ShouldBePlayedOnTarget(HeroFriend))
                                {
                                    if (c.ChoiceTwoTarget)
                                        availableActions.Add(a);
                                }
                            }
                            else
                            {
                                a = new Action(Action.ActionType.CAST_SPELL, c, HeroFriend);
                                if (c.Behavior.ShouldBePlayedOnTarget(HeroFriend))
                                {
                                    availableActions.Add(a);
                                }
                            }
                        }
                        else if (c.Type == Card.CType.WEAPON)
                        {
                            a = new Action(Action.ActionType.CAST_WEAPON, c, HeroFriend);
                            if (c.Behavior.ShouldBePlayedOnTarget(HeroFriend))
                            {
                                availableActions.Add(a);
                            }
                        }
                    }

                    if (c.TargetTypeOnPlay == Card.TargetType.NONE || (c.HasChoices && (!c.ChoiceOneTarget || !c.ChoiceTwoTarget)))
                    {
                        Action a = null;
                        if (c.Type == Card.CType.MINION && MinionFriend.Count < 7 && ProfileInterface.Behavior.ShouldPlayMoreMinions(this))
                        {
                            if (c.TestAllIndexOnPlay || HasFriendBuffer())
                            {
                                if (c.HasChoices)
                                {
                                    for (int i = 0; i <= MinionFriend.Count; i++)
                                    {
                                        a = new Action(Action.ActionType.CAST_MINION, c, null, i, 1);
                                        if (!c.ChoiceOneTarget)
                                            availableActions.Add(a);

                                        a = new Action(Action.ActionType.CAST_MINION, c, null, i, 2);
                                        if (!c.ChoiceTwoTarget)
                                            availableActions.Add(a);
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i <= MinionFriend.Count; i++)
                                    {
                                        a = new Action(Action.ActionType.CAST_MINION, c, null, i);
                                        availableActions.Add(a);
                                    }
                                }

                            }
                            else
                            {
                                if (c.HasChoices)
                                {
                                    a = new Action(Action.ActionType.CAST_MINION, c, null, 0, 1);
                                    if (!c.ChoiceOneTarget)
                                        availableActions.Add(a);
                                    a = new Action(Action.ActionType.CAST_MINION, c, null, 0, 2);
                                    if (!c.ChoiceTwoTarget)
                                        availableActions.Add(a);
                                }
                                else
                                {
                                    a = new Action(Action.ActionType.CAST_MINION, c, null);
                                    availableActions.Add(a);
                                }
                            }
                        }
                        else if (c.Type == Card.CType.SPELL)
                        {
                            if (c.HasChoices)
                            {
                                a = new Action(Action.ActionType.CAST_SPELL, c, null, 0, 1);
                                if (!c.ChoiceOneTarget)
                                    availableActions.Add(a);
                                a = new Action(Action.ActionType.CAST_SPELL, c, null, 0, 2);
                                if (!c.ChoiceTwoTarget)
                                    availableActions.Add(a);
                            }
                            else
                            {
                                a = new Action(Action.ActionType.CAST_SPELL, c, null);
                                availableActions.Add(a);
                            }
                        }
                        else if (c.Type == Card.CType.WEAPON)
                        {
                            a = new Action(Action.ActionType.CAST_WEAPON, c);
                            availableActions.Add(a);
                        }
                    }
                }
            }


            /*foreach (Action aa in availableActions)
            {
                if (aa.Type == Action.ActionType.CAST_MINION)
                    Console.WriteLine("");

            }
            */
            return availableActions;
        }

        public void EndTurn()
        {
            foreach (Card c in MinionEnemy)
            {
                c.OnEndTurn(this);
                c.TempAtk = 0;
                c.IsImmune = false;
            }
            foreach (Card c in MinionFriend)
            {
                c.OnEndTurn(this);
                c.TempAtk = 0;
                c.IsImmune = false;
            }
            Update();

            /*  if(ActionsStack.Count == 1)
              {
                  if (ActionsStack[0].Type == Action.ActionType.RESIMULATE)
                      ActionsStack.Clear();
              }*/
        }

        public void CalculateEnemyTurn()
        {
            if (EnemyTurnCalculated)
                return;

            EnemyTurnWorseBoard = CalculateEnemyTurnValue();
            EnemyTurnCalculated = true;
        }

        public void Update()
        {
            foreach (Card c in MinionEnemy.ToArray())
            {
                c.OnUpdate(this);
                if (c.IsDestroyed)
                {
                    RemoveCardFromBoard(c.Id);

                }
            }
            foreach (Card c in MinionFriend.ToArray())
            {
                c.OnUpdate(this);
                if (c.IsDestroyed)
                {
                    RemoveCardFromBoard(c.Id);
                }
            }


        }

        public List<Card> GetAllCards()
        {
            List<Card> ret = new List<Card>();

            foreach (Card c in Hand)
            {
                ret.Add(c);
            }
            foreach (Card c in MinionEnemy)
            {
                ret.Add(c);
            }
            foreach (Card c in MinionFriend)
            {
                ret.Add(c);
            }
            foreach (Card c in Secret)
            {
                ret.Add(c);
            }
            ret.Add(WeaponEnemy);
            ret.Add(WeaponFriend);
            ret.Add(HeroEnemy);
            ret.Add(HeroFriend);
            ret.Add(Ability);

            return ret;
        }

        public List<Card> GetAllCardsOnBoard()
        {
            List<Card> ret = new List<Card>();

            foreach (Card c in MinionEnemy)
            {
                ret.Add(c);
            }
            foreach (Card c in MinionFriend)
            {
                ret.Add(c);
            }
            foreach (Card c in Secret)
            {
                ret.Add(c);
            }
            if (WeaponEnemy != null)
            {
                ret.Add(WeaponEnemy);
            }
            if (WeaponFriend != null)
            {
                ret.Add(WeaponFriend);
            }
            if (HeroEnemy != null)
            {
                ret.Add(HeroEnemy);
            }
            if (HeroFriend != null)
            {
                ret.Add(HeroFriend);
            }

            return ret;
        }

        public List<Card> GetAllMinionsOnBoard()
        {
            List<Card> ret = new List<Card>();

            foreach (Card c in MinionEnemy)
            {
                ret.Add(c);
            }
            foreach (Card c in MinionFriend)
            {
                ret.Add(c);
            }
            if (HeroEnemy != null)
            {
                ret.Add(HeroEnemy);
            }
            if (HeroFriend != null)
            {
                ret.Add(HeroFriend);
            }
            return ret;
        }

        public List<Card> GetHandCards()
        {
            return Hand;
        }


        public Card GetCard(int id)
        {
            foreach (Card c in Hand)
            {
                if (c.Id == id)
                    return c;
            }
            foreach (Card c in MinionEnemy)
            {
                if (c.Id == id)
                    return c;
            }
            foreach (Card c in MinionFriend)
            {
                if (c.Id == id)
                    return c;
            }
            foreach (Card c in Secret)
            {
                if (c.Id == id)
                    return c;
            }

            if (HeroEnemy != null)
            {
                if (HeroEnemy.Id == id)
                    return HeroEnemy;
            }

            if (HeroFriend != null)
            {
                if (HeroFriend.Id == id)
                    return HeroFriend;
            }

            if (WeaponFriend != null)
            {
                if (WeaponFriend.Id == id)
                    return WeaponFriend;
            }

            if (WeaponEnemy != null)
            {
                if (WeaponEnemy.Id == id)
                    return WeaponEnemy;
            }

            if (Ability != null)
            {
                if (Ability.Id == id)
                    return Ability;
            }

            return null;
        }

        public static Board Clone(Board baseInstance)
        {
            Board newBoard = new Board();
            newBoard.TurnCount = baseInstance.TurnCount;
            foreach (Card c in baseInstance.Hand)
            {
                newBoard.Hand.Add(Card.Clone(c));
            }
            foreach (Card c in baseInstance.MinionFriend)
            {
                newBoard.MinionFriend.Add(Card.Clone(c));
            }
            foreach (Card c in baseInstance.MinionEnemy)
            {
                newBoard.MinionEnemy.Add(Card.Clone(c));
            }
            if (baseInstance.WeaponEnemy != null)
            {
                newBoard.WeaponEnemy = Card.Clone(baseInstance.WeaponEnemy);
            }
            else
            {
                newBoard.WeaponEnemy = null;
            }
            if (baseInstance.WeaponFriend != null)
            {
                newBoard.WeaponFriend = Card.Clone(baseInstance.WeaponFriend);

            }
            else
            {
                newBoard.WeaponFriend = null;
            }
            if (baseInstance.Ability != null)
            {
                newBoard.Ability = Card.Clone(baseInstance.Ability);

            }
            else
            {
                newBoard.Ability = null;
            }
            if (baseInstance.EnemyAbility != null)
            {
                newBoard.EnemyAbility = Card.Clone(baseInstance.EnemyAbility);

            }
            else
            {
                newBoard.EnemyAbility = null;
            }

            newBoard.HeroEnemy = Card.Clone(baseInstance.HeroEnemy);
            newBoard.HeroFriend = Card.Clone(baseInstance.HeroFriend);


            foreach (Card c in baseInstance.Secret)
            {
                newBoard.Secret.Add(Card.Clone(c));
            }


            newBoard.SecretEnemy = baseInstance.SecretEnemy;
            newBoard.ManaAvailable = baseInstance.ManaAvailable;

            foreach (Action a in baseInstance.ActionsStack)
            {
                newBoard.ActionsStack.Add(a);
            }
            newBoard.HealFactor = baseInstance.HealFactor;
            newBoard.EnemyHealFactor = baseInstance.EnemyHealFactor;

            newBoard.EnemyCardDraw = baseInstance.EnemyCardDraw;
            newBoard.FriendCardDraw = baseInstance.FriendCardDraw;

            return newBoard;
        }


        public override string ToString()
        {
            string ret = "";

            ret += "Board --- (" + HeroFriend.CurrentHealth.ToString() + "/" + HeroFriend.CurrentArmor.ToString() + "-" + HeroEnemy.CurrentHealth.ToString() + "/" + HeroEnemy.CurrentArmor.ToString() + "): " + Environment.NewLine;
            ret += "Mana : " + ManaAvailable.ToString() + Environment.NewLine;
            ret += "Enemy secret: " + SecretEnemy.ToString() + Environment.NewLine;

            ret += "Friends : " + Environment.NewLine;

            foreach (Card c in MinionFriend)
            {
                ret += c.ToString() + Environment.NewLine;
            }
            ret += "Enemy : " + Environment.NewLine;

            foreach (Card c in MinionEnemy)
            {
                ret += c.ToString() + Environment.NewLine;
            }

            ret += "Hand : " + Environment.NewLine;

            foreach (Card c in Hand)
            {
                ret += c.ToString() + Environment.NewLine;
            }
            if (WeaponFriend != null)
            {
                ret += "Weapon : " + Environment.NewLine;


                ret += WeaponFriend.ToString() + Environment.NewLine;
            }

            ret += "draw : " + FriendCardDraw.ToString();


            ret += "Value : " + GetValue().ToString();

            return ret;
        }

        public bool ListEquals(List<Card> list1, List<Card> list2)
        {
            if (list1.Count != list2.Count)
                return false;

            foreach (Card c1 in list1)
            {
                bool found = false;
                foreach (Card c2 in list2)
                {
                    if (c2.Id == c1.Id)
                    {
                        if (!c2.Equals(c1))
                        {
                            return false;
                        }
                        found = true;
                    }
                }
                if (!found)
                    return false;
            }
            /*
                        for (int i = 0; i < list1.Count; i++)
                        {
                            if (!list1[i].Equals(list2[i]))
                                return false;
                        }
                        */
            return true;
        }

        public bool Equals(Board b)
        {
            if (ManaAvailable != b.ManaAvailable)
                return false;
            if (FriendCardDraw != b.FriendCardDraw)
                return false;
            if (EnemyCardDraw != b.EnemyCardDraw)
                return false;
            if (Hand.Count != b.Hand.Count)
                return false;
            if (MinionEnemy.Count != b.MinionEnemy.Count)
                return false;
            if (MinionFriend.Count != b.MinionFriend.Count)
                return false;
            if (Secret.Count != b.Secret.Count)
                return false;
            if (HeroEnemy.CurrentHealth + HeroEnemy.CurrentArmor != b.HeroEnemy.CurrentArmor + b.HeroEnemy.CurrentHealth)
                return false;
            if (HeroFriend.CurrentHealth + HeroFriend.CurrentArmor != b.HeroFriend.CurrentArmor + b.HeroFriend.CurrentHealth)
                return false;

            /*if (GetValue() != b.GetValue())
                return false;
            */
            /*
            if (HeroEnemy != null)
            {
                if (b.HeroEnemy == null)
                    return false;
                if (!HeroEnemy.Equals(b.HeroEnemy))
                {
                    return false;
                }
            }
            if (HeroFriend != null)
            {
                if (b.HeroFriend == null)
                    return false;
                if (!HeroFriend.Equals(b.HeroFriend))
                {
                    return false;
                }
            }
             * */
            /*
            if (Ability != null)
            {
                if (b.Ability == null)
                    return false;
                if (!Ability.Equals(b.Ability))
                {
                    return false;
                }
            }
             * */
            /*
            if (WeaponEnemy != null)
            {
                if (b.WeaponEnemy == null)
                    return false;
                if (!WeaponEnemy.Equals(b.WeaponEnemy))
                {
                    return false;
                }
            }
            if (WeaponFriend != null)
            {
                if (b.WeaponFriend == null)
                    return false;
                if (!WeaponFriend.Equals(b.WeaponFriend))
                {
                    return false;
                }
            }
             * */
            if (!ListEquals(MinionEnemy, b.MinionEnemy))
                return false;
            if (!ListEquals(MinionFriend, b.MinionFriend))
                return false;




            return true;
        }
    }
}
