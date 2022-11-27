using UnityEngine;

public class DrawGhostShow_AI : DrawGhostShow
{
    public override void OnChangeChoose(ClickRemain newRemain)
    {
        if (currentRemain != null)
            currentRemain.Image.color = Color.gray;
        if (!control.Player.User.Choosing)
        {
            currentRemain = null;
            return;
        }
        else
        {
            if (newRemain != null)
                newRemain.Image.color = Color.white;
            currentRemain = newRemain;
        }
    }

    protected override void OnChoosing()
    {
        user.Recall(0, 0);
    }
}
