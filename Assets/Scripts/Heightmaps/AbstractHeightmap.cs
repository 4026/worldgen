using System;

public abstract class AbstractHeightmap
{
	public readonly int size;
	protected float[,] m_heights;

	public AbstractHeightmap (int newSize)
	{
		size = getRealSize (newSize);
		m_heights = new float[size, size];
	}

	protected virtual int getRealSize (int newSize)
	{
		return newSize;
	}

	public abstract void generate ();

	public float[,] getHeights ()
	{
		return m_heights;
	}

	public float getHeightAt (int x, int y)
	{
		return m_heights [y, x];
	}
}
