namespace VetLineApp.ViewModels
{
    public class HomeViewModel
    {
        public List<HomeSliderItem> Slider { get; set; } = new();
        public string AppointmentCtaHref { get; set; } = "/Appointments/Create";
        public List<HomeInfoBox> InfoBoxes { get; set; } = new();
        public List<ProductCard> FeaturedProducts { get; set; } = new();
        public List<ServiceCard> Services { get; set; } = new();
        public List<TestimonialCard> Testimonials { get; set; } = new();
    }
}
