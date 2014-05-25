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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using SmartCompiler;
using System.Reflection;
using System.Diagnostics;

namespace SeedEditor
{
    public partial class Form1 : Form
    {
        public enum ActionDenotator { NEW, OPEN, SAVEAS}
        TextWriter _writer = null;
        Simulation s = null;
        string CurrentSeedPath = "";
        Board Seed = null;
        int CurrentId = -1;
        public Form1()
        {

            InitializeComponent();
            _writer = new TextBoxStreamWriter(LogOutput);
            Console.SetOut(_writer);

            CardTemplate.DatabasePath = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\";
            CardTemplate.LoadAll();
            StreamReader str = new StreamReader(CardTemplate.DatabasePath + "Bots/SmartCC/Config/useProfiles");
            string useDefaut = str.ReadLine();

            str.Close();

            if (useDefaut == "true")
            {
               string star =  CardTemplate.DatabasePath;
                Application.EnableVisualStyles();
                Application.Run(new ProfileSelector(CardTemplate.DatabasePath));
            }
            else
            {

                using (CodeCompiler compiler = new CodeCompiler(CardTemplate.DatabasePath + "Bots\\SmartCC\\Profiles\\Defaut\\", CardTemplate.DatabasePath))
                {
                    if (compiler.Compile())
                    {
                    }
                }
                String path = CardTemplate.DatabasePath + "Bots/SmartCC/Profile.current";
                using (var stream = new FileStream(path, FileMode.Truncate))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.WriteLine("Defaut");
                        writer.Close();

                    }
                }

            }
            s = new Simulation();
            ValuesInterface.LoadValuesFromFile();

            Seed = new Board();
            Seed.HeroEnemy = Card.Create("HERO_01", true, GenerateId());
            Seed.HeroFriend = Card.Create("HERO_02", false, GenerateId());

            Seed.HeroFriend.CurrentHealth = 30;
            Seed.HeroFriend.MaxHealth = 30;

            Seed.HeroEnemy.CurrentHealth = 30;
            Seed.HeroEnemy.MaxHealth = 30;

            Seed.ManaAvailable = 10;

            ClearUI(ActionDenotator.NEW);
            UpdateUI();
        }

        public int GenerateId()
        {
            return CurrentId++;
        }

        private void newSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Added N3HL - needs to be continued
            if (CurrentSeedLabel.Text != "-")
            {
                DialogResult result = MessageBox.Show("Are you sure? All unsaved changes to this .seed file will be lost", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    Seed = new Board();
                    Seed.HeroEnemy = Card.Create("HERO_01", true, GenerateId());
                    Seed.HeroFriend = Card.Create("HERO_02", false, GenerateId());

                    Seed.HeroFriend.CurrentHealth = 30;
                    Seed.HeroFriend.MaxHealth = 30;

                    Seed.HeroEnemy.CurrentHealth = 30;
                    Seed.HeroEnemy.MaxHealth = 30;

                    Seed.ManaAvailable = 10;

                    ClearUI(ActionDenotator.NEW);
                    UpdateUI();
                }
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure? This .seed file wasn't saved !", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    Seed = new Board();
                    Seed.HeroEnemy = Card.Create("HERO_01", true, GenerateId());
                    Seed.HeroFriend = Card.Create("HERO_02", false, GenerateId());

                    Seed.HeroFriend.CurrentHealth = 30;
                    Seed.HeroFriend.MaxHealth = 30;

                    Seed.HeroEnemy.CurrentHealth = 30;
                    Seed.HeroEnemy.MaxHealth = 30;

                    Seed.ManaAvailable = 10;

                    ClearUI(ActionDenotator.NEW);
                    UpdateUI();
                }
            }
        }

        public void ClearUI(ActionDenotator referer)
        {
            //Clean comboboxes
            FriendMinionComboBox.Items.Clear();
            MinionEnemyComboBox.Items.Clear();
            HandComboBox.Items.Clear();
            SecretComboBox.Items.Clear();
            FriendMinionComboBox.SelectedItem = null;
            MinionEnemyComboBox.SelectedItem = null;
            HandComboBox.SelectedItem = null;
            SecretComboBox.SelectedItem = null;

            //Added N3HL
            if (referer.Equals(ActionDenotator.NEW))
            {
                CurrentSeedLabel.Text = "-";
            }
            checkBoxEnemyWeapon.Enabled = false;
            checkBoxEnemyWeapon.Checked = false;
            checkBoxLocalWeapon.Checked = false;
            checkBoxLocalWeapon.Enabled = false;
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            Stream thisStream = thisAssembly.GetManifestResourceStream("SeedEditor.Resources.noimage.png");
            pictureBoxPreview.Image = Image.FromStream(thisStream);
            //You forgot to clean the log u.u
            LogOutput.Text = "";
            //Other stuff
            HandInputText.Clear();
            //Disabling things...
            EnemyHeroHp.Enabled = false;
            EnemyHeroArmor.Enabled = false;
            EnemyHeroAtk.Enabled = false;
            //Disabling our things
            FriendHeroHp.Enabled = false;
            FriendHeroArmor.Enabled = false;
            FriendHeroAtk.Enabled = false;

            EnemyHeroHp.Value = 0;
            EnemyHeroArmor.Value = 0;
            EnemyHeroAtk.Value = 0;
            FriendHeroHp.Value = 0;
            FriendHeroArmor.Value = 0;
            FriendHeroAtk.Value = 0;
            WeaponEnemyId.Text = "";
            WeaponEnnemyDurability.Value = 0;
            WeaponEnnemyAtk.Value = 0;
            WeaponFriendId.Text = "";
            WeaponFriendDurability.Value = 0;
            WeaponFriendAtk.Value = 0;
            Mana.Value = 0;
            DisplayGrid.SelectedObject = null;
        }

        public void UpdateUI()
        {
            //UpdateHeroEnemy
            if (Seed.HeroEnemy != null)
            {
                EnemyHeroHp.Value = Seed.HeroEnemy.CurrentHealth;
                EnemyHeroArmor.Value = Seed.HeroEnemy.CurrentArmor;
                EnemyHeroAtk.Value = Seed.HeroEnemy.CurrentAtk;
            }
            //UpdateHeroFriend
            if (Seed.HeroFriend != null)
            {
                FriendHeroHp.Value = Seed.HeroFriend.CurrentHealth;
                FriendHeroArmor.Value = Seed.HeroFriend.CurrentArmor;
                FriendHeroAtk.Value = Seed.HeroFriend.CurrentAtk;
            }
            //UpdateWeaponEnemy
            if (Seed.WeaponEnemy != null)
            {
                WeaponEnemyId.Text = Seed.WeaponEnemy.template.Id;
                WeaponEnnemyDurability.Value = Seed.WeaponEnemy.CurrentDurability;
                WeaponEnnemyAtk.Value = Seed.WeaponEnemy.CurrentAtk;
            }
            //UpdateWeaponFriend
            if (Seed.WeaponFriend != null)
            {
                WeaponFriendId.Text = Seed.WeaponFriend.template.Id;
                WeaponFriendDurability.Value = Seed.WeaponFriend.CurrentDurability;
                WeaponFriendAtk.Value = Seed.WeaponFriend.CurrentAtk;
            }
            //UpdateHand
            foreach (Card c in Seed.Hand)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Label = "[" + c.template.Id + "]" + c.template.Name;
                item.Value = c;
                HandComboBox.Items.Add(item);
            }
            //UpdateMinionsFriend
            foreach (Card c in Seed.MinionFriend)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Label = "[" + c.template.Id + "]" + c.template.Name + " - " + c.Index.ToString();
                item.Value = c;
                FriendMinionComboBox.Items.Add(item);
            }
            //UpdateMinionsEnemy
            foreach (Card c in Seed.MinionEnemy)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Label = "[" + c.template.Id + "]" + c.template.Name + " - " + c.Index.ToString();
                item.Value = c;
                MinionEnemyComboBox.Items.Add(item);
            }
            //UpdateSecrets
            foreach (Card c in Seed.Secret)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Label = "[" + c.template.Id + "]" + c.template.Name;
                item.Value = c;
                SecretComboBox.Items.Add(item);
            }
            //UpdateMana
            Mana.Value = Seed.ManaAvailable;
        }

        public void UpdateSeed()
        {
            //UpdateHeroEnemy
            if (Seed.HeroEnemy != null)
            {
                Seed.HeroEnemy.CurrentHealth = (int)EnemyHeroHp.Value;
                Seed.HeroEnemy.CurrentArmor = (int)EnemyHeroArmor.Value;
                Seed.HeroEnemy.CurrentAtk = (int)EnemyHeroAtk.Value;
            }
            //UpdateHeroFriend
            if (Seed.HeroFriend != null)
            {
                Seed.HeroFriend.CurrentHealth = (int)FriendHeroHp.Value;
                Seed.HeroFriend.CurrentArmor = (int)FriendHeroArmor.Value;
                Seed.HeroFriend.CurrentAtk = (int)FriendHeroAtk.Value;
            }
            //UpdateWeaponEnemy
            if (WeaponEnemyId.BackColor != Color.Red)
            {
                string className = WeaponEnemyId.Text;
                Card tmp = Card.Create(className, true, GenerateId());

                if (tmp != null)
                {
                    Seed.WeaponEnemy = tmp;
                    Seed.WeaponEnemy.CurrentDurability = (int)WeaponEnnemyDurability.Value;
                    Seed.WeaponEnemy.CurrentAtk = (int)WeaponEnnemyAtk.Value;
                }
                else
                {
                    Seed.WeaponEnemy = null;
                }
            }
            //UpdateWeaponFriend
            if (WeaponFriendId.BackColor != Color.Red)
            {
                string className = WeaponFriendId.Text;
                Card tmp = Card.Create(className, true, GenerateId());

                if (tmp != null)
                {
                    Seed.WeaponFriend = tmp;
                    Seed.WeaponFriend.CurrentDurability = (int)WeaponFriendDurability.Value;
                    Seed.WeaponFriend.CurrentAtk = (int)WeaponFriendAtk.Value;
                }
                else
                {
                    Seed.WeaponFriend = null;
                }
            }
            //UpdateHand
            foreach (object o in HandComboBox.Items)
            {
                ComboBoxItem item = o as ComboBoxItem;
                Seed.Hand.Add(item.Value);
            }
            //UpdateMinionsFriend
            foreach (object o in FriendMinionComboBox.Items)
            {
                ComboBoxItem item = o as ComboBoxItem;
                Seed.MinionFriend.Add(item.Value);
            }
            //UpdateMinionsEnemy
            foreach (object o in MinionEnemyComboBox.Items)
            {
                ComboBoxItem item = o as ComboBoxItem;
                Seed.MinionEnemy.Add(item.Value);
            }
            //UpdateSecrets
            foreach (object o in SecretComboBox.Items)
            {
                ComboBoxItem item = o as ComboBoxItem;
                Seed.Secret.Add(item.Value);
            }
            Seed.ManaAvailable = (int)Mana.Value;
        }

        public void ClearSeed()
        {
            Seed.Hand.Clear();
            Seed.MinionEnemy.Clear();
            Seed.MinionFriend.Clear();
            Seed.Secret.Clear();
            Seed.ManaAvailable = 0;
            Seed.WeaponEnemy = null;
            Seed.WeaponFriend = null;
        }

        private void openSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "/";
            openFileDialog1.Filter = "Seed files (*.seed)|*.seed";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Assembly.LoadFile(CardTemplate.DatabasePath + "Bots/SmartCC/Profile.dll");
                    Seed = HREngine.Bots.Debugger.BinaryDeSerialize(File.ReadAllBytes(openFileDialog1.FileName)) as Board;
                    CurrentSeedPath = openFileDialog1.FileName;
                    CurrentSeedLabel.Text = CurrentSeedPath;

                    ClearUI(ActionDenotator.OPEN);
                    UpdateUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't open seed file  " + ex.Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void HandDelB_Click(object sender, EventArgs e)
        {
            if (HandComboBox.SelectedItem != null)
                HandComboBox.Items.Remove(HandComboBox.SelectedItem);
            DisplayGrid.SelectedObject = null;

        }

        private void FriendMinionDelB_Click(object sender, EventArgs e)
        {
            if (FriendMinionComboBox.SelectedItem != null)
                FriendMinionComboBox.Items.Remove(FriendMinionComboBox.SelectedItem);
            DisplayGrid.SelectedObject = null;
        }

        private void EnemyMinionDelB_Click(object sender, EventArgs e)
        {
            if (MinionEnemyComboBox.SelectedItem != null)
                MinionEnemyComboBox.Items.Remove(MinionEnemyComboBox.SelectedItem);
            DisplayGrid.SelectedObject = null;

        }

        private void SecretsDelB_Click(object sender, EventArgs e)
        {
            if (SecretComboBox.SelectedItem != null)
                SecretComboBox.Items.Remove(SecretComboBox.SelectedItem);
            DisplayGrid.SelectedObject = null;

        }

        private void HandAddB_Click(object sender, EventArgs e)
        {
            string className = HandInputText.Text;
            Card tmp = Card.Create(className, true, GenerateId());
            //We should validate the user doesn't have more than 2 cards of each type really...
            int cardCounter = 0;
            //Added N3HL, you can't have more than 10 cards in the hand, no matter what
            if (HandComboBox.Items.Count < 10)
            {
                if (tmp != null)
                {
                    //Added dummie validations :\
                    string cardID = tmp.template.Id;
                    foreach (var item in HandComboBox.Items)
                    {
                        int pos = item.ToString().IndexOf("]");
                        string itemID = item.ToString().Substring(1, pos - 1);
                        if (cardID == itemID)
                        {
                            cardCounter++;
                        }
                    }

                    if ((cardID == "GAME_005" && cardCounter < 1) || (cardID != "GAME_005" && cardCounter < 2))
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Label = "[" + tmp.template.Id + "]" + tmp.template.Name;
                        item.Value = tmp;
                        HandComboBox.Items.Add(item);
                        //Added N3HL - preview image
                        pictureBoxPreview.Load(Application.StartupPath + "/Common/Cards/" + HandInputText.Text.ToUpper() + ".png");
                    }
                    else
                    {
                        MessageBox.Show("You can't add more than 1 coin or 2 cards of the same type", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Your card couldn't be found", "Warning",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("You can't have more than 10 cards in your hand", "Information", MessageBoxButtons.OK,MessageBoxIcon.Information);
            }


        }

        private void FriendMinionAddB_Click(object sender, EventArgs e)
        {
            if (FriendMinionComboBox.Items.Count < 7)
            {
                string className = FriendMinionInputText.Text;
                Card tmp = Card.Create(className, true, GenerateId());
                if (tmp != null)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Label = "[" + tmp.template.Id + "]" + tmp.template.Name + " - " + tmp.Index.ToString();
                    item.Value = tmp;
                    FriendMinionComboBox.Items.Add(item);
                    pictureBoxPreview.Load(Application.StartupPath + "/Common/Cards/" + FriendMinionInputText.Text.ToUpper() + ".png");
                }
                else
                {
                    MessageBox.Show("Your card couldn't be found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("You can't have more than 7 minions in the board", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void EnemyMinionAddB_Click(object sender, EventArgs e)
        {
            string className = EnnemyMinionInputText.Text;
            Card tmp = Card.Create(className, false, GenerateId());
            if (tmp != null)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Label = "[" + tmp.template.Id + "]" + tmp.template.Name + " - " + tmp.Index.ToString();
                item.Value = tmp;
                MinionEnemyComboBox.Items.Add(item);
            }
            else
            {
                MessageBox.Show("Can't find your card sorry =/");
            }
        }

        private void SecretsAddB_Click(object sender, EventArgs e)
        {
            if (SecretComboBox.Items.Count < 3)
            {
                string className = SecretInputText.Text;
                Card tmp = Card.Create(className, true, GenerateId());
                if (tmp != null)
                {
                    bool hasSecret = false;
                    string secretID = tmp.template.Id;
                    foreach (var secret in SecretComboBox.Items)
                    {
                        if (secret.ToString().Substring(1,7) == secretID && !hasSecret)
                        {
                            hasSecret = true;
                            break;
                        }
                    }
                    if (!hasSecret)
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Label = "[" + tmp.template.Id + "]" + tmp.template.Name;
                        item.Value = tmp;
                        SecretComboBox.Items.Add(item);
                        pictureBoxPreview.Load(Application.StartupPath + "/Common/Cards/" + SecretInputText.Text.ToUpper() + ".png");
                    }
                    else
                    {
                        MessageBox.Show("You can't add more than 1 secret of each type", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Your secret couldn't be found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("You can't add more than 3 secrets", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void WeaponEnemyId_TextChanged(object sender, EventArgs e)
        {
            string className = WeaponEnemyId.Text;
            Card tmp = Card.Create(className, true, GenerateId());
            if (tmp != null)
            {
                WeaponEnemyId.BackColor = Color.Green;
            }
            else
            {
                WeaponEnemyId.BackColor = Color.Red;
            }
            if (WeaponEnemyId.Text == "")
            {
                WeaponEnemyId.BackColor = Color.White;
            }

        }

        private void SecretComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayGrid.SelectedObject = ((ComboBoxItem)SecretComboBox.SelectedItem).Value;
            string secretID = HandComboBox.Text.Substring(1, HandComboBox.Text.IndexOf("]") - 1);
            pictureBoxPreview.Load(Application.StartupPath + "/Common/Cards/" + secretID.ToUpper() + ".png");

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Seed files (*.seed)|*.seed";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            //Added
            saveFileDialog1.FileName = "MySeedFile";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ClearSeed();
                    UpdateSeed();

                    Stream stream = new FileStream(saveFileDialog1.FileName,FileMode.Create,FileAccess.Write);
                    byte[] mem = HREngine.Bots.Debugger.Serialize(Seed);
                    stream.Write(mem, 0, mem.GetLength(0));
                    stream.Close();
                    CurrentSeedLabel.Text = saveFileDialog1.FileName;
                    CurrentSeedPath = saveFileDialog1.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't open seed file  " + ex.Message);
                }
            }            
        }

        private void HandComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayGrid.SelectedObject = ((ComboBoxItem)HandComboBox.SelectedItem).Value;
            string cardID = HandComboBox.Text.Substring(1, HandComboBox.Text.IndexOf("]") - 1);
            pictureBoxPreview.Load(Application.StartupPath + "/Common/Cards/" + cardID.ToUpper() + ".png");


        }

        private void MinionEnemyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayGrid.SelectedObject = ((ComboBoxItem)MinionEnemyComboBox.SelectedItem).Value;

        }

        private void FriendMinionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayGrid.SelectedObject = ((ComboBoxItem)FriendMinionComboBox.SelectedItem).Value;

        }

        private void SimulateButton_Click(object sender, EventArgs e)
        {
            ClearSeed();
            UpdateSeed();
            s = new Simulation();
            s.CreateLogFolder();
            s.SeedSimulation(Seed);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            s.Simulate(false);

            stopWatch.Stop();
            Console.WriteLine("Simulation stopped after :" + (stopWatch.ElapsedMilliseconds / 1000.0f).ToString());
        }

        private void Mana_ValueChanged(object sender, EventArgs e)
        {
            //Added - N3HL
            if (Mana.Value > 10)
            {
                Mana.Value = 10;
            }
        }

        private void EnemyHeroHp_ValueChanged(object sender, EventArgs e)
        {
            //Added N3HL
            if (EnemyHeroHp.Value > 30)
            {
                EnemyHeroHp.Value = 30;
            }
        }

        private void EnemyHeroArmor_ValueChanged(object sender, EventArgs e)
        {
            //Added
            if (EnemyHeroArmor.Value > 30)
            {
                EnemyHeroArmor.Value = 30;
            }
        }

        private void EnemyHeroAtk_ValueChanged(object sender, EventArgs e)
        {
        }

        private void FriendHeroHp_ValueChanged(object sender, EventArgs e)
        {
            //Added N3HL
            //You can't have more than 30 hp
            if (FriendHeroHp.Value > 30)
            {
                FriendHeroHp.Value = 30;
            }
        }

        private void FriendHeroArmor_ValueChanged(object sender, EventArgs e)
        {
            //Added N3HL
            //You can't have more than 30 armor
            if (FriendHeroArmor.Value > 30)
            {
                FriendHeroArmor.Value = 30;
            }
        }

        private void FriendHeroAtk_ValueChanged(object sender, EventArgs e)
        {
            //Unfortunately there is no way to calculate max attack
        }

        private void WeaponEnnemyDurability_ValueChanged(object sender, EventArgs e)
        {
        }

        private void WeaponEnnemyAtk_ValueChanged(object sender, EventArgs e)
        {
        }

        private void WeaponFriendDurability_ValueChanged(object sender, EventArgs e)
        {
        }

        private void WeaponFriendAtk_ValueChanged(object sender, EventArgs e)
        {
        }

        private void ValuesEditorB_Click(object sender, EventArgs e)
        {
            ValuesEditor form2 = new ValuesEditor();
            form2.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Added a confirmation to exit
            DialogResult result = MessageBox.Show("Are you sure?", "Exit confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void checkBoxEnemyWeapon_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEnemyWeapon.Checked)
            {
                WeaponEnemyId.Enabled = true;
                WeaponEnnemyDurability.Enabled = true;
                WeaponEnnemyAtk.Enabled = true;
            }
            else
            {
                WeaponEnemyId.Text = "";
                WeaponEnnemyDurability.Value = 0;
                WeaponEnnemyAtk.Value = 0;
                WeaponEnemyId.Enabled = false;
                WeaponEnnemyDurability.Enabled = false;
                WeaponEnnemyAtk.Enabled = false;
            }
        }

        private void checkBoxLocalWeapon_CheckedChanged(object sender, EventArgs e)
        {
            //Added N3HL
            //Just a visual thing <3
            if (checkBoxLocalWeapon.Checked)
            {
                WeaponFriendId.Enabled = true;
                WeaponFriendAtk.Enabled = true;
                WeaponFriendDurability.Enabled = true;
            }
            else
            {
                WeaponFriendId.Text = "";
                WeaponFriendAtk.Value = 0;
                WeaponFriendDurability.Value = 0;
                WeaponFriendId.Enabled = false;
                WeaponFriendAtk.Enabled = false;
                WeaponFriendDurability.Enabled = false;
            }
        }

        private void cmbBoxLocalHero_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Added N3HL
            //Do what you need to do here apart from visuals :3
            if (FriendHeroHp.Enabled == false && FriendHeroAtk.Enabled == false
                && FriendHeroArmor.Enabled == false && checkBoxLocalWeapon.Enabled == false)
            {
                FriendHeroHp.Enabled = true;
                FriendHeroArmor.Enabled = true;
                FriendHeroAtk.Enabled = true;
                checkBoxLocalWeapon.Enabled = true;
            }
            string hero = "";
            switch (cmbBoxLocalHero.Text)
            {
                case "Druid":
                    hero = "HERO_06";
                    break;
                case "Hunter":
                    hero = "HERO_05";
                    break;
                case "Mage":
                    hero = "HERO_08";
                    break;
                case "Paladin":
                    hero = "HERO_04";
                    break;
                case "Priest":
                    hero = "HERO_09";
                    break;
                case "Rogue":
                    hero = "HERO_03";
                    break;
                case "Shaman":
                    hero = "HERO_02";
                    break;
                case "Warlock":
                    hero = "HERO_07";
                    break;
                case "Warrior":
                    hero = "HERO_01";
                    break;
                default:
                    break;
            }

            pictureBoxPreview.Load(Application.StartupPath + "/Common/Cards/" + hero + ".png");
        }

        private void WeaponFriendId_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBoxPreview.Load(Application.StartupPath + "/Common/Cards/" + WeaponFriendId.Text.ToUpper() + ".png");
        }

        private void WeaponEnemyId_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBoxPreview.Load(Application.StartupPath + "/Common/Cards/" + WeaponEnemyId.Text.ToUpper() + ".png");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coded by: wirmate\n\"Styled\" by: N3HL", "Credits", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void githubRepositoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/wirmate/SmartCC");
        }

        private void crawlerforumThreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.thecrawlerforum.com/index.php/Thread/6321-SmartCC/");
        }

        private void cmbEnemyClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EnemyHeroHp.Enabled == false && EnemyHeroAtk.Enabled == false
                && EnemyHeroArmor.Enabled == false && checkBoxEnemyWeapon.Enabled == false)
            {
                EnemyHeroHp.Enabled = true;
                EnemyHeroArmor.Enabled = true;
                EnemyHeroAtk.Enabled = true;
                checkBoxEnemyWeapon.Enabled = true;
            }

            string hero = "";
            switch (cmbEnemyClass.Text)
            {
                case "Druid":
                    hero = "HERO_06";
                    break;
                case "Hunter":
                    hero = "HERO_05";
                    break;
                case "Mage":
                    hero = "HERO_08";
                    break;
                case "Paladin":
                    hero = "HERO_04";
                    break;
                case "Priest":
                    hero = "HERO_09";
                    break;
                case "Rogue":
                    hero = "HERO_03";
                    break;
                case "Shaman":
                    hero = "HERO_02";
                    break;
                case "Warlock":
                    hero = "HERO_07";
                    break;
                case "Warrior":
                    hero = "HERO_01";
                    break;
                default:
                    break;
            }

            pictureBoxPreview.Load(Application.StartupPath + "/Common/Cards/" + hero + ".png");
        }
    }
    public class ComboBoxItem
    {
        public string Label { get; set; }
        public Card Value { get; set; }

        public override string ToString()
        {
            return Label ?? string.Empty;
        }
    }

    public class TextBoxStreamWriter : TextWriter
    {
        TextBox _output = null;

        public TextBoxStreamWriter(TextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            _output.AppendText(value.ToString());
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
