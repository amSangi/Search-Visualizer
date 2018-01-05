/// <summary>
/// Generic Priority Queue Implementation Reference: https://visualstudiomagazine.com/Articles/2012/11/01/Priority-Queues-with-C.aspx?Page=2
/// </summary>
using System; 
using System.Collections.Generic;

public class PriorityQueue <T> where T : IComparable <T> {

    private List<T> data; 

    public PriorityQueue()
    {
        data = new List<T>(); 
    }

    public void Enqueue(T item)
    {
        IsConsistent(); 

        data.Add(item); 
        int c = data.Count - 1;
         
        // Swap up new item
        while (c > 0)
        {
            int p = (c - 1) / 2;
             
            // Stop if child is greater or equal to parent
            if (data[c].CompareTo(data[p]) >= 0) { break; }

            T temp = data[c]; 
            data[c] = data[p]; 
            data[p] = temp; 
            c = p; 
        }
    }

    public T Dequeue()
    {
        IsConsistent(); 

        int last = data.Count - 1; 

        // Swap front/last items
        T front = data[0]; 
        data[0] = data[last]; 
        data.RemoveAt(last); 
        --last; 
        int p = 0; 

        // Swap down item 
        while (true)
        {
            int l = p * 2 + 1; 
            int r = l + 1; 

            // No children 
            if (l > last) { break; } 
            // Get the smaller child
            if (r <= last && data[r].CompareTo(data[l]) < 0) { l = r; } 
            // Stop if child is larger or equal to parent
            if (data[p].CompareTo(data[l]) <= 0) { break; } 

            T temp = data[p]; 
            data[p] = data[l]; 
            data[l] = temp; 

            p = l; 
        }
       
        return front; 
    }
  
    public T Peek()
    {
        return data[0]; 
    }

    public int Count()
    {
        return data.Count; 
    }

    public void Clear()
    {
        data.Clear(); 
    }

    // Binary Heap Invariant Check
    private void IsConsistent()
    {
        if (data.Count == 0) { return; } 
        int l = data.Count - 1; 
        for (int p = 0; p < data.Count; ++p) 
        {
            int lc = 2 * p + 1; 
            int rc = 2 * p + 2; 
            if (lc <= l && data[p].CompareTo(data[lc]) > 0) { throw new ArgumentException("Binary heap is not consistent with the invariant"); } 
            if (rc <= l && data[p].CompareTo(data[rc]) > 0) { throw new ArgumentException("Binary heap is not consistent with the invariant"); }
        }
    }

}
