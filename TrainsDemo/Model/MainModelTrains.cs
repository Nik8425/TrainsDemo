using System.Collections.Generic;
using TrainsDemo.Model.IntermediateData;

namespace TrainsDemo.Model
{
	public partial class MainModel
	{
		#region Fields

		// список поездов, Key - ключ поезда, Value - поезд
		private SortedList<Train.Key, Train> trains = new SortedList<Train.Key, Train>();

		// Список пар поездов. Key - поезд "туда". Value - пара поездов
		private SortedList<Train.Key, TrainPair> trainPairs = new SortedList<Train.Key, TrainPair>();

		#endregion Fields

		#region Properties

		/// <summary>
		/// Список всех загруженных поездов
		/// </summary>
		public IList<Train> Trains
		{
			get { return (trains.Values); }
		}

		/// <summary>
		/// Список пар поездов
		/// </summary>
		public IList<TrainPair> TrainPairs
		{
			get { return (trainPairs.Values); }
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Первичное создание поездов и вариантов из скаченных данных.
		/// Варианты создаются один на график.
		/// </summary>
		public void MakeTrainsAndInitialVariants()
		{
			//FIX ME: возможно будет загрузка из базы
		}

		/// <summary>
		/// Добавляет поезд
		/// </summary>
		public void AddTrain(Train train)
		{
			if (train != null && !trains.ContainsKey(train.TrainKey))
			{
				trains.Add(train.TrainKey, train);
			}
		}

		/// <summary>
		/// Создание списка пар поездов
		/// </summary>
		public void MakeTrainPairs()
		{
			SortedDictionary<Train.Key, bool> copyTrains = new SortedDictionary<Train.Key, bool>();
			foreach (Train train in trains.Values)
			{
				copyTrains.Add(train.TrainKey, false);
			}

			foreach (Train train1 in trains.Values)
			{
				if (!copyTrains[train1.TrainKey])
				{
					TrainPair trainPair = new TrainPair();
					Train train2 = trainPair.FindPairTrain(trains.Values, train1);
					if (train2 == null)
					{
						trainPair.aTrain = train1;
						copyTrains[train1.TrainKey] = true;
					}
					else
					{
						trainPair.SetDirectAndReturnTrains(train1, train2);
						copyTrains[train1.TrainKey] = true;
						copyTrains[train2.TrainKey] = true;
					}

					train1.TrainPair = trainPair;
					if (train2 != null)
					{
						train2.TrainPair = trainPair;
					}

					if (!trainPairs.ContainsKey(trainPair.aTrain.TrainKey))
					{
						trainPairs.Add(trainPair.aTrain.TrainKey, trainPair);
					}
				}
			}
		}

		#endregion Methods
	}
}
