INSERT INTO `Customer` (`idCustomer`, `CustomerName`, `PhoneNumber`, `Email`, `Country`, `City`, `Address`, `Balance`) 
			VALUES (NULL, 'Foivos Panagi', 99000000, 'fnp@hotmail.com', 'Cyprus', 'Nicosia', 'Kapou', 1000),
				   (NULL, 'Chrysis Eftychiou', 99000000, 'chrysis@hotmail.com', 'Cyprus', 'Nicosia', 'Kapou', 1300);
                   
INSERT INTO `Product` (`idProduct`, `ProductName`, `Description`, `Stock`, `MinStock`, `Cost`, `SellPrice`, `VAT`, `Category`) 
			VALUES (NULL, 'aporipantiko', 'kati kami', 10, 15, 3.50, 5.5, 0.19, 'cat 1'), 
				   (NULL, 'katharistiko', 'kati kami', 30, 5, 4.50, 7.24, 0.19, 'cat 1'),
				   (NULL, 'xlorini', 'kati kami', 5, 30, 4.50, 7.15, 0.19, 'cat 1'),
				   (NULL, 'tidepods', 'kati kami', 30, 5, 4.50, 7.68, 0.19, 'cat 2');		

INSERT INTO `Invoice` (`idInvoice`, `idCustomer`, `Cost`, `VAT`, `TotalCost`, `CreatedDate`, `DueDate`, `IssuedBy`) 
			VALUES (NULL, '1', '62.24', '11.83', '74.07', '2020-02-29', '2020-02-29', 'chr');		

INSERT INTO `InvoiceProduct` (`idInvoice`, `idProduct`, `Quantity`, `Cost`, `VAT`) 
			VALUES ('1', '1', '10', '55', '0.19'),	
				   ('1', '2', '1', '7.24', '0.19');				

INSERT INTO `Order` (`idOrder`, `idCustomer`, `IssuedDate`, `ShippingDate`, `Cost`, `VAT`, `TotalCost`, `IssuedBy`, `Status`) 
			VALUES (NULL, '1', '2020-02-12', '2020-02-14', '10', '1.9', '11.9', 'cgr', 'Pending');

INSERT INTO `OrderProduct` (`idOrder`, `idProduct`, `Quantity`, `Cost`, `VAT`) VALUES ('1', '1', '1', '10', '0.19');

INSERT INTO `Expense` (`idExpense`, `CompanyName`, `Category`, `PhoneNumber`, `Description`, `InvoiceNo`, `CreatedDate`, `Cost`, `VAT`, `TotalCost`, `IsPaid`, `IssuedBy`) 
			VALUES (NULL, 'Company', 'Chemicals', '99454545', 'smth about smth', '456', '2020-03-21', '100', '0.19', '119', '1', 'me');

INSERT INTO `Receipt` (`idReceipt`, `idCustomer`, `Amount`, `IssuedBy`, `IssuedDate`) VALUES (NULL, '2', '250', 'me', '2020-02-27');

INSERT INTO `Payment` (`idPayment`, `idReceipt`, `PaymentMethod`, `Amount`, `PaymentNumber`, `PaymentDate`) 
			VALUES (NULL, '1', 'Cash', '150', '-1', '2020-02-26'), 
				   (NULL, '1', 'Bank', '50', '12ad34', '2020-02-26'),
				   (NULL, '1', 'Cheque', '50', '45698sdfb7', '2020-02-29');
				   
INSERT INTO `CreditNote` (`idCreditNote`, `idCustomer`, `Cost`, `VAT`, `TotalCost`, `CreatedDate`, `IssuedBy`) 
			VALUES (NULL, '1', '62.24', '11.83', '74.07', '2020-02-29', 'chr');		

INSERT INTO `CreditNoteProduct` (`idCreditNote`, `idProduct`, `idInvoice`, `Quantity`) 
			VALUES ('1', '1', '1', '5');				   
				   
INSERT INTO `Quote` (`idQuote`, `idCustomer`, `IssuedBy`, `CreatedDate`) 
			VALUES (NULL, '1', 'me', '2020-02-27');

INSERT INTO `QuoteProduct` (`idQuote`, `idProduct`, `Price`) 
			VALUES ('1', '1', 5.0);  	   

INSERT INTO `User` (`idUser`, `Hash`, `Salt`, `AdminPrivileges`) 
			VALUES ('admin', 'v4UkaqDMyrjCmnS2aFKLlUAw0XStkqp7h5Q1xNfxHfZTf0x4WEd39Q5PwTnGu2C1WgSzSmPKYmSARvzSZpX5m61dy51OjhHyTBHyYkSAHruelWbjwnQp55DoMIXVgY99Wo2ohBCKxjVqv7upyCKsXi9G3zjY04SLPcKijaWuHOZuDewyoMZTn5t03/hsmHzluTuuysIuV4DLzPZlY1BOvHoX4d84zYrdpKy34J7HKf0vOZBVonnxJxCQgJoaehDsIXdscBvyIkLC0cXUVY1IFtmolVRRelWe4oOMtUSiyC3uMpBdyzhvyNKfphqavUzCakzO4zpPDqav3BsDFONHjQ==', '6wgnx4XubS6U7n88LMMR4B397+P+XlNA9huDxytXYHAUs6mkW/+6SdWOhZF5t8aZwnkBqcMEnfICjOKIKy8EEuE1R4Pp//E4Fg8E', 1 );
			