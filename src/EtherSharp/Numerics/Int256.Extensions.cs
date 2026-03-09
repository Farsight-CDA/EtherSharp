namespace EtherSharp.Numerics;

/// <summary>
/// Provides aggregation helpers for sequences of <see cref="Int256"/> values.
/// </summary>
public static class Int256Extensions
{
    extension(IEnumerable<Int256> e)
    {
        /// <summary>
        /// Computes the sum of all values in the sequence.
        /// </summary>
        /// <returns>The sum of the values in <c>e</c>.</returns>
        public Int256 Sum()
        {
            var result = Int256.Zero;
            foreach(var value in e)
            {
                result += value;
            }
            return result;
        }
    }

    extension<T>(IEnumerable<T> e)
    {
        /// <summary>
        /// Computes the sum of the projected <see cref="Int256"/> values in the sequence.
        /// </summary>
        /// <param name="selector">Projects each element of the sequence to an <see cref="Int256"/> value.</param>
        /// <returns>The sum of the projected values in <c>e</c>.</returns>
        public Int256 Sum(Func<T, Int256> selector)
        {
            var result = Int256.Zero;
            foreach(var item in e)
            {
                result += selector(item);
            }
            return result;
        }
    }
}
