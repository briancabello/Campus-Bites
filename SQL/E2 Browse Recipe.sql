USE [campus-bites];
GO

---------- [CATEGORIES] ----------

SELECT * FROM Categories;

-- DELETE CATEGORY
-- DELETE FROM Categories WHERE ID = 1002;

-- (INDIVIDUAL) APPROVED A CATEGORIES
UPDATE Categories SET IsApproved = 1, IsPendingModification = 0 WHERE ID = 2008;

-- (ALL) APPOVED A CATEGORIES
UPDATE Categories SET IsApproved = 1, IsPendingModification = 0;

---------- [INGREDIENTS] ----------

SELECT * FROM Ingredients;

-- (INDIVIDUAL) APPROVE A INGREDIENTS
UPDATE Ingredients 
SET 
	IsApproved = 1, IsPendingModification = 0, DecidedBy = 'John.Smith@campusbites.com',
	ApprovedDate = CAST(GETDATE() AS DATE)
WHERE Id = 1079;

-- (ALL) APPROVE A INGREDIENTS
UPDATE Ingredients 
SET 
	IsApproved = 1, IsPendingModification = 0, DecidedBy = 'John.Smith@campusbites.com',
	ApprovedDate = CAST(GETDATE() AS DATE)
WHERE IsApproved = 0 AND IsPendingModification = 1;

-- (INDIVIDUAL) REQUEST
UPDATE Ingredients
SET
	Name = '', Type = '', Description = '',
	PendingName = '', PendingType = '', PendingDescription = '',
	IsApproved = 1, IsPendingModification = 0, LastModifiedDate = CAST(GETDATE() AS DATE)
WHERE Id = 1055


---------- [RECIPES] ----------

SELECT * FROM Recipes;

-- (INDIVIDUAL) Recipe -> PUBLIC
UPDATE Recipes SET Status = 'Public' WHERE ID = 2028;

-- (ALL) Recipe -> PUBLIC
UPDATE Recipes SET Status = 'Public';

SELECT * FROM Images;

-- (GROUP) APPROVED A Image
UPDATE Images SET IsApproved = 1 WHERE RecipeId = 2028;

-- (INDIVIDUAL) APPROVED A Image
UPDATE Images SET IsApproved = 1 WHERE ID = 2135

-- (ALL) APPROVED A Image
UPDATE Images SET IsApproved = 1;

---------- [CONTACTS] ----------

SELECT * FROM Contacts;