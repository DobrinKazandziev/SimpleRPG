# SimpleRPG
## 1. Description
This application in a very simplistic **text RPG** game ,created for a class assignment. You take control of a character to complete quests ,fight monsters ,gain levels and earn rewards.   

This game is based on a [tutorial](http://scottlilly.com/learn-c-by-building-a-simple-rpg-index/) to learn **C#** programming from [Scott Lilly](https://scottlilly.com/) which is very detailed hands-on way to learn to code.
## 2. How to play
### 2.1 How it looks
![Main screen](https://i0.wp.com/scottlilly.com/wp-content/uploads/2014/05/Lesson-00.1-Game-screenshot.png?s)
### 2.2 Playing the game
You start in the _Home_ area, and have buttons for **North,South,East,West**.  
Some areas you go to can give you a quest asking you to return with some quantity of items.  
You have to go to specific areas to fight with monsters, which when you kill them, give you experience points to level, gold , and random loot.  
You then go back to the quest giver and turn in your quest items for a reward.  
There are also vendors where you can buy and sell items.      
In short:  
* The player goes to locations.
* The player may need to have certain items to enter a location.
* The location might have a quest available.
* To complete a quest, the player must collect certain items and turn them in.
* The player can collect items by going to a location and fighting monsters there.
* The player fights monsters with weapons.
* The player can use a healing potion while fighting.
* The player receives loot items after defeating a monster.
* After turning in the quest, the player receives reward items.
* The player can go to vendors, to buy and sell items using gold.  

## 3. Game logic
### 3.1 MoveTo() logic
![MoveTo](http://puu.sh/oOy9L/7b3758b455.png)
### 3.2 Weapon and potion logic
![Weapon](http://puu.sh/oPeXJ/fda4a08b45.png)
![Potion](http://puu.sh/oPeYh/354d73bae2.png)
### 3.2 Player class
**Attributes**: ( see souprce code for ... )
```c#
 public class Player : LivingCreature
    {
        private int _gold;
        private int _experiencePoints;
        private Location _currentLocation;
        private Monster _currentMonster;
        public event EventHandler<MessageEventArgs> OnMessage;
        public int Gold{...}
        public int ExperiencePoints{...}
        public int Level
        {
            get { return ((ExperiencePoints / 100) + 1); }
        }
        public Location CurrentLocation{...}
        public Weapon CurrentWeapon { get; set; }
        public BindingList<InventoryItem> Inventory { get; set; }
        public List<Weapon> Weapons{...}
        public List<HealingPotion> Potions{...}
        public BindingList<PlayerQuest> Quests { get; set; }
```
**Methods**: ( see source code for ... )
```c#
private Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints) : base(currentHitPoints, maximumHitPoints){...}
public static Player CreateDefaultPlayer(){...}
public static Player CreatePlayerFromXmlString(string xmlPlayerData){...}  

public void AddExperiencePoints(int experiencePointsToAdd){...}
public bool HasRequiredItemToEnterThisLocation(Location location){...}
public bool HasThisQuest(Quest quest){...}
public bool CompletedThisQuest(Quest quest){...}
public bool HasAllQuestCompletionItems(Quest quest){...}
public void RemoveQuestCompletionItems(Quest quest){...}
public void MarkQuestCompleted(Quest quest){...}
public void AddItemToInventory(Item itemToAdd, int quantity = 1){...}
public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1){...}
private void RaiseInventoryChangedEvent(Item item){...}
private void RaiseMessage(string message, bool addExtraNewLine = false){...}
public void MoveTo(Location newLocation){...}
public void UseWeapon(Weapon weapon){...}
public void UsePotion(HealingPotion potion){...}
public string ToXmlString(){...}  
private void MoveHome(){...}
public void MoveNorth(){...}
public void MoveEast(){...}
public void MoveSouth(){...}
public void MoveWest(){...}
```
## 4. Future enhancements

* (**DONE**)Save the player’s current game to disk, and re-load it later  
* (**DONE 1/3**)As the player gains experience, increase their level
  * (**DONE**)Increase MaximumHitPoints with each new level
  * Add a minimum level requirement for some items
  * Add a minimum level requirement for some locations
* Add randomization to battles
  * Determine if the player hits the monster
  * Determine if the monster hits the player
* Add player attributes (strength, dexterity, etc.)
  * Use attributes in battle: who attacks first, amount of damage, etc.
* Add armor and jewelry
  * Makes it more difficult for the monster to hit the player
  * Has special benefits: increased chance to hit, increased damage, etc.
* Add crafting skills the player can acquire
* Add crafting recipes the player use
  * Require the appropriate skill
  * Requires components (inventory items)
* Make some quests repeatable 
* Make quest chains (player must complete “Quest A” before they can receive “Quest B”)
* Add magic scrolls
* Add spells
  * Level requirements for the spells
  * Spells require components to cast
* Add more potions
  * More powerful healing potions
  * Potions to improve player’s “to hit” chances, or damage
* Add poisons to use in battle
* Add pets
  * Help the player in battle by attacking opponents
  * Help the player in battle by healing the player
* (**DONE PARTIALLY**)Add stores/vendors
  * (**DONE PARTIALLY**)Player can sell useless items and buy new equipment, scrolls, potions, poisons, and crafting/spell components
