

namespace Common
{
    /// <summary>
    /// 无返回值无参数事件
    /// </summary>
    public delegate void INonReturnAndNonParam();

    /// <summary>
    /// 通过一个参数判断返回True还是False
    /// </summary>
    /// <typeparam name="T">传入的数据类型</typeparam>
    /// <param name="inValue">传入的值</param>
    /// <returns>true还是false</returns>
    public delegate bool IGetBoolByOneParam<T>(T inValue);

    /// <summary>
    /// 传入一个参数，用这个参数去执行一些行为
    /// </summary>
    /// <typeparam name="T">传入的数据的类型</typeparam>
    /// <param name="inValue">传入的数据</param>
    public delegate void ISetOneParam<T>(T inValue);

    [System.Serializable]
    public struct Pair<T1, T2>
    {
        public T1 Key;
        public T2 Value;
        public Pair(T1 key, T2 value)
        {
            Key = key;
            Value = value;
        }
    }
}