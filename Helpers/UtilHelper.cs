using System.Collections.Generic;

namespace XiaoTasiBackend.Helpers
{
    public class UtilHelper
    {
        public UtilHelper()
        {
        }

        public string[] stringArrPop(string[] stringArr)
        {
            Stack<string> stack = new Stack<string>(stringArr);
            stack.Pop();
            return stack.ToArray();
        }

        public int[] stringArrPop(int[] intArr)
        {
            Stack<int> stack = new Stack<int>(intArr);
            stack.Pop();
            return stack.ToArray();
        }
    }
}
