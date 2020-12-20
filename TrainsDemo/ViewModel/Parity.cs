namespace TrainsDemo.ViewModel
{
	/// <summary>
	/// extension для реализации инвертирования чётности
	/// </summary>
	public static class ParityMethods
	{
		/// <summary>
		/// Возвращает чётность, обратную входящей.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static Parity Invert(this Parity s)
		{
			if (s == Parity.Odd)
				return (Parity.Even);
			else
				return (Parity.Odd);
		}
	}
}
