using System.Collections.Generic;
using System.Linq;

public static class Extentions 
{
    public static IEnumerable<(T item, int index)> LoopI<T>(this IEnumerable<T> self) => self.Select((item, index) => (item, index));
}