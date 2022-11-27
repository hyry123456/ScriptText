

namespace Common
{
    /// <summary>
    /// �޷���ֵ�޲����¼�
    /// </summary>
    public delegate void INonReturnAndNonParam();

    /// <summary>
    /// ͨ��һ�������жϷ���True����False
    /// </summary>
    /// <typeparam name="T">�������������</typeparam>
    /// <param name="inValue">�����ֵ</param>
    /// <returns>true����false</returns>
    public delegate bool IGetBoolByOneParam<T>(T inValue);

    /// <summary>
    /// ����һ�����������������ȥִ��һЩ��Ϊ
    /// </summary>
    /// <typeparam name="T">��������ݵ�����</typeparam>
    /// <param name="inValue">���������</param>
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