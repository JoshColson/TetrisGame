﻿using UnityEngine.SceneManagement;

namespace Assets.Controllers
{
	public static class NavigationController
	{
		public static void SceneNavigate(SceneNames scene)
		{
			SceneManager.LoadScene((int)scene);
		}
	}
}
