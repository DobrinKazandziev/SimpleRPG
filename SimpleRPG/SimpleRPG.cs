using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;
using System.IO;


namespace SimpleRPG
{
    public partial class SimpleRPG : Form
    {
        private Player _player;
        private Monster _currentMonster;
        private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";
        public SimpleRPG()
        {
            InitializeComponent();

            if (File.Exists(PLAYER_DATA_FILE_NAME))
            {
                _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
            }
            else
            {
                _player = Player.CreateDefaultPlayer();
            }
            MoveTo(_player.CurrentLocation);
            UpdatePlayerStats();
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToNorth);
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToSouth);
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToEast);
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToWest);
        }


        private void MoveTo(Location newLocation)
        {
            //Does the location have any reqired items
            if (newLocation.ItemRequiredToEnter != null)
            {
                //See if the player has the reqired item in their invetory
                if (!_player.HasRequiredItemToEnterThisLocation(newLocation))
                {
                    //We didn't found the required item in their invetory
                    rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                    return;
                }
            }
            //Update player's current location
            _player.CurrentLocation = newLocation;

            //Show/hide available movement buttons
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnEast.Visible = (newLocation.LocationToEast != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnWest.Visible = (newLocation.LocationToWest != null);

            //Update location text
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;

            //Heal player
            _player.CurrentHitPoints = _player.MaximumHitPoints;
            UpdatePlayerStats();

            //Does the location have a quest?
            if (newLocation.QuestAvailableHere != null)
            {
                //Chech if player has quest,and if complete
                bool playerAlreadyHasQuest = _player.HasThisQuest(newLocation.QuestAvailableHere);
                bool playerAlreadyCompletedQuest = _player.CompletedThisQuest(newLocation.QuestAvailableHere);

                //See if the player already has the quest
                if (playerAlreadyHasQuest)
                {
                    //If the player has not completed the quest yet
                    if (!playerAlreadyCompletedQuest)
                    {
                        //See if player has all the items to complete quest
                        bool playerHasAllItemsToCompleteQuest = _player.HasAllQuestCompletionItems(newLocation.QuestAvailableHere);

                        //The player has all items to complete quest
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            //Display msg
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You completed the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;

                            //Remove quest items from invetory
                            _player.RemoveQuestCompletionItems(newLocation.QuestAvailableHere);
                        }
                        //Give quest rewards
                        rtbMessages.Text += "You recive: " + Environment.NewLine;
                        rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() +
                            " experience points" + Environment.NewLine;
                        rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() +
                            " gold" + Environment.NewLine;
                        rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                        rtbMessages.Text += Environment.NewLine;

                        _player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperiencePoints;
                        _player.Gold += newLocation.QuestAvailableHere.RewardGold;
                        //Add reward item to player invetory
                        _player.AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);

                        //Find quest in player's quest list, and mark complete
                        _player.MarkQuestCompleted(newLocation.QuestAvailableHere);
                        UpdatePlayerStats();
                    }
                }
                else
                {
                    //The player does not already have the quest
                    rtbMessages.Text += "You receive the " + newLocation.QuestAvailableHere.Name +
                        " quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "To complete it, return with:" + Environment.NewLine;
                    foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if (qci.Quantity == 1)
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                        }
                        else
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
                        }
                    }
                    rtbMessages.Text += Environment.NewLine;

                    //Add the quest to the player's quest list
                    _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }
            //Does the location have a monster?
            if (newLocation.MonsterLivingHere != null)
            {
                rtbMessages.Text += "You have a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;

                //Make a new monster, from Worlds.monster list
                Monster standartMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);
                _currentMonster = new Monster(
                    standartMonster.ID,
                    standartMonster.Name,
                    standartMonster.MaximumDamage,
                    standartMonster.RewardExperiencePoints,
                    standartMonster.RewardGold,
                    standartMonster.CurrentHitPoints,
                    standartMonster.MaximumHitPoints
                    );

                foreach (LootItem lootItem in standartMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }

                cboWeapons.Visible = true;
                cboPotions.Visible = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
            }
            else
            {
                _currentMonster = null;

                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
            }

            //Update UI
            UpdateInventoryListInUI();
            UpdateQuestListInUI();
            UpdateWeaponListInUI();
            UpdatePotionListInUI();
            UpdatePlayerStats();
        }
        private void UpdateInventoryListInUI()
        {
            dgvInventory.RowHeadersVisible = false;
            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";
            dgvInventory.Rows.Clear();
            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[]
                    {
                             inventoryItem.Details.Name,
                             inventoryItem.Quantity.ToString()
                        });
                }
            }
        }
        private void UpdateQuestListInUI()
        {
            dgvQuests.RowHeadersVisible = false;
            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Done?";
            dgvQuests.Rows.Clear();
            foreach (PlayerQuest playerQuest in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] {
                     playerQuest.Details.Name,
                     playerQuest.IsCompleted.ToString()
                });
            }
        }
        private void UpdateWeaponListInUI()
        {
            List<Weapon> weapons = new List<Weapon>();
            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is Weapon)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        weapons.Add((Weapon)inventoryItem.Details);
                    }
                }
            }
            if (weapons.Count == 0)
            {
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                cboWeapons.DataSource = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";
                cboWeapons.SelectedIndex = 0;
            }
        }
        private void UpdatePotionListInUI()
        {
            List<HealingPotion> healingPotions = new List<HealingPotion>();
            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is HealingPotion)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        healingPotions.Add(
                        (HealingPotion)inventoryItem.Details);
                    }
                }
            }
            if (healingPotions.Count == 0)
            {
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";
                cboPotions.SelectedIndex = 0;
            }
        }
        private void UpdatePlayerStats()
        {
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;
            int damageToMonster = RandomNumberGenerator.NumberBetween(currentWeapon.MinimumDamage, currentWeapon.MaximumDamage);
            _currentMonster.CurrentHitPoints -= damageToMonster;

            rtbMessages.Text += "You hit the " + _currentMonster.Name + " for " + damageToMonster.ToString() + " points." + Environment.NewLine;

            //Chech if monster dead
            if (_currentMonster.CurrentHitPoints <= 0)
            {
                //Is dead
                rtbMessages.Text += Environment.NewLine;
                rtbMessages.Text += "You defeated the " + _currentMonster.Name + Environment.NewLine;

                //Give rewards
                _player.ExperiencePoints += _currentMonster.RewardExperiencePoints;
                rtbMessages.Text += "You receive " + _currentMonster.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                _player.Gold += _currentMonster.RewardGold;
                rtbMessages.Text += "You receive " + _currentMonster.RewardGold.ToString() + " gold" + Environment.NewLine;
                List<InventoryItem> lootedItems = new List<InventoryItem>();
                foreach (LootItem lootItem in _currentMonster.LootTable)
                {
                    if (RandomNumberGenerator.NumberBetween(1, 100) <= lootItem.DropPercentage)
                    {
                        lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                    }
                }
                if (lootedItems.Count == 0)
                {
                    foreach (LootItem lootItem in _currentMonster.LootTable)
                    {
                        if (lootItem.IsDefaultItem)
                        {
                            lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                        }
                    }
                }
                foreach (InventoryItem ii in lootedItems)
                {
                    _player.AddItemToInventory(ii.Details);
                    if (ii.Quantity == 1)
                    {
                        rtbMessages.Text += "You loot " + ii.Quantity.ToString() + " " + ii.Details.Name + Environment.NewLine;
                    }
                    else
                    {
                        rtbMessages.Text += "You loot " + ii.Quantity.ToString() + " " + ii.Details.NamePlural + Environment.NewLine;
                    }
                }
                //Refresh player inf and controls
                UpdatePlayerStats();
                UpdateInventoryListInUI();
                UpdateWeaponListInUI();
                UpdatePotionListInUI();
                rtbMessages.Text += Environment.NewLine;
                MoveTo(_player.CurrentLocation);
            }
            else
            {
                //Monster is still alive
                int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);
                rtbMessages.Text += "The " + _currentMonster.Name + " did " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine;
                _player.CurrentHitPoints -= damageToPlayer;
                lblHitPoints.Text = _player.CurrentHitPoints.ToString();

                if (_player.CurrentHitPoints <= 0)
                {
                    rtbMessages.Text += "The " + _currentMonster.Name + " killed you." + Environment.NewLine;
                    MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
                }
                UpdatePlayerStats();
            }
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;
            _player.CurrentHitPoints = _player.CurrentHitPoints + potion.AmountToHeal;
            if (_player.CurrentHitPoints > _player.MaximumHitPoints)
            {
                _player.CurrentHitPoints = _player.MaximumHitPoints;
            }
            //Remove potion from invetory
            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Details.ID == potion.ID)
                {
                    ii.Quantity--;
                    break;
                }
            }

            rtbMessages.Text += "You drink a " + potion.Name + Environment.NewLine;
            // Monster gets their turn to attack
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);
            rtbMessages.Text += "The " + _currentMonster.Name + " did " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine;
            _player.CurrentHitPoints -= damageToPlayer;
            if (_player.CurrentHitPoints <= 0)
            {
                rtbMessages.Text += "The " + _currentMonster.Name + " killed you." + Environment.NewLine;
                MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            }
            //Refresh UI
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            UpdateInventoryListInUI();
            UpdatePotionListInUI();
        }

        private void rtbMessages_TextChanged(object sender, EventArgs e)
        {
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }

        private void SimpleRPG_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, _player.ToXmlString());
        }
    }
}
