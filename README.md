SerializableDictionary
===
SerializableDictionary(SerializableCollections) is fast, native serializable collections for Unity, includes SerializableDictionary, SerializableLookup, SerializableTuple.

What is this?
---
If using C# deserialization in Unity, it is slow than native serialization process such as ScriptableObject but Unity's native serialization does not support dictionary and llookup(multi dictionary). SerializableDictionary provides native dictionary serialize and deserialize, it is extreme fast.

How to use
---
Download Zip or clone GithHub and put  under Assetss/SerializableCollections in your project. You have to inheritant `SerializableDictionary<TKey, TValue>` for serialization, for example TKey:Int, TValue:string is
 
```csharp
[Serializable]
public class IntStringSerializableDictionary : SerializableDictionary<int, string>
{

}
```

and shows inspector
 

```csharp
#if UNITY_EDITOR

[UnityEditor.CustomPropertyDrawer(typeof(IntStringSerializableDictionary))]
[UnityEditor.CustomPropertyDrawer(typeof(IntDoubleSerializableDictionary))]
[UnityEditor.CustomPropertyDrawer(typeof(IntIntStringSerializableDictionary))]
public class ExtendedSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer
{

}

#endif
```

![image](https://cloud.githubusercontent.com/assets/46207/13866974/da6f9fac-ec77-11e5-93a7-a8eb43c08680.png)

currenly only supports viewer, edit in inspector  plans future releases.

Multiple Key Dictionary
---
You have to use `SerializableTuple`.

```csharp
[Serializable]
public class IntIntTuple : SerializableTuple<int, int>
{
    public IntIntTuple()
    {

    }

    public IntIntTuple(int item1, int item2)
        : base(item1, item2)
    {

    }
}

[Serializable]
public class IntIntStringSerializableDictionary : SerializableDictionary<IntIntTuple, string>
{

}
```

and Tuple is key for SerializableDictionary.

SerializableLookup
---
ILookup is MultiDictionary(like Dictionary[Key, Value[]). You can use SerializableLookup like lookup builder.

```csharp
[Serializable]
public class IntIntSerializableLookup : SerializableLookup<int, int>
{
}

#if UNITY_EDITOR

[UnityEditor.CustomPropertyDrawer(typeof(IntIntSerializableLookup))]
public class ExtendedSerializableLookupPropertyDrawer : SerializableLookupPropertyDrawer
{

}

#endif
```
TrimExcess
---
`TrimExcess` of `SerializableDictionary`, `SerializableLookup` cut the buffer and reduce memory usage. 

License
---
under the MIT Licesne.
Dictionary's original code is from [dotnet/corefx](https://github.com/dotnet/corefx), check the original license.
