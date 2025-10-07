namespace Minefield.Persistence
{
	public class Bomb
	{
		#region Fields

		private Int32 _sinkRate;
		private Double lastUpdated;

		#endregion

		#region Properties

		public Weight Weight { get; private set; }

		#endregion

		#region Constructor

		public Bomb(Weight weight)
		{
			Weight = weight;
			SetSinkRate();
		}

		#endregion

		#region Public methods

		public Boolean UpdatePosition(Double dt)
		{
			lastUpdated += dt;
			if (lastUpdated >= _sinkRate)
			{
				lastUpdated = 0;
				return true;
			}
			return false;
		}

		#endregion

		#region Private methods

		private void SetSinkRate()
		{
			// 1 second for starting
			switch (Weight)
			{
				case Weight.LIGHT:
					_sinkRate = 1300;
					break;
				case Weight.MEDIUM:
					_sinkRate = 1000;
					break;
				case Weight.HEAVY:
					_sinkRate = 800;
					break;
			}
		}

		#endregion
	}
}
