using UnityEngine;

public class chao_set_window : WindowBase
{
	private void Start()
	{
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (ChaoSetUI.IsChaoTutorial())
		{
			return;
		}
		if (msg != null)
		{
			msg.StaySequence();
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_window_close");
		if (gameObject != null)
		{
			UIButtonMessage component = gameObject.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.SendMessage("OnClick");
			}
		}
	}
}
