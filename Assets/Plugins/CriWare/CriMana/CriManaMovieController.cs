﻿/****************************************************************************
 *
 * Copyright (c) 2015 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

using UnityEngine;

/**
 * \addtogroup CRIMANA_UNITY_COMPONENT
 * @{
 */

/**
 * <summary>GameObjectに貼り付けてムービ再生するためのコンポーネントです。</summary>
 * \par 説明:
 * GameObjectに貼り付けてムービ再生するためのコンポーネントです。<br/>
 * UnityEngine.Renderer にマテリアルを設定することで、ムービを表示します。<br/>
 * CriManaMovieMaterial を継承しています。<br/>
 * \par 注意:
 * 本クラスでは、再生・停止・ポーズの基本操作しか行えません。<br/>
 * 複雑な再生制御を行う場合は、playerプロパティでコアプレーヤに対して操作を行って下さい。<br/>
 */
[AddComponentMenu("CRIWARE/CriManaMovieController")]
public class CriManaMovieController : CriManaMovieMaterial
{
	#region Properties
	/**
	 * <summary>ムービマテリアルの設定対象の UnityEngine.Renderer です。</summary>
	 * \par 説明:
	 * ムービマテリアルの設定対象の UnityEngine.Renderer です。<br/>
	 * 指定されていない場合はアタッチしているゲームオブジェクトの UnityEngine.Renderer を使用します。
	 */	
	public Renderer		target;


	/**
	 * <summary>ムービフレームが使用できない場合にオリジナルのマテリアルを表示するか。</summary>
	 * \par 説明:
	 * ムービフレームが使用できない場合にオリジナルのマテリアルを表示するか。<br/>
	 * true : ムービフレームが使用できない場合、オリジナルのマテリアルを表示します。<br/>
	 * false : ムービフレームが使用できない場合、target の描画を無効にします。<br/>
	 */	
	public bool			useOriginalMaterial;
	#endregion


	#region Internal Variables
	private Material	originalMaterial;
	#endregion


	protected override void Start()
	{
		base.Start();
		if (target == null) {
			target = gameObject.GetComponent<Renderer>();
		}
		if (target == null) {
			Debug.LogError("[CRIWARE] error");
			Destroy(this);
			return;
		}
		originalMaterial = target.material;
		if (!useOriginalMaterial) {
			target.enabled = false;
		}
	}


	protected override void Update()
	{
		base.Update();

		// If there is a target connected but current GameObject is not a Renderer,
		// we check target visibility an then update movie material if visible.
		if (renderMode == RenderMode.OnVisibility) {
			if (HaveRendererOwner == false && target != null && target.isVisible) {
				player.OnWillRenderObject(this);
			}
		}
	}


	protected override void OnDestroy()
	{
		target.material = originalMaterial;
		if (!useOriginalMaterial) {
			target.enabled = false;
		}
		originalMaterial = null;
		base.OnDestroy();
	}


	protected override void OnMaterialAvailableChanged()
	{
		if (isMaterialAvailable) {
			target.material	= material;
			target.enabled	= true;
		} else {
			target.material = originalMaterial;
			if (!useOriginalMaterial) {
				target.enabled = false;
			}
		}
	}
}

/**
 * @}
 */


/* end of file */