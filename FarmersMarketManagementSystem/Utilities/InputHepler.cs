namespace FarmersMarketManagementSystem.Utilities
{
    internal static class InputHelper
    {
        public static int? GetIntInput(string message)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int output))
            {
                return output;
            }
            else
            {
                return null;
            }
        }
        public static string? GetUpdateValue(string fieldName, string currentValue)
        {
            Console.Write($"請輸入新的{fieldName}（目前：{currentValue}，直接按 Enter 保留）：");
            return Console.ReadLine();
        }
    }
}
