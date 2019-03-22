using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers
{
    public static class AdditionGroupAlgorithm
    {
       public static List<int> GetGroups(int[] numbers)
        {
            List<int> t = new List<int>();
            var result = new int[numbers.Length];
            Backtrace(0, result,numbers, t);
            return t;
        }

        static void Backtrace(int i, int[] result,int[] numbers, List<int> t)
        {
            if (i >= numbers.Length)
            {
                return;
            }
            result[i] = 1; //选中当前数字

            PrintResult(result,numbers, t); //输出结果

            Backtrace(i + 1, result,numbers, t); //选择下一数字

            result[i] = 0; //剔除当前数字   

            Backtrace(i + 1, result,numbers, t); //选择下一数字

        }

        static void PrintResult(int[] result,int[] numbers, List<int> t)
        {

            int count = 0;
            int total = 0;
            for (int i = 0; i < result.Length; i++)
            {
                count += result[i];

                if (result[i] == 1)
                {
                    total += numbers[i];
                }
            }
            t.Add(total);
        } 
    }
}
