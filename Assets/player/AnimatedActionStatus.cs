using UnityEngine;

// TODO: doksi ~Tamas
public class AnimatedActionStatus
{
	public Animator animator = null;
	public string animatorBoolField;

	[SerializeField] private EActionStatus _status;

	public EActionStatus Status
	{
		get
		{
			return _status;
		}
		set
		{
			if (value != _status)
			{
				_status = value;

				if (animator == null)
				{
					Debug.LogError($"AnimatedActionStatus (\"{animatorBoolField}\"): nem állítottad be Start()-ban az animátort!");
					return;
				}

				animator.SetBool(animatorBoolField, _status == EActionStatus.PERFORMING);
			}
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