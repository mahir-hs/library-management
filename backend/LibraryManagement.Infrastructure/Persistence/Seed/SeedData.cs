using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Persistence.Seed;

public static class SeedData
{
    #region Author GUIDs

    public static readonly Guid Author1Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567801");
    public static readonly Guid Author2Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567802");
    public static readonly Guid Author3Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567803");
    public static readonly Guid Author4Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567804");
    public static readonly Guid Author5Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567805");

    #endregion

    #region Category GUIDs

    public static readonly Guid Category1Id = Guid.Parse("b1b2c3d4-e5f6-7890-abcd-ef1234567801");
    public static readonly Guid Category2Id = Guid.Parse("b1b2c3d4-e5f6-7890-abcd-ef1234567802");
    public static readonly Guid Category3Id = Guid.Parse("b1b2c3d4-e5f6-7890-abcd-ef1234567803");
    public static readonly Guid Category4Id = Guid.Parse("b1b2c3d4-e5f6-7890-abcd-ef1234567804");
    public static readonly Guid Category5Id = Guid.Parse("b1b2c3d4-e5f6-7890-abcd-ef1234567805");
    public static readonly Guid Category6Id = Guid.Parse("b1b2c3d4-e5f6-7890-abcd-ef1234567806");

    #endregion

    #region Branch GUIDs (match existing seed)

    public static readonly Guid Branch1Id = Guid.Parse("c1b2c3d4-e5f6-7890-abcd-ef1234567801");
    public static readonly Guid Branch2Id = Guid.Parse("c1b2c3d4-e5f6-7890-abcd-ef1234567802");
    public static readonly Guid Branch3Id = Guid.Parse("c1b2c3d4-e5f6-7890-abcd-ef1234567803");
    public static readonly Guid Branch4Id = Guid.Parse("c1b2c3d4-e5f6-7890-abcd-ef1234567804");
    public static readonly Guid Branch5Id = Guid.Parse("c1b2c3d4-e5f6-7890-abcd-ef1234567805");

    #endregion

    #region User GUIDs

    public static readonly Guid AdminUserId = Guid.Parse("d1b2c3d4-e5f6-7890-abcd-ef1234567801");
    public static readonly Guid Librarian1UserId = Guid.Parse("d1b2c3d4-e5f6-7890-abcd-ef1234567802");
    public static readonly Guid Librarian2UserId = Guid.Parse("d1b2c3d4-e5f6-7890-abcd-ef1234567803");
    public static readonly Guid Member1UserId = Guid.Parse("d1b2c3d4-e5f6-7890-abcd-ef1234567804");
    public static readonly Guid Member2UserId = Guid.Parse("d1b2c3d4-e5f6-7890-abcd-ef1234567805");
    public static readonly Guid Member3UserId = Guid.Parse("d1b2c3d4-e5f6-7890-abcd-ef1234567806");
    public static readonly Guid Member4UserId = Guid.Parse("d1b2c3d4-e5f6-7890-abcd-ef1234567807");
    public static readonly Guid Member5UserId = Guid.Parse("d1b2c3d4-e5f6-7890-abcd-ef1234567808");

    #endregion

    #region Member GUIDs

    public static readonly Guid Member1Id = Guid.Parse("e1b2c3d4-e5f6-7890-abcd-ef1234567801");
    public static readonly Guid Member2Id = Guid.Parse("e1b2c3d4-e5f6-7890-abcd-ef1234567802");
    public static readonly Guid Member3Id = Guid.Parse("e1b2c3d4-e5f6-7890-abcd-ef1234567803");
    public static readonly Guid Member4Id = Guid.Parse("e1b2c3d4-e5f6-7890-abcd-ef1234567804");
    public static readonly Guid Member5Id = Guid.Parse("e1b2c3d4-e5f6-7890-abcd-ef1234567805");

    #endregion

    #region Book GUIDs

    public static readonly Guid Book1Id = Guid.Parse("f1b2c3d4-e5f6-7890-abcd-ef1234567801");
    public static readonly Guid Book2Id = Guid.Parse("f1b2c3d4-e5f6-7890-abcd-ef1234567802");
    public static readonly Guid Book3Id = Guid.Parse("f1b2c3d4-e5f6-7890-abcd-ef1234567803");
    public static readonly Guid Book4Id = Guid.Parse("f1b2c3d4-e5f6-7890-abcd-ef1234567804");
    public static readonly Guid Book5Id = Guid.Parse("f1b2c3d4-e5f6-7890-abcd-ef1234567805");
    public static readonly Guid Book6Id = Guid.Parse("f1b2c3d4-e5f6-7890-abcd-ef1234567806");
    public static readonly Guid Book7Id = Guid.Parse("f1b2c3d4-e5f6-7890-abcd-ef1234567807");
    public static readonly Guid Book8Id = Guid.Parse("f1b2c3d4-e5f6-7890-abcd-ef1234567808");
    public static readonly Guid Book9Id = Guid.Parse("f1b2c3d4-e5f6-7890-abcd-ef1234567809");
    public static readonly Guid Book10Id = Guid.Parse("f1b2c3d4-e5f6-7890-abcd-ef1234567810");

    #endregion

    #region BookCopy GUIDs

    public static readonly Guid BookCopy1Id = Guid.Parse("11b2c3d4-e5f6-7890-abcd-ef1234567801");
    public static readonly Guid BookCopy2Id = Guid.Parse("11b2c3d4-e5f6-7890-abcd-ef1234567802");
    public static readonly Guid BookCopy3Id = Guid.Parse("11b2c3d4-e5f6-7890-abcd-ef1234567803");
    public static readonly Guid BookCopy4Id = Guid.Parse("11b2c3d4-e5f6-7890-abcd-ef1234567804");
    public static readonly Guid BookCopy5Id = Guid.Parse("11b2c3d4-e5f6-7890-abcd-ef1234567805");
    public static readonly Guid BookCopy6Id = Guid.Parse("11b2c3d4-e5f6-7890-abcd-ef1234567806");
    public static readonly Guid BookCopy7Id = Guid.Parse("11b2c3d4-e5f6-7890-abcd-ef1234567807");
    public static readonly Guid BookCopy8Id = Guid.Parse("11b2c3d4-e5f6-7890-abcd-ef1234567808");
    public static readonly Guid BookCopy9Id = Guid.Parse("11b2c3d4-e5f6-7890-abcd-ef1234567809");
    public static readonly Guid BookCopy10Id = Guid.Parse("11b2c3d4-e5f6-7890-abcd-ef1234567810");
    public static readonly Guid BookCopy11Id = Guid.Parse("11b2c3d4-e5f6-7890-abcd-ef1234567811");
    public static readonly Guid BookCopy12Id = Guid.Parse("11b2c3d4-e5f6-7890-abcd-ef1234567812");

    #endregion

    #region BorrowRecord GUIDs

    public static readonly Guid BorrowRecord1Id = Guid.Parse("21b2c3d4-e5f6-7890-abcd-ef1234567801");
    public static readonly Guid BorrowRecord2Id = Guid.Parse("21b2c3d4-e5f6-7890-abcd-ef1234567802");
    public static readonly Guid BorrowRecord3Id = Guid.Parse("21b2c3d4-e5f6-7890-abcd-ef1234567803");
    public static readonly Guid BorrowRecord4Id = Guid.Parse("21b2c3d4-e5f6-7890-abcd-ef1234567804");
    public static readonly Guid BorrowRecord5Id = Guid.Parse("21b2c3d4-e5f6-7890-abcd-ef1234567805");

    #endregion

    #region Reservation GUIDs

    public static readonly Guid Reservation1Id = Guid.Parse("31b2c3d4-e5f6-7890-abcd-ef1234567801");
    public static readonly Guid Reservation2Id = Guid.Parse("31b2c3d4-e5f6-7890-abcd-ef1234567802");
    public static readonly Guid Reservation3Id = Guid.Parse("31b2c3d4-e5f6-7890-abcd-ef1234567803");

    #endregion

    #region RefreshToken GUIDs

    public static readonly Guid RefreshToken1Id = Guid.Parse("41b2c3d4-e5f6-7890-abcd-ef1234567801");
    public static readonly Guid RefreshToken2Id = Guid.Parse("41b2c3d4-e5f6-7890-abcd-ef1234567802");

    #endregion

    #region Authors

    public static List<Author> GetAuthors()
    {
        return new List<Author>
        {
            new()
            {
                Id = Author1Id,
                Name = "J.K. Rowling",
                Biography = "British author best known for the Harry Potter fantasy series, which has sold over 500 million copies worldwide and is one of the best-selling book series in history."
            },
            new()
            {
                Id = Author2Id,
                Name = "George Orwell",
                Biography = "English novelist, essayist, journalist, and critic. His work is marked by lucid prose, awareness of social injustice, opposition to totalitarianism, and outspoken democratic socialism."
            },
            new()
            {
                Id = Author3Id,
                Name = "Jules Verne",
                Biography = "French novelist, poet, and playwright best known for his adventure novels and his profound influence on the science fiction genre. Often referred to as the Father of Science Fiction."
            },
            new()
            {
                Id = Author4Id,
                Name = "Agatha Christie",
                Biography = "English author known for her 66 detective novels and 14 short story collections, particularly those revolving around her characters Hercule Poirot and Miss Marple. She is the best-selling novelist of all time."
            },
            new()
            {
                Id = Author5Id,
                Name = "Stephen King",
                Biography = "American author of horror, supernatural fiction, suspense, science fiction, and fantasy. His books have sold more than 350 million copies, and many have been adapted into films, television series, and comic books."
            }
        };
    }

    #endregion

    #region Categories

    public static List<Category> GetCategories()
    {
        return new List<Category>
        {
            new()
            {
                Id = Category1Id,
                Name = "Fiction",
                Description = "Literary works of imagination, including novels, short stories, and novellas that are not based strictly on history or fact."
            },
            new()
            {
                Id = Category2Id,
                Name = "Science Fiction",
                Description = "Fiction based on hypothetical scientific discoveries or futuristic concepts such as advanced science, technology, space exploration, and extraterrestrial life."
            },
            new()
            {
                Id = Category3Id,
                Name = "Mystery",
                Description = "Fiction that revolves around a mysterious crime, puzzle, or secret that the protagonist must solve, typically following a detective or amateur sleuth."
            },
            new()
            {
                Id = Category4Id,
                Name = "Horror",
                Description = "Fiction designed to frighten, unsettle, or disgust the reader through supernatural elements, psychological terror, or grotesque imagery."
            },
            new()
            {
                Id = Category5Id,
                Name = "Classics",
                Description = "Timeless literary works of enduring merit that have stood the test of time and are considered masterpieces of world literature."
            },
            new()
            {
                Id = Category6Id,
                Name = "Fantasy",
                Description = "Fiction set in imaginary worlds, often involving magic, mythical creatures, and epic quests that depart from the rules of reality."
            }
        };
    }

    #endregion

    #region Branches (existing)

    public static List<Branch> GetBranches()
    {
        return new List<Branch>
        {
            new Branch
            {
                Id = Branch1Id,
                Name = "Main Library",
                Code = "HQ",
                Address = "100 Central Avenue, Downtown",
                Phone = "555-0100",
                Email = "main@librarymgmt.com",
                IsActive = true
            },
            new Branch
            {
                Id = Branch2Id,
                Name = "North Branch",
                Code = "NTH",
                Address = "250 North Street, North District",
                Phone = "555-0200",
                Email = "north@librarymgmt.com",
                IsActive = true
            },
            new Branch
            {
                Id = Branch3Id,
                Name = "South Branch",
                Code = "STH",
                Address = "75 South Boulevard, South District",
                Phone = "555-0300",
                Email = "south@librarymgmt.com",
                IsActive = true
            },
            new Branch
            {
                Id = Branch4Id,
                Name = "East Branch",
                Code = "EST",
                Address = "300 East Road, East District",
                Phone = "555-0400",
                Email = "east@librarymgmt.com",
                IsActive = true
            },
            new Branch
            {
                Id = Branch5Id,
                Name = "West Branch",
                Code = "WST",
                Address = "150 West Lane, West District",
                Phone = "555-0500",
                Email = "west@librarymgmt.com",
                IsActive = false
            }
        };
    }

    #endregion

    #region Users

    public static List<User> GetUsers(IPasswordHasher<User> passwordHasher)
    {
        var users = new List<User>
        {
            new User
            {
                Id = AdminUserId,
                Username = "admin",
                Email = "admin@librarymgmt.com",
                FullName = "System Administrator",
                PhoneNumber = "555-1000",
                Role = UserRole.Admin,
                IsActive = true,
                BranchId = Branch1Id
            },
            new User
            {
                Id = Librarian1UserId,
                Username = "librarian_jane",
                Email = "jane.librarian@librarymgmt.com",
                FullName = "Jane Smith",
                PhoneNumber = "555-1100",
                Role = UserRole.Librarian,
                IsActive = true,
                BranchId = Branch1Id
            },
            new User
            {
                Id = Librarian2UserId,
                Username = "librarian_bob",
                Email = "bob.librarian@librarymgmt.com",
                FullName = "Bob Johnson",
                PhoneNumber = "555-1200",
                Role = UserRole.Librarian,
                IsActive = true,
                BranchId = Branch2Id
            },
            new User
            {
                Id = Member1UserId,
                Username = "member_alice",
                Email = "alice.member@librarymgmt.com",
                FullName = "Alice Williams",
                PhoneNumber = "555-2000",
                Role = UserRole.Member,
                IsActive = true,
                BranchId = Branch1Id
            },
            new User
            {
                Id = Member2UserId,
                Username = "member_charlie",
                Email = "charlie.member@librarymgmt.com",
                FullName = "Charlie Brown",
                PhoneNumber = "555-2100",
                Role = UserRole.Member,
                IsActive = true,
                BranchId = Branch2Id
            },
            new User
            {
                Id = Member3UserId,
                Username = "member_diana",
                Email = "diana.member@librarymgmt.com",
                FullName = "Diana Prince",
                PhoneNumber = "555-2200",
                Role = UserRole.Member,
                IsActive = true,
                BranchId = Branch3Id
            },
            new User
            {
                Id = Member4UserId,
                Username = "member_edward",
                Email = "edward.member@librarymgmt.com",
                FullName = "Edward Norton",
                PhoneNumber = "555-2300",
                Role = UserRole.Member,
                IsActive = true,
                BranchId = Branch3Id
            },
            new User
            {
                Id = Member5UserId,
                Username = "member_fiona",
                Email = "fiona.member@librarymgmt.com",
                FullName = "Fiona Gallagher",
                PhoneNumber = "555-2400",
                Role = UserRole.Member,
                IsActive = false,
                BranchId = Branch4Id
            }
        };

        // Hash passwords using ASP.NET Identity PasswordHasher so they are
        // compatible with PasswordHasher.VerifyHashedPassword used at login.
        foreach (var user in users)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, GetDefaultPassword(user.Role));
        }

        return users;
    }

    private static string GetDefaultPassword(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => "Admin@123!",
            UserRole.Librarian => "Librarian@123!",
            UserRole.Member => "Member@123!",
            _ => "Member@123!"
        };
    }

    #endregion

    #region Members

    public static List<Member> GetMembers()
    {
        return new List<Member>
        {
            new Member
            {
                Id = Member1Id,
                UserId = Member1UserId,
                MembershipNumber = "MBR-000001",
                Address = "42 Maple Street, Downtown, 10001",
                JoinedDate = new DateTimeOffset(2023, 1, 15, 0, 0, 0, TimeSpan.Zero)
            },
            new Member
            {
                Id = Member2Id,
                UserId = Member2UserId,
                MembershipNumber = "MBR-000002",
                Address = "88 Oak Avenue, North District, 20001",
                JoinedDate = new DateTimeOffset(2023, 3, 22, 0, 0, 0, TimeSpan.Zero)
            },
            new Member
            {
                Id = Member3Id,
                UserId = Member3UserId,
                MembershipNumber = "MBR-000003",
                Address = "15 Pine Road, South District, 30001",
                JoinedDate = new DateTimeOffset(2023, 5, 10, 0, 0, 0, TimeSpan.Zero)
            },
            new Member
            {
                Id = Member4Id,
                UserId = Member4UserId,
                MembershipNumber = "MBR-000004",
                Address = "77 Elm Drive, East District, 40001",
                JoinedDate = new DateTimeOffset(2023, 7, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new Member
            {
                Id = Member5Id,
                UserId = Member5UserId,
                MembershipNumber = "MBR-000005",
                Address = "23 Cedar Lane, West District, 50001",
                JoinedDate = new DateTimeOffset(2023, 9, 18, 0, 0, 0, TimeSpan.Zero)
            }
        };
    }

    #endregion

    #region Books

    public static List<Book> GetBooks()
    {
        return new List<Book>
        {
            new Book
            {
                Id = Book1Id,
                Title = "Harry Potter and the Philosopher's Stone",
                ISBN = "9780747532699",
                Description = "The first novel in the Harry Potter series following Harry Potter's first year at Hogwarts School of Witchcraft and Wizardry.",
                Publisher = "Bloomsbury",
                PublishedYear = 1997,
                Language = "English",
                AuthorId = Author1Id,
                CategoryId = Category6Id
            },
            new Book
            {
                Id = Book2Id,
                Title = "1984",
                ISBN = "9780451524935",
                Description = "A dystopian social science fiction novel set in a totalitarian society ruled by Big Brother, exploring themes of surveillance, truth, and individual freedom.",
                Publisher = "Secker & Warburg",
                PublishedYear = 1949,
                Language = "English",
                AuthorId = Author2Id,
                CategoryId = Category2Id
            },
            new Book
            {
                Id = Book3Id,
                Title = "Twenty Thousand Leagues Under the Seas",
                ISBN = "9781503260426",
                Description = "A classic science fiction novel about Captain Nemo and his submarine Nautilus, journeying through the depths of the ocean.",
                Publisher = "Pierre-Jules Hetzel",
                PublishedYear = 1870,
                Language = "French",
                AuthorId = Author3Id,
                CategoryId = Category2Id
            },
            new Book
            {
                Id = Book4Id,
                Title = "Murder on the Orient Express",
                ISBN = "9780062693662",
                Description = "Hercule Poirot must solve a murder aboard the luxurious Orient Express train after a passenger is found stabbed to death in their compartment.",
                Publisher = "Collins Crime Club",
                PublishedYear = 1934,
                Language = "English",
                AuthorId = Author4Id,
                CategoryId = Category3Id
            },
            new Book
            {
                Id = Book5Id,
                Title = "The Shining",
                ISBN = "9780385121675",
                Description = "A horror novel about Jack Torrance, who takes a job as the winter caretaker of the isolated Overlook Hotel, where supernatural forces drive him to madness.",
                Publisher = "Doubleday",
                PublishedYear = 1977,
                Language = "English",
                AuthorId = Author5Id,
                CategoryId = Category4Id
            },
            new Book
            {
                Id = Book6Id,
                Title = "Animal Farm",
                ISBN = "9780451526342",
                Description = "A satirical allegorical novella in which farm animals overthrow their human farmer and establish a totalitarian society ruled by pigs.",
                Publisher = "Secker & Warburg",
                PublishedYear = 1945,
                Language = "English",
                AuthorId = Author2Id,
                CategoryId = Category5Id
            },
            new Book
            {
                Id = Book7Id,
                Title = "A Journey to the Center of the Earth",
                ISBN = "9781503261102",
                Description = "A science fiction novel following Professor Lidenbrock and his nephew as they descend into the core of a volcano and discover a prehistoric world.",
                Publisher = "Pierre-Jules Hetzel",
                PublishedYear = 1864,
                Language = "French",
                AuthorId = Author3Id,
                CategoryId = Category2Id
            },
            new Book
            {
                Id = Book8Id,
                Title = "Harry Potter and the Chamber of Secrets",
                ISBN = "9780747538493",
                Description = "The second novel in the Harry Potter series where Harry discovers a hidden chamber within Hogwarts that houses a deadly monster.",
                Publisher = "Bloomsbury",
                PublishedYear = 1998,
                Language = "English",
                AuthorId = Author1Id,
                CategoryId = Category6Id
            },
            new Book
            {
                Id = Book9Id,
                Title = "And Then There Were None",
                ISBN = "9780062073488",
                Description = "Ten strangers are invited to a secluded island where they are accused of past crimes and Begin to be murdered one by one.",
                Publisher = "Collins Crime Club",
                PublishedYear = 1939,
                Language = "English",
                AuthorId = Author4Id,
                CategoryId = Category3Id
            },
            new Book
            {
                Id = Book10Id,
                Title = "It",
                ISBN = "9781501142970",
                Description = "A horror novel about a shape-shifting entity that preys on the children of Derry, Maine, and a group of friends who confront it as adults.",
                Publisher = "Viking Press",
                PublishedYear = 1986,
                Language = "English",
                AuthorId = Author5Id,
                CategoryId = Category4Id
            }
        };
    }

    #endregion

    #region BookCopies

    public static List<BookCopy> GetBookCopies()
    {
        return new List<BookCopy>
        {
            // Harry Potter and the Philosopher's Stone - 3 copies
            new BookCopy
            {
                Id = BookCopy1Id,
                BookId = Book1Id,
                Barcode = "BK-CP-00001",
                Status = BookCopyStatus.Available,
                ShelfLocation = "A-01-01",
                AcquiredDate = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
                BranchId = Branch1Id
            },
            new BookCopy
            {
                Id = BookCopy2Id,
                BookId = Book1Id,
                Barcode = "BK-CP-00002",
                Status = BookCopyStatus.Borrowed,
                ShelfLocation = "A-01-02",
                AcquiredDate = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
                BranchId = Branch1Id
            },
            new BookCopy
            {
                Id = BookCopy3Id,
                BookId = Book1Id,
                Barcode = "BK-CP-00003",
                Status = BookCopyStatus.Available,
                ShelfLocation = "A-01-03",
                AcquiredDate = new DateTimeOffset(2023, 2, 15, 0, 0, 0, TimeSpan.Zero),
                BranchId = Branch2Id
            },

            // 1984 - 2 copies
            new BookCopy
            {
                Id = BookCopy4Id,
                BookId = Book2Id,
                Barcode = "BK-CP-00004",
                Status = BookCopyStatus.Available,
                ShelfLocation = "B-02-01",
                AcquiredDate = new DateTimeOffset(2023, 2, 1, 0, 0, 0, TimeSpan.Zero),
                BranchId = Branch1Id
            },
            new BookCopy
            {
                Id = BookCopy5Id,
                BookId = Book2Id,
                Barcode = "BK-CP-00005",
                Status = BookCopyStatus.Reserved,
                ShelfLocation = "B-02-02",
                AcquiredDate = new DateTimeOffset(2023, 2, 1, 0, 0, 0, TimeSpan.Zero),
                BranchId = Branch1Id
            },

            // Twenty Thousand Leagues Under the Seas - 2 copies
            new BookCopy
            {
                Id = BookCopy6Id,
                BookId = Book3Id,
                Barcode = "BK-CP-00006",
                Status = BookCopyStatus.Available,
                ShelfLocation = "C-03-01",
                AcquiredDate = new DateTimeOffset(2023, 3, 10, 0, 0, 0, TimeSpan.Zero),
                BranchId = Branch2Id
            },
            new BookCopy
            {
                Id = BookCopy7Id,
                BookId = Book3Id,
                Barcode = "BK-CP-00007",
                Status = BookCopyStatus.Available,
                ShelfLocation = "C-03-02",
                AcquiredDate = new DateTimeOffset(2023, 3, 10, 0, 0, 0, TimeSpan.Zero),
                BranchId = Branch3Id
            },

            // Murder on the Orient Express - 2 copies
            new BookCopy
            {
                Id = BookCopy8Id,
                BookId = Book4Id,
                Barcode = "BK-CP-00008",
                Status = BookCopyStatus.Borrowed,
                ShelfLocation = "D-04-01",
                AcquiredDate = new DateTimeOffset(2023, 1, 15, 0, 0, 0, TimeSpan.Zero),
                BranchId = Branch1Id
            },
            new BookCopy
            {
                Id = BookCopy9Id,
                BookId = Book4Id,
                Barcode = "BK-CP-00009",
                Status = BookCopyStatus.Available,
                ShelfLocation = "D-04-02",
                AcquiredDate = new DateTimeOffset(2023, 1, 15, 0, 0, 0, TimeSpan.Zero),
                BranchId = Branch1Id
            },

            // The Shining - 2 copies
            new BookCopy
            {
                Id = BookCopy10Id,
                BookId = Book5Id,
                Barcode = "BK-CP-00010",
                Status = BookCopyStatus.Available,
                ShelfLocation = "E-05-01",
                AcquiredDate = new DateTimeOffset(2023, 4, 1, 0, 0, 0, TimeSpan.Zero),
                BranchId = Branch3Id
            },
            new BookCopy
            {
                Id = BookCopy11Id,
                BookId = Book5Id,
                Barcode = "BK-CP-00011",
                Status = BookCopyStatus.Lost,
                ShelfLocation = "E-05-02",
                AcquiredDate = new DateTimeOffset(2023, 4, 1, 0, 0, 0, TimeSpan.Zero),
                BranchId = Branch3Id
            },

            // Animal Farm - 1 copy
            new BookCopy
            {
                Id = BookCopy12Id,
                BookId = Book6Id,
                Barcode = "BK-CP-00012",
                Status = BookCopyStatus.Available,
                ShelfLocation = "F-06-01",
                AcquiredDate = new DateTimeOffset(2023, 5, 20, 0, 0, 0, TimeSpan.Zero),
                BranchId = Branch4Id
            }
        };
    }

    #endregion

    #region BorrowRecords

    public static List<BorrowRecord> GetBorrowRecords()
    {
        return new List<BorrowRecord>
        {
            // Active borrow - Alice borrows copy of 1984 (BK-CP-00005 Reserved)
            new BorrowRecord
            {
                Id = BorrowRecord1Id,
                MemberId = Member1Id,
                BookCopyId = BookCopy5Id,
                BorrowedAt = new DateTimeOffset(2025, 6, 1, 10, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 6, 29, 10, 0, 0, TimeSpan.Zero),
                ReturnedAt = null,
                Status = BorrowStatus.Borrowed,
                FineAmount = 0.00m
            },
            // Active borrow - Charlie borrows a copy of Harry Potter (BK-CP-00002 Borrowed)
            new BorrowRecord
            {
                Id = BorrowRecord2Id,
                MemberId = Member2Id,
                BookCopyId = BookCopy2Id,
                BorrowedAt = new DateTimeOffset(2025, 6, 10, 9, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 7, 8, 9, 0, 0, TimeSpan.Zero),
                ReturnedAt = null,
                Status = BorrowStatus.Borrowed,
                FineAmount = 0.00m
            },
            // Overdue borrow - Edward borrows Murder on the Orient Express (BK-CP-00008 Borrowed)
            new BorrowRecord
            {
                Id = BorrowRecord3Id,
                MemberId = Member4Id,
                BookCopyId = BookCopy8Id,
                BorrowedAt = new DateTimeOffset(2025, 5, 15, 14, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 6, 12, 14, 0, 0, TimeSpan.Zero),
                ReturnedAt = null,
                Status = BorrowStatus.Overdue,
                FineAmount = 5.00m
            },
            // Returned borrow - Alice returned Animal Farm
            new BorrowRecord
            {
                Id = BorrowRecord4Id,
                MemberId = Member1Id,
                BookCopyId = BookCopy12Id,
                BorrowedAt = new DateTimeOffset(2025, 4, 20, 11, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 5, 18, 11, 0, 0, TimeSpan.Zero),
                ReturnedAt = new DateTimeOffset(2025, 5, 10, 15, 30, 0, TimeSpan.Zero),
                Status = BorrowStatus.Returned,
                FineAmount = 0.00m
            },
            // Returned borrow - Diana returned The Shining (BK-CP-00010)
            new BorrowRecord
            {
                Id = BorrowRecord5Id,
                MemberId = Member3Id,
                BookCopyId = BookCopy10Id,
                BorrowedAt = new DateTimeOffset(2025, 3, 1, 10, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 3, 29, 10, 0, 0, TimeSpan.Zero),
                ReturnedAt = new DateTimeOffset(2025, 3, 25, 9, 0, 0, TimeSpan.Zero),
                Status = BorrowStatus.Returned,
                FineAmount = 0.00m
            }
        };
    }

    #endregion

    #region Reservations

    public static List<Reservation> GetReservations()
    {
        return new List<Reservation>
        {
            // Pending reservation - Edward reserves Harry Potter and the Philosopher's Stone
            new Reservation
            {
                Id = Reservation1Id,
                MemberId = Member4Id,
                BookId = Book1Id,
                ReservedAt = new DateTimeOffset(2025, 6, 25, 8, 30, 0, TimeSpan.Zero),
                ExpiresAt = new DateTimeOffset(2025, 7, 2, 8, 30, 0, TimeSpan.Zero),
                Status = ReservationStatus.Pending
            },
            // Fulfilled reservation - Charlie reserved and got 1984
            new Reservation
            {
                Id = Reservation2Id,
                MemberId = Member2Id,
                BookId = Book2Id,
                ReservedAt = new DateTimeOffset(2025, 5, 1, 13, 0, 0, TimeSpan.Zero),
                ExpiresAt = new DateTimeOffset(2025, 5, 8, 13, 0, 0, TimeSpan.Zero),
                Status = ReservationStatus.Fulfilled
            },
            // Cancelled reservation - Diana cancelled a reservation for The Shining
            new Reservation
            {
                Id = Reservation3Id,
                MemberId = Member3Id,
                BookId = Book5Id,
                ReservedAt = new DateTimeOffset(2025, 4, 10, 10, 0, 0, TimeSpan.Zero),
                ExpiresAt = new DateTimeOffset(2025, 4, 17, 10, 0, 0, TimeSpan.Zero),
                Status = ReservationStatus.Cancelled
            }
        };
    }

    #endregion

    #region RefreshTokens

    public static List<RefreshToken> GetRefreshTokens()
    {
        return new List<RefreshToken>
        {
            new RefreshToken
            {
                Id = RefreshToken1Id,
                UserId = AdminUserId,
                Token = "rt_admin_abc123def456ghi789jkl012mno345pqr678stu901vwx234yz",
                ExpiresAt = new DateTimeOffset(2026, 7, 31, 0, 0, 0, TimeSpan.Zero),
                RevokedAt = null
            },
            new RefreshToken
            {
                Id = RefreshToken2Id,
                UserId = Librarian1UserId,
                Token = "rt_librarian_xyz987wvu654tsr321qpo098nml765kjh432ghi109fed",
                ExpiresAt = new DateTimeOffset(2026, 6, 30, 0, 0, 0, TimeSpan.Zero),
                RevokedAt = null
            },
            new RefreshToken
            {
                Id = Guid.Parse("41b2c3d4-e5f6-7890-abcd-ef1234567803"),
                UserId = Member1UserId,
                Token = "rt_member_revoked_token_xyz_abc123def456ghi789jkl012mno345",
                ExpiresAt = new DateTimeOffset(2025, 12, 31, 0, 0, 0, TimeSpan.Zero),
                RevokedAt = new DateTimeOffset(2025, 6, 15, 0, 0, 0, TimeSpan.Zero)
            }
        };
    }

    #endregion

    #region Database Seeding

    /// <summary>
    /// Seeds the database with initial data if it has not been seeded already.
    /// Checks for existing Users to determine if seeding is needed.
    /// </summary>
    public static async Task SeedDatabaseAsync(
        ApplicationDbContext context,
        IPasswordHasher<User> passwordHasher,
        CancellationToken cancellationToken = default)
    {
        // Skip seeding if the database already has data
        if (await context.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Seed in FK dependency order
            await context.Authors.AddRangeAsync(GetAuthors(), cancellationToken);
            await context.Categories.AddRangeAsync(GetCategories(), cancellationToken);
            await context.Branches.AddRangeAsync(GetBranches(), cancellationToken);
            await context.Books.AddRangeAsync(GetBooks(), cancellationToken);
            await context.BookCopies.AddRangeAsync(GetBookCopies(), cancellationToken);
            await context.Users.AddRangeAsync(GetUsers(passwordHasher), cancellationToken);
            await context.Members.AddRangeAsync(GetMembers(), cancellationToken);
            await context.BorrowRecords.AddRangeAsync(GetBorrowRecords(), cancellationToken);
            await context.Reservations.AddRangeAsync(GetReservations(), cancellationToken);
            await context.RefreshTokens.AddRangeAsync(GetRefreshTokens(), cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    #endregion
}
