using System;
using System.Collections.Generic;

namespace TrainsDemo.Model.IntermediateData
{
	/// <summary>
	/// Промежуточный класс "поезд".
	/// Составляется из TrainRow с одинаковыми номерами поездов и конечными станциями.
	/// Внутри поезда TrainRow разбиваются на варианты поезда.
	/// </summary>
	public class Train : IEquatable<Train>
	{
		#region Fields

		private static int idGenerator = 0;

		private List<TrainVariant> trainVariants = new List<TrainVariant>();

		#endregion Fields

		#region Properties

		/// <summary>
		/// Сгенерировать новый отрицательный ID
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
		/// ID поезда в базе данных "Компас", для новых поездов ID = -1
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Название поезда
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Описание поезда
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Ключ поезда - основные данные, определяющие поезд.
		/// </summary>
		public Key TrainKey { get; private set; }

		/// <summary>
		/// Название начальной станции поезда.
		/// </summary>
		public string InitStationName { get; set; }

		/// <summary>
		/// Название конечной станции поезда.
		/// </summary>
		public string FinStationName { get; set; }

		/// <summary>
		/// ID начальной станции
		/// </summary>
		public int InitMapStationID
		{
			get
			{
				return 0;// AddonMain.ImportHelper.Stations.MapStationIndex[TrainKey.InitStationExpress].MapID;
			}
		}

		/// <summary>
		/// ID конечной станции
		/// </summary>
		public int FinMapStationID
		{
			get
			{
				return 0;// AddonMain.ImportHelper.Stations.MapStationIndex[TrainKey.FinStationExpress].MapID;
			}
		}

		/// <summary>
		/// Пара, содержащая этот поезд
		/// </summary>
		public TrainPair TrainPair { get; set; }

		/// <summary>
		/// ID парного поезда из базы
		/// </summary>
		public int? OtherPairTrainIDfromDB { get; set; }

		/// <summary>
		/// Список всех вариантов поезда
		/// </summary>
		public IList<TrainVariant> Variants
		{
			get { return (trainVariants); }
		}

		/// <summary>
		/// Признак, запрещающий поиск существующего поезда в базе. Необходимо, что бы исключить попадание поезда в 2 пары, когда
		/// один из поездов полной пары преобразуется в группу и исчезает, а пара была уже существующей и полной, но стала неполной,
		/// т.е новой.
		/// </summary>
		public bool DoNotSearchDB { get; set; }

		/// <summary>
		/// Буква поезда, используется только при обновлении из ЭКСПРЕСС
		/// </summary>
		public string Letter { get; set; }

		#endregion Properties

		public Train(Key k)
		{
			// --- создаём новый ID (отрицательный, положительный будет присвоен при прокладывании маршрута) ---
			ID = --idGenerator;
			TrainKey = k;
			Description = string.Empty;
			Name = string.Empty;
			OtherPairTrainIDfromDB = null;
			DoNotSearchDB = false;
		}

		/// <summary>
		/// Создаёт новый вариант поезда по графику.
		/// Добавляет в список и возвращает.
		/// </summary>
		/// <param name="graphic">График движения, к которому относится вариант поезда</param>
		/// <returns>Новый вариант</returns>
		public TrainVariant AddVariant(TrainGraphic.Graphic graphic, TimeSpan initTime)
		{
			TrainVariant newVariant = new TrainVariant(this, graphic, initTime);

			trainVariants.Add(newVariant);

			return (newVariant);
		}

		/// <summary>
		/// Реализация IEquatable
		/// </summary>
		public bool Equals(Train other)
		{
			if (other == null)
			{
				return false;
			}

			if (TrainKey != other.TrainKey)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Класс - ключ, по которому сравниваются поезда, содержит часть свойств поезда. 
		/// Использовать в качастве индекса в списках. хоть в Dictionary, хоть в SortedList
		/// Если у поездов однаковй Key - значит это один и тот же поезд.
		/// </summary>
		public class Key : IComparable<Key>, IEquatable<Key>
		{
			/// <summary>
			/// Номер поезда
			/// </summary>
			public int Number { get; private set; }

			/// <summary>
			/// Код эспресс начальной станции
			/// </summary>
			public int InitStationExpress { get; private set; }

			/// <summary>
			/// Код экспресс конечной станции
			/// </summary>
			public int FinStationExpress { get; private set; }

			#region Constructors
			//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
			public Key(int num, int init, int fin)
			{
				Number = num;
				InitStationExpress = init;
				FinStationExpress = fin;
			}
			//---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---
			public Key(Key other)
			{
				Number = other.Number;
				InitStationExpress = other.InitStationExpress;
				FinStationExpress = other.FinStationExpress;
			}

			#endregion Constructors

			/// <summary>
			/// Реализация IComparable
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public int CompareTo(Key other)
			{
				if (other == null) return 1;

				if (Number != other.Number)
					return Number.CompareTo(other.Number);
				else if (InitStationExpress != other.InitStationExpress)
					return InitStationExpress.CompareTo(other.InitStationExpress);
				else
					return FinStationExpress.CompareTo(other.FinStationExpress);
			}

			/// <summary>
			/// Реализация IEquatable
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public bool Equals(Key other)
			{
				if (other == null)
					return false;

				if (Number == other.Number &&
					InitStationExpress == other.InitStationExpress &&
					FinStationExpress == other.FinStationExpress)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			public override bool Equals(object obj)
			{
				if (!(obj is Train.Key))
					return false;
				else if (ReferenceEquals(this, obj))
					return true;
				else
					return Equals(obj as Train.Key);
			}

			/// <summary>
			/// Implements the operator ==.
			/// </summary>
			/// <param name="x">The x.</param>
			/// <param name="y">The y.</param>
			/// <returns>
			/// The result of the operator.
			/// </returns>
			public static bool operator ==(Train.Key x, Train.Key y)
			{
				return x?.Equals(y) ?? ReferenceEquals(y, null);
			}

			/// <summary>
			/// Implements the operator !=.
			/// </summary>
			/// <param name="x">The x.</param>
			/// <param name="y">The y.</param>
			/// <returns>
			/// The result of the operator.
			/// </returns>
			public static bool operator !=(Train.Key x, Train.Key y)
			{
				return !(x == y);
			}

			public override int GetHashCode()
			{
				int hash = 17;

				unchecked                               // не учитываем Overflow
				{
					hash = hash * 23 + Number.GetHashCode();
					hash = hash * 23 + InitStationExpress.GetHashCode();
					hash = hash * 23 + FinStationExpress.GetHashCode();
					return hash;
				}
			}
		}
	}
}
