TRUNCATE TABLE Ingredients;

-- ===== Vegetables =====
INSERT INTO Ingredients 
(
    Name, Type, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Carrot', 'Produce', 'Orange root vegetable, sliced or diced', 
     1, '2025-10-14', 'user2@examples.com', 0, '2025-10-18', 'admin@examples.com'
);

INSERT INTO Ingredients 
(
    Name, Type, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Green Beans', 'Produce', 'Fresh trimmed green beans', 
     1, '2025-10-15', 'user2@examples.com', 0, '2025-10-18', 'admin@examples.com'
);

INSERT INTO Ingredients 
(
    Name, Type, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Garlic', 'Produce', 'Fresh cloves, minced', 
     1, '2025-10-15', 'user2@examples.com', 0, '2025-10-18', 'admin@examples.com'
);

-- ===== Dairy Products =====
INSERT INTO Ingredients 
(
    Name, Type, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Large Eggs', 'Dairy', 'Grade A large eggs', 
     1, '2025-10-20', 'user1@examples.com', 0, '2025-10-22', 'admin@examples.com'
);

INSERT INTO Ingredients 
(
    Name, Type, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Milk', 'Dairy', '2% milk for cooking and baking', 
     1, '2025-10-20', 'user1@examples.com', 0, '2025-10-22', 'admin@examples.com'
);

INSERT INTO Ingredients 
(
    Name, Type, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Cheddar Cheese', 'Dairy', 'Shredded cheddar cheese', 
     1, '2025-10-20', 'user1@examples.com', 0, '2025-10-22', 'admin@examples.com'
);

-- ===== Bakery Items (plus pantry items used with bread) =====
INSERT INTO Ingredients 
(
    Name, Type, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Baguette', 'Bakery', 'Crusty baguette, freshly baked', 
     1, '2025-10-06', 'user3@examples.com', 0, '2025-10-09', 'admin@examples.com'
);

INSERT INTO Ingredients 
(
    Name, Type, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Butter', 'Dairy', 'Salted or unsalted butter', 
     1, '2025-10-06', 'user3@examples.com', 0, '2025-10-09', 'admin@examples.com'
);

INSERT INTO Ingredients 
(
    Name, Type, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Olive Oil', 'Pantry', 'Extra-virgin olive oil', 
     1, '2025-10-06', 'user3@examples.com', 0, '2025-10-09', 'admin@examples.com'
);

INSERT INTO Ingredients 
(
    Name, Type, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Salt', 'Pantry', 'Fine-grain salt used for seasoning', 
     1, '2025-10-06', 'user3@examples.com', 0, '2025-10-09', 'admin@examples.com'
);

INSERT INTO Ingredients 
(
    Name, Type, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Black Pepper', 'Pantry', 'Ground black pepper', 
     1, '2025-10-06', 'user3@examples.com', 0, '2025-10-09', 'admin@examples.com'
);

INSERT INTO Ingredients 
(
    Name, Type, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Green Onion', 'Produce', 'Chopped scallions', 
     1, '2025-10-20', 'user1@examples.com', 0, '2025-10-22', 'admin@examples.com'
);
