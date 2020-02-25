INSERT INTO `Customer` (`idCustomer`, `CustomerName`, `PhoneNumber`, `Email`, `Country`, `City`, `Address`, `Balance`) 
			VALUES (NULL, 'Foivos Panagi', 99000000, 'fnp@hotmail.com', 'Cyprus', 'Nicosia', 'Kapou', 1000),
				   (NULL, 'Chrysis Eftychiou', 99000000, 'chrysis@hotmail.com', 'Cyprus', 'Nicosia', 'Kapou', 1300);
                   
INSERT INTO `Product` (`idProduct`, `ProductName`, `Description`, `Stock`, `MinStock`, `Cost`, `SellPrice`, `VAT`, `Category`) 
			VALUES (NULL, 'aporipantiko', 'kati kami', 10, 15, 3.50, 5.5, 0.19, 'cat 1'), 
				   (NULL, 'katharistiko', 'kati kami', 30, 5, 4.50, 7.24, 0.19, 'cat 1'),
				   (NULL, 'xlorini', 'kati kami', 5, 30, 4.50, 7.15, 0.19, 'cat 1'),
				   (NULL, 'tidepods', 'kati kami', 30, 5, 4.50, 7.68, 0.19, 'cat 2');				   

			