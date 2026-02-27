namespace BookAuditTrail;

public static class SeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BookAuditTrailDbContext>();

        if (context.Books.Any()) return;

        var authors = new List<Author>
        {
            new() { Name = "J. R. R. Tolkien" },
            new() { Name = "J. K. Rowling" },
            new() { Name = "George Orwell" },
            new() { Name = "F. Scott Fitzgerald" },
            new() { Name = "Jane Austen" },
            new() { Name = "Mark Twain" },
            new() { Name = "Leo Tolstoy" },
            new() { Name = "Agatha Christie" },
            new() { Name = "Stephen King" },
            new() { Name = "Isaac Asimov" }
        };
        await context.Authors.AddRangeAsync(authors);
        await context.SaveChangesAsync();

        var books = new List<Book>
        {
            new() {
                Title = "The Hobbit",
                PublishDate = new DateTime(1937, 9, 21),
                ShortDescription = "Fantasy novel about Bilbo Baggins.",
                Authors = new List<Author> { authors[0] }
            },
            new()
            {
                Title = "Harry Potter and the Philosopher's Stone",
                PublishDate = new DateTime(1997, 6, 26),
                ShortDescription = "First book in Harry Potter series.",
                Authors = new List<Author> { authors[1] }
            },
            new()
            {
                Title = "1984",
                PublishDate = new DateTime(1949, 6, 8),
                ShortDescription = "Dystopian novel about totalitarian regime.",
                Authors = new List<Author> { authors[2] }
            },
            new()
            {
                Title = "The Great Gatsby",
                PublishDate = new DateTime(1925, 4, 10),
                ShortDescription = "Classic novel about the American Dream.",
                Authors = new List<Author> { authors[3] }
            },
            new()
            {
                Title = "Pride and Prejudice",
                PublishDate = new DateTime(1813, 1, 28),
                ShortDescription = "Romantic novel about manners and marriage.",
                Authors = new List<Author> { authors[4] }
            },
            new()
            {
                Title = "Adventures of Huckleberry Finn",
                PublishDate = new DateTime(1884, 12, 10),
                ShortDescription = "Story of a boy and a runaway slave on a raft journey.",
                Authors = new List<Author> { authors[5] }
            },
            new()
            {
                Title = "War and Peace",
                PublishDate = new DateTime(1869, 1, 1),
                ShortDescription = "Epic novel about Russian society during Napoleonic wars.",
                Authors = new List<Author> { authors[6] }
            },
            new()
            {
                Title = "Murder on the Orient Express",
                PublishDate = new DateTime(1934, 1, 1),
                ShortDescription = "Hercule Poirot investigates a murder on a train.",
                Authors = new List<Author> { authors[7] }
            },
            new()
            {
                Title = "The Shining",
                PublishDate = new DateTime(1977, 1, 28),
                ShortDescription = "Horror novel about a haunted hotel and a family in peril.",
                Authors = new List<Author> { authors[8] }
            },
            new()
            {
                Title = "Foundation",
                PublishDate = new DateTime(1951, 5, 1),
                ShortDescription = "Science fiction novel about a galactic empire.",
                Authors = new List<Author> { authors[9] }
            }
        };

        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();
    }
}