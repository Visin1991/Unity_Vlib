using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace V
{
    public abstract class EditorTimeBuildInSerializableData : BaseSerializableData
    {

        public sealed override void DeSerializeData(BinaryReader reader)
        {
            reader.ReadString(); // We need to Consumpt the typeName
            DeSerializeDataInternal(reader);
        }
    }
}
