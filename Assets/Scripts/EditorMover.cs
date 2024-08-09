using UnityEngine;

namespace DefaultNamespace
{
	
	[RequireComponent(typeof(PositionSaver))]
	public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;
		private float _currentDelay;

		//todo comment: Что произойдёт, если _delay > _duration?
		//answer: в список _save.Records будет добавлена только одна запись, так как компонент будет отключен раньше чем  _currentDelay снова станет <=0 и перемещение объекта не будет происходить.
		[SerializeField, Range(0.2f, 1.0f)]        
		private float _delay = 0.5f;
		[SerializeField, Min(0.2f)]
		private float _duration = 5f;

		private void Start()
		{
			//todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
			//достаточно получить компонент 1 раз. Если получать его в каждом кадре (в unpdate), это ухудшит производительность.
			_save = GetComponent<PositionSaver>();
			_save.Records.Clear();
			if (_duration <= _delay)
			{
				_duration = _delay * 5f;
			}
		}

		private void Update()
		{
			_duration -= Time.deltaTime;
			if (_duration <= 0f)
			{
				enabled = false;
				Debug.Log($"<b>{name}</b> finished", this);
				return;
			}

            //todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
            //answer: Потому, что в ней храниться значение, которое нам нужно использовать несколько раз, для перезаписи остатка времени до следующего сохранения, а _duration используется только один раз. 
            _currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				_currentDelay = _delay;
				_save.Records.Add(new PositionSaver.Data
				{
					Position = transform.position,
                    //todo comment: Для чего сохраняется значение игрового времени?
                    //answer: это значение используется в компоненте ReplayMove для расчета коэффециента интерполяции.
                    Time = Time.time,
				});
			}
		}
	}
}