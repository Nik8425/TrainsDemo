using System;
using System.Collections.Generic;
using System.Linq;
using TrainsDemo.Model;
using TrainsDemo.Model.IntermediateData;

namespace TrainsDemo
{
	public class Station
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public Station(int id, string name)
		{
			ID = id;
			Name = name;
		}
	}

	/// <summary>
	/// FIX ME: реализовать базу данных
	/// Класс для заполнения демонстрационного проекта случайными данными
	/// </summary>
	public class InfoGenerator
	{
		List<Station> stations;
		List<Train.Key> trainKeys;
		List<Train> trains;
		readonly Random random = new Random();
		MainModel mainModel;

		public InfoGenerator(MainModel model)
		{
			mainModel = model;
		}

		public void LoadTrains()
		{
			LoadStations();

			trainKeys = new List<Train.Key>()
			{
				new Train.Key(1, 1, 18),
				new Train.Key(2, 18, 1),
				new Train.Key(1, 1, 9),
				new Train.Key(2, 9, 1),
				new Train.Key(3, 1, 2),
				new Train.Key(4, 2, 1),
				new Train.Key(5, 1, 2),
				new Train.Key(6, 2, 1),
				new Train.Key(7, 1, 8),
				new Train.Key(8, 8, 1),
				new Train.Key(8, 1, 10)
			};

			trains = new List<Train>();
			TrainGraphic.Graphic graphic = new TrainGraphic.Graphic();
			graphic.Description = "График 2018/2019";

			Dictionary<int, Station> sts = stations.ToDictionary(s => s.ID, s => s);

			foreach (Train.Key trainKey in trainKeys)
			{
				Train train = new Train(trainKey);
				train.InitStationName = sts[trainKey.InitStationExpress].Name.ToUpper();
				train.FinStationName = sts[trainKey.FinStationExpress].Name.ToUpper();

				int maxCountVariants = random.Next(1, 10);

				List<int> delimiters = new List<int>();
				int totalRunsCount = random.Next(20, 365);
				for (int i = 0; i < maxCountVariants; i++)
				{
					int d = random.Next(1, totalRunsCount);
					if (!delimiters.Contains(d))
					{
						delimiters.Add(d);
					}
				}

				delimiters.Sort();

				for (int i = 0; i < delimiters.Count; i++)
				{
					TimeSpan initTime = new TimeSpan(random.Next(0, 23), random.Next(0, 59), 0);
					TrainVariant trainVariant = train.AddVariant(graphic, initTime);
					int dice = random.Next(1, 18);
					if (dice == 8)
					{
						trainVariant.Comment = "Комментарий";
					}

					GenerateRuns(train, delimiters, i, trainVariant);
				}

				mainModel.AddTrain(train);
			}

			mainModel.MakeTrainPairs();
		}

		public void LoadSchedule(TrainVariant trainVariant)
		{
			GenerateSchedule(trainVariant);
			RandomizeIncorrectSchedule(trainVariant);
		}

		private void LoadStations()
		{
			stations = new List<Station>()
			{
				new Station(1, "Москва"),
				new Station(2, "Санкт-Петербург"),
				new Station(3, "Красноярск"),
				new Station(4, "Челябинск"),
				new Station(5, "Воркута"),
				new Station(6, "Владивосток"),
				new Station(7, "Екатеринбург"),
				new Station(8, "Киев"),
				new Station(9, "Гомель"),
				new Station(10, "Краснодар"),
				new Station(11, "Томск"),
				new Station(12, "Улан-Батор"),
				new Station(13, "Озёрск"),
				new Station(14, "Долгопрудный"),
				new Station(15, "Арзамас 16"),
				new Station(16, "Киров"),
				new Station(17, "Благовещенск"),
				new Station(18, "Нью-Йорк :-)"),
				new Station(19, "Новосибирск"),
				new Station(20, "Минск"),
			};
		}

		public void GenerateSchedule(TrainVariant trainVariant)
		{
			int scheduleLength = random.Next(10, 80);
			int typeSpeed = random.Next(40, 180);
			DateTime startScheduleDate = new DateTime(1900, 1, 1).Add(trainVariant.InitTime);
			for (int p = 0; p < scheduleLength; p++)
			{
				int km = 0;

				if (trainVariant.Schedule.LastPart != null)
				{
					km = trainVariant.Schedule.LastPart.DistanceKM + random.Next(1, 100);
				}

				bool teh = random.Next(0, 10) > 8 ? true : false;
				int speed = typeSpeed + random.Next(-20, 20);

				DateTime? arrivalTime = GenerateArrivalTime(p, scheduleLength, trainVariant, km, speed);
				DateTime? departureTime = GenerateDepartureTime(trainVariant, p, startScheduleDate, 
					scheduleLength, arrivalTime);

				RandomizeHasTimes(ref arrivalTime, ref departureTime, p, scheduleLength);

				trainVariant.Schedule.AddNewPart(random.Next(1000000, 9999999),
												 "Станция " + (p + 1),
												 arrivalTime,
												 departureTime,
												 km,
												 teh);
			}
		}

		private int GenerateStayTime()
		{
			int stayTimeDice = random.Next(1, 100);
			int stayTimeMinutes = 0;
			if (stayTimeDice < 10)
			{
				stayTimeMinutes = random.Next(60, 240);
			}
			else if (stayTimeDice >= 10 && stayTimeDice < 30)
			{
				stayTimeMinutes = random.Next(10, 60);
			}
			else if (stayTimeDice >= 30 && stayTimeDice < 92)
			{
				stayTimeMinutes = random.Next(0, 12);
			}

			return stayTimeMinutes;
		}

		private DateTime? GenerateArrivalTime(int p, int scheduleLength, 
			TrainVariant trainVariant, int km, int speed)
		{
			DateTime? prevDepartureTime = null;
			int prevKM = 0;
			if (p > 0 && p < scheduleLength)
			{
				SchedulePart prevPart = trainVariant.Schedule.LastPart;
				if (prevPart.DepartureTime.HasValue)
				{
					prevDepartureTime = prevPart.DepartureTime;
					prevKM = prevPart.DistanceKM;
				}
				else
				{
					SchedulePart prevPartWithDeparture = prevPart.GetPreviousPartWithDeparture();
					if (prevPartWithDeparture != null)
					{
						prevDepartureTime = prevPartWithDeparture.DepartureTime;
						prevKM = prevPart.DistanceKM;
					}
				}
			}

			double hoursFromPrevHasDepartureStation = 0;
			if (prevDepartureTime.HasValue)
			{
				double kmDifference = km - prevKM;
				hoursFromPrevHasDepartureStation = kmDifference / speed;
				return prevDepartureTime.Value.AddHours(hoursFromPrevHasDepartureStation);
			}

			return null;
		}

		private DateTime? GenerateDepartureTime(TrainVariant trainVariant, int p, 
			DateTime startScheduleDate, int scheduleLength, DateTime? arrivalTime)
		{
			if (p == 0)
			{
				return startScheduleDate.Add(trainVariant.InitTime);
			}

			int stayTimeMinutes = GenerateStayTime();

			if (p != scheduleLength && p != 0 && arrivalTime.HasValue)
			{
				return arrivalTime.Value.AddMinutes(stayTimeMinutes);
			}

			return null;
		}

		private void RandomizeHasTimes(ref DateTime? arrivalTime, ref DateTime? departureTime,
			int p, int scheduleLength)
		{
			if (p != 0 && p < scheduleLength)
			{
				if (random.Next(1, 12) > 11)
				{
					arrivalTime = null;
				}

				if (random.Next(1, 12) > 11)
				{
					departureTime = null;
				}
			}
		}

		public void RandomizeIncorrectSchedule(TrainVariant trainVariant)
		{
			int incorrectSpeedDice = random.Next(1, 6);
			if (incorrectSpeedDice == 1)
			{
				trainVariant.Schedule.LastPart.ArrivalTime =
					trainVariant.Schedule.LastPart.ArrivalTime.Value.AddDays(4);
			}
			else if (incorrectSpeedDice == 2)
			{
				trainVariant.Schedule.FirstPart.DepartureTime = null;
			}
		}

		private void GenerateRuns(Train train, List<int> delimiters, int i, TrainVariant trainVariant)
		{
			int runsCount = 0;
			if (i == 0)
			{
				runsCount = random.Next(1, delimiters[i]);
			}
			else
			{
				runsCount = random.Next(1, delimiters[i] - delimiters[i - 1]);
			}

			DateTime? firstRunDate = null;
			if (i == 0)
			{
				firstRunDate = mainModel.BeginDate;
			}
			else
			{
				DateTime dateTime = train.Variants[train.Variants.Count - 2].TrainRuns.Last().Date;
				firstRunDate = dateTime.AddDays(delimiters[i]);
			}

			for (int j = 0; j < runsCount; j++)
			{
				DateTime runDate = firstRunDate.Value.AddDays(j + 1);
				TrainRun trainRun = new TrainRun(runDate);
				trainVariant.TryAddRun(trainRun);
			}
		}
	}
}