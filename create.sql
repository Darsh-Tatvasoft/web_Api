-- Drop table if exists
DROP TABLE IF EXISTS Books;
 
-- Create enhanced table
CREATE TABLE Books (
    Id SERIAL PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    Author VARCHAR(100) NOT NULL,
    ISBN VARCHAR(20) UNIQUE,
    Genre VARCHAR(50),
    Language VARCHAR(50),
    Publisher VARCHAR(100),
    Price DECIMAL(10, 2) NOT NULL,
    PageCount INT,
    StockQuantity INT DEFAULT 0,
    IsAvailable BOOLEAN DEFAULT TRUE,
    PublishedDate DATE NOT NULL
);
 
-- Insert sample data
INSERT INTO Books (Title, Author, ISBN, Genre, Language, Publisher, Price, PageCount, StockQuantity, IsAvailable, PublishedDate) VALUES
('The Pragmatic Programmer', 'Andrew Hunt', '978-0201616224', 'Programming', 'English', 'Addison-Wesley', 42.50, 352, 10, TRUE, '1999-10-30'),
('Clean Code', 'Robert C. Martin', '978-0132350884', 'Programming', 'English', 'Prentice Hall', 39.99, 464, 15, TRUE, '2008-08-11'),
('C# in Depth', 'Jon Skeet', '978-1617294532', 'Programming', 'English', 'Manning', 45.00, 528, 5, TRUE, '2019-04-02'),
('Effective PostgreSQL', 'John Doe', '978-1234567890', 'Databases', 'English', 'OpenSource Press', 29.95, 300, 20, TRUE, '2023-01-15'),
('Design Patterns', 'Erich Gamma', '978-0201633610', 'Software Engineering', 'English', 'Addison-Wesley', 55.00, 395, 0, FALSE, '1994-10-21');