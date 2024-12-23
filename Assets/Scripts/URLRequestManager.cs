using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class URLRequestManager : MonoBehaviour
{
	private enum CancelState
	{
		IDLE,
		CANCELING,
		CANCELED
	}

	private const float EMULATION_WAIT_TIME = 1f;

	public bool mEmulation;

	private List<URLRequest> mRequestList;

	private List<URLRequest> mExecuteList;

	private List<URLRequest> mRemainingList;

	private bool mExecuting;

	private static URLRequestManager mInstance;

	private URLRequest mCurrentRequest;

	private CancelState mCancelState;

	private bool mCancelRequest;

	public static URLRequestManager Instance
	{
		get
		{
			return mInstance;
		}
	}

	public bool Emulation
	{
		get
		{
			return mEmulation;
		}
		set
		{
			mEmulation = value;
		}
	}

	private void Awake()
	{
		if (mInstance == null)
		{
			mInstance = this;
			mEmulation = false;
			Object.DontDestroyOnLoad(this);
			mRequestList = new List<URLRequest>();
			mExecuteList = new List<URLRequest>();
			mRemainingList = new List<URLRequest>();
			mExecuting = false;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!mExecuting && 0 < mRequestList.Count)
		{
			mExecuteList.Clear();
			mRemainingList.Clear();
			int count = mRequestList.Count;
			for (int i = 0; i < count; i++)
			{
				URLRequest item = mRequestList[i];
				mExecuteList.Add(item);
				mRemainingList.Add(item);
			}
			mRequestList.Clear();
			mExecuting = true;
			StartCoroutine("ExecuteRequest");
		}
	}

	private IEnumerator ExecuteRequest()
	{
		int count = mExecuteList.Count;
		for (int i = 0; i < count; i++)
		{
			URLRequest request = mExecuteList[i];
			if (request == null)
			{
				continue;
			}
			bool cancel = false;
			bool exec = true;
			while (exec)
			{
				if (Emulation || request.Emulation)
				{
					request.PreBegin();
					WWWForm form = request.CreateWWWForm();
					if (form == null)
					{
						continue;
					}
					string param = Encoding.ASCII.GetString(form.data);
					Debug.Log($"URLRequest Emulation : [{request.url}] [{param}]", DebugTraceManager.TraceType.SERVER);
					float startTime = Time.realtimeSinceStartup;
					do
					{
						yield return null;
					}
					while (Time.realtimeSinceStartup - startTime < 1f);
					request.DidReceiveSuccess(null);
				}
				else
				{
					mCurrentRequest = request;
					request.Begin();
					
					while (!request.IsDone() && !cancel)
					{
						request.UpdateElapsedTime(0.1f);
						if (request.IsTimeOut())
						{
							cancel = true;
						}
						yield return new WaitForSeconds(0.1f);
					}
					
					request.Result();
					mCurrentRequest = null;
				}
				exec = false;
			}
			
			if (cancel)
			{
				mRemainingList.Remove(request);
			}
		}
		
		mExecuting = false;
	}

	public void Request(URLRequest request)
	{
		mRequestList.Add(request);
	}

	public void Cancel()
	{
		if (0 >= mExecuteList.Count)
		{
			mCancelState = CancelState.IDLE;
			return;
		}
		mCancelRequest = true;
		mCancelState = CancelState.CANCELING;
	}

	public bool IsCanceled()
	{
		if (mCancelState == CancelState.IDLE)
		{
			return true;
		}
		return CancelState.CANCELED == mCancelState;
	}

	public bool IsCancelRequest()
	{
		return mCancelRequest;
	}

	public void ClearCancel()
	{
		mCancelRequest = false;
		mCancelState = CancelState.IDLE;
	}

	public int GetRemainingCount()
	{
		if (mRemainingList == null)
		{
			return 0;
		}
		return mRemainingList.Count;
	}

	public URLRequest GetRemainingRequest(int index)
	{
		if (mRemainingList != null && 0 <= index && mRemainingList.Count > index)
		{
			return mRemainingList[index];
		}
		return null;
	}
}
