using UnityEngine;

public static class FalloffGenerator
{
    public static float[,] GenerateFallofMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }

        return map;
    }

    // fonction de transfert pour lisser le ton monochrome et contrôler le niveau de gris pour le contour de la map
    // f(x) = x ^ a / ( x ^ a + ( b - bx ) ^ a)
    static float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow((b - b * value), a));
    }
}
