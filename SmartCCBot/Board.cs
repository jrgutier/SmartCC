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


        public int HealFactor { get; set; }
        public int FriendCardDraw { get; set; }
        public int EnemyCardDraw { get; set; }
        public int WastedATK { get; set; }

        static int lastIdGen = 10000;

        public float GetValue()
        {
            float value = 0;

            foreach (Card c in MinionEnemy)
            {
                value -= c.GetValue(this);
            }

            value += Secret.Count * ValuesInterface.ValueSecret;

            value -= (HeroEnemy.CurrentHealth * ValuesInterface.ValueHealthEnemy) + HeroEnemy.CurrentArmor * ValuesInterface.ValueArmorEnemy;

            foreach (Card c in MinionFriend)
            {
                value += c.GetValue(this);
            }
            value += (HeroFriend.CurrentHealth * ValuesInterface.ValueHealthFriend) + HeroFriend.CurrentArmor * ValuesInterface.ValueArmorFriend;

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


            return value;
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
            Secret = new List<Card>();
            SecretEnemy = false;
            ManaAvailable = 0;
            ActionsStack = new List<Action>();
            HealFactor = 1;
        }
        public Board(List<Card> hand, List<Card> minionFriend, List<Card> minionEnemy, Card weaponEnemy, Card weaponFriend, Card heroEnemy, Card heroFriend, Card ability, List<Card> secret, bool secretEnemy, int manaAvailable)
        {
            foreach (Card c in hand)
            {
                Hand.Add(Card.Clone(c));
            }
            foreach (Card c in minionFriend)
            {
                MinionFriend.Add(Card.Clone(c));
            }
            foreach (Card c in minionEnemy)
            {
                MinionEnemy.Add(Card.Clone(c));
            }
            WeaponEnemy = Card.Clone(weaponEnemy);
            WeaponFriend = Card.Clone(weaponFriend);
            HeroEnemy = Card.Clone(heroEnemy);
            HeroFriend = Card.Clone(heroFriend);
            Ability = Card.Clone(ability);

            foreach (Card c in secret)
            {
                Secret.Add(Card.Clone(c));
            }
            SecretEnemy = secretEnemy;
            ManaAvailable = manaAvailable;
            ActionsStack = new List<Action>();
        }

        public bool PlayCardFromHand(int id)
        {
            foreach (Card c in Hand.ToArray())
            {
                if (c.Id == id)
                {
                    if (SecretEnemy)
                    {
                        Resimulate();
                    }
                    Hand.Remove(c);
                    if (c.Type != Card.CType.WEAPON)
                        FriendCardDraw--;
                    ManaAvailable -= c.CurrentCost;
                    if (c.Type == Card.CType.WEAPON)
                    {
                        WeaponFriend = c;
                    }

                    return true;
                }
            }

            return false;
        }

        public bool PlayMinion(int id)
        {

            foreach (Card c in Hand.ToArray())
            {
                if (c.Id == id)
                {
                    if (SecretEnemy)
                    {
                        Resimulate();
                    }
                    Hand.Remove(c);
                    ManaAvailable -= c.CurrentCost;
                    if (c.Type == Card.CType.MINION)
                    {
                        if (c.IsCharge)
                        {
                            c.IsTired = false;
                        }
                        else
                        {
                            c.IsTired = true;
                        }

                        foreach (Card ca in MinionFriend)
                        {
                            if (ca.Index >= c.Index)
                                ca.Index++;
                        }

                        MinionFriend.Add(c);

                    }

                    return true;
                }
            }

            return false;
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

        public void Resimulate()
        {
            ActionsStack.Add(new Action(Action.ActionType.RESIMULATE, null));
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
            int foundIndex = -1;

            foreach (Card c in MinionFriend)
            {
                if (c.TestAllIndexOnPlay)
                {
                    foundIndex = c.Index;
                    break;
                }
            }
            if (foundIndex != -1)
            {
                bool right = false;
                bool left = false;

                foreach (Card c in MinionFriend)
                {
                    if (c.Index == foundIndex - 1)
                        right = true;
                    if (c.Index == foundIndex + 1)
                        left = true;
                }

                if (right && left)
                    return false;

                return true;
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
            foreach (Card c in MinionFriend.ToArray())
            {
                if (c.Id == id)
                {
                    foreach (Card ca in MinionFriend)
                    {
                        if (ca == c)
                            continue;

                        if (c.Index < ca.Index)
                            ca.Index--;
                    }

                    MinionFriend.Remove(c);
                    return true;
                }
            }
            foreach (Card c in MinionEnemy.ToArray())
            {
                if (c.Id == id)
                {
                    foreach (Card ca in MinionEnemy)
                    {
                        if (ca == c)
                            continue;

                        if (c.Index < ca.Index)
                            ca.Index--;
                    }
                    MinionEnemy.Remove(c);
                    return true;
                }
            }
            foreach (Card c in Secret.ToArray())
            {
                if (c.Id == id)
                {
                    Secret.Remove(c);
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
                        a.Actor.OnPlay(ref child, child.GetCard(a.Target.Id), a.Index);
                    }
                    else
                    {
                        a.Actor.OnPlay(ref child, null, a.Index);
                    }

                    foreach (Card c in child.GetAllMinionsOnBoard())
                    {
                        if (a.Actor.Equals(c))
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
                        a.Actor.OnPlay(ref child, child.GetCard(a.Target.Id));
                    else
                        a.Actor.OnPlay(ref child, null);
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
            /*if(ActionsStack.Count > 0)
            {
                if (ActionsStack[ActionsStack.Count - 1].Type == Action.ActionType.RESIMULATE)
                {
                    return new List<Action>();
                }
            }*/

            List<Action> availableActions = new List<Action>();

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
                        availableActions.Add(a);
                    }
                    if (Ability.TargetTypeOnPlay == Card.TargetType.HERO_FRIEND || Ability.TargetTypeOnPlay == Card.TargetType.HERO_BOTH
                        || Ability.TargetTypeOnPlay == Card.TargetType.BOTH_FRIEND || Ability.TargetTypeOnPlay == Card.TargetType.ALL)
                    {
                        Action a = null;
                        a = new Action(Action.ActionType.CAST_ABILITY, Ability, HeroFriend);
                        availableActions.Add(a);
                    }

                    if (Ability.TargetTypeOnPlay == Card.TargetType.NONE)
                    {
                        Action a = null;
                        a = new Action(Action.ActionType.CAST_ABILITY, Ability);
                        availableActions.Add(a);
                    }
                }
            }

            foreach (Card c in Hand)
            {
                if (c.CurrentCost <= ManaAvailable && c.Behavior.ShouldBePlayed(this))
                {
                    if (c.TargetTypeOnPlay == Card.TargetType.MINION_ENEMY || c.TargetTypeOnPlay == Card.TargetType.MINION_BOTH
                        || c.TargetTypeOnPlay == Card.TargetType.BOTH_ENEMY || c.TargetTypeOnPlay == Card.TargetType.ALL)
                    {
                        foreach (Card Enemy in MinionEnemy)
                        {
                            if (c.Type == Card.CType.SPELL)
                            {
                                if (!Enemy.IsTargetable)
                                    continue;
                            }

                            if (Enemy.IsStealth)
                                continue;
                            Action a = null;
                            if (c.Type == Card.CType.MINION && MinionFriend.Count < 7)
                            {
                                if (c.TestAllIndexOnPlay || HasFriendBuffer())
                                {
                                    for (int i = 0; i < MinionFriend.Count; i++)
                                    {
                                        if (MinionFriend.Count > 1 && !HasFriendBuffer())
                                        {
                                            if (i == 0)
                                                continue;
                                            if (i == MinionFriend.Count)
                                                continue;
                                        }


                                        a = new Action(Action.ActionType.CAST_MINION, c, Enemy, i);
                                        if (c.Behavior.ShouldBePlayedOnTarget(Enemy))
                                        {
                                            availableActions.Add(a);
                                        }
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
                            else if (c.Type == Card.CType.SPELL)
                            {
                                a = new Action(Action.ActionType.CAST_SPELL, c, Enemy);
                                if (c.Behavior.ShouldBePlayedOnTarget(Enemy))
                                {
                                    availableActions.Add(a);
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

                        foreach (Card friend in MinionFriend)
                        {
                            if (c.Type == Card.CType.SPELL)
                            {
                                if (!friend.IsTargetable)
                                    continue;
                            }

                            Action a = null;
                            if (c.Type == Card.CType.MINION && MinionFriend.Count < 7)
                            {
                                if (c.TestAllIndexOnPlay || HasFriendBuffer())
                                {
                                    for (int i = 0; i < MinionFriend.Count; i++)
                                    {
                                        if (MinionFriend.Count > 1 && !HasFriendBuffer())
                                        {
                                            if (i == 0)
                                                continue;
                                            if (i == MinionFriend.Count)
                                                continue;
                                        }
                                        a = new Action(Action.ActionType.CAST_MINION, c, friend, i);
                                        if (c.Behavior.ShouldBePlayedOnTarget(friend))
                                        {
                                            availableActions.Add(a);
                                        }

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
                            else if (c.Type == Card.CType.SPELL)
                            {
                                a = new Action(Action.ActionType.CAST_SPELL, c, friend);
                                if (c.Behavior.ShouldBePlayedOnTarget(friend))
                                {
                                    availableActions.Add(a);
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
                        if (c.Type == Card.CType.MINION && MinionFriend.Count < 7)
                        {
                            if (c.TestAllIndexOnPlay || HasFriendBuffer())
                            {
                                for (int i = 0; i < MinionFriend.Count; i++)
                                {
                                    if (MinionFriend.Count > 1 && !HasFriendBuffer())
                                    {
                                        if (i == 0)
                                            continue;
                                        if (i == MinionFriend.Count)
                                            continue;
                                    }
                                    a = new Action(Action.ActionType.CAST_MINION, c, HeroEnemy, i);
                                    if (c.Behavior.ShouldBePlayedOnTarget(HeroEnemy))
                                    {
                                        availableActions.Add(a);
                                    }
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
                        else if (c.Type == Card.CType.SPELL)
                        {
                            a = new Action(Action.ActionType.CAST_SPELL, c, HeroEnemy);
                            if (c.Behavior.ShouldBePlayedOnTarget(HeroEnemy))
                            {
                                availableActions.Add(a);
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
                        if (c.Type == Card.CType.MINION && MinionFriend.Count < 7)
                        {
                            if (c.TestAllIndexOnPlay || HasFriendBuffer())
                            {
                                for (int i = 0; i < MinionFriend.Count; i++)
                                {
                                    if (MinionFriend.Count > 1 && !HasFriendBuffer())
                                    {
                                        if (i == 0)
                                            continue;
                                        if (i == MinionFriend.Count)
                                            continue;
                                    }
                                    a = new Action(Action.ActionType.CAST_MINION, c, HeroFriend, i);
                                    if (c.Behavior.ShouldBePlayedOnTarget(HeroFriend))
                                    {
                                        availableActions.Add(a);
                                    }
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
                        else if (c.Type == Card.CType.SPELL)
                        {
                            a = new Action(Action.ActionType.CAST_SPELL, c, HeroFriend);
                            if (c.Behavior.ShouldBePlayedOnTarget(HeroFriend))
                            {
                                availableActions.Add(a);
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

                    if (c.TargetTypeOnPlay == Card.TargetType.NONE || c.TargetTypeOnPlay == Card.TargetType.ALL)
                    {
                        Action a = null;
                        if (c.Type == Card.CType.MINION && MinionFriend.Count < 7)
                        {
                            if (c.TestAllIndexOnPlay || HasFriendBuffer())
                            {
                                for (int i = 0; i < MinionFriend.Count; i++)
                                {
                                    if (MinionFriend.Count > 1 && !HasFriendBuffer())
                                    {
                                        if (i == 0)
                                            continue;
                                        if (i == MinionFriend.Count)
                                            continue;
                                    }
                                    a = new Action(Action.ActionType.CAST_MINION, c, null, i);

                                    availableActions.Add(a);
                                }
                            }
                            else
                            {
                                a = new Action(Action.ActionType.CAST_MINION, c);
                                availableActions.Add(a);
                            }
                        }
                        else if (c.Type == Card.CType.SPELL)
                        {
                            a = new Action(Action.ActionType.CAST_SPELL, c);
                            availableActions.Add(a);
                        }
                        else if (c.Type == Card.CType.WEAPON)
                        {
                            a = new Action(Action.ActionType.CAST_WEAPON, c);
                            availableActions.Add(a);
                        }
                    }
                }
            }

            List<Card> taunts = new List<Card>();

            foreach (Card Enemy in MinionEnemy)
            {
                if (Enemy.IsTaunt && !Enemy.IsStealth)
                    taunts.Add(Enemy);
            }

            List<Card> attackers = new List<Card>();
            foreach (Card minion in MinionFriend)
            {
                bool alreadyAttacked = false;
                foreach(Card c in attackers)
                {
                    if (c.Equals(minion))
                    {
                        alreadyAttacked  = true;
                        break;
                    } 
                }
                if (alreadyAttacked)
                    continue;

                attackers.Add(minion);

                if (!minion.CanAttack || !minion.Behavior.ShouldAttack(this))
                    continue;

                if (taunts.Count == 0)
                {
                    foreach (Card Enemy in MinionEnemy)
                    {
                        if (Enemy.IsStealth)
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
                if (WeaponFriend.CurrentDurability > 0 && WeaponFriend.CanAttack)
                {
                    if (taunts.Count == 0)
                    {
                        foreach (Card Enemy in MinionEnemy)
                        {
                            if (Enemy.IsStealth)
                                continue;
                            Action a = new Action(Action.ActionType.HERO_ATTACK, WeaponFriend, Enemy);
                            availableActions.Add(a);
                        }
                        Action ac = new Action(Action.ActionType.HERO_ATTACK, WeaponFriend, HeroEnemy);
                        availableActions.Add(ac);
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



            return availableActions;
        }

        public void EndTurn()
        {
            foreach (Card c in MinionEnemy)
            {
                c.OnEndTurn(this);
            }
            foreach (Card c in MinionFriend)
            {
                c.OnEndTurn(this);
            }

            /*  if(ActionsStack.Count == 1)
              {
                  if (ActionsStack[0].Type == Action.ActionType.RESIMULATE)
                      ActionsStack.Clear();
              }*/
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

            newBoard.EnemyCardDraw = baseInstance.EnemyCardDraw;
            newBoard.FriendCardDraw = baseInstance.FriendCardDraw;

            return newBoard;
        }


        public override string ToString()
        {
            string ret = "";

            ret += "Board --- (" + HeroFriend.CurrentHealth.ToString() + "-" + HeroEnemy.CurrentHealth.ToString() + "): " + Environment.NewLine;
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
            if (Ability != null)
            {
                if (b.Ability == null)
                    return false;
                if (!Ability.Equals(b.Ability))
                {
                    return false;
                }
            }
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
            if (!ListEquals(MinionEnemy, b.MinionEnemy))
                return false;
            if (!ListEquals(MinionFriend, b.MinionFriend))
                return false;




            return true;
        }
    }
}
