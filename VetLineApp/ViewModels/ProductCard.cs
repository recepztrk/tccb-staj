namespace VetLineApp.ViewModels
{
    public record ProductCard(
        string ImageUrl, 
        string Name, 
        string? Excerpt, 
        decimal? Price, 
        string LinkHref
    );
}
