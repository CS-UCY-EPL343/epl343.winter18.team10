-- MySQL Script generated by MySQL Workbench
-- Mon Feb 10 19:15:18 2020
-- Model: New Model    Version: 1.0
-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema invoice
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema invoice
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `invoice` DEFAULT CHARACTER SET utf8 ;
USE `invoice` ;

-- -----------------------------------------------------
-- Table `invoice`.`Customer`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `invoice`.`Customer` ;

CREATE TABLE IF NOT EXISTS `invoice`.`Customer` (
  `idCustomer` INT NOT NULL AUTO_INCREMENT,
  `CustomerName` VARCHAR(45) NULL,
  `PhoneNumber` INT NULL,
  `Email` VARCHAR(45) NULL,
  `Country` VARCHAR(45) NULL,
  `City` VARCHAR(45) NULL,
  `Address` VARCHAR(45) NULL,
  `Balance` FLOAT NULL,
  PRIMARY KEY (`idCustomer`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `invoice`.`Product`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `invoice`.`Product` ;

CREATE TABLE IF NOT EXISTS `invoice`.`Product` (
  `idProduct` INT NOT NULL AUTO_INCREMENT,
  `ProductName` VARCHAR(45) NOT NULL,
  `Description` VARCHAR(45) NULL,
  `Stock` INT NOT NULL,
  `MinStock` INT NOT NULL,
  `Cost` FLOAT NULL,
  `SellPrice` FLOAT NULL,
  `VAT` FLOAT NULL,
  PRIMARY KEY (`idProduct`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `invoice`.`Invoice`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `invoice`.`Invoice` ;

CREATE TABLE IF NOT EXISTS `invoice`.`Invoice` (
  `idInvoice` INT NOT NULL AUTO_INCREMENT,
  `idCustomer` INT NOT NULL,
  `Cost` FLOAT NULL,
  `VAT` FLOAT NULL,
  `TotalCost` FLOAT NULL,
  `CreatedDate` DATE NULL,
  `DueDate` DATE NULL,
  `IssuedBy` VARCHAR(45) NULL,
  PRIMARY KEY (`idInvoice`),
  INDEX `fk_Invoice_Customer_idx` (`idCustomer` ASC),
  CONSTRAINT `fk_Invoice_Customer`
    FOREIGN KEY (`idCustomer`)
    REFERENCES `invoice`.`Customer` (`idCustomer`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `invoice`.`InvoiceProduct`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `invoice`.`InvoiceProduct` ;

CREATE TABLE IF NOT EXISTS `invoice`.`InvoiceProduct` (
  `idInvoice` INT NOT NULL,
  `idProduct` INT NOT NULL,
  `Quantity` INT NOT NULL,
  `Cost` FLOAT NULL,
  `VAT` FLOAT NULL,
  INDEX `fk_Invoice_has_Product_Product1_idx` (`idProduct` ASC),
  INDEX `fk_Invoice_has_Product_Invoice1_idx` (`idInvoice` ASC),
  PRIMARY KEY (`idProduct`, `idInvoice`),
  CONSTRAINT `fk_Invoice_has_Product_Invoice1`
    FOREIGN KEY (`idInvoice`)
    REFERENCES `invoice`.`Invoice` (`idInvoice`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Invoice_has_Product_Product1`
    FOREIGN KEY (`idProduct`)
    REFERENCES `invoice`.`Product` (`idProduct`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `invoice`.`CreditNote`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `invoice`.`CreditNote` ;

CREATE TABLE IF NOT EXISTS `invoice`.`CreditNote` (
  `idCreditNote` INT NOT NULL AUTO_INCREMENT,
  `idCustomer` INT NOT NULL,
  `Cost` FLOAT NULL,
  `VAT` FLOAT NULL,
  `TotalCost` FLOAT NULL,
  `Date` DATE NULL,
  PRIMARY KEY (`idCreditNote`),
  INDEX `fk_CreditNote_Customer1_idx` (`idCustomer` ASC),
  CONSTRAINT `fk_CreditNote_Customer1`
    FOREIGN KEY (`idCustomer`)
    REFERENCES `invoice`.`Customer` (`idCustomer`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `invoice`.`Order`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `invoice`.`Order` ;

CREATE TABLE IF NOT EXISTS `invoice`.`Order` (
  `idOrder` INT NOT NULL AUTO_INCREMENT,
  `idCustomer` INT NOT NULL,
  `DateIssued` DATE NULL,
  `DateDelivered` DATE NULL,
  PRIMARY KEY (`idOrder`),
  INDEX `fk_Order_Customer1_idx` (`idCustomer` ASC),
  CONSTRAINT `fk_Order_Customer1`
    FOREIGN KEY (`idCustomer`)
    REFERENCES `invoice`.`Customer` (`idCustomer`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `invoice`.`Receipt`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `invoice`.`Receipt` ;

CREATE TABLE IF NOT EXISTS `invoice`.`Receipt` (
  `idReceipt` INT NOT NULL AUTO_INCREMENT,
  `idCustomer` INT NOT NULL,
  `Amount` FLOAT NULL,
  `PaymentMethod` VARCHAR(45) NULL,
  `ChequeNumber` VARCHAR(45) NULL,
  `Bank` VARCHAR(45) NULL,
  PRIMARY KEY (`idReceipt`),
  INDEX `fk_Receipt_Customer1_idx` (`idCustomer` ASC),
  CONSTRAINT `fk_Receipt_Customer1`
    FOREIGN KEY (`idCustomer`)
    REFERENCES `invoice`.`Customer` (`idCustomer`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `invoice`.`OrderProduct`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `invoice`.`OrderProduct` ;

CREATE TABLE IF NOT EXISTS `invoice`.`OrderProduct` (
  `idOrder` INT NOT NULL,
  `idProduct` INT NOT NULL,
  `Quantity` INT NULL,
  PRIMARY KEY (`idOrder`, `idProduct`),
  INDEX `fk_Order_has_Product_Product1_idx` (`idProduct` ASC),
  INDEX `fk_Order_has_Product_Order1_idx` (`idOrder` ASC),
  CONSTRAINT `fk_Order_has_Product_Order1`
    FOREIGN KEY (`idOrder`)
    REFERENCES `invoice`.`Order` (`idOrder`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Order_has_Product_Product1`
    FOREIGN KEY (`idProduct`)
    REFERENCES `invoice`.`Product` (`idProduct`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `invoice`.`CreditNoteProduct`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `invoice`.`CreditNoteProduct` ;

CREATE TABLE IF NOT EXISTS `invoice`.`CreditNoteProduct` (
  `idCreditNote` INT NOT NULL,
  `idProduct` INT NOT NULL,
  `idInvoice` INT NOT NULL,
  `Quantity` INT NULL,
  PRIMARY KEY (`idCreditNote`, `idProduct`, `idInvoice`),
  INDEX `fk_table1_InvoiceProduct1_idx` (`idProduct` ASC, `idInvoice` ASC),
  CONSTRAINT `fk_table1_CreditNote1`
    FOREIGN KEY (`idCreditNote`)
    REFERENCES `invoice`.`CreditNote` (`idCreditNote`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_table1_InvoiceProduct1`
    FOREIGN KEY (`idProduct` , `idInvoice`)
    REFERENCES `invoice`.`InvoiceProduct` (`idProduct` , `idInvoice`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `invoice`.`Offer`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `invoice`.`Offer` ;

CREATE TABLE IF NOT EXISTS `invoice`.`Offer` (
  `idCustomer` INT NOT NULL,
  `idProduct` INT NOT NULL,
  `Price` FLOAT NULL,
  PRIMARY KEY (`idCustomer`, `idProduct`),
  INDEX `fk_Customer_has_Product_Product1_idx` (`idProduct` ASC),
  INDEX `fk_Customer_has_Product_Customer1_idx` (`idCustomer` ASC),
  CONSTRAINT `fk_Customer_has_Product_Customer1`
    FOREIGN KEY (`idCustomer`)
    REFERENCES `invoice`.`Customer` (`idCustomer`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Customer_has_Product_Product1`
    FOREIGN KEY (`idProduct`)
    REFERENCES `invoice`.`Product` (`idProduct`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

USE `invoice` ;

-- -----------------------------------------------------
-- Placeholder table for view `invoice`.`viewInvoice`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `invoice`.`viewInvoice` (`InvoiceID` INT, `CustomerName` INT, `InvoiceCost` INT, `VAT` INT, `InvoiceTotalCost` INT, `CreatedDate` INT, `ProductName` INT, `Quantity` INT, `IPCost` INT);

-- -----------------------------------------------------
-- View `invoice`.`viewInvoice`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `invoice`.`viewInvoice`;
DROP VIEW IF EXISTS `invoice`.`viewInvoice` ;
USE `invoice`;
CREATE  OR REPLACE VIEW `viewInvoice` AS SELECT 
i.idInvoice AS InvoiceID,
c.CustomerName,
i.Cost AS InvoiceCost,
i.VAT,
i.TotalCost AS InvoiceTotalCost,
i.CreatedDate,
p.ProductName,
ip.Quantity,
ip.Cost AS IPCost
FROM Invoice AS i
INNER JOIN Customer AS c ON i.idCustomer = c.idCustomer 
INNER JOIN InvoiceProduct AS ip ON i.idInvoice = ip.idInvoice 
INNER JOIN Product AS p ON ip.idProduct = p.idProduct
order by InvoiceID;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;