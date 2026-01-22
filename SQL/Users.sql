USE [campus-bites];
GO

-- DELETE AspNetUsers;
-- DELETE AspNetUserRoles;

SELECT * FROM AspNetRoles;
SELECT * FROM AspNetUsers;
SELECT * FROM AspNetUserRoles;

-- ADMINISTRATOR ROLE UPDATE
UPDATE AspNetUserRoles 
SET RoleId = '9916553d-9b60-46c1-89ba-64dde06e15b4' 
WHERE UserId = '876e080c-236c-4ce5-ae11-6b445cfd9504';