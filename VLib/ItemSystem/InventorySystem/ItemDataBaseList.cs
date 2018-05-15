using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


namespace V
{
    [CreateAssetMenu(menuName = "V/ItemDataBaseList")]
    public class ItemDataBaseList : ScriptableObject
    {

        [SerializeField]
        public List<Item> itemList = new List<Item>();

        public Item getItemCopyByID(int id)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].itemID == id)
                {
                    return itemList[i].getCopy();
                }
            }
            return null;
        }

        public Item getNewInstanceByID(int id)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].itemID == id)
                {
                    Item item = itemList[i].getCopy();
                    return new Item(item.itemName, item.itemID, item.itemDesc, item.itemIcon, item.itemPrefab, item.maxStack, item.itemType, item.itemAttributes);
                }
            }
            return null;
        }

        public Item getItemCopyByName(string name)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].itemName.ToLower().Equals(name.ToLower()))
                    return itemList[i].getCopy();
            }
            return null;
        }
    }

    public enum ItemType
    {
        None = 0,
        Weapon = 1,
        Consumable = 2,
        Quest = 3,
        Head = 4,
        Shoe = 5,
        Chest = 6,
        Trouser = 7,
        Earrings = 8,
        Necklace = 9,
        Ring = 10,
        Hands = 11,
        Blueprint = 12,
        Backpack = 13,
        Ammo,
        UFPS_Grenade,
        UFPS_Weapon,
        UFPS_Ammo
    }

    [System.Serializable]
    public class ItemAttribute
    {
        public string attributeName;
        public int attributeValue;
        public ItemAttribute(string _attributeName, int _attributeValue)
        {
            attributeName = _attributeName;
            attributeValue = _attributeValue;
        }
        public ItemAttribute() { }
    }

    [System.Serializable]
    public class Item
    {
        public string itemName;
        public int itemID;
        public string itemDesc;
        public Sprite itemIcon;
        public GameObject itemPrefab;
        public int itemValue = 1;
        public ItemType itemType;
        public float itemWeight;
        public int maxStack = 1;
        public int indexItemInList = 999;
        public int rarity;

        public System.Action<ItemHandleOnGUI> OnGUIDoubleClick;


        [SerializeField]
        public List<ItemAttribute> itemAttributes = new List<ItemAttribute>();

        public Item() { }

        public Item(string name, int id, string desc, Sprite icon, GameObject model, int _maxStack, ItemType type, List<ItemAttribute> _itemAttributes)
        {
            itemName = name;
            itemID = id;
            itemDesc = desc;
            itemIcon = icon;
            itemPrefab = model;
            itemType = type;
            maxStack = _maxStack;
            itemAttributes = _itemAttributes;
        }

        public Item getCopy()
        {
            return (Item)this.MemberwiseClone(); //Creates a shallow copy of the current Object.
                                                 /*
                                                  The MemberwiseClone method creates a shallow copy by creating a new object, and then copying the nonstatic
                                                  fields of the current object to the new object. If a field is a value type, a bit-by-bit copy of the field
                                                  is performed. If a field is a reference type, the reference is copied but the referred object is not;
                                                  therefore, the original object and its clone refer to the same object.
                                                */
        }
    }
}


