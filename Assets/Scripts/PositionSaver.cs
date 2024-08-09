using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
		[Serializable]
		public struct Data
		{
			public Vector3 Position;
			public float Time;
		}

		[SerializeField, ReadOnly, Tooltip("Для заполнения этого поля нужно воспользоваться контекстным меню в инспекторе и командой “Create File”")]
        private TextAsset _json;

		[field: SerializeField]
		public List<Data> Records { get; private set; }

		private void Awake()
		{
			//todo comment: Что будет, если в теле этого условия не сделать выход из метода?
			//answer: Программа дойдет до строки 31 "JsonUtility", не смотря на то, что у нас _json = null, а FromJsonOverwrite ожидает полчить на вход данные, которые необходимо перезаписать. Будет ошибка NullReferenceException.

			if (_json == null)
			{
				gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");
				return;
			}

			JsonUtility.FromJsonOverwrite(_json.text, this);
			//todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
			//answer: если уже есть записи в списке Records, они не перезатрутся. А если нет, будет создан пустой список указанной длины. Но для чего это нужно в дальнейшем, пока не понял.
			if (Records == null)
				Records = new List<Data>(10);
		}

		private void OnDrawGizmos()
		{
            //todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
            //answer: если не сделать данную проверку, то при инициализации переменной prev может возникнуть ошибка, так как может не оказаться первого элемента в списке.
            if (Records == null || Records.Count == 0) return;
			var data = Records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
            //todo comment: Почему итерация начинается не с нулевого элемента?
            //answer: потому, что значение с индексом 0 мы уже передали в переменную prev ранее и далее используем его для отрисовки линии к следующей позицией с индексом 1.
            for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}
		
#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
            //todo comment: Что происходит в этой строке?
            //answer: создаем файл с названием Path.txt, по пути хранящемуся в Application.dataPath, при этом конкатенируем название файла и путь
            var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
            //todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав)
            //answer: отключаем поток, в котором открыт созданный файл (чистка памяти, для предотвращения утечки памяти).
            stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
                //Этой командой можно получить путь к ассету через его гуид
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                //Этой командой можно загрузить сам ассет
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                //todo comment: Для чего нужны эти проверки?
                //answer: чтобы убедиться, что в asset удалось загрузить сам ассет и его имя Path, которое мы создавали.
                if (asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
                    //todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
                    //answer: мы нашли нужный нам файл и сохранили в него текущий объект, последующее итерирование ничего не даст.
                    return;
				}
			}
		}

		private void OnDestroy()
		{

			if (_json == null) return;
			string text = JsonUtility.ToJson(this, true);
			Debug.Log(text);
			var path = UnityEditor.AssetDatabase.GetAssetPath(_json);
			path = Path.Combine(Application.dataPath.Replace("Assets", ""), path);
			File.WriteAllText(path, text);
			UnityEditor.EditorUtility.SetDirty(_json);
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}