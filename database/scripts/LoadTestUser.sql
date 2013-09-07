IF NOT EXISTS
(
	SELECT * 
	FROM [dbo].[Users] u
	WHERE u.Email = 'user0@test.com'
)
INSERT INTO [dbo].[Users]
( 
	UserName, 
	Email, 
	Password, 
	PasswordSalt, 
	JoinDate, 
	LastModifiedDate, 
	LastLoginDate, 
	IsActivated, 
	IsLockedOut
)
VALUES 
(
	'User1', 
	'user0@test.com', 
	'201BBFA8F01A5D745DC83A7262D187ED776505C0', 
	'u0tQIlnObtYKQlb9kBf7RUh2l2/EwMlkNTZmOiLIiqM=', 
	GETDATE(), 
	GETDATE(), 
	GETDATE(), 
	1, 
	0
)

GO