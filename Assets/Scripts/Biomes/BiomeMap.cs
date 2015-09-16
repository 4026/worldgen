using System.Collections.Generic;
using UnityEngine;

namespace Biomes
{
    public class BiomeMap
    {
        private BiomeMapPixel[,] m_data;
        private TerrainGenerator m_terrainData;
        public int Size
        {
            get { return m_terrainData.Heightmap.size; }
        }

        public BiomeMap(TerrainGenerator newTerrainData)
        {
            m_terrainData = newTerrainData;
            m_data = new BiomeMapPixel[Size, Size];

            //Set biome weights for all pixels
            Point position;
            for (int x = 0; x < Size; ++x)
            {
                for (int y = 0; y < Size; ++y)
                {
                    position = new Point(x, y);
                    m_data[y, x] = new BiomeMapPixel(CalculateBiomeWeightsAtPoint(position));
                }
            }

            //Iterate through all pixels, setting their parent Biome

            Biome biome;
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
                    biome = new Biome(m_data[y, x].GetPrimaryBiome());

                    //Flood fill the map with that biome.
                    floodFill(new Point(x, y), biome);

                    //Generate a name for that biome, now that it knows how big it is.
                    biome.GenerateName();
                }
            }

        }

        private void floodFill(Point startPoint, Biome parentBiome)
        {
            Queue<Point> queue = new Queue<Point>();
            HashSet<Point> queuedPoints = new HashSet<Point>();
            queue.Enqueue(startPoint);
            queuedPoints.Add(startPoint);

            Point currentPoint;
            BiomeMapPixel currentPixel;
            while (queue.Count > 0)
            {
                currentPoint = queue.Dequeue();
                currentPixel = m_data[currentPoint.y, currentPoint.x];
                if (currentPixel.GetPrimaryBiome() == parentBiome.Type)
                {
                    currentPixel.ParentBiome = parentBiome;
                    ++parentBiome.Size;
                    foreach (Point neighbour in currentPoint.Neighbours)
                    {
                        if (neighbour.IsInBounds(0, 0, Size, Size) && GetPixelAtPoint(neighbour).ParentBiome == null && !queuedPoints.Contains(neighbour))
                        {
                            queue.Enqueue(neighbour);
                            queuedPoints.Add(neighbour);
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

        private float[] CalculateBiomeWeightsAtPoint(Point position)
        {
            float heightAboveSeaLevel = m_terrainData.getHeightAboveSeaLevelAt(position);
            float latitude = Mathf.Clamp(position.y / (float)m_terrainData.Heightmap.size, 0f, 1f);
            float temperature = latitude * (1 - Mathf.Clamp(heightAboveSeaLevel, 0f, 1f));

            float precipitation = m_terrainData.Rainmap.getValueAt(position);

            return BiomeCalculator.Instance.getBiomeWeights(temperature, precipitation, heightAboveSeaLevel);
        }

    }
}