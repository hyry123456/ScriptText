
namespace Info
{
    /// <summary>  /// 角色状态，用来添加角色Buffer  /// </summary>
    public interface CharacterState
    {
        /// <summary>   /// 逐帧执行的方法     /// </summary>
        /// <param name="finalHp">最终改变的HP</param>
        /// <param name="finalSolid">最终改变的Solid</param>
        /// <returns>返回是否结束该状态，true为结束</returns>
        public bool OnUpdate(CharacterInfo info, int finalHp, int finalSolid);

        /// <summary>  /// 修改HP时执行的方法，注意HP可正可负   /// </summary>
        /// <param name="hp"></param>
        /// <param name="info">角色信息</param>
        /// <returns>改变成的hp</returns>
        public int OnHPChange(int hp, CharacterInfo info);

        /// <summary>     /// 修改坚硬值时执行的方法   /// </summary>
        /// <param name="solid">经过前面的处理后剩余的改变solid</param>
        /// <param name="info">角色信息</param>
        /// <returns>改变成的solid</returns>
        public int OnSolidChange(int solid, CharacterInfo info);
    }
}