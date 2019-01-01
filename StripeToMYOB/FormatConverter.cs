using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper.Configuration;

namespace StripeToMYOB
{
    public class FormatConverter : IFormatConverter
    {
        private readonly string _creditAccount;
        private readonly string _debitAccount;

        public FormatConverter(string creditAccount, string debitAccount)
        {
            _creditAccount = creditAccount;
            _debitAccount = debitAccount;
        }

        public string Convert(string stripeFormat)
        {
            var configuration = GetConfiguration();

            var list = ReadString(stripeFormat, configuration);

            return WriteString(list);
        }

        private string WriteString(IEnumerable<StripeFormat> list)
        {
            var writer = new StringWriter();

            writer.WriteLine("{},,,,,,,,,");
            writer.WriteLine("Journal Number,Date,Memo,Account Number,Is Credit,Amount,Job,Allocation Memo,Category,Is Year-End Adjustment");

            var first = true;
            foreach (var row in list)
            {
                if (!first)
                {
                    writer.WriteLine(",,,,,,,,,");
                }

                writer.WriteLine(ToDebitLine(row));
                writer.WriteLine(ToCreditLine(row));

                first = false;
            }

            return writer.ToString();
        }

        private string ToCreditLine(StripeFormat item)
        {
            return $",{item.Date},\"{item.Description} {item.Id}\",{_creditAccount},Y,{item.Amount},,,,";
        }

        private string ToDebitLine(StripeFormat item)
        {
            return $",{item.Date},\"{item.Description} {item.Id}\",{_debitAccount},N,{item.Amount},,,,";
        }

        private List<StripeFormat> ReadString(string civiFormat, Configuration configuration)
        {
            using (var reader = new StringReader(civiFormat))
            {
                var csv = new CsvHelper.CsvReader(reader, configuration);
                csv.Configuration.RegisterClassMap(new StripeFormatMap());
                csv.Configuration.MissingFieldFound = null;
                var records = csv.GetRecords<StripeFormat>();
                return records.ToList();
            }
        }

        private static Configuration GetConfiguration()
        {
            var configuration = new Configuration
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                HeaderValidated = (opt1, opt2, opt3, opt4) => { },
                TrimOptions = TrimOptions.Trim
            };
            return configuration;
        }
    }
}
