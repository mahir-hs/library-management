using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Common.Specifications;

public class BookByISBNSpecification : SpecificationBase<Book>
{
    public BookByISBNSpecification(string isbn)
    {
        Criteria = b => b.ISBN == isbn;
    }
}

public class BookCopiesByBookSpecification : SpecificationBase<BookCopy>
{
    public BookCopiesByBookSpecification(Guid bookId)
    {
        Criteria = bc => bc.BookId == bookId;
    }
}

public class GetAllBooksSpecification : SpecificationBase<Book>
{
    public GetAllBooksSpecification(int pageNumber = 1, int pageSize = 10)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderBy(b => b.Title);
    }

    public GetAllBooksSpecification()
    {
        ApplyOrderBy(b => b.Title);
    }
}

public class SearchBooksSpecification : SpecificationBase<Book>
{
    public SearchBooksSpecification(
        string? title = null,
        string? author = null,
        string? isbn = null,
        Guid? categoryId = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var criteria = PredicateBuilder.True<Book>();

        if (!string.IsNullOrWhiteSpace(title))
        {
            criteria = criteria.And(b => b.Title.Contains(title));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            criteria = criteria.And(b => b.Author.Name.Contains(author));
        }

        if (!string.IsNullOrWhiteSpace(isbn))
        {
            criteria = criteria.And(b => b.ISBN.Contains(isbn));
        }

        if (categoryId.HasValue)
        {
            criteria = criteria.And(b => b.CategoryId == categoryId.Value);
        }

        Criteria = criteria;
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderBy(b => b.Title);
    }

    public SearchBooksSpecification(
        string? title = null,
        string? author = null,
        string? isbn = null,
        Guid? categoryId = null)
    {
        var criteria = PredicateBuilder.True<Book>();

        if (!string.IsNullOrWhiteSpace(title))
        {
            criteria = criteria.And(b => b.Title.Contains(title));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            criteria = criteria.And(b => b.Author.Name.Contains(author));
        }

        if (!string.IsNullOrWhiteSpace(isbn))
        {
            criteria = criteria.And(b => b.ISBN.Contains(isbn));
        }

        if (categoryId.HasValue)
        {
            criteria = criteria.And(b => b.CategoryId == categoryId.Value);
        }

        Criteria = criteria;
        ApplyOrderBy(b => b.Title);
    }
}

public class AvailableBooksSpecification : SpecificationBase<Book>
{
    public AvailableBooksSpecification(int pageNumber = 1, int pageSize = 10)
    {
        Criteria = b => b.Copies.Any(c => c.Status == BookCopyStatus.Available);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderBy(b => b.Title);
    }

    public AvailableBooksSpecification()
    {
        Criteria = b => b.Copies.Any(c => c.Status == BookCopyStatus.Available);
        ApplyOrderBy(b => b.Title);
    }
}