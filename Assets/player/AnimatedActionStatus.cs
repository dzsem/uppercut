using UnityEngine;

/// <summary>
/// Egy wrapper osztály az EActionStatus köré. Amikor a .Status-t PERFORMING-ra állítod, akkor
/// a megadott animátor mezőt (animatorBoolField) igaz-ra billenti, különben hamisra.
/// </summary>
public class AnimatedActionStatus
{
	public Animator animator = null;
	public string animatorBoolField;

	[SerializeField] private EActionStatus _status;

	public EActionStatus Status
	{
		get => _status;
		set
		{
			if (value == _status)
				return;

			_status = value;

			if (animator == null)
			{
				Debug.LogError($"AnimatedActionStatus (\"{animatorBoolField}\"): nem állítottad be Start()-ban az animátort!");
				return;
			}

			animator.SetBool(animatorBoolField, _status == EActionStatus.PERFORMING);
		}
	}

	public bool IsPerforming()
	{
		return _status == EActionStatus.PERFORMING;
	}

	public AnimatedActionStatus(string field, EActionStatus defaultStatus = EActionStatus.AVAILABLE)
	{
		animatorBoolField = field;
		_status = defaultStatus;
	}
};