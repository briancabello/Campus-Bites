-- Approved #1
INSERT INTO Categories 
(
    Name, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Vegetables', 'Fresh vegetables including leafy greens, root vegetables, and seasonal produce', 
     1, '2025-10-14', 'user2@examples.com', 0, '2025-10-19', 'admin@examples.com'
);

-- Approved #2
INSERT INTO Categories 
(
    Name, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Dairy Products', 'All types of dairy products including milk, cheese, yogurt, and butter', 
     1, '2025-10-20', 'user1@examples.com', 0, '2025-10-22', 'admin@example.com'
);

-- Approved #3
INSERT INTO Categories 
(
    Name, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy
)
VALUES 
(
    'Bakery Items', 'Freshly baked breads, pastries, and desserts', 
     1, '2025-10-05', 'user3@examples.com', 0, '2025-10-10', 'admin@example.com'
);

-- (Optional) Pending Approval example (note: no ApprovedDate/DecidedBy here)
INSERT INTO Categories 
(
    Name, Description, 
    IsApproved, CreatedDate, CreatedBy, IsPendingModification
)
VALUES 
(
    'Herbs & Spices', 'Common herbs and spices for seasoning', 
     0, '2025-10-22', 'user4@examples.com', 1
);
