using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.PhotoSession
{
	public interface IPhotoSessionModule
	{

		void Start(PhotoSession photoSession);
		void Update();
		void OnEnable();
		void OnDisable();
		void OnDrawGizmos();

	}
}