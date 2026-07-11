namespace FinanceConnect.Client.Services
{
    public class IndianNumberFormatter
    {
        public static string Format(decimal? amount)
        {
            if (amount == null)
                return "-";

            var value = amount.Value;

            var parts = value.ToString("0.00").Split('.');
            var integerPart = parts[0];
            var decimalPart = parts.Length > 1 ? "." + parts[1] : "";

            if (integerPart.Length <= 3)
                return integerPart + decimalPart;

            var lastThree = integerPart.Substring(integerPart.Length - 3);
            var remaining = integerPart.Substring(0, integerPart.Length - 3);

            var result = "";

            while (remaining.Length > 2)
            {
                result = "," + remaining.Substring(remaining.Length - 2) + result;
                remaining = remaining.Substring(0, remaining.Length - 2);
            }

            result = remaining + result;

            return result + "," + lastThree + decimalPart;
        }
    }
}
