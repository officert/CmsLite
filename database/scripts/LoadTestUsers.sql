INSERT INTO [dbo].[Users]
( UserName, Email, PasswordSalt, Password, CreateDate, LastModifiedDate, LastLoginDate, IsActivated, IsLockedOut )
VALUES 
('User1', 'user1@test.com', 'Gw/t1RQ+BTG5vWpABwZByg==', 'AGNbW18h6HM5u7bJnhO+xM+/FYfZnOtk6n68mH8Tf8GUHnhrGl1EELshTsEr5OiwwA==', GETDATE(), GETDATE(), GETDATE(), 1, 0),
('User2', 'user2@test.com', 'Gw/t1RQ+BTG5vWpABwZByg==', 'AGNbW18h6HM5u7bJnhO+xM+/FYfZnOtk6n68mH8Tf8GUHnhrGl1EELshTsEr5OiwwA==', GETDATE(), GETDATE(), GETDATE(), 1, 0),
('User3', 'user3@test.com', 'Gw/t1RQ+BTG5vWpABwZByg==', 'AGNbW18h6HM5u7bJnhO+xM+/FYfZnOtk6n68mH8Tf8GUHnhrGl1EELshTsEr5OiwwA==', GETDATE(), GETDATE(), GETDATE(), 1, 0),
('User4', 'user4@test.com', 'Gw/t1RQ+BTG5vWpABwZByg==', 'AGNbW18h6HM5u7bJnhO+xM+/FYfZnOtk6n68mH8Tf8GUHnhrGl1EELshTsEr5OiwwA==', GETDATE(), GETDATE(), GETDATE(), 1, 0),
('User5', 'user5@test.com', 'Gw/t1RQ+BTG5vWpABwZByg==', 'AGNbW18h6HM5u7bJnhO+xM+/FYfZnOtk6n68mH8Tf8GUHnhrGl1EELshTsEr5OiwwA==', GETDATE(), GETDATE(), GETDATE(), 1, 0)

GO