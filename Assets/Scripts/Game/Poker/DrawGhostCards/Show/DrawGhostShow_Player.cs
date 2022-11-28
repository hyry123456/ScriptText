using UnityEngine;

public class DrawGhostShow_Player : DrawGhostShow
{
    public override void OnChangeChoose(ClickRemain newRemain)
    {
        if (currentRemain != null)
            currentRemain.Image.color = Color.gray;
        //当庄家在选择时才可以选中
        if (control.Banker.User.Choosing)
        {
            if (newRemain != null)
                newRemain.Image.color = Color.white;
            currentRemain = newRemain;
            return;
        }
        else
            currentRemain = null;
    }

    protected override void OnChoosing()
    {
        if (control.Banker.CurrentRemain == null)
            return;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            control.Banker.CurrentRemain.Image.color = Color.grey;
            user.Recall(control.Banker.CurrentRemain.Index);
        }
    }
}
