using System;
using System.Collections.Generic;

namespace TrainsDemo.Model.IntermediateData
{
	/// <summary>
	/// Класс для пар поездов. Пара - это два поезда: "Туда-Обратно".
	/// Для непарных поездов не будет поезда "Обратно".
	/// </summary>
	public class TrainPair
	{
		// Для генерации ID
		private static int idGenerator = 0;

		/// <summary>
		/// Сгенерировать новый ID
		/// </summary>
		public static int IdGenerator
		{
			get
			{
				idGenerator--;
				return idGenerator;
			}
		}

		/// <summary>
		/// ID пары поезда
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Описание пары поездов
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Поезд "Туда"
		/// </summary>
		public Train aTrain { get; set; }

		/// <summary>
		/// Поезд "Обратно"
		/// </summary>
		public Train bTrain { get; set; }

		public TrainPair()
		{
			// --- создаём новый ID ---
			ID = --idGenerator;

			Description = string.Empty;
		}

		/// <summary>
		/// Возвращает парный поезд
		/// </summary>
		public Train FindPairTrain(IEnumerable<Train> trains, Train train1)
		{
			foreach (Train train2 in trains)
			{
				//у парных поездов взаимнообратные маршруты
				if (train2.TrainKey.InitStationExpress == train1.TrainKey.FinStationExpress &&
					train2.TrainKey.FinStationExpress == train1.TrainKey.InitStationExpress)
				{
					if (train1.TrainKey.Number % 2 != 0)
					{
						//номер нечётный, номер парного поезда должен быть на единицу больше или такой же
						if (train2.TrainKey.Number == train1.TrainKey.Number || train2.TrainKey.Number - train1.TrainKey.Number == 1)
						{
							return train2;
						}
					}
					else
					{
						//номер чётный, номер парного поезда должен быть на единицу меньше или такой же
						if (train2.TrainKey.Number == train1.TrainKey.Number || train1.TrainKey.Number - train2.TrainKey.Number == 1)
						{
							return train2;
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Из двух парных поездов, определяет какой поезд "туда", а какой "обратно", и устанавливает соответствующие свойства
		/// </summary>
		public void SetDirectAndReturnTrains(Train train1, Train train2)
		{
			if (train1.TrainKey.Number == train2.TrainKey.Number || Math.Abs(train1.TrainKey.Number - train2.TrainKey.Number) == 1)
			{
				if (train1.TrainKey.Number <= train2.TrainKey.Number)
				{
					aTrain = train1;
					bTrain = train2;
				}
				else if (train1.TrainKey.Number > train2.TrainKey.Number)
				{
					aTrain = train2;
					bTrain = train1;
				}
			}
		}
	}
}