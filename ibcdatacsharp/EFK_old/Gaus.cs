namespace ibcdatacsharp.EFK_old
{
    public static class Gaus
    {
        public static float[,] Inverse(ref float[,] matrix)
        {
            float[,] result = new float[matrix.GetLength(0), matrix.GetLength(1)];
            for(int i = 0; i < result.GetLength(0); i++)
            {
                for(int j = 0; j < result.GetLength(1); j++)
                {
                    if(i == j)
                    {
                        result[i, j] = 1;
                    }
                    else
                    {
                        result[i, j] = 0;
                    }
                }
            }
            for (int r = 0; r < matrix.GetLength(0); r++)
            {
                float fraction = 1 / matrix[r, r];
                for (int c = 0; c < matrix.GetLength(1); c++)
                {
                    matrix[r, c] *= fraction;
                    result[r, c] *= fraction;
                }
                for (int r_ = r + 1; r_ < matrix.GetLength(0); r_++)
                {
                    float fraction_ = -matrix[r_, r];
                    for (int c = 0; c < matrix.GetLength(1); c++)
                    {
                        matrix[r_, c] = matrix[r_, c] + matrix[r, c] * fraction_;
                        result[r_, c] = result[r_, c] + result[r, c] * fraction_;
                    }
                }
            }
            for (int r = matrix.GetLength(0) - 1; r > 0; r--)
            {
                for(int r_ = r - 1; r_ >= 0; r_--)
                {
                    float fraction = -matrix[r_, r];
                    for(int c = 0; c < matrix.GetLength(1); c++)
                    {
                        matrix[r_, c] = matrix[r_, c] + matrix[r, c] * fraction;
                        result[r_, c] = result[r_, c] + result[r, c] * fraction;
                    }
                }
            }
            return result;
        }
    }
}
