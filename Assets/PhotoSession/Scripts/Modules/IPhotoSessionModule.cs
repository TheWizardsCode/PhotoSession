using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.PhotoSession
{
	public interface IPhotoSessionModule
	{

		public void Start(PhotoSession photoSession);
		public void Update();
		public void OnEnable();
		public void OnDisable();
		public void OnDrawGizmos();

	}
}