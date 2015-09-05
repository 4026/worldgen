using UnityEngine;
using System.Collections.Generic;

public class DiamondSquareHeightmap : AbstractHeightmap
{
	struct Point
	{
		public int x, y;
		public Point (int newX, int newY)
		{
			x = newX;
			y = newY;
		}
	}

	struct Square
	{
		public Point topLeft;
		public int size;

		public Square (Point newTopLeft, int newSize)
		{
			topLeft = newTopLeft;
			size = newSize;
		}

		public Point topRight { 
			get { return new Point (topLeft.x + size, topLeft.y); } 
		}
		public Point bottomLeft { 
			get { return new Point (topLeft.x, topLeft.y + size); } 
		}
		public Point bottomRight { 
			get { return new Point (topLeft.x + size, topLeft.y + size); } 
		}
		public Point[] corners { 
			get { return new Point[] { topLeft, topRight, bottomLeft, bottomRight}; }
		}
		public Point center { 
			get { return new Point (topLeft.x + (size / 2), topLeft.y + (size / 2)); } 
		}
	}

	struct Diamond
	{
		public Point center;
		public int size;
		public int wrapBoundary;
		
		public Diamond (Point newCenter, int newSize, int newWrapBoundary)
		{
			center = newCenter;
			size = newSize;
			wrapBoundary = newWrapBoundary;
		}
		
		public Point top { 
			get { return new Point (center.x, (center.y - size + wrapBoundary) % wrapBoundary); } 
		}
		public Point bottom { 
			get { return new Point (center.x, (center.y + size) % wrapBoundary); } 
		}
		public Point left { 
			get { return new Point ((center.x - size + wrapBoundary) % wrapBoundary, center.y); } 
		}
		public Point right { 
			get { return new Point ((center.x + size) % wrapBoundary, center.y); } 
		}

		public Point[] corners { 
			get { return new Point[] { left, top, right, bottom }; }
		}
	}

	/// <summary>
	/// The values that will be used to seed the array before generation.
	/// </summary>
	private Dictionary<int, float> m_seedValues = new Dictionary<int, float> ();

	public DiamondSquareHeightmap (int newSize) : base(newSize)
	{
		//By default, seed the four corners with a value of 0.5
		SetSeedValue (0, 0, 0.5f);
		SetSeedValue (0, size - 1, 0.5f);
		SetSeedValue (size - 1, 0, 0.5f);
		SetSeedValue (size - 1, size - 1, 0.5f);
	}

	protected override int getRealSize (int newSize)
	{
		int realSize = (int)System.Math.Pow (2, System.Math.Ceiling (System.Math.Log (newSize, 2))) + 1;
		return realSize;
	}

	public void SetSeedValue (int x, int y, float value)
	{
		m_seedValues [y * size + x] = value;
	}

	public override void generate ()
	{
		//Reset the array and apply any seed values before generation.
		initialiseArray ();

		int squareWidth = size - 1;
		float randomScale = 1.0f;
		float totalValue, randomValue, mean, finalValue;
		int halfSquareWidth;
		Square square;
		Diamond diamond;
		Point corner;

		while (squareWidth > 1) {
			halfSquareWidth = squareWidth / 2;

			//Iterate through squares in the array
			for (int x = 0; x < size - 1; x += squareWidth) {
				for (int y = 0; y < size - 1; y += squareWidth) {

					//Get a square of points in the array
					square = new Square (new Point (x, y), squareWidth);

					//If the value for this square has already been set, move on.
					if (m_heights [square.center.y, square.center.x] != -1.0f) {
						continue;
					}

					//Iterate through corners of the square, calculating their mean value.
					totalValue = 0f;
					for (int i = 0; i < 4; ++i) {
						corner = square.corners [i];
						totalValue += m_heights [corner.y, corner.x];
					}

					//Set center of square to mean value plus random value
					mean = (totalValue / 4.0f);
					randomValue = Random.Range (-randomScale, randomScale);
					finalValue = Mathf.Clamp (mean + randomValue, 0.0f, 1.0f);
					m_heights [square.center.y, square.center.x] = finalValue;
				}
			}

			//Iterate through diamonds in the array
			for (int x = 0; x < size - 1; x += halfSquareWidth) {
				for (int y = (x + halfSquareWidth) % squareWidth; y < size - 1; y += squareWidth) {

					//If the value for this diamond has already been set, move on.
					if (m_heights [y, x] != -1.0f) {
						continue;
					}

					//Get a diamond of points in the array
					diamond = new Diamond (new Point (x, y), halfSquareWidth, size - 1);
					
					//Iterate through corners of the diamond, calculating their mean value.
					totalValue = 0f;
					for (int i = 0; i < 4; ++i) {
						corner = diamond.corners [i];
						totalValue += m_heights [corner.y, corner.x];
					}
					
					//Set center of diamond to mean value plus random value
					mean = (totalValue / 4.0f);
					randomValue = Random.Range (-randomScale, randomScale);
					finalValue = Mathf.Clamp (mean + randomValue, 0.0f, 1.0f);
					m_heights [diamond.center.y, diamond.center.x] = finalValue;

					//Wrap values on the edges
					if (x == 0) {
						m_heights [y, size - 1] = finalValue;
					}
					if (y == 0) {
						m_heights [size - 1, x] = finalValue;
					}
				}
			}

			//Reduce square width and random scale
			squareWidth /= 2;
			randomScale /= 2;
		}
	}

	/// <summary>
	/// Initialises the array by setting any seed values that have been specified, and setting everything else to -1.
	/// </summary>
	private void initialiseArray ()
	{
		int seedValueKey;
		for (int x = 0; x < size; ++x) {
			for (int y = 0; y < size; ++y) {
				seedValueKey = y * size + x;
				if (m_seedValues.ContainsKey (seedValueKey)) {
					m_heights [y, x] = m_seedValues [seedValueKey];
				} else {
					m_heights [y, x] = -1.0f;
				}
			}
		}
	}
}
