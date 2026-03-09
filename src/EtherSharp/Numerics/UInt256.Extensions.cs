namespace EtherSharp.Numerics;

/// <summary>
/// Provides aggregation helpers for sequences of <see cref="UInt256"/> values.
/// </summary>
public static class UInt256Extensions
{
    extension(IEnumerable<UInt256> e)
    {
        /// <summary>
        /// Computes the sum of all values in the sequence.
        /// </summary>
        /// <returns>The sum of the values in <c>e</c>.</returns>
        public UInt256 Sum()
        {
            var result = UInt256.Zero;
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
        /// Computes the sum of the projected <see cref="UInt256"/> values in the sequence.
        /// </summary>
        /// <param name="selector">Projects each element of the sequence to a <see cref="UInt256"/> value.</param>
        /// <returns>The sum of the projected values in <c>e</c>.</returns>
        public UInt256 Sum(Func<T, UInt256> selector)
        {
            var result = UInt256.Zero;
            foreach(var item in e)
            {
                result += selector(item);
            }
            return result;
        }
    }
}
