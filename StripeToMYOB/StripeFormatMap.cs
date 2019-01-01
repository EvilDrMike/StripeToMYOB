using CsvHelper.Configuration;

namespace StripeToMYOB
{
    public sealed class StripeFormatMap : ClassMap<StripeFormat>
    {
        public StripeFormatMap()
        {
            Map(m => m.Date).Name("Arrival Date (UTC)");
            Map(m => m.Description);
            Map(m => m.Id).Name("id");
            Map(m => m.Amount);
        }
    }
}