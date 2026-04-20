using UnityEngine;

public class ObjectIndexer : MonoBehaviour
{
   //Used to assing some sort of unique identifier to an Object that is active in the pool. 
   //Thats when the State is true
   //The index always coresponds to the index of the object in the ObjectPool array
  
    public int Index { get; set; }

}
