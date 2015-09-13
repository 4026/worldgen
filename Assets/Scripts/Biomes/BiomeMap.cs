using System.Collections.Generic;
using UnityEngine;

public class BiomeMap
{
	private BiomeMapPixel[,] m_data;
    private TerrainGenerator m_terrainData;
    public int Size {
        get { return m_terrainData.Heightmap.size; }
    }

    public BiomeMap (TerrainGenerator newTerrainData)
	{
        m_terrainData = newTerrainData;
        m_data = new BiomeMapPixel[Size, Size];

        //Set biome weights for all pixels
        for (int x = 0; x < Size; ++x)
        {
            for (int y = 0; y < Size; ++y)
            {
                m_data[y, x] = new BiomeMapPixel(CalculateBiomeWeightsAtPoint(x, y));
            }
        }

        //Iterate through all pixels, setting their parent Biome
        for (int x = 0; x < Size; ++x)
        {
            for (int y = 0; y < Size; ++y)
            {
                //Ignore pixels that have already had their parent biome set.
                if (m_data[y, x].ParentBiome != null)
                {
                    continue;
                }

                //Generate a new Biome object from the primary biome type of this pixel.
                Biome biome = new Biome(m_data[y, x].GetPrimaryBiome());

                //Flood fill the map with that biome.
                floodFill(new Point(x, y), biome);
            }
        }
    }

    private void floodFill(Point startPoint, Biome parentBiome)
    {
        Queue<Point> q = new Queue<Point>();
        q.Enqueue(startPoint);

        while (q.Count > 0)
        {
            Point currentPoint = q.Dequeue();
            BiomeMapPixel currentPixel = m_data[currentPoint.y, currentPoint.x];
            if (currentPixel.GetPrimaryBiome() == parentBiome.Type)
            {
                currentPixel.ParentBiome = parentBiome;
                foreach (Point neighbour in currentPoint.Neighbours)
                {
                    if (neighbour.IsInBounds(0, 0, Size, Size) && GetPixelAtPoint(neighbour).ParentBiome == null && !q.Contains(neighbour))
                    {
                        q.Enqueue(neighbour);
                    }
                }
            }
        }
    }

    public BiomeMapPixel GetPixelAtPoint(int x, int y)
    {
        return m_data[y, x];
    }

    public BiomeMapPixel GetPixelAtPoint(Point point)
    {
        return m_data[point.y, point.x];
    }

    private float[] CalculateBiomeWeightsAtPoint(int x, int y)
    {
        float heightAboveSeaLevel = Mathf.Clamp((m_terrainData.Heightmap.getValueAt(x, y) - m_terrainData.SeaLevel) / (1 - m_terrainData.SeaLevel), 0f, 1f);
        float latitude = Mathf.Clamp(y / (float) m_terrainData.Heightmap.size, 0f, 1f);
        float temperature = (4 * latitude + 1 - heightAboveSeaLevel) / 5f;
        float precipitation = m_terrainData.Rainmap.getValueAt(x, y);
        return BiomeCalculator.Instance.getBiomeWeights(temperature, precipitation);
    }

}
