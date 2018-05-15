using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class ItemAttributeList : ScriptableObject
    {

        [SerializeField]
        public List<ItemAttribute> itemAttributeList = new List<ItemAttribute>();
    }
}
