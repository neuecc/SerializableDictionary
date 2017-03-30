using UnityEngine;
using System;
using System.Collections.Generic;
using SerializableCollections;

[System.Serializable]
public class TestDictionary :  SerializableDictionary<string, int>  {

}

[System.Serializable]
public class TestLookup :  SerializableLookup<string,int>  {

}

#if UNITY_EDITOR

[UnityEditor.CustomPropertyDrawer(typeof(TestDictionary))]
public class ExtendedSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer
{

}

[UnityEditor.CustomPropertyDrawer(typeof(TestLookup))]
public class ExtendedSerializableLookupPropertyDrawer : SerializableLookupPropertyDrawer
{

}


#endif