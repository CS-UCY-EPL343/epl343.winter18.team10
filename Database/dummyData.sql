INSERT INTO `Customer` (`idCustomer`, `CustomerName`, `PhoneNumber`, `Email`, `Country`, `City`, `Address`, `Balance`) 
			VALUES (NULL, 'Foivos Panagi', 99000000, 'fnp@hotmail.com', 'Cyprus', 'Nicosia', 'Kapou', 0.0),
				   (NULL, 'Chrysis Eftychiou', 99000000, 'chrysis@hotmail.com', 'Cyprus', 'Nicosia', 'Kapou', 0.0);
                   
INSERT INTO `Product` (`idProduct`, `ProductName`, `Description`, `Stock`, `MinStock`, `Cost`, `SellPrice`) 
			VALUES (NULL, 'aporipantiko', 'kati kami', 15, 3, 3.50, 5.0), 
				   (NULL, 'katharistiko', 'kati kami', 30, 5, 4.50, 7.0),
				   (NULL, 'xlorini', 'kati kami', 30, 5, 4.50, 7.0),
				   (NULL, 'tidepods', 'kati kami', 30, 5, 4.50, 7.0);
				   
INSERT INTO `Invoice` (`idInvoice`, `idCustomer`, `Cost`, `VAT`, `TotalCost`, `Date`) 
			VALUES (NULL, 1, 50, 10, 60, '2020-02-05'),
				   (NULL, 1, 60, 15, 75, '2020-02-01'),
				   (NULL, 2, 60, 15, 75, '2020-02-01'),
				   (NULL, 2, 60, 15, 75, '2020-02-01');			   

INSERT INTO `InvoiceProduct` (`idInvoice`, `idProduct`, `Quantity`, `Cost`) 
			VALUES (1, 1, 1, 50),
				   (1, 2, 2, 50),
				   (2, 3, 3, 50),
				   (2, 4, 4, 50),
				   (3, 1, 3, 50),
				   (3, 2, 4, 50),
				   (4, 3, 3, 50),
				   (4, 4, 4, 50);