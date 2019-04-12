using Rodonaves.EDI.Helpers.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Rodonaves.EDI.Helpers
{
    public class QueryValidationHelper
    {
        public static void ValidQuery(string query)
        {
            string validation = string.Empty;

            List<string> words = new List<string>();
            words.AddRange(query.Split(' '));

            List<string> unallowedWords = new List<string> { "UPDATE", "DELETE", "EXEC", "CALL", "INSERT", "ALTER", "CREATE", "DROP", "TRUNCATE", "RENAME", "SET", "COMMENT" };

            var result = words.Where(x => unallowedWords.Contains(x.ToUpper())).ToList();

            if (result.Count > 0)
            {
                string message = (result.Count == 1 ? "O comando {0} não é permitido."  : "Os comandos {0} não são permitidos." ) + " Por favor, verifique a query informada.";

                validation = string.Format(message, ConvertToString(result));
            }

            if (!string.IsNullOrEmpty(validation))
                throw new InvalidQueryException(validation);
        }

        private static string ConvertToString(List<string> list)
        {
            string result = string.Empty;

            foreach (var item in list)
            {
                result += string.Format("{0}{1}", string.IsNullOrEmpty(result) ? string.Empty : ", " ,item);
            }

            return result;
        }
    }
}
