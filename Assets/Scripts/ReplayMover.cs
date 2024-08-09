using System;
using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(PositionSaver))]
	public class ReplayMover : MonoBehaviour
	{
		private PositionSaver _save;

		private int _index;
		private PositionSaver.Data _prev;
		private float _duration;

		private void Start()
		{
            ////todo comment: зачем нужны эти проверки?
            ///answer: Проверяем, что у нас есть компонент и он не пустой. Если хотя бы одно не выполнено попадаем в if и получаем ошибку. Так же если компонент есть, сохраняем его в _save
			///не понял только почему в TryGetComponent не указано какой именно компонент пытаемся получить? В документации сказано, что выполняется поиск компонента заданного типа, но тип указывается в <>, 
			///а здесь он не указан. Предполагаю, что достаточно того, что в данном случае тип определяется по типу переменной в параметре out.
            if (!TryGetComponent(out _save) || _save.Records.Count == 0)
			{
				Debug.LogError("Records incorrect value", this);
                //todo comment: Для чего выключается этот компонент?
                //answer: чтобы при отсутствии записей в _save не выполнялся метод Unpdate, в котором из-за этого будут ошибки. Видимо предполагается, что он должен включаться из другого скрипта.
                enabled = false;
			}
		}

		private void Update()
		{
			var curr = _save.Records[_index];
            //todo comment: Что проверяет это условие (с какой целью)? 
            //answer: проверяем, что время с момента запуска игры больше чем время из считанной записи в _save
            if (Time.time > curr.Time)
			{
				_prev = curr;
				_index++;
                //todo comment: Для чего нужна эта проверка?
                //answer: _index увеличивается каждый кадр и если индекс больше, чем количество записей в _save, т.е. все записи изменения позиций считали, то выключаем компонент, так как он выполнил свою задачу. 
                if (_index >= _save.Records.Count)
				{
					enabled = false;
					Debug.Log($"<b>{name}</b> finished", this);
				}
			}
            //todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
            //answer: Данные вычисления сохраняют в delta отношение между тем, сколько времени прошло от запуска игры до предыдущего события и тем, сколько времени прошло от последнего события до текущего события.
			//Коэффециент delta используется далее для смещения объекта через интерполяцию
            var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
            //todo comment: Зачем нужна эта проверка?
            //ansewr:проверяем, что delta не является числом и в этом случае присваивает в delta значение 0 .
            if (float.IsNaN(delta)) delta = 0f;
            //todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
            //answer: изменяем текущую позицию объекта чеоез линейную интерполяцию между двумя позициями: _prev.Position и curr.Position, со смещением на delta, т.е. от позции смещает объект в сторону позиции curr.Position на значение в delta.
            transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
		}
	}
}