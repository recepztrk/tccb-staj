namespace VetLineApp.ViewModels
{
    public record HomeSliderItem(
        string ImageUrl, 
        string Title, 
        string Subtitle, 
        string? CtaText, 
        string? CtaHref
    );
}
