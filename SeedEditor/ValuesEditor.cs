using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HREngine.Bots;

namespace SeedEditor
{
    public partial class ValuesEditor : Form
    {
        public ValuesEditor()
        {
            InitializeComponent();
        }

        private void ValuesEditor_Load(object sender, EventArgs e)
        {
            ValueArmorEnemy.Value = ValuesInterface.ValueArmorEnemy;
            ValueArmorFriend.Value = ValuesInterface.ValueArmorFriend;
            ValueAttackMinion.Value = ValuesInterface.ValueAttackMinion;
            ValueAttackWeapon.Value = ValuesInterface.ValueAttackWeapon;
            ValueDivineShield.Value = ValuesInterface.ValueDivineShield;
            ValueDurabilityWeapon.Value = ValuesInterface.ValueDurabilityWeapon;
            ValueEnemyCardDraw.Value = ValuesInterface.ValueEnemyCardDraw;
            ValueEnemyMinionCount.Value = ValuesInterface.ValueEnemyMinionCount;
            ValueFriendCardDraw.Value = ValuesInterface.ValueFriendCardDraw;
            ValueFriendMinionCount.Value = ValuesInterface.ValueFriendMinionCount;
            ValueFrozen.Value = ValuesInterface.ValueFrozen;
            ValueHealthEnemy.Value = ValuesInterface.ValueHealthEnemy;
            ValueHealthFriend.Value = ValuesInterface.ValueHealthFriend;
            ValueHealthMinion.Value = ValuesInterface.ValueHealthMinion;
            ValueSecret.Value = ValuesInterface.ValueSecret;
            ValueTaunt.Value = ValuesInterface.ValueTaunt;


        }

        private void ValueHealthEnemy_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueHealthEnemy = (int)ValueHealthEnemy.Value;
        }

        private void ValueHealthFriend_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueHealthFriend = (int)ValueHealthFriend.Value;

        }

        private void ValueArmorEnemy_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueArmorEnemy = (int)ValueArmorEnemy.Value;

        }

        private void ValueArmorFriend_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueArmorFriend = (int)ValueArmorFriend.Value;

        }

        private void ValueSecret_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueSecret = (int)ValueSecret.Value;

        }

        private void ValueEnemyCardDraw_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueEnemyCardDraw = (int)ValueEnemyCardDraw.Value;

        }

        private void ValueEnemyMinionCount_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueEnemyMinionCount = (int)ValueEnemyMinionCount.Value;
        }

        private void ValueFriendMinionCount_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueFriendMinionCount = (int)ValueFriendMinionCount.Value;

        }

        private void ValueFriendCardDraw_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueFriendCardDraw = (int)ValueFriendCardDraw.Value;

        }

        private void ValueDurabilityWeapon_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueDurabilityWeapon = (int)ValueDurabilityWeapon.Value;

        }

        private void ValueHealthMinion_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueHealthMinion = (int)ValueHealthMinion.Value;

        }

        private void ValueAttackMinion_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueAttackMinion = (int)ValueAttackMinion.Value;

        }

        private void ValueTaunt_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueTaunt = (int)ValueTaunt.Value;
        }

        private void ValueDivineShield_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueDivineShield = (int)ValueDivineShield.Value;
        }

        private void ValueAttackWeapon_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueAttackWeapon = (int)ValueAttackWeapon.Value;

        }

        private void ValueFrozen_ValueChanged(object sender, EventArgs e)
        {
            ValuesInterface.ValueFrozen = (int)ValueFrozen.Value;

        }
    }
}
