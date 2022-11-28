using Common;

/// <summary>/// 抽鬼牌的AI，本质上就是庄家/// </summary>
public class DrawGhost_AI : DrawGhost_User
{
    public override void OnEnd(bool isDie)
    {

    }

    public override void BeginChoose(ISetOneParam<int> recall, DrawGhostCard drawGhost)
    {
        base.BeginChoose(recall, drawGhost);
    }
}
