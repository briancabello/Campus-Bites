-- BAKING INGREDIENTS
INSERT INTO Ingredients (Name, Type, Description, IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy) VALUES
('All-Purpose Flour', 'Grain', 'Versatile wheat flour for baking and cooking', 1, '2025-10-01', 'user1@example.com', 0, '2025-10-05', 'admin@example.com'),
('Granulated Sugar', 'Sweetener', 'Refined white sugar for baking and sweetening', 1, '2025-10-01', 'user2@example.com', 0, '2025-10-05', 'admin@example.com'),
('Brown Sugar', 'Sweetener', 'Moist sugar with molasses for rich flavor', 1, '2025-10-01', 'user3@example.com', 0, '2025-10-05', 'admin@example.com'),
('Baking Powder', 'Leavening', 'Chemical leavener for baked goods', 1, '2025-10-01', 'user4@example.com', 0, '2025-10-05', 'admin@example.com'),
('Baking Soda', 'Leavening', 'Sodium bicarbonate for baking', 1, '2025-10-01', 'user5@example.com', 0, '2025-10-05', 'admin@example.com'),
('Vanilla Extract', 'Flavoring', 'Pure vanilla bean extract for baking', 1, '2025-10-01', 'user6@example.com', 0, '2025-10-05', 'admin@example.com'),
('Cocoa Powder', 'Chocolate', 'Unsweetened cocoa for baking and drinks', 1, '2025-10-01', 'user7@example.com', 0, '2025-10-05', 'admin@example.com'),
('Chocolate Chips', 'Chocolate', 'Semi-sweet chocolate morsels for baking', 1, '2025-10-01', 'user8@example.com', 0, '2025-10-05', 'admin@example.com');

-- DAIRY & EGGS
INSERT INTO Ingredients (Name, Type, Description, IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy) VALUES
('Whole Milk', 'Dairy', 'Fresh whole milk for drinking and cooking', 1, '2025-10-02', 'user1@example.com', 0, '2025-10-06', 'admin@example.com'),
('Butter', 'Dairy', 'Unsalted butter for cooking and baking', 1, '2025-10-02', 'user2@example.com', 0, '2025-10-06', 'admin@example.com'),
('Eggs', 'Protein', 'Large chicken eggs for baking and cooking', 1, '2025-10-02', 'user3@example.com', 0, '2025-10-06', 'admin@example.com'),
('Heavy Cream', 'Dairy', 'High-fat cream for whipping and cooking', 1, '2025-10-02', 'user4@example.com', 0, '2025-10-06', 'admin@example.com'),
('Cream Cheese', 'Dairy', 'Soft cheese for spreads and baking', 1, '2025-10-02', 'user5@example.com', 0, '2025-10-06', 'admin@example.com'),
('Sour Cream', 'Dairy', 'Cultured cream for dips and baking', 1, '2025-10-02', 'user6@example.com', 0, '2025-10-06', 'admin@example.com'),
('Parmesan Cheese', 'Dairy', 'Hard Italian cheese for grating', 1, '2025-10-02', 'user7@example.com', 0, '2025-10-06', 'admin@example.com'),
('Mozzarella Cheese', 'Dairy', 'Soft Italian cheese for melting', 1, '2025-10-02', 'user8@example.com', 0, '2025-10-06', 'admin@example.com');

-- OILS & VINEGARS
INSERT INTO Ingredients (Name, Type, Description, IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy) VALUES
('Olive Oil', 'Oil', 'Extra virgin olive oil for cooking and dressings', 1, '2025-10-03', 'user1@example.com', 0, '2025-10-07', 'admin@example.com'),
('Vegetable Oil', 'Oil', 'Neutral oil for frying and baking', 1, '2025-10-03', 'user2@example.com', 0, '2025-10-07', 'admin@example.com'),
('Sesame Oil', 'Oil', 'Toasted sesame oil for Asian dishes', 1, '2025-10-03', 'user3@example.com', 0, '2025-10-07', 'admin@example.com'),
('Balsamic Vinegar', 'Vinegar', 'Aged vinegar for dressings and reductions', 1, '2025-10-03', 'user4@example.com', 0, '2025-10-07', 'admin@example.com'),
('Apple Cider Vinegar', 'Vinegar', 'Tangy vinegar for dressings and marinades', 1, '2025-10-03', 'user5@example.com', 0, '2025-10-07', 'admin@example.com'),
('Red Wine Vinegar', 'Vinegar', 'Sharp vinegar for Mediterranean dishes', 1, '2025-10-03', 'user6@example.com', 0, '2025-10-07', 'admin@example.com');

-- SPICES & SEASONINGS
INSERT INTO Ingredients (Name, Type, Description, IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy) VALUES
('Sea Salt', 'Spice', 'Coarse sea salt for cooking and finishing', 1, '2025-10-04', 'user1@example.com', 0, '2025-10-08', 'admin@example.com'),
('Black Pepper', 'Spice', 'Freshly ground black pepper', 1, '2025-10-04', 'user2@example.com', 0, '2025-10-08', 'admin@example.com'),
('Garlic Powder', 'Spice', 'Dehydrated garlic for seasoning', 1, '2025-10-04', 'user3@example.com', 0, '2025-10-08', 'admin@example.com'),
('Onion Powder', 'Spice', 'Dehydrated onion for seasoning', 1, '2025-10-04', 'user4@example.com', 0, '2025-10-08', 'admin@example.com'),
('Paprika', 'Spice', 'Ground red pepper for color and flavor', 1, '2025-10-04', 'user5@example.com', 0, '2025-10-08', 'admin@example.com'),
('Cumin', 'Spice', 'Warm earthy spice for Mexican and Indian dishes', 1, '2025-10-04', 'user6@example.com', 0, '2025-10-08', 'admin@example.com'),
('Cinnamon', 'Spice', 'Sweet spice for baking and drinks', 1, '2025-10-04', 'user7@example.com', 0, '2025-10-08', 'admin@example.com'),
('Nutmeg', 'Spice', 'Warm spice for baking and sauces', 1, '2025-10-04', 'user8@example.com', 0, '2025-10-08', 'admin@example.com');

-- FRUITS & VEGETABLES
INSERT INTO Ingredients (Name, Type, Description, IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy) VALUES
('Fresh Basil', 'Herb', 'Aromatic herb for Italian dishes', 1, '2025-10-05', 'user1@example.com', 0, '2025-10-09', 'admin@example.com'),
('Fresh Parsley', 'Herb', 'Bright herb for garnishes and sauces', 1, '2025-10-05', 'user2@example.com', 0, '2025-10-09', 'admin@example.com'),
('Fresh Cilantro', 'Herb', 'Pungent herb for Mexican and Asian dishes', 1, '2025-10-05', 'user3@example.com', 0, '2025-10-09', 'admin@example.com'),
('Garlic', 'Vegetable', 'Aromatic bulb for savory dishes', 1, '2025-10-05', 'user4@example.com', 0, '2025-10-09', 'admin@example.com'),
('Onion', 'Vegetable', 'Base vegetable for countless recipes', 1, '2025-10-05', 'user5@example.com', 0, '2025-10-09', 'admin@example.com'),
('Tomatoes', 'Vegetable', 'Fresh tomatoes for salads and cooking', 1, '2025-10-05', 'user6@example.com', 0, '2025-10-09', 'admin@example.com'),
('Bell Peppers', 'Vegetable', 'Sweet peppers in various colors', 1, '2025-10-05', 'user7@example.com', 0, '2025-10-09', 'admin@example.com'),
('Carrots', 'Vegetable', 'Sweet root vegetable for cooking and snacking', 1, '2025-10-05', 'user8@example.com', 0, '2025-10-09', 'admin@example.com'),
('Lemons', 'Fruit', 'Citrus fruit for juice and zest', 1, '2025-10-05', 'user9@example.com', 0, '2025-10-09', 'admin@example.com'),
('Bananas', 'Fruit', 'Sweet fruit for eating and baking', 1, '2025-10-05', 'user10@example.com', 0, '2025-10-09', 'admin@example.com');

-- MEATS & PROTEINS
INSERT INTO Ingredients (Name, Type, Description, IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy) VALUES
('Chicken Breast', 'Protein', 'Lean boneless chicken for various dishes', 1, '2025-10-06', 'user1@example.com', 0, '2025-10-10', 'admin@example.com'),
('Ground Beef', 'Protein', 'Beef for burgers, meatballs, and sauces', 1, '2025-10-06', 'user2@example.com', 0, '2025-10-10', 'admin@example.com'),
('Bacon', 'Protein', 'Cured pork for breakfast and flavoring', 1, '2025-10-06', 'user3@example.com', 0, '2025-10-10', 'admin@example.com'),
('Salmon', 'Protein', 'Fatty fish rich in omega-3', 1, '2025-10-06', 'user4@example.com', 0, '2025-10-10', 'admin@example.com'),
('Tofu', 'Protein', 'Soybean curd for vegetarian dishes', 1, '2025-10-06', 'user5@example.com', 0, '2025-10-10', 'admin@example.com');

-- GRAINS & PASTA
INSERT INTO Ingredients (Name, Type, Description, IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy) VALUES
('Spaghetti', 'Pasta', 'Long thin pasta for Italian dishes', 1, '2025-10-07', 'user1@example.com', 0, '2025-10-11', 'admin@example.com'),
('Penne', 'Pasta', 'Tube-shaped pasta for holding sauces', 1, '2025-10-07', 'user2@example.com', 0, '2025-10-11', 'admin@example.com'),
('Rice', 'Grain', 'Long grain white rice for side dishes', 1, '2025-10-07', 'user3@example.com', 0, '2025-10-11', 'admin@example.com'),
('Oats', 'Grain', 'Rolled oats for breakfast and baking', 1, '2025-10-07', 'user4@example.com', 0, '2025-10-11', 'admin@example.com'),
('Bread Crumbs', 'Grain', 'Dried bread for coating and binding', 1, '2025-10-07', 'user5@example.com', 0, '2025-10-11', 'admin@example.com');

-- PENDING APPROVAL INGREDIENTS (Some new submissions)
INSERT INTO Ingredients (Name, Type, Description, IsApproved, CreatedDate, CreatedBy, IsPendingModification) VALUES
('Truffle Oil', 'Oil', 'Infused oil with truffle aroma', 0, '2025-10-25', 'user11@example.com', 1),
('Matcha Powder', 'Tea', 'Green tea powder for drinks and baking', 0, '2025-10-25', 'user12@example.com', 1),
('Gochujang', 'Condiment', 'Korean fermented chili paste', 0, '2025-10-25', 'user13@example.com', 1),
('Tahini', 'Condiment', 'Sesame seed paste for Middle Eastern dishes', 0, '2025-10-25', 'user14@example.com', 1);

-- PENDING MODIFICATION INGREDIENTS (Approved but with update requests)
INSERT INTO Ingredients (Name, Type, Description, IsApproved, CreatedDate, CreatedBy, IsPendingModification, ApprovedDate, DecidedBy, PendingName, PendingType, PendingDescription, LastModifiedDate) VALUES
('All-Purpose Flour', 'Grain', 'Standard wheat flour', 1, '2025-10-01', 'user1@example.com', 1, '2025-10-05', 'admin@example.com', 'All-Purpose Wheat Flour', 'Grain', 'Unbleached wheat flour for all baking needs', '2025-10-26'),
('Sea Salt', 'Spice', 'Coarse sea salt', 1, '2025-10-04', 'user1@example.com', 1, '2025-10-08', 'admin@example.com', 'Fine Sea Salt', 'Spice', 'Finely ground sea salt for precise measuring', '2025-10-26');