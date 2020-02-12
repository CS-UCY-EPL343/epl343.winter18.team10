INSERT INTO `Customer` (`idCustomer`, `CustomerName`, `PhoneNumber`, `Email`, `Country`, `City`, `Address`, `Balance`) 
			VALUES (NULL, 'Foivos Panagi', 99000000, 'fnp@hotmail.com', 'Cyprus', 'Nicosia', 'Kapou', 1000),
				   (NULL, 'Chrysis Eftychiou', 99000000, 'chrysis@hotmail.com', 'Cyprus', 'Nicosia', 'Kapou', 1300);
                   
INSERT INTO `Product` (`idProduct`, `ProductName`, `Description`, `Stock`, `MinStock`, `Cost`, `SellPrice`, `VAT`) 
			VALUES (NULL, 'aporipantiko', 'kati kami', 10, 15, 3.50, 5.0, 0.19), 
				   (NULL, 'katharistiko', 'kati kami', 30, 5, 4.50, 7.0, 0.19),
				   (NULL, 'xlorini', 'kati kami', 5, 30, 4.50, 7.0, 0.19),
				   (NULL, 'tidepods', 'kati kami', 30, 5, 4.50, 7.0, 0.19);
				   
INSERT INTO `Invoice` (`idInvoice`, `idCustomer`, `Cost`, `VAT`, `TotalCost`, `CreatedDate`, `DueDate`, `IssuedBy`) 
			VALUES (NULL, 1, 50.0, 10, 60, '2020-02-05', '2020-02-10', 'Foivos'),
				   (NULL, 1, 60.0, 15, 75, '2020-02-01', '2020-02-10', 'Foivos'),
				   (NULL, 2, 60.0, 15, 75, '2020-02-01', '2020-02-10', 'Chrysis'),
				   (NULL, 2, 60.0, 15, 75, '2020-02-01', '2020-02-10', 'Giannis');			   

INSERT INTO `InvoiceProduct` (`idInvoice`, `idProduct`, `Quantity`, `Cost`, `VAT`) 
			VALUES (1, 1, 1, 50, 0.19),
				   (1, 2, 2, 50, 0.19),
				   (2, 3, 3, 50, 0.19),
				   (2, 4, 4, 50, 0.19),
				   (3, 1, 3, 50, 0.19),
				   (3, 2, 4, 50, 0.19),
				   (4, 3, 3, 50, 0.19),
				   (4, 4, 4, 50, 0.19);
				   
INSERT INTO `User` (`idUser`, `Password_p`, `AdminPrivileges`) 
			VALUES ('admin', 'pass', 1);
			