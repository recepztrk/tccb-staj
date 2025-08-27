namespace VetLineApp.ViewModels
{
    public class ContactViewModel
    {
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Instagram { get; set; } = string.Empty;
        public string Youtube { get; set; } = string.Empty;
        public string Facebook { get; set; } = string.Empty;
        public string X { get; set; } = string.Empty;
        public double MapLat { get; set; }
        public double MapLng { get; set; }
        public IEnumerable<TestimonialCard> Testimonials { get; set; } = new List<TestimonialCard>();
    }
}
