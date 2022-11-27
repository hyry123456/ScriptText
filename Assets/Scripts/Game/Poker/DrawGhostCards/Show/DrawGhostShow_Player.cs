using UnityEngine;

public class DrawGhostShow_Player : DrawGhostShow
{
    public override void OnChangeChoose(ClickRemain newRemain)
    {
        if (currentRemain != null)
            currentRemain.Image.color = Color.gray;
        if (!user.Choosing)
        {
            currentRemain = null;
            return;
        }
        else
        {
            if(newRemain != null)
                newRemain.Image.color = Color.white;
            currentRemain = newRemain;
        }
    }

    protected override void OnChoosing()
    {
        if (currentRemain == null || control.Banker.CurrentRemain == null)
            return;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentRemain.Image.color = Color.grey;
            control.Banker.CurrentRemain.Image.color = Color.grey;
            user.Recall(currentRemain.Index, control.Banker.CurrentRemain.Index);
        }
    }
}
