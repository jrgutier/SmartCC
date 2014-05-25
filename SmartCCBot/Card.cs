using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
namespace HREngine.Bots
{
    [Serializable]
    public class Card : IEquatable<Card>
    {
        static Assembly assembly =Assembly.LoadFile(CardTemplate.DatabasePath + "/Bots/SmartCC/Profile.dll");


        public Behavior Behavior { get; set; }


        public virtual int GetValue(Board board)
        {
            int value = 0;
            if (Type == CType.MINION)
            {
                value += ValuesInterface.ValueHealthMinion * CurrentHealth;
                value += ValuesInterface.ValueAttackMinion * CurrentAtk;

                if (IsTaunt && IsFriend)
                {
                    if (board.HeroFriend.CurrentHealth > 20 && board.HeroFriend.CurrentHealth < 25)
                    {
                        value += ValuesInterface.ValueTaunt * 2;
                    }
                    else if (board.HeroFriend.CurrentHealth < 20)
                    {
                        value += ValuesInterface.ValueTaunt * 3;
                    }

                }
                else if (IsTaunt && !IsFriend)
                {
                    value += ValuesInterface.ValueTaunt;
                }

                if (IsDivineShield)
                    value += ValuesInterface.ValueDivineShield;

                if (IsFrozen)
                    value -= ValuesInterface.ValueFrozen;

                if (currentAtk == 0)
                    value -= 2;

            }
            else if (Type == CType.WEAPON)
            {
                value += ValuesInterface.ValueDurabilityWeapon * CurrentDurability;
                value += ValuesInterface.ValueAttackWeapon * CurrentAtk;
            }

            return value;
        }

        public virtual void OnUpdate(Board board)
        {
            if (CurrentHealth < 1)
            {
                if (!IsSilenced)
                    OnDeath(ref board);
            }

        }

        public virtual void OnDamage()
        {

        }

        public virtual void OnHeal()
        {

        }

        public virtual void Init()
        {

        }

        public virtual void OnPlay(ref Board board, Card target = null, int index = 0)
        {
            if (Type == CType.HERO_POWER)
            {
                board.PlayAbility();
            }
            else
            {
                if (Type == CType.MINION)
                {
                    board.GetCard(Id).Index = index;
                    board.PlayMinion(Id);

                }
                else if (Type == CType.SPELL || Type == CType.WEAPON)
                {
                    board.PlayCardFromHand(Id);

                }
            }
        }

        public virtual void OnDeath(ref Board board)
        {

            foreach (Card c in board.MinionFriend)
            {
                c.OnOtherMinionDeath(ref board, IsFriend, this);
            }

            foreach (Card c in board.MinionEnemy)
            {
                c.OnOtherMinionDeath(ref board, IsFriend, this);
            }


        }

        public virtual void OnOtherMinionDeath(ref Board board, bool friend, Card minion)
        {

        }

        public virtual void OnPlayOtherMinion(ref Board board, Card Minion)
        {

        }

        public virtual void OnEndTurn(Board board)
        {
            TempAtk = 0;
            IsImmune = false;
        }

        public virtual void OnCastSpell(ref Board board, Card Spell)
        {

            if (Type == CType.MINION)
            {
                if (CurrentHealth <= 0)
                {
                    if (!IsSilenced)
                        board.GetCard(Id).OnDeath(ref board);
                    board.RemoveCardFromBoard(Id);

                }
            }

        }

        public virtual void OnAttack(ref Board board, Card target)
        {
           
            Card me = board.GetCard(Id);
            Card tar = board.GetCard(target.Id);

            if (me.Type == CType.MINION)
            {
                me.CountAttack++;

                tar.OnHit(ref board, me);
                me.OnHit(ref board, tar);
            }
            else if (me.Type == CType.WEAPON)
            {
                me.CurrentDurability--;
                me.CountAttack++;
                board.HeroFriend.CountAttack++;
                board.HeroFriend.TempAtk += me.CurrentAtk;
                board.HeroFriend.OnHit(ref board, tar);
                tar.OnHit(ref board, board.HeroFriend);
                // tar.OnHit(ref board, me);
                if (me.CurrentDurability < 1)
                    board.WeaponFriend = null;
            }
            else if (me.Type == CType.HERO)
            {
                me.CountAttack++;

                tar.OnHit(ref board, me);
                me.OnHit(ref board, tar);
            }
        }

        public virtual void OnOtherMinionDamage(ref Board board)
        {

        }
        public virtual void OnOtherMinionHeal()
        {

        }

        public virtual void OnHit(ref Board board, Card actor)
        {
            if (IsImmune)
                return;

            if (Type != CType.HERO)
            {
                foreach (Card c in board.MinionFriend)
                {
                    c.OnOtherMinionDamage(ref board);
                }
                foreach (Card c in board.MinionEnemy)
                {
                    c.OnOtherMinionDamage(ref board);
                }
            }
            
            OnDamage();

            if (actor.HasFreeze)
                IsFrozen = true;

            if (IsDivineShield && actor.CurrentAtk > 0)
            {
                IsDivineShield = false;
            }
            else
            {
                if (CurrentArmor <= 0)
                {

                    CurrentHealth -= actor.CurrentAtk;

                }
                else
                {
                    
                    CurrentHealth -= (actor.CurrentAtk - CurrentArmor);
                    CurrentArmor -= (actor.CurrentAtk);
                    if (CurrentArmor <= 0)
                    {
                        CurrentArmor = 0;
                    }
                }

                if (CurrentHealth < MaxHealth)
                {
                    if (HasEnrage && !IsEnraged)
                    {
                        board.GetCard(Id).OnEnrage(true, ref board);
                        board.GetCard(Id).IsEnraged = true;
                    }
                }

                if (CurrentHealth <= 0 || actor.HasPoison)
                {
                    board.RemoveCardFromBoard(Id);
                    if (!IsSilenced)
                        OnDeath(ref board);
                }
            }
        }

        public virtual void OnEnrage(bool enraged, ref Board board)
        {
        }

        public void Heal(int amount, ref Board board)
        {
            if (amount < 0)
            {
                Damage(amount, ref board);
                return;
            }

            OnHeal();

            if (this.Type == CType.MINION)
            {
                foreach (Card c in board.MinionFriend)
                {
                    c.OnOtherMinionHeal();
                }

                foreach (Card c in board.MinionEnemy)
                {
                    c.OnOtherMinionHeal();
                }


                if (CurrentHealth + amount >= MaxHealth)
                {
                    CurrentHealth = MaxHealth;
                    if (HasEnrage && IsEnraged)
                    {
                        OnEnrage(false, ref board);
                        IsEnraged = false;
                    }
                }
                else
                {
                    CurrentHealth += amount;
                }
            }
            else
            {
                if (CurrentHealth + amount > 30)
                {
                    CurrentHealth = 30;
                }
                else
                {
                    CurrentHealth += amount;
                }
            }

        }

        public void Damage(int amount, ref Board board)
        {
            if (Type != CType.HERO)
            {
                foreach (Card c in board.MinionFriend)
                {
                    c.OnOtherMinionDamage(ref board);
                }

                foreach (Card c in board.MinionEnemy)
                {
                    c.OnOtherMinionDamage(ref board);
                }
            }


            OnDamage();
            if (IsDivineShield && amount > 0)
            {
                IsDivineShield = false;
            }
            else
            {
                if (CurrentArmor <= 0)
                {
                    CurrentHealth -= amount;

                }
                else
                {
                    CurrentHealth -= (amount - CurrentArmor);
                    CurrentArmor -= (amount);
                    if (CurrentArmor <= 0)
                    {
                        CurrentArmor = 0;
                    }
                }
                if (CurrentHealth < MaxHealth)
                {
                    if (HasEnrage && !IsEnraged)
                    {
                        OnEnrage(true, ref board);
                        IsEnraged = true;
                    }
                }


                if (CurrentHealth <= 0)
                {
                    IsDestroyed = true;
                }
            }
        }

        public enum CType
        {
            HERO = 0,
            MINION = 1,
            SPELL = 2,
            WEAPON = 3,
            HERO_POWER = 4
        }
        public enum CRace
        {
            MURLOC = 0,
            BEAST = 1,
            DEMON = 2,
            PIRATE = 3,
            TOTEM = 4,
            DRAGON = 5,
            NONE = 6
        }
        public enum TargetType
        {
            NONE = 0,
            HERO_FRIEND = 1,
            HERO_ENEMY = 2,
            HERO_BOTH = 3,
            MINION_FRIEND = 4,
            MINION_ENEMY = 5,
            MINION_BOTH = 6,
            BOTH_FRIEND = 7,
            BOTH_ENEMY = 8,
            ALL = 9
        }

        public string GetTargetTypeStr()
        {
            switch (TargetTypeOnPlay)
            {
                case TargetType.NONE:
                    return "NONE";
                case TargetType.HERO_FRIEND:
                    return "HERO_FRIEND";
                case TargetType.HERO_ENEMY:
                    return "HERO_ENEMY";
                case TargetType.HERO_BOTH:
                    return "HERO_BOTH";
                case TargetType.MINION_FRIEND:
                    return "MINION_FRIEND";
                case TargetType.MINION_ENEMY:
                    return "MINION_ENEMY";
                case TargetType.MINION_BOTH:
                    return "MINION_BOTH";
                case TargetType.BOTH_FRIEND:
                    return "BOTH_FRIEND";
                case TargetType.BOTH_ENEMY:
                    return "BOTH_ENEMY";
                case TargetType.ALL:
                    return "ALL";
            }
            return "";
        }

        public CardTemplate template { get; set; }

        public int CurrentCost { get; set; }

        public CType Type { get; set; }

        public CRace Race { get; set; }

        public int currentAtk = 0;

        public int CurrentAtk
        {
            get
            {
                return currentAtk + TempAtk;
            }
            set
            {
                currentAtk = value;
            }
        }

        public int maxHealth = 0;

        public int MaxHealth
        {
            get
            {
                int t = 0;

                t += maxHealth;

                foreach (Buff b in buffs)
                {
                    t += b.Hp;
                }

                return t;
            }
            set
            {
                maxHealth = value;
            }
        }

        public int CurrentHealth { get; set; }


        public int TempAtk { get; set; }

        public List<Buff> buffs { get; set; }

        public int CurrentArmor { get; set; }

        public int CurrentDurability { get; set; }

        public bool IsFriend { get; set; }

        public int Id { get; set; }

        public int Index { get; set; }

        public bool TestAllIndexOnPlay { get; set; }

        //------BATTLECRY

        public TargetType TargetTypeOnPlay { get; set; }

        //------EFFECTS

        public bool IsTargetable { get; set; }
        public bool IsTaunt { get; set; }
        public bool IsCharge { get; set; }
        public bool IsDivineShield { get; set; }
        public bool IsWindfury { get; set; }
        public bool IsStealth { get; set; }
        public bool IsTired { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsEnraged { get; set; }
        public bool IsSilenced { get; set; }
        public bool IsDestroyed { get; set; }
        public bool IsImmune { get; set; }

        public bool HasFreeze { get; set; }
        public bool HasPoison { get; set; }
        public bool HasEnrage { get; set; }

        public int SpellPower { get; set; }


        public int CountAttack { get; set; }
        public bool CanAttack
        {
            get
            {
                if (IsTired)
                    return false;
                if (CurrentAtk < 1)
                    return false;
                if (IsFrozen)
                    return false;
                if (CountAttack > 0 && !IsWindfury)
                    return false;
                if (CountAttack > 1 && IsWindfury)
                    return false;

                return true;
            }
        }

        public Card()
        {

        }

        public void AddBuff(Buff b)
        {
            buffs.Add(b);

            currentAtk += b.Atk;
            CurrentHealth += b.Hp;
            
        }

        public void RemoveBuffById(int id)
        {
            foreach (Buff b in buffs.ToArray())
            {
                if (b.OwnerId == id)
                {
                    buffs.Remove(b);
                    currentAtk -= b.Atk;
                    if(CurrentHealth > MaxHealth)
                        CurrentHealth -= b.Hp;
                }
            }
        }

        public void InitInstance(CardTemplate newTemplate, bool isFriend, int id)
        {
            buffs = new List<Buff>();
            Id = id;
            IsFriend = isFriend;
            template = newTemplate;
            CurrentCost = template.Cost;
            Index = 0;
            SpellPower = 0;
            TempAtk = 0;
            TestAllIndexOnPlay = false;
            if (template.Type.Contains("Minion"))
            {
                Type = CType.MINION;
            }
            else if (template.Type.Contains("Spell"))
            {
                Type = CType.SPELL;
            }
            else if (template.Type.Contains("Weapon"))
            {
                Type = CType.WEAPON;
            }
            else if (template.Type.Contains("Hero") && !template.Type.Contains("Power"))
            {
                Type = CType.HERO;
            }
            else if (template.Type.Contains("Hero Power"))
            {
                Type = CType.HERO_POWER;
            }

            if (template.Race == string.Empty)
            {
                Race = Card.CRace.NONE;
            }
            else if (template.Race.Contains("Murloc"))
            {
                Race = Card.CRace.MURLOC;
            }
            else if (template.Race.Contains("Beast"))
            {
                Race = Card.CRace.BEAST;
            }
            else if (template.Race.Contains("Demon"))
            {
                Race = Card.CRace.DEMON;
            }
            else if (template.Race.Contains("Pirate"))
            {
                Race = Card.CRace.PIRATE;
            }
            else if (template.Race.Contains("Totem"))
            {
                Race = Card.CRace.TOTEM;
            }
            else if (template.Race.Contains("Dragon"))
            {
                Race = Card.CRace.DRAGON;
            }

            CurrentAtk = template.Atk;
            CurrentHealth = template.Health;
            MaxHealth = template.Health;
            CurrentDurability = template.Durability;
            CurrentArmor = 0;
            TargetTypeOnPlay = TargetType.NONE;
            IsTaunt = false;
            IsCharge = false;
            IsDestroyed = false;
            IsDivineShield = false;
            IsEnraged = false;
            IsFrozen = false;
            IsSilenced = false;
            IsStealth = false;
            IsTired = false;
            IsWindfury = false;
            IsTargetable = true;
            HasEnrage = false;
            HasFreeze = false;
            HasPoison = false;
            IsImmune = false;
            CountAttack = 0;
            Init();
        }
        public Card(CardTemplate newTemplate, bool isFriend, int id)
        {
            buffs = new List<Buff>();
            Id = id;
            IsFriend = isFriend;
            template = newTemplate;
            CurrentCost = template.Cost;
            Index = 0;
            SpellPower = 0;
            TempAtk = 0;
            TestAllIndexOnPlay = false;
            if (template.Type.Contains("Minion"))
            {
                Type = CType.MINION;
            }
            else if (template.Type.Contains("Spell"))
            {
                Type = CType.SPELL;
            }
            else if (template.Type.Contains("Weapon"))
            {
                Type = CType.WEAPON;
            }
            else if (template.Type.Contains("Hero") && !template.Type.Contains("Power"))
            {
                Type = CType.HERO;
            }
            else if (template.Type.Contains("Hero Power"))
            {
                Type = CType.HERO_POWER;
            }

            if (template.Race == string.Empty)
            {
                Race = Card.CRace.NONE;
            }
            else if (template.Race.Contains("Murloc"))
            {
                Race = Card.CRace.MURLOC;
            }
            else if (template.Race.Contains("Beast"))
            {
                Race = Card.CRace.BEAST;
            }
            else if (template.Race.Contains("Demon"))
            {
                Race = Card.CRace.DEMON;
            }
            else if (template.Race.Contains("Pirate"))
            {
                Race = Card.CRace.PIRATE;
            }
            else if (template.Race.Contains("Totem"))
            {
                Race = Card.CRace.TOTEM;
            }
            else if (template.Race.Contains("Dragon"))
            {
                Race = Card.CRace.DRAGON;
            }

            CurrentAtk = template.Atk;
            CurrentHealth = template.Health;
            MaxHealth = template.Health;
            CurrentDurability = template.Durability;
            CurrentArmor = 0;
            TargetTypeOnPlay = TargetType.NONE;
            IsTaunt = false;
            IsCharge = false;
            IsDestroyed = false;
            IsDivineShield = false;
            IsEnraged = false;
            IsFrozen = false;
            IsSilenced = false;
            IsStealth = false;
            IsTired = false;
            IsWindfury = false;
            IsTargetable = true;
            HasEnrage = false;
            HasFreeze = false;
            HasPoison = false;
            IsImmune = false;
            CountAttack = 0;
            Init();
        }

        public void Silence()
        {
            IsTaunt = false;
            IsDivineShield = false;
            IsWindfury = false;
            IsFrozen = false;
            IsEnraged = false;
            IsCharge = false;
            IsSilenced = true;



            if (maxHealth > template.Health)
            {
                maxHealth = template.Health;
                if (CurrentHealth > maxHealth)
                    CurrentHealth = maxHealth;
            }
            else if (MaxHealth < template.Health)
            {
                maxHealth = template.Health + (CurrentHealth - MaxHealth);
            }


            CurrentAtk = template.Atk;
        }

        public static Card Create(string cardId, bool isFriend, int id, int index = 0)
        {
            CardTemplate template = CardTemplate.LoadFromId(cardId);
            if (template == null)
            {
                HREngine.API.Utilities.HRLog.Write("template null");
                return null;
            }

            Card c = null;

            if (cardId == "EX1_129")
            {
                c = new EX1_129(template, isFriend, id);
            }
            else if (cardId == "EX1_593")
            {
                c = new EX1_593(template, isFriend, id);
            }
            else if (cardId == "HERO_04")
            {
                c = new HERO_04(template, isFriend, id);
            }
            else if (cardId == "HERO_01")
            {
                c = new HERO_01(template, isFriend, id);
            }
            else if (cardId == "CS2_142")
            {
                c = new CS2_142(template, isFriend, id);
            }
            else if (cardId == "CS1_113")
            {
                c = new CS1_113(template, isFriend, id);
            }
            else if (cardId == "EX1_350")
            {
                c = new EX1_350(template, isFriend, id);
            }
            else if (cardId == "EX1_055")
            {
                c = new EX1_055(template, isFriend, id);
            }
            else if (cardId == "EX1_573t")
            {
                c = new EX1_573t(template, isFriend, id);
            }
            else if (cardId == "EX1_349")
            {
                c = new EX1_349(template, isFriend, id);
            }
            else if (cardId == "NEW1_027")
            {
                c = new NEW1_027(template, isFriend, id);
            }
            else if (cardId == "TU4c_003")
            {
                c = new TU4c_003(template, isFriend, id);
            }
            else if (cardId == "CS2_049")
            {
                c = new CS2_049(template, isFriend, id);
            }
            else if (cardId == "EX1_607")
            {
                c = new EX1_607(template, isFriend, id);
            }
            else if (cardId == "EX1_320")
            {
                c = new EX1_320(template, isFriend, id);
            }
            else if (cardId == "EX1_155b")
            {
                c = new EX1_155b(template, isFriend, id);
            }
            else if (cardId == "EX1_165")
            {
                c = new EX1_165(template, isFriend, id);
            }
            else if (cardId == "CS2_236")
            {
                c = new CS2_236(template, isFriend, id);
            }
            else if (cardId == "EX1_621")
            {
                c = new EX1_621(template, isFriend, id);
            }
            else if (cardId == "EX1_622")
            {
                c = new EX1_622(template, isFriend, id);
            }
            else if (cardId == "NEW1_011")
            {
                c = new NEW1_011(template, isFriend, id);
            }
            else if (cardId == "EX1_623")
            {
                c = new EX1_623(template, isFriend, id);
            }
            else if (cardId == "EX1_339")
            {
                c = new EX1_339(template, isFriend, id);
            }
            else if (cardId == "EX1_624")
            {
                c = new EX1_624(template, isFriend, id);
            }
            else if (cardId == "EX1_626")
            {
                c = new EX1_626(template, isFriend, id);
            }
            else if (cardId == "HERO_05")
            {
                c = new HERO_05(template, isFriend, id);
            }
            else if (cardId == "DS1_178")
            {
                c = new DS1_178(template, isFriend, id);
            }
            else if (cardId == "CS2_234")
            {
                c = new CS2_234(template, isFriend, id);
            }
            else if (cardId == "NEW1_010")
            {
                c = new NEW1_010(template, isFriend, id);
            }
            else if (cardId == "EX1_625")
            {
                c = new EX1_625(template, isFriend, id);
            }
            else if (cardId == "EX1_396")
            {
                c = new EX1_396(template, isFriend, id);
            }
            else if (cardId == "CS2_120")
            {
                c = new CS2_120(template, isFriend, id);
            }
            else if (cardId == "EX1_023")
            {
                c = new EX1_023(template, isFriend, id);
            }
            else if (cardId == "CS2_119")
            {
                c = new CS2_119(template, isFriend, id);
            }
            else if (cardId == "EX1_573")
            {
                c = new EX1_573(template, isFriend, id);
            }
            else if (cardId == "NEW1_041")
            {
                c = new NEW1_041(template, isFriend, id);
            }
            else if (cardId == "EX1_076")
            {
                c = new EX1_076(template, isFriend, id);
            }
            else if (cardId == "EX1_620")
            {
                c = new EX1_620(template, isFriend, id);
            }
            else if (cardId == "EX1_137")
            {
                c = new EX1_137(template, isFriend, id);
            }
            else if (cardId == "EX1_363")
            {
                c = new EX1_363(template, isFriend, id);
            }
            else if (cardId == "CS2_121")
            {
                c = new CS2_121(template, isFriend, id);
            }
            else if (cardId == "CS2_075")
            {
                c = new CS2_075(template, isFriend, id);
            }
            else if (cardId == "CS2_186")
            {
                c = new CS2_186(template, isFriend, id);
            }
            else if (cardId == "EX1_390")
            {
                c = new EX1_390(template, isFriend, id);
            }
            else if (cardId == "EX1_616")
            {
                c = new EX1_616(template, isFriend, id);
            }
            else if (cardId == "CS2_065")
            {
                c = new CS2_065(template, isFriend, id);
            }
            else if (cardId == "TU4b_001")
            {
                c = new TU4b_001(template, isFriend, id);
            }
            else if (cardId == "CS2_039")
            {
                c = new CS2_039(template, isFriend, id);
            }
            else if (cardId == "Mekka3")
            {
                c = new Mekka3(template, isFriend, id);
            }
            else if (cardId == "ds1_whelptoken")
            {
                c = new ds1_whelptoken(template, isFriend, id);
            }
            else if (cardId == "CS2_146")
            {
                c = new CS2_146(template, isFriend, id);
            }
            else if (cardId == "CS2_102")
            {
                c = new CS2_102(template, isFriend, id);
            }
            else if (cardId == "EX1_371")
            {
                c = new EX1_371(template, isFriend, id);
            }
            else if (cardId == "EX1_025t")
            {
                c = new EX1_025t(template, isFriend, id);
            }
            else if (cardId == "EX1_160a")
            {
                c = new EX1_160a(template, isFriend, id);
            }
            else if (cardId == "CS2_083b")
            {
                c = new CS2_083b(template, isFriend, id);
            }
            else if (cardId == "CS2_221")
            {
                c = new CS2_221(template, isFriend, id);
            }
            else if (cardId == "EX1_165a")
            {
                c = new EX1_165a(template, isFriend, id);
            }
            else if (cardId == "EX1_085")
            {
                c = new EX1_085(template, isFriend, id);
            }
            else if (cardId == "CS2_012")
            {
                c = new CS2_012(template, isFriend, id);
            }
            else if (cardId == "EX1_062")
            {
                c = new EX1_062(template, isFriend, id);
            }
            else if (cardId == "CS2_122")
            {
                c = new CS2_122(template, isFriend, id);
            }
            else if (cardId == "CS2_173")
            {
                c = new CS2_173(template, isFriend, id);
            }
            else if (cardId == "CS2_127")
            {
                c = new CS2_127(template, isFriend, id);
            }
            else if (cardId == "EX1_028")
            {
                c = new EX1_028(template, isFriend, id);
            }
            else if (cardId == "CS2_011")
            {
                c = new CS2_011(template, isFriend, id);
            }
            else if (cardId == "CS2_151")
            {
                c = new CS2_151(template, isFriend, id);
            }
            else if (cardId == "CS2_087")
            {
                c = new CS2_087(template, isFriend, id);
            }
            else if (cardId == "EX1_612")
            {
                c = new EX1_612(template, isFriend, id);
            }
            else if (cardId == "EX1_012")
            {
                c = new EX1_012(template, isFriend, id);
            }
            else if (cardId == "EX1_407")
            {
                c = new EX1_407(template, isFriend, id);
            }
            else if (cardId == "EX1_598")
            {
                c = new EX1_598(template, isFriend, id);
            }
            else if (cardId == "CS2_022")
            {
                c = new CS2_022(template, isFriend, id);
            }
            else if (cardId == "CS2_222")
            {
                c = new CS2_222(template, isFriend, id);
            }
            else if (cardId == "EX1_048")
            {
                c = new EX1_048(template, isFriend, id);
            }
            else if (cardId == "EX1_590")
            {
                c = new EX1_590(template, isFriend, id);
            }
            else if (cardId == "EX1_619")
            {
                c = new EX1_619(template, isFriend, id);
            }
            else if (cardId == "EX1_008")
            {
                c = new EX1_008(template, isFriend, id);
            }
            else if (cardId == "NEW1_007")
            {
                c = new NEW1_007(template, isFriend, id);
            }
            else if (cardId == "EX1_032")
            {
                c = new EX1_032(template, isFriend, id);
            }
            else if (cardId == "CS2_182")
            {
                c = new CS2_182(template, isFriend, id);
            }
            else if (cardId == "EX1_082")
            {
                c = new EX1_082(template, isFriend, id);
            }
            else if (cardId == "EX1_093")
            {
                c = new EX1_093(template, isFriend, id);
            }
            else if (cardId == "EX1_164")
            {
                c = new EX1_164(template, isFriend, id);
            }
            else if (cardId == "NEW1_009")
            {
                c = new NEW1_009(template, isFriend, id);
            }
            else if (cardId == "EX1_021")
            {
                c = new EX1_021(template, isFriend, id);
            }
            else if (cardId == "EX1_246")
            {
                c = new EX1_246(template, isFriend, id);
            }
            else if (cardId == "EX1_165b")
            {
                c = new EX1_165b(template, isFriend, id);
            }
            else if (cardId == "EX1_245")
            {
                c = new EX1_245(template, isFriend, id);
            }
            else if (cardId == "NEW1_034")
            {
                c = new NEW1_034(template, isFriend, id);
            }
            else if (cardId == "EX1_399")
            {
                c = new EX1_399(template, isFriend, id);
            }
            else if (cardId == "EX1_130a")
            {
                c = new EX1_130a(template, isFriend, id);
            }
            else if (cardId == "CS2_007")
            {
                c = new CS2_007(template, isFriend, id);
            }
            else if (cardId == "EX1_258")
            {
                c = new EX1_258(template, isFriend, id);
            }
            else if (cardId == "EX1_323")
            {
                c = new EX1_323(template, isFriend, id);
            }
            else if (cardId == "EX1_556")
            {
                c = new EX1_556(template, isFriend, id);
            }
            else if (cardId == "EX1_287")
            {
                c = new EX1_287(template, isFriend, id);
            }
            else if (cardId == "EX1_313")
            {
                c = new EX1_313(template, isFriend, id);
            }
            else if (cardId == "TU4c_002")
            {
                c = new TU4c_002(template, isFriend, id);
            }
            else if (cardId == "CS2_108")
            {
                c = new CS2_108(template, isFriend, id);
            }
            else if (cardId == "EX1_393")
            {
                c = new EX1_393(template, isFriend, id);
            }
            else if (cardId == "EX1_011")
            {
                c = new EX1_011(template, isFriend, id);
            }
            else if (cardId == "EX1_044")
            {
                c = new EX1_044(template, isFriend, id);
            }
            else if (cardId == "CS2_161")
            {
                c = new CS2_161(template, isFriend, id);
            }
            else if (cardId == "CS2_tk1")
            {
                c = new CS2_tk1(template, isFriend, id);
            }
            else if (cardId == "EX1_341")
            {
                c = new EX1_341(template, isFriend, id);
            }
            else if (cardId == "EX1_154b")
            {
                c = new EX1_154b(template, isFriend, id);
            }
            else if (cardId == "NEW1_021")
            {
                c = new NEW1_021(template, isFriend, id);
            }
            else if (cardId == "EX1_136")
            {
                c = new EX1_136(template, isFriend, id);
            }
            else if (cardId == "EX1_059")
            {
                c = new EX1_059(template, isFriend, id);
            }
            else if (cardId == "CS2_084")
            {
                c = new CS2_084(template, isFriend, id);
            }
            else if (cardId == "TU4c_001")
            {
                c = new TU4c_001(template, isFriend, id);
            }
            else if (cardId == "EX1_408")
            {
                c = new EX1_408(template, isFriend, id);
            }
            else if (cardId == "EX1_345")
            {
                c = new EX1_345(template, isFriend, id);
            }
            else if (cardId == "CS2_034")
            {
                c = new CS2_034(template, isFriend, id);
            }
            else if (cardId == "Mekka4")
            {
                c = new Mekka4(template, isFriend, id);
            }
            else if (cardId == "DREAM_04")
            {
                c = new DREAM_04(template, isFriend, id);
            }
            else if (cardId == "EX1_303")
            {
                c = new EX1_303(template, isFriend, id);
            }
            else if (cardId == "CS2_041")
            {
                c = new CS2_041(template, isFriend, id);
            }
            else if (cardId == "EX1_411")
            {
                c = new EX1_411(template, isFriend, id);
            }
            else if (cardId == "EX1_155")
            {
                c = new EX1_155(template, isFriend, id);
            }
            else if (cardId == "EX1_595")
            {
                c = new EX1_595(template, isFriend, id);
            }
            else if (cardId == "EX1_160t")
            {
                c = new EX1_160t(template, isFriend, id);
            }
            else if (cardId == "HERO_09")
            {
                c = new HERO_09(template, isFriend, id);
            }
            else if (cardId == "EX1_609")
            {
                c = new EX1_609(template, isFriend, id);
            }
            else if (cardId == "CS2_162")
            {
                c = new CS2_162(template, isFriend, id);
            }
            else if (cardId == "CS2_053")
            {
                c = new CS2_053(template, isFriend, id);
            }
            else if (cardId == "EX1_080")
            {
                c = new EX1_080(template, isFriend, id);
            }
            else if (cardId == "TU4c_007")
            {
                c = new TU4c_007(template, isFriend, id);
            }
            else if (cardId == "EX1_173")
            {
                c = new EX1_173(template, isFriend, id);
            }
            else if (cardId == "NEW1_003")
            {
                c = new NEW1_003(template, isFriend, id);
            }
            else if (cardId == "EX1_284")
            {
                c = new EX1_284(template, isFriend, id);
            }
            else if (cardId == "EX1_244")
            {
                c = new EX1_244(template, isFriend, id);
            }
            else if (cardId == "CS2_031")
            {
                c = new CS2_031(template, isFriend, id);
            }
            else if (cardId == "NEW1_030")
            {
                c = new NEW1_030(template, isFriend, id);
            }
            else if (cardId == "EX1_160b")
            {
                c = new EX1_160b(template, isFriend, id);
            }
            else if (cardId == "EX1_582")
            {
                c = new EX1_582(template, isFriend, id);
            }
            else if (cardId == "EX1_154")
            {
                c = new EX1_154(template, isFriend, id);
            }
            else if (cardId == "EX1_587")
            {
                c = new EX1_587(template, isFriend, id);
            }
            else if (cardId == "CS2_231")
            {
                c = new CS2_231(template, isFriend, id);
            }
            else if (cardId == "EX1_355")
            {
                c = new EX1_355(template, isFriend, id);
            }
            else if (cardId == "CS2_072")
            {
                c = new CS2_072(template, isFriend, id);
            }
            else if (cardId == "CS1_112")
            {
                c = new CS1_112(template, isFriend, id);
            }
            else if (cardId == "EX1_178b")
            {
                c = new EX1_178b(template, isFriend, id);
            }
            else if (cardId == "EX1_316")
            {
                c = new EX1_316(template, isFriend, id);
            }
            else if (cardId == "EX1_057")
            {
                c = new EX1_057(template, isFriend, id);
            }
            else if (cardId == "CS2_097")
            {
                c = new CS2_097(template, isFriend, id);
            }
            else if (cardId == "CS2_042")
            {
                c = new CS2_042(template, isFriend, id);
            }
            else if (cardId == "CS2_051")
            {
                c = new CS2_051(template, isFriend, id);
            }
            else if (cardId == "CS2_168")
            {
                c = new CS2_168(template, isFriend, id);
            }
            else if (cardId == "EX1_295")
            {
                c = new EX1_295(template, isFriend, id);
            }
            else if (cardId == "EX1_360")
            {
                c = new EX1_360(template, isFriend, id);
            }
            else if (cardId == "NEW1_029")
            {
                c = new NEW1_029(template, isFriend, id);
            }
            else if (cardId == "EX1_294")
            {
                c = new EX1_294(template, isFriend, id);
            }
            else if (cardId == "NEW1_004")
            {
                c = new NEW1_004(template, isFriend, id);
            }
            else if (cardId == "EX1_112")
            {
                c = new EX1_112(template, isFriend, id);
            }
            else if (cardId == "TU4c_008")
            {
                c = new TU4c_008(template, isFriend, id);
            }
            else if (cardId == "EX1_312")
            {
                c = new EX1_312(template, isFriend, id);
            }
            else if (cardId == "EX1_317")
            {
                c = new EX1_317(template, isFriend, id);
            }
            else if (cardId == "EX1_131")
            {
                c = new EX1_131(template, isFriend, id);
            }
            else if (cardId == "EX1_554t")
            {
                c = new EX1_554t(template, isFriend, id);
            }
            else if (cardId == "CS2_232")
            {
                c = new CS2_232(template, isFriend, id);
            }
            else if (cardId == "EX1_241")
            {
                c = new EX1_241(template, isFriend, id);
            }
            else if (cardId == "EX1_405")
            {
                c = new EX1_405(template, isFriend, id);
            }
            else if (cardId == "NEW1_008b")
            {
                c = new NEW1_008b(template, isFriend, id);
            }
            else if (cardId == "EX1_586")
            {
                c = new EX1_586(template, isFriend, id);
            }
            else if (cardId == "CS2_009")
            {
                c = new CS2_009(template, isFriend, id);
            }
            else if (cardId == "CS2_172")
            {
                c = new CS2_172(template, isFriend, id);
            }
            else if (cardId == "DREAM_05")
            {
                c = new DREAM_05(template, isFriend, id);
            }
            else if (cardId == "DS1_185")
            {
                c = new DS1_185(template, isFriend, id);
            }
            else if (cardId == "EX1_334")
            {
                c = new EX1_334(template, isFriend, id);
            }
            else if (cardId == "NEW1_022")
            {
                c = new NEW1_022(template, isFriend, id);
            }
            else if (cardId == "NEW1_033")
            {
                c = new NEW1_033(template, isFriend, id);
            }
            else if (cardId == "Mekka1")
            {
                c = new Mekka1(template, isFriend, id);
            }
            else if (cardId == "EX1_335")
            {
                c = new EX1_335(template, isFriend, id);
            }
            else if (cardId == "NEW1_014")
            {
                c = new NEW1_014(template, isFriend, id);
            }
            else if (cardId == "DS1h_292")
            {
                c = new DS1h_292(template, isFriend, id);
            }
            else if (cardId == "EX1_383")
            {
                c = new EX1_383(template, isFriend, id);
            }
            else if (cardId == "EX1_379")
            {
                c = new EX1_379(template, isFriend, id);
            }
            else if (cardId == "EX1_058")
            {
                c = new EX1_058(template, isFriend, id);
            }
            else if (cardId == "EX1_161")
            {
                c = new EX1_161(template, isFriend, id);
            }
            else if (cardId == "HERO_07")
            {
                c = new HERO_07(template, isFriend, id);
            }
            else if (cardId == "EX1_591")
            {
                c = new EX1_591(template, isFriend, id);
            }
            else if (cardId == "EX1_544")
            {
                c = new EX1_544(template, isFriend, id);
            }
            else if (cardId == "EX1_248")
            {
                c = new EX1_248(template, isFriend, id);
            }
            else if (cardId == "CS2_045")
            {
                c = new CS2_045(template, isFriend, id);
            }
            else if (cardId == "CS2_188")
            {
                c = new CS2_188(template, isFriend, id);
            }
            else if (cardId == "EX1_549")
            {
                c = new EX1_549(template, isFriend, id);
            }
            else if (cardId == "EX1_124")
            {
                c = new EX1_124(template, isFriend, id);
            }
            else if (cardId == "CS2_094")
            {
                c = new CS2_094(template, isFriend, id);
            }
            else if (cardId == "EX1_096")
            {
                c = new EX1_096(template, isFriend, id);
            }
            else if (cardId == "EX1_066")
            {
                c = new EX1_066(template, isFriend, id);
            }
            else if (cardId == "EX1_154a")
            {
                c = new EX1_154a(template, isFriend, id);
            }
            else if (cardId == "EX1_169")
            {
                c = new EX1_169(template, isFriend, id);
            }
            else if (cardId == "CS2_196")
            {
                c = new CS2_196(template, isFriend, id);
            }
            else if (cardId == "EX1_558")
            {
                c = new EX1_558(template, isFriend, id);
            }
            else if (cardId == "CS2_057")
            {
                c = new CS2_057(template, isFriend, id);
            }
            else if (cardId == "Mekka4t")
            {
                c = new Mekka4t(template, isFriend, id);
            }
            else if (cardId == "EX1_584")
            {
                c = new EX1_584(template, isFriend, id);
            }
            else if (cardId == "TU4d_001")
            {
                c = new TU4d_001(template, isFriend, id);
            }
            else if (cardId == "TU4d_002")
            {
                c = new TU4d_002(template, isFriend, id);
            }
            else if (cardId == "CS2_061")
            {
                c = new CS2_061(template, isFriend, id);
            }
            else if (cardId == "NEW1_008")
            {
                c = new NEW1_008(template, isFriend, id);
            }
            else if (cardId == "EX1_017")
            {
                c = new EX1_017(template, isFriend, id);
            }
            else if (cardId == "CS2_073")
            {
                c = new CS2_073(template, isFriend, id);
            }
            else if (cardId == "CS1_042")
            {
                c = new CS1_042(template, isFriend, id);
            }
            else if (cardId == "EX1_091")
            {
                c = new EX1_091(template, isFriend, id);
            }
            else if (cardId == "EX1_597")
            {
                c = new EX1_597(template, isFriend, id);
            }
            else if (cardId == "HERO_06")
            {
                c = new HERO_06(template, isFriend, id);
            }
            else if (cardId == "NEW1_007b")
            {
                c = new NEW1_007b(template, isFriend, id);
            }
            else if (cardId == "HERO_03")
            {
                c = new HERO_03(template, isFriend, id);
            }
            else if (cardId == "EX1_095")
            {
                c = new EX1_095(template, isFriend, id);
            }
            else if (cardId == "CS1_130")
            {
                c = new CS1_130(template, isFriend, id);
            }
            else if (cardId == "EX1_067")
            {
                c = new EX1_067(template, isFriend, id);
            }
            else if (cardId == "EX1_625t")
            {
                c = new EX1_625t(template, isFriend, id);
            }
            else if (cardId == "EX1_625t2")
            {
                c = new EX1_625t2(template, isFriend, id);
            }
            else if (cardId == "EX1_126")
            {
                c = new EX1_126(template, isFriend, id);
            }
            else if (cardId == "EX1_534t")
            {
                c = new EX1_534t(template, isFriend, id);
            }
            else if (cardId == "EX1_015")
            {
                c = new EX1_015(template, isFriend, id);
            }
            else if (cardId == "CS2_114")
            {
                c = new CS2_114(template, isFriend, id);
            }
            else if (cardId == "EX1_603")
            {
                c = new EX1_603(template, isFriend, id);
            }
            else if (cardId == "EX1_594")
            {
                c = new EX1_594(template, isFriend, id);
            }
            else if (cardId == "NEW1_005")
            {
                c = new NEW1_005(template, isFriend, id);
            }
            else if (cardId == "CS2_092")
            {
                c = new CS2_092(template, isFriend, id);
            }
            else if (cardId == "CS2_124")
            {
                c = new CS2_124(template, isFriend, id);
            }
            else if (cardId == "CS2_203")
            {
                c = new CS2_203(template, isFriend, id);
            }
            else if (cardId == "CS2_089")
            {
                c = new CS2_089(template, isFriend, id);
            }
            else if (cardId == "EX1_004")
            {
                c = new EX1_004(template, isFriend, id);
            }
            else if (cardId == "DS1_183")
            {
                c = new DS1_183(template, isFriend, id);
            }
            else if (cardId == "TU4e_001")
            {
                c = new TU4e_001(template, isFriend, id);
            }
            else if (cardId == "CS2_062")
            {
                c = new CS2_062(template, isFriend, id);
            }
            else if (cardId == "TU4e_002")
            {
                c = new TU4e_002(template, isFriend, id);
            }
            else if (cardId == "TU4e_003")
            {
                c = new TU4e_003(template, isFriend, id);
            }
            else if (cardId == "EX1_539")
            {
                c = new EX1_539(template, isFriend, id);
            }
            else if (cardId == "TU4e_004")
            {
                c = new TU4e_004(template, isFriend, id);
            }
            else if (cardId == "TU4e_005")
            {
                c = new TU4e_005(template, isFriend, id);
            }
            else if (cardId == "CS2_boar")
            {
                c = new CS2_boar(template, isFriend, id);
            }
            else if (cardId == "TU4e_006")
            {
                c = new TU4e_006(template, isFriend, id);
            }
            else if (cardId == "EX1_251")
            {
                c = new EX1_251(template, isFriend, id);
            }
            else if (cardId == "TU4e_007")
            {
                c = new TU4e_007(template, isFriend, id);
            }
            else if (cardId == "CS2_056")
            {
                c = new CS2_056(template, isFriend, id);
            }
            else if (cardId == "TU4e_002t")
            {
                c = new TU4e_002t(template, isFriend, id);
            }
            else if (cardId == "DREAM_02")
            {
                c = new DREAM_02(template, isFriend, id);
            }
            else if (cardId == "NEW1_032")
            {
                c = new NEW1_032(template, isFriend, id);
            }
            else if (cardId == "EX1_247")
            {
                c = new EX1_247(template, isFriend, id);
            }
            else if (cardId == "CS2_112")
            {
                c = new CS2_112(template, isFriend, id);
            }
            else if (cardId == "EX1_577")
            {
                c = new EX1_577(template, isFriend, id);
            }
            else if (cardId == "EX1_613")
            {
                c = new EX1_613(template, isFriend, id);
            }
            else if (cardId == "CS2_235")
            {
                c = new CS2_235(template, isFriend, id);
            }
            else if (cardId == "CS2_117")
            {
                c = new CS2_117(template, isFriend, id);
            }
            else if (cardId == "CS2_147")
            {
                c = new CS2_147(template, isFriend, id);
            }
            else if (cardId == "CS2_101t")
            {
                c = new CS2_101t(template, isFriend, id);
            }
            else if (cardId == "CS2_mirror")
            {
                c = new CS2_mirror(template, isFriend, id);
            }
            else if (cardId == "CS2_118")
            {
                c = new CS2_118(template, isFriend, id);
            }
            else if (cardId == "EX1_315")
            {
                c = new EX1_315(template, isFriend, id);
            }
            else if (cardId == "DS1_188")
            {
                c = new DS1_188(template, isFriend, id);
            }
            else if (cardId == "EX1_001")
            {
                c = new EX1_001(template, isFriend, id);
            }
            else if (cardId == "CS2_037")
            {
                c = new CS2_037(template, isFriend, id);
            }
            else if (cardId == "NEW1_008a")
            {
                c = new NEW1_008a(template, isFriend, id);
            }
            else if (cardId == "EX1_002")
            {
                c = new EX1_002(template, isFriend, id);
            }
            else if (cardId == "EX1_005")
            {
                c = new EX1_005(template, isFriend, id);
            }
            else if (cardId == "CS2_029")
            {
                c = new CS2_029(template, isFriend, id);
            }
            else if (cardId == "EX1_308")
            {
                c = new EX1_308(template, isFriend, id);
            }
            else if (cardId == "TU4c_006")
            {
                c = new TU4c_006(template, isFriend, id);
            }
            else if (cardId == "EX1_506")
            {
                c = new EX1_506(template, isFriend, id);
            }
            else if (cardId == "EX1_006")
            {
                c = new EX1_006(template, isFriend, id);
            }
            else if (cardId == "EX1_110t")
            {
                c = new EX1_110t(template, isFriend, id);
            }
            else if (cardId == "EX1_007")
            {
                c = new EX1_007(template, isFriend, id);
            }
            else if (cardId == "EX1_323w")
            {
                c = new EX1_323w(template, isFriend, id);
            }
            else if (cardId == "EX1_102")
            {
                c = new EX1_102(template, isFriend, id);
            }
            else if (cardId == "EX1_166b")
            {
                c = new EX1_166b(template, isFriend, id);
            }
            else if (cardId == "EX1_409t")
            {
                c = new EX1_409t(template, isFriend, id);
            }
            else if (cardId == "EX1_536")
            {
                c = new EX1_536(template, isFriend, id);
            }
            else if (cardId == "CS2_063")
            {
                c = new CS2_063(template, isFriend, id);
            }
            else if (cardId == "EX1_164b")
            {
                c = new EX1_164b(template, isFriend, id);
            }
            else if (cardId == "EX1_162")
            {
                c = new EX1_162(template, isFriend, id);
            }
            else if (cardId == "TU4f_001")
            {
                c = new TU4f_001(template, isFriend, id);
            }
            else if (cardId == "EX1_166a")
            {
                c = new EX1_166a(template, isFriend, id);
            }
            else if (cardId == "TU4f_002")
            {
                c = new TU4f_002(template, isFriend, id);
            }
            else if (cardId == "TU4c_004")
            {
                c = new TU4c_004(template, isFriend, id);
            }
            else if (cardId == "TU4f_003")
            {
                c = new TU4f_003(template, isFriend, id);
            }
            else if (cardId == "EX1_128")
            {
                c = new EX1_128(template, isFriend, id);
            }
            else if (cardId == "Mekka2")
            {
                c = new Mekka2(template, isFriend, id);
            }
            else if (cardId == "TU4f_004")
            {
                c = new TU4f_004(template, isFriend, id);
            }
            else if (cardId == "TU4f_005")
            {
                c = new TU4f_005(template, isFriend, id);
            }
            else if (cardId == "EX1_105")
            {
                c = new EX1_105(template, isFriend, id);
            }
            else if (cardId == "TU4f_006")
            {
                c = new TU4f_006(template, isFriend, id);
            }
            else if (cardId == "EX1_tk29")
            {
                c = new EX1_tk29(template, isFriend, id);
            }
            else if (cardId == "EX1_010")
            {
                c = new EX1_010(template, isFriend, id);
            }
            else if (cardId == "CS2_197")
            {
                c = new CS2_197(template, isFriend, id);
            }
            else if (cardId == "TU4f_007")
            {
                c = new TU4f_007(template, isFriend, id);
            }
            else if (cardId == "NEW1_025")
            {
                c = new NEW1_025(template, isFriend, id);
            }
            else if (cardId == "EX1_249")
            {
                c = new EX1_249(template, isFriend, id);
            }
            else if (cardId == "EX1_165t1")
            {
                c = new EX1_165t1(template, isFriend, id);
            }
            else if (cardId == "NEW1_018")
            {
                c = new NEW1_018(template, isFriend, id);
            }
            else if (cardId == "EX1_165t2")
            {
                c = new EX1_165t2(template, isFriend, id);
            }
            else if (cardId == "EX1_414")
            {
                c = new EX1_414(template, isFriend, id);
            }
            else if (cardId == "TU4d_003")
            {
                c = new TU4d_003(template, isFriend, id);
            }
            else if (cardId == "CS2_141")
            {
                c = new CS2_141(template, isFriend, id);
            }
            else if (cardId == "DREAM_01")
            {
                c = new DREAM_01(template, isFriend, id);
            }
            else if (cardId == "CS2_200")
            {
                c = new CS2_200(template, isFriend, id);
            }
            else if (cardId == "DS1_070")
            {
                c = new DS1_070(template, isFriend, id);
            }
            else if (cardId == "CS2_032")
            {
                c = new CS2_032(template, isFriend, id);
            }
            else if (cardId == "CS2_201")
            {
                c = new CS2_201(template, isFriend, id);
            }
            else if (cardId == "EX1_009")
            {
                c = new EX1_009(template, isFriend, id);
            }
            else if (cardId == "CS2_103")
            {
                c = new CS2_103(template, isFriend, id);
            }
            else if (cardId == "EX1_finkle")
            {
                c = new EX1_finkle(template, isFriend, id);
            }
            else if (cardId == "CS2_076")
            {
                c = new CS2_076(template, isFriend, id);
            }
            else if (cardId == "CS2_105")
            {
                c = new CS2_105(template, isFriend, id);
            }
            else if (cardId == "EX1_565")
            {
                c = new EX1_565(template, isFriend, id);
            }
            else if (cardId == "EX1_046")
            {
                c = new EX1_046(template, isFriend, id);
            }
            else if (cardId == "EX1_084")
            {
                c = new EX1_084(template, isFriend, id);
            }
            else if (cardId == "EX1_014")
            {
                c = new EX1_014(template, isFriend, id);
            }
            else if (cardId == "EX1_014t")
            {
                c = new EX1_014t(template, isFriend, id);
            }
            else if (cardId == "NEW1_037")
            {
                c = new NEW1_037(template, isFriend, id);
            }
            else if (cardId == "NEW1_020")
            {
                c = new NEW1_020(template, isFriend, id);
            }
            else if (cardId == "EX1_567")
            {
                c = new EX1_567(template, isFriend, id);
            }
            else if (cardId == "EX1_050")
            {
                c = new EX1_050(template, isFriend, id);
            }
            else if (cardId == "CS2_064")
            {
                c = new CS2_064(template, isFriend, id);
            }
            else if (cardId == "EX1_tk9")
            {
                c = new EX1_tk9(template, isFriend, id);
            }
            else if (cardId == "EX1_398t")
            {
                c = new EX1_398t(template, isFriend, id);
            }
            else if (cardId == "EX1_362")
            {
                c = new EX1_362(template, isFriend, id);
            }
            else if (cardId == "EX1_606")
            {
                c = new EX1_606(template, isFriend, id);
            }
            else if (cardId == "EX1_562")
            {
                c = new EX1_562(template, isFriend, id);
            }
            else if (cardId == "EX1_573b")
            {
                c = new EX1_573b(template, isFriend, id);
            }
            else if (cardId == "NEW1_036")
            {
                c = new NEW1_036(template, isFriend, id);
            }
            else if (cardId == "EX1_144")
            {
                c = new EX1_144(template, isFriend, id);
            }
            else if (cardId == "tt_010")
            {
                c = new tt_010(template, isFriend, id);
            }
            else if (cardId == "EX1_538t")
            {
                c = new EX1_538t(template, isFriend, id);
            }
            else if (cardId == "NEW1_026")
            {
                c = new NEW1_026(template, isFriend, id);
            }
            else if (cardId == "EX1_345t")
            {
                c = new EX1_345t(template, isFriend, id);
            }
            else if (cardId == "EX1_016")
            {
                c = new EX1_016(template, isFriend, id);
            }
            else if (cardId == "EX1_298")
            {
                c = new EX1_298(template, isFriend, id);
            }
            else if (cardId == "EX1_178")
            {
                c = new EX1_178(template, isFriend, id);
            }
            else if (cardId == "EX1_317t")
            {
                c = new EX1_317t(template, isFriend, id);
            }
            else if (cardId == "EX1_043")
            {
                c = new EX1_043(template, isFriend, id);
            }
            else if (cardId == "CS1_129")
            {
                c = new CS1_129(template, isFriend, id);
            }
            else if (cardId == "CS2_013t")
            {
                c = new CS2_013t(template, isFriend, id);
            }
            else if (cardId == "NEW1_026t")
            {
                c = new NEW1_026t(template, isFriend, id);
            }
            else if (cardId == "EX1_158")
            {
                c = new EX1_158(template, isFriend, id);
            }
            else if (cardId == "EX1_383t")
            {
                c = new EX1_383t(template, isFriend, id);
            }
            else if (cardId == "CS2_091")
            {
                c = new CS2_091(template, isFriend, id);
            }
            else if (cardId == "GAME_002")
            {
                c = new GAME_002(template, isFriend, id);
            }
            else if (cardId == "DS1_184")
            {
                c = new DS1_184(template, isFriend, id);
            }
            else if (cardId == "CS2_005")
            {
                c = new CS2_005(template, isFriend, id);
            }
            else if (cardId == "EX1_274")
            {
                c = new EX1_274(template, isFriend, id);
            }
            else if (cardId == "CS2_189")
            {
                c = new CS2_189(template, isFriend, id);
            }
            else if (cardId == "EX1_133")
            {
                c = new EX1_133(template, isFriend, id);
            }
            else if (cardId == "EX1_537")
            {
                c = new EX1_537(template, isFriend, id);
            }
            else if (cardId == "CS2_033")
            {
                c = new CS2_033(template, isFriend, id);
            }
            else if (cardId == "GAME_005")
            {
                c = new GAME_005(template, isFriend, id);
            }
            else if (cardId == "tt_004")
            {
                c = new tt_004(template, isFriend, id);
            }
            else if (cardId == "GAME_006")
            {
                c = new GAME_006(template, isFriend, id);
            }
            else if (cardId == "EX1_507")
            {
                c = new EX1_507(template, isFriend, id);
            }
            else if (cardId == "EX1_392")
            {
                c = new EX1_392(template, isFriend, id);
            }
            else if (cardId == "CS2_233")
            {
                c = new CS2_233(template, isFriend, id);
            }
            else if (cardId == "EX1_614t")
            {
                c = new EX1_614t(template, isFriend, id);
            }
            else if (cardId == "CS2_106")
            {
                c = new CS2_106(template, isFriend, id);
            }
            else if (cardId == "HERO_02")
            {
                c = new HERO_02(template, isFriend, id);
            }
            else if (cardId == "PRO_001")
            {
                c = new PRO_001(template, isFriend, id);
            }
            else if (cardId == "CS2_088")
            {
                c = new CS2_088(template, isFriend, id);
            }
            else if (cardId == "CS2_038")
            {
                c = new CS2_038(template, isFriend, id);
            }
            else if (cardId == "NEW1_012")
            {
                c = new NEW1_012(template, isFriend, id);
            }
            else if (cardId == "NEW1_019")
            {
                c = new NEW1_019(template, isFriend, id);
            }
            else if (cardId == "EX1_391")
            {
                c = new EX1_391(template, isFriend, id);
            }
            else if (cardId == "EX1_560")
            {
                c = new EX1_560(template, isFriend, id);
            }
            else if (cardId == "skele11")
            {
                c = new skele11(template, isFriend, id);
            }
            else if (cardId == "CS2_150")
            {
                c = new CS2_150(template, isFriend, id);
            }
            else if (cardId == "EX1_506a")
            {
                c = new EX1_506a(template, isFriend, id);
            }
            else if (cardId == "EX1_049")
            {
                c = new EX1_049(template, isFriend, id);
            }
            else if (cardId == "EX1_559")
            {
                c = new EX1_559(template, isFriend, id);
            }
            else if (cardId == "EX1_110")
            {
                c = new EX1_110(template, isFriend, id);
            }
            else if (cardId == "CS2_027")
            {
                c = new CS2_027(template, isFriend, id);
            }
            else if (cardId == "CS2_080")
            {
                c = new CS2_080(template, isFriend, id);
            }
            else if (cardId == "tt_010a")
            {
                c = new tt_010a(template, isFriend, id);
            }
            else if (cardId == "EX1_279")
            {
                c = new EX1_279(template, isFriend, id);
            }
            else if (cardId == "EX1_583")
            {
                c = new EX1_583(template, isFriend, id);
            }
            else if (cardId == "EX1_319")
            {
                c = new EX1_319(template, isFriend, id);
            }
            else if (cardId == "EX1_533")
            {
                c = new EX1_533(template, isFriend, id);
            }
            else if (cardId == "EX1_302")
            {
                c = new EX1_302(template, isFriend, id);
            }
            else if (cardId == "EX1_617")
            {
                c = new EX1_617(template, isFriend, id);
            }
            else if (cardId == "EX1_275")
            {
                c = new EX1_275(template, isFriend, id);
            }
            else if (cardId == "EX1_170")
            {
                c = new EX1_170(template, isFriend, id);
            }
            else if (cardId == "EX1_365")
            {
                c = new EX1_365(template, isFriend, id);
            }
            else if (cardId == "CS2_003")
            {
                c = new CS2_003(template, isFriend, id);
            }
            else if (cardId == "EX1_563")
            {
                c = new EX1_563(template, isFriend, id);
            }
            else if (cardId == "EX1_309")
            {
                c = new EX1_309(template, isFriend, id);
            }
            else if (cardId == "NEW1_031")
            {
                c = new NEW1_031(template, isFriend, id);
            }
            else if (cardId == "EX1_097")
            {
                c = new EX1_097(template, isFriend, id);
            }
            else if (cardId == "NEW1_017")
            {
                c = new NEW1_017(template, isFriend, id);
            }
            else if (cardId == "CS2_104")
            {
                c = new CS2_104(template, isFriend, id);
            }
            else if (cardId == "CS2_213")
            {
                c = new CS2_213(template, isFriend, id);
            }
            else if (cardId == "CS2_025")
            {
                c = new CS2_025(template, isFriend, id);
            }
            else if (cardId == "CS2_181")
            {
                c = new CS2_181(template, isFriend, id);
            }
            else if (cardId == "EX1_164a")
            {
                c = new EX1_164a(template, isFriend, id);
            }
            else if (cardId == "EX1_134")
            {
                c = new EX1_134(template, isFriend, id);
            }
            else if (cardId == "EX1_103")
            {
                c = new EX1_103(template, isFriend, id);
            }
            else if (cardId == "EX1_554")
            {
                c = new EX1_554(template, isFriend, id);
            }
            else if (cardId == "NEW1_024")
            {
                c = new NEW1_024(template, isFriend, id);
            }
            else if (cardId == "CS2_028")
            {
                c = new CS2_028(template, isFriend, id);
            }
            else if (cardId == "CS2_227")
            {
                c = new CS2_227(template, isFriend, id);
            }
            else if (cardId == "CS2_017")
            {
                c = new CS2_017(template, isFriend, id);
            }
            else if (cardId == "CS2_052")
            {
                c = new CS2_052(template, isFriend, id);
            }
            else if (cardId == "CS2_013")
            {
                c = new CS2_013(template, isFriend, id);
            }
            else if (cardId == "CS2_074")
            {
                c = new CS2_074(template, isFriend, id);
            }
            else if (cardId == "NEW1_040t")
            {
                c = new NEW1_040t(template, isFriend, id);
            }
            else if (cardId == "EX1_581")
            {
                c = new EX1_581(template, isFriend, id);
            }
            else if (cardId == "EX1_132")
            {
                c = new EX1_132(template, isFriend, id);
            }
            else if (cardId == "EX1_089")
            {
                c = new EX1_089(template, isFriend, id);
            }
            else if (cardId == "EX1_522")
            {
                c = new EX1_522(template, isFriend, id);
            }
            else if (cardId == "CS2_008")
            {
                c = new CS2_008(template, isFriend, id);
            }
            else if (cardId == "EX1_155a")
            {
                c = new EX1_155a(template, isFriend, id);
            }
            else if (cardId == "EX1_100")
            {
                c = new EX1_100(template, isFriend, id);
            }
            else if (cardId == "CS2_059")
            {
                c = new CS2_059(template, isFriend, id);
            }
            else if (cardId == "skele21")
            {
                c = new skele21(template, isFriend, id);
            }
            else if (cardId == "CS2_101")
            {
                c = new CS2_101(template, isFriend, id);
            }
            else if (cardId == "CS2_187")
            {
                c = new CS2_187(template, isFriend, id);
            }
            else if (cardId == "EX1_250")
            {
                c = new EX1_250(template, isFriend, id);
            }
            else if (cardId == "EX1_596")
            {
                c = new EX1_596(template, isFriend, id);
            }
            else if (cardId == "EX1_509")
            {
                c = new EX1_509(template, isFriend, id);
            }
            else if (cardId == "EX1_tk34")
            {
                c = new EX1_tk34(template, isFriend, id);
            }
            else if (cardId == "EX1_543")
            {
                c = new EX1_543(template, isFriend, id);
            }
            else if (cardId == "CS2_093")
            {
                c = new CS2_093(template, isFriend, id);
            }
            else if (cardId == "EX1_573a")
            {
                c = new EX1_573a(template, isFriend, id);
            }
            else if (cardId == "CS1h_001")
            {
                c = new CS1h_001(template, isFriend, id);
            }
            else if (cardId == "EX1_557")
            {
                c = new EX1_557(template, isFriend, id);
            }
            else if (cardId == "EX1_578")
            {
                c = new EX1_578(template, isFriend, id);
            }
            else if (cardId == "CS2_152")
            {
                c = new CS2_152(template, isFriend, id);
            }
            else if (cardId == "CS2_082")
            {
                c = new CS2_082(template, isFriend, id);
            }
            else if (cardId == "EX1_131t")
            {
                c = new EX1_131t(template, isFriend, id);
            }
            else if (cardId == "PRO_001a")
            {
                c = new PRO_001a(template, isFriend, id);
            }
            else if (cardId == "DREAM_03")
            {
                c = new DREAM_03(template, isFriend, id);
            }
            else if (cardId == "PRO_001at")
            {
                c = new PRO_001at(template, isFriend, id);
            }
            else if (cardId == "TU4c_005")
            {
                c = new TU4c_005(template, isFriend, id);
            }
            else if (cardId == "PRO_001b")
            {
                c = new PRO_001b(template, isFriend, id);
            }
            else if (cardId == "PRO_001c")
            {
                c = new PRO_001c(template, isFriend, id);
            }
            else if (cardId == "EX1_412")
            {
                c = new EX1_412(template, isFriend, id);
            }
            else if (cardId == "EX1_323h")
            {
                c = new EX1_323h(template, isFriend, id);
            }
            else if (cardId == "EX1_571")
            {
                c = new EX1_571(template, isFriend, id);
            }
            else if (cardId == "EX1_145")
            {
                c = new EX1_145(template, isFriend, id);
            }
            else if (cardId == "CS2_226")
            {
                c = new CS2_226(template, isFriend, id);
            }
            else if (cardId == "NEW1_007a")
            {
                c = new NEW1_007a(template, isFriend, id);
            }
            else if (cardId == "EX1_160")
            {
                c = new EX1_160(template, isFriend, id);
            }
            else if (cardId == "EX1_238")
            {
                c = new EX1_238(template, isFriend, id);
            }
            else if (cardId == "EX1_382")
            {
                c = new EX1_382(template, isFriend, id);
            }
            else if (cardId == "EX1_508")
            {
                c = new EX1_508(template, isFriend, id);
            }
            else if (cardId == "CS2_046")
            {
                c = new CS2_046(template, isFriend, id);
            }
            else if (cardId == "EX1_409")
            {
                c = new EX1_409(template, isFriend, id);
            }
            else if (cardId == "EX1_283")
            {
                c = new EX1_283(template, isFriend, id);
            }
            else if (cardId == "EX1_384")
            {
                c = new EX1_384(template, isFriend, id);
            }
            else if (cardId == "EX1_575")
            {
                c = new EX1_575(template, isFriend, id);
            }
            else if (cardId == "EX1_tk33")
            {
                c = new EX1_tk33(template, isFriend, id);
            }
            else if (cardId == "EX1_301")
            {
                c = new EX1_301(template, isFriend, id);
            }
            else if (cardId == "EX1_611")
            {
                c = new EX1_611(template, isFriend, id);
            }
            else if (cardId == "CS2_125")
            {
                c = new CS2_125(template, isFriend, id);
            }
            else if (cardId == "EX1_025")
            {
                c = new EX1_025(template, isFriend, id);
            }
            else if (cardId == "EX1_572")
            {
                c = new EX1_572(template, isFriend, id);
            }
            else if (cardId == "CS2_155")
            {
                c = new CS2_155(template, isFriend, id);
            }
            else if (cardId == "EX1_332")
            {
                c = new EX1_332(template, isFriend, id);
            }
            else if (cardId == "NEW1_038")
            {
                c = new NEW1_038(template, isFriend, id);
            }
            else if (cardId == "EX1_tk28")
            {
                c = new EX1_tk28(template, isFriend, id);
            }
            else if (cardId == "NEW1_016")
            {
                c = new NEW1_016(template, isFriend, id);
            }
            else if (cardId == "EX1_564")
            {
                c = new EX1_564(template, isFriend, id);
            }
            else if (cardId == "EX1_tk11")
            {
                c = new EX1_tk11(template, isFriend, id);
            }
            else if (cardId == "CS2_050")
            {
                c = new CS2_050(template, isFriend, id);
            }
            else if (cardId == "EX1_398")
            {
                c = new EX1_398(template, isFriend, id);
            }
            else if (cardId == "DS1_233")
            {
                c = new DS1_233(template, isFriend, id);
            }
            else if (cardId == "EX1_410")
            {
                c = new EX1_410(template, isFriend, id);
            }
            else if (cardId == "hexfrog")
            {
                c = new hexfrog(template, isFriend, id);
            }
            else if (cardId == "EX1_116t")
            {
                c = new EX1_116t(template, isFriend, id);
            }
            else if (cardId == "CS2_023")
            {
                c = new CS2_023(template, isFriend, id);
            }
            else if (cardId == "EX1_614")
            {
                c = new EX1_614(template, isFriend, id);
            }
            else if (cardId == "EX1_304")
            {
                c = new EX1_304(template, isFriend, id);
            }
            else if (cardId == "EX1_116")
            {
                c = new EX1_116(template, isFriend, id);
            }
            else if (cardId == "EX1_277")
            {
                c = new EX1_277(template, isFriend, id);
            }
            else if (cardId == "EX1_033")
            {
                c = new EX1_033(template, isFriend, id);
            }
            else if (cardId == "EX1_083")
            {
                c = new EX1_083(template, isFriend, id);
            }
            else if (cardId == "EX1_278")
            {
                c = new EX1_278(template, isFriend, id);
            }
            else if (cardId == "CS2_237")
            {
                c = new CS2_237(template, isFriend, id);
            }
            else if (cardId == "EX1_570")
            {
                c = new EX1_570(template, isFriend, id);
            }
            else if (cardId == "EX1_178a")
            {
                c = new EX1_178a(template, isFriend, id);
            }
            else if (cardId == "EX1_538")
            {
                c = new EX1_538(template, isFriend, id);
            }
            else if (cardId == "EX1_561")
            {
                c = new EX1_561(template, isFriend, id);
            }
            else if (cardId == "DS1_055")
            {
                c = new DS1_055(template, isFriend, id);
            }
            else if (cardId == "EX1_130")
            {
                c = new EX1_130(template, isFriend, id);
            }
            else if (cardId == "EX1_610")
            {
                c = new EX1_610(template, isFriend, id);
            }
            else if (cardId == "CS2_026")
            {
                c = new CS2_026(template, isFriend, id);
            }
            else if (cardId == "EX1_306")
            {
                c = new EX1_306(template, isFriend, id);
            }
            else if (cardId == "EX1_354")
            {
                c = new EX1_354(template, isFriend, id);
            }
            else if (cardId == "EX1_534")
            {
                c = new EX1_534(template, isFriend, id);
            }
            else if (cardId == "EX1_402")
            {
                c = new EX1_402(template, isFriend, id);
            }
            else if (cardId == "EX1_158t")
            {
                c = new EX1_158t(template, isFriend, id);
            }
            else if (cardId == "EX1_166")
            {
                c = new EX1_166(template, isFriend, id);
            }
            else if (cardId == "CS1_069")
            {
                c = new CS1_069(template, isFriend, id);
            }
            else if (cardId == "EX1_045")
            {
                c = new EX1_045(template, isFriend, id);
            }
            else if (cardId == "DS1_175")
            {
                c = new DS1_175(template, isFriend, id);
            }
            else if (cardId == "EX1_019")
            {
                c = new EX1_019(template, isFriend, id);
            }
            else if (cardId == "NEW1_023")
            {
                c = new NEW1_023(template, isFriend, id);
            }
            else if (cardId == "CS2_004")
            {
                c = new CS2_004(template, isFriend, id);
            }
            else if (cardId == "EX1_608")
            {
                c = new EX1_608(template, isFriend, id);
            }
            else if (cardId == "EX1_531")
            {
                c = new EX1_531(template, isFriend, id);
            }
            else if (cardId == "EX1_243")
            {
                c = new EX1_243(template, isFriend, id);
            }
            else if (cardId == "EX1_289")
            {
                c = new EX1_289(template, isFriend, id);
            }
            else if (cardId == "CS2_131")
            {
                c = new CS2_131(template, isFriend, id);
            }
            else if (cardId == "EX1_259")
            {
                c = new EX1_259(template, isFriend, id);
            }
            else if (cardId == "CS2_077")
            {
                c = new CS2_077(template, isFriend, id);
            }
            else if (cardId == "EX1_310")
            {
                c = new EX1_310(template, isFriend, id);
            }
            else if (cardId == "TU4a_001")
            {
                c = new TU4a_001(template, isFriend, id);
            }
            else if (cardId == "CS2_179")
            {
                c = new CS2_179(template, isFriend, id);
            }
            else if (cardId == "EX1_400")
            {
                c = new EX1_400(template, isFriend, id);
            }
            else if (cardId == "HERO_08")
            {
                c = new HERO_08(template, isFriend, id);
            }
            else if (cardId == "NEW1_040")
            {
                c = new NEW1_040(template, isFriend, id);
            }
            else if (cardId == "CS2_169")
            {
                c = new CS2_169(template, isFriend, id);
            }
            else if (cardId == "EX1_020")
            {
                c = new EX1_020(template, isFriend, id);
            }
            else if (cardId == "EX1_366")
            {
                c = new EX1_366(template, isFriend, id);
            }
            else if (cardId == "CS2_171")
            {
                c = new CS2_171(template, isFriend, id);
            }
            else if (cardId == "TU4a_002")
            {
                c = new TU4a_002(template, isFriend, id);
            }
            else if (cardId == "TU4a_003")
            {
                c = new TU4a_003(template, isFriend, id);
            }
            else if (cardId == "EX1_604")
            {
                c = new EX1_604(template, isFriend, id);
            }
            else if (cardId == "TU4a_004")
            {
                c = new TU4a_004(template, isFriend, id);
            }
            else if (cardId == "TU4a_005")
            {
                c = new TU4a_005(template, isFriend, id);
            }
            else if (cardId == "TU4a_006")
            {
                c = new TU4a_006(template, isFriend, id);
            }
            else if (cardId == "EX1_029")
            {
                c = new EX1_029(template, isFriend, id);
            }
            else if (cardId == "CS2_024")
            {
                c = new CS2_024(template, isFriend, id);
            }

            else
            {
                return null;
            }
            if (c == null)
            {
                HREngine.API.Utilities.HRLog.Write("CARD null");
            }
            c.InitInstance(template, isFriend, id);
            c.Index = index;

            Type type = assembly.GetType("HREngine.Bots.b" + cardId);

            c.Behavior = (Behavior)Activator.CreateInstance(type);


            return c;
        }

        public static Card Clone(Card baseInstance)
        {
            if (baseInstance == null)
            {
                return null;
            }
            Card clone = new Card();

            clone = (Card)Activator.CreateInstance(baseInstance.GetType());
            clone.InitInstance(baseInstance.template, baseInstance.IsFriend, baseInstance.Id);
            clone.Behavior = baseInstance.Behavior;
            clone.IsTargetable = baseInstance.IsTargetable;
            clone.Race = baseInstance.Race;
            clone.Type = baseInstance.Type;
            clone.TempAtk = baseInstance.TempAtk;
            clone.CurrentAtk = baseInstance.currentAtk;
            clone.CurrentCost = baseInstance.CurrentCost;
            clone.CurrentDurability = baseInstance.CurrentDurability;
            clone.CurrentHealth = baseInstance.CurrentHealth;
            clone.MaxHealth = baseInstance.maxHealth;
            clone.CurrentArmor = baseInstance.CurrentArmor;
            clone.TargetTypeOnPlay = baseInstance.TargetTypeOnPlay;
            clone.IsTaunt = baseInstance.IsTaunt;
            clone.IsCharge = baseInstance.IsCharge;
            clone.IsDestroyed = baseInstance.IsDestroyed;
            clone.IsDivineShield = baseInstance.IsDivineShield;
            clone.IsEnraged = baseInstance.IsEnraged;
            clone.IsFrozen = baseInstance.IsFrozen;
            clone.IsSilenced = baseInstance.IsSilenced;
            clone.IsStealth = baseInstance.IsStealth;
            clone.IsTired = baseInstance.IsTired;
            clone.IsWindfury = baseInstance.IsWindfury;
            clone.SpellPower = baseInstance.SpellPower;
            clone.HasEnrage = baseInstance.HasEnrage;
            clone.HasFreeze = baseInstance.HasFreeze;
            clone.HasPoison = baseInstance.HasPoison;
            clone.IsImmune = baseInstance.IsImmune;
            clone.CountAttack = baseInstance.CountAttack;
            clone.Index = baseInstance.Index;
            clone.TestAllIndexOnPlay = baseInstance.TestAllIndexOnPlay;

            foreach (Buff b in baseInstance.buffs)
            {
                Buff ba = new Buff();
                ba.Atk = b.Atk;
                ba.Hp = b.Hp;
                ba.OwnerId = b.OwnerId;
                clone.buffs.Add(ba);
            }


            return clone;
        }

        public virtual string ToString()
        {
            string ret = "";

            ret += template.Name + "[" + template.Id + "]";
            if (Type == CType.MINION)
            {
                ret += "-T[" + IsTired.ToString() + "]CA[" + CountAttack.ToString() + "][" + CurrentAtk.ToString() + "/" + CurrentHealth.ToString() + "]CAN[" + CanAttack.ToString() + "]IDX[" + Index.ToString() + "]SP[" + SpellPower.ToString() + "]BUF[" + buffs.Count.ToString() + "]";

            }
            else if (Type == CType.WEAPON)
            {
                ret += "-CA[" + CountAttack.ToString() + "][" + CurrentAtk.ToString() + "/" + CurrentDurability.ToString() + "]";

            }

            return ret;
        }
        public virtual string ToStringShort()
        {
            string ret = "";

            ret += template.Name + "[" + template.Id + "]";
            if (Type == CType.MINION)
            {
                ret += "[" + CurrentAtk.ToString() + "/" + CurrentHealth.ToString() + "]";

            }
            else if (Type == CType.WEAPON)
            {
                ret += "-CA[" + CountAttack.ToString() + "][" + CurrentAtk.ToString() + "/" + CurrentDurability.ToString() + "]";

            }

            return ret;
        }
        public bool Equals(Card c)
        {

            if (c == null)
                return false;

            if (TestAllIndexOnPlay)
            {
                if (Index != c.Index)
                    return false;
            }
            if (CanAttack != c.CanAttack)
                return false;
            /*if (template != c.template)
                return false;*/
            if (CurrentAtk != c.CurrentAtk)
                return false;
            if (CurrentArmor != c.CurrentArmor)
                return false;
            /*if (CurrentCost != c.CurrentCost)
                return false;*/
            if (CurrentDurability != c.CurrentDurability)
                return false;
            if (CurrentHealth != c.CurrentHealth)
                return false;
            /*  if (MaxHealth != c.MaxHealth)
                  return false;
              /* if (IsTaunt != c.IsTaunt)
                   return false;
               if (IsCharge != c.IsCharge)
                   return false;*/
            if (IsDivineShield != c.IsDivineShield)
                return false;
            /* if (IsEnraged != c.IsEnraged)
                 return false;
             if (IsFrozen != c.IsFrozen)
                 return false;*/
            /* if (IsSilenced != c.IsSilenced)
                 return false;*/
            /*if (IsStealth != c.IsStealth)
                return false;*/
            /* if (IsTired != c.IsTired)
                 return false;*/

            /*  if (IsWindfury != c.IsWindfury)
                  return false;*/

            return true;
        }
    }
}
