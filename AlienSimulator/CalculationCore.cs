using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlienSimulator
{
    static class CalculationCore
    {
        /// <summary>
        /// 计算精度。
        /// </summary>
        public static double Precision { get; set; }

        /// <summary>
        /// 列表中第 i 个数组表示 i 个外星人时下一代时出现各数量的概率。
        /// </summary>
        static readonly List<double[]> reproduces = new List<double[]>
        {
            new double[] { 1 },                         // 外星人数为 0 时，下一代数量只能为 0。
            new double[] { 0.25, 0.25, 0.25, 0.25 },    // 外星人数为 1 时，下一代数量为 0~3 个的概率相等。
        };

        /// <summary>
        /// 获取指定数量外星人下一代时出现各数量的概率。
        /// </summary>
        /// <param name="population">当前外星人的人口数量。</param>
        /// <returns>一个数组表示下一代时出现各数量的概率。</returns>
        public static double[] GetReproduce(int population)
        {
            if (population < 0) throw new ArgumentOutOfRangeException("population", "外星人数量不能小于 0。");
            if (population >= reproduces.Count)
            {
                double[] result = new double [population * 3 + 1];
                double[] previous = GetReproduce(population - 1);
                double[] one = GetReproduce(1);
                for (int i = 0; i < previous.Length; i++)
                {
                    for (int j = 0; j < one.Length; j++)
                    {
                        result[i + j] += previous[i] * one[j];
                    }
                }
                reproduces.Add(result);
            }
            return reproduces[population];
        }

        /// <summary>
        /// 获取下一代外星人数量概率。
        /// </summary>
        /// <param name="current">一个数组，数组中第 i 个元素表示当前外星人数量为 i 的概率。</param>
        /// <returns>一个数组，数组中第 i 个元素表示下一代外星人数量为 i 的概率。</returns>
        public static double[] GetNextGenerationProbability(double[] current)
        {
            if (current == null) throw new ArgumentNullException("current");
            if (current.Length == 0) throw new ArgumentOutOfRangeException("current", "表示当前外星人数量概率的数组不得为空。");
            int currentMax = current.Length - 1;    // 当前外星人最大数量
            int nextMax = currentMax * 3;           // 下一代外星人最大数量
            double[] next = new double[nextMax + 1];
            for (int i = 0; i < current.Length; i++)
            {
                double[] reproduce = GetReproduce(i);
                for (int j = 0; j < reproduce.Length; j++)
                {
                    next[j] += current[i] * reproduce[j];
                }
            }
            int pruneCount = 0;
            while (nextMax - pruneCount > 0 && next[nextMax - pruneCount] <= Precision) pruneCount++;
            double[] result = new double[nextMax - pruneCount + 1];
            Array.Copy(next, result, nextMax - pruneCount + 1);
            return result;
        }

        /// <summary>
        /// 异步获取下一代外星人数量概率。
        /// </summary>
        /// <param name="current">一个数组，数组中第 i 个元素表示当前外星人数量为 i 的概率。</param>
        /// <returns>一个任务。任务的结果为一个数组，数组中第 i 个元素表示下一代外星人数量为 i 的概率。</returns>
        public static async Task<double[]> GetNextGenerationProbabilityAsync(double[] current)
        {
            double[] result = null;
            await Task.Run(() => result = GetNextGenerationProbability(current));
            return result;
        }
    }
}
