using Common;

/// <summary>/// ����Ƶ�AI�������Ͼ���ׯ��/// </summary>
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
