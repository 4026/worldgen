using System;
using UnityEngine;

public abstract class AbstractValueMap
{
	public readonly int size;
	protected float[,] m_values;

	protected Texture2D m_texture;

	public AbstractValueMap (int newSize)
	{
		size = getRealSize (newSize);
		m_values = new float[size, size];
	}

	protected virtual int getRealSize (int newSize)
	{
		return newSize;
	}

	public abstract void generate ();

	public float[,] getValues ()
	{
		return m_values;
	}

	public float getValueAt (int x, int y)
	{
		return m_values [y, x];
	}

	public Texture2D getTexture ()
	{
		Texture2D texture = new Texture2D (size, size);
		Color[] pixels = texture.GetPixels ();
		for (int x = 0; x < texture.width; ++x) {
			for (int y = 0; y < texture.height; ++y) {
				pixels [y * texture.width + x] = new Color (m_values [y, x], m_values [y, x], m_values [y, x]);
			}
		}
		
		texture.SetPixels (pixels);
		texture.Apply ();

		return texture;
	}
}
