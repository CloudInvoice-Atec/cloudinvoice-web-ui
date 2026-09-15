using System.Text.Json.Serialization;

namespace cloudinvoice_web_ui.Enums
{
    public class InvoiceEnums
{
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum InvoiceStatus
        {
            Draft,
            Issued,
            Canceled
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PaymentStatus
        {
            Unpaid,
            PartiallyPaid,
            Paid
        }
    }
}
