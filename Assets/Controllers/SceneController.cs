using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.Controllers
{
	public static class SceneController
	{
		public static void SceneNavigate(SceneNames scene)
		{
			SceneManager.LoadScene((int)scene);
		}
	}
}
