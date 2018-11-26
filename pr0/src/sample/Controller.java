package sample;

import java.net.URL;
import java.util.ResourceBundle;

import com.jfoenix.controls.JFXButton;
import com.jfoenix.controls.JFXComboBox;
import com.jfoenix.controls.JFXDatePicker;

import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.event.ActionEvent;
import javafx.event.Event;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.chart.LineChart;
import javafx.scene.chart.NumberAxis;
import javafx.scene.chart.PieChart;
import javafx.scene.chart.XYChart;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.control.TextField;
import javafx.scene.control.cell.PropertyValueFactory;
import javafx.scene.effect.DropShadow;
import javafx.scene.layout.Pane;
import javafx.scene.text.Text;

public class Controller implements Initializable{
	@FXML
	LineChart lineChart_Dashboard;
	@FXML
	PieChart ordersPieChart;
    @FXML
    private Pane receipt_pane,creditNote_pane,statement_pane,customer_pane,product_pane,order_pane,invoice_pane,dashboard_pane,registerpro_pane,modifypro_pane,modifysucc_pane,registerclient_pane,editClient_pane
    ,editClientSucc_pane,allInvoices_pane;

    @FXML
    private JFXButton receipt_button,credit_button,statement_button,customer_button,product_button,order_button,invoice_button,dashboard_button,addProduct,chooseproduct_name_submitbutton,editexistingcustomer_button,
    registernewcustomer_button,addnewClient_Button,submitEditClient_button,editClient_Button,viewallinvoices_button;
    
    @FXML
    private Button registerpro_button,modifypro_button,addproductinvoice_button,modify_changebutton,createInvoice_button,saveInvoice;
    
    @FXML private ComboBox chooseproduct_name_modify;
    
    DropShadow shadow = new DropShadow();
    
    @FXML private TextField qty_Product,value,productName,productPrice,productDescr,product_current_price_modify,product_new_price_modify,invoiceBillTo,clientRegisterName,clientRegisterSurname
    ,clientRegisterEmail,clientRegisterPhone,clientRegisterAddress,clientEditName,clientEditSurname,clientEditEmail,clientEditAddress,clientEdit,clientEditID,clientEditPhone;
    
    @FXML private TableView productTable,productTable1,clientTable,clientTable1,invoiceProductsTable,allInvoices_TableView;
    
    @FXML private TableColumn productName_c,productDescr_c,productPrice_c,productName_c1,productDescr_c1,productPrice_c1
    ,clientName_c,clientSurname_c,clientPhone_c,clientEmail_c,clientAddress_c,clientBalance_c,clientName_c1,clientSurname_c1,clientPhone_c1,clientEmail_c1,clientAddress_c1,clientBalance_c1,clientID_c,clientID_c1,
    productqty_c,invproductName_c,productValue_c,productTotal_c,invoiceId_c,invoiceName_c,invoiceDate_c,invoiceBillTo_c,invoiceTotal_c,invoicePaid_c;
    
    
    @FXML private JFXComboBox incoiceCustomerID_combo,select_product_combo,editClient_combo;
    
    @FXML private JFXDatePicker invoiceDate;
    @FXML private Text invoiceidText,vat,totalwovat,ttotal,vatfront,orders_front,invoicefront,receiptfront;
    
    ObservableList<Product> productsData=FXCollections.observableArrayList();
	ObservableList<String> productOptions=FXCollections.observableArrayList();
	ObservableList<Customer> clientsData=FXCollections.observableArrayList();
	ObservableList<Integer> clientOptions=FXCollections.observableArrayList();
	ObservableList<Invoice> invoiceData=FXCollections.observableArrayList();
	ObservableList<invoiceProducts> invoiceProd=FXCollections.observableArrayList();
	ObservableList<Invoice> savedInvoice=FXCollections.observableArrayList();

	int productIndex=-1;
	int clientId=1;
	int invoiceId=1;

    public void handleButtonAction(ActionEvent event){
        if (event.getSource() == receipt_button) {
            receipt_pane.toFront();
        }else  if (event.getSource() == credit_button) {
			creditNote_pane.toFront();
        }else  if (event.getSource() == statement_button) {
            statement_pane.toFront();
        }else  if (event.getSource() == customer_button) {
            customer_pane.toFront();
        }else  if (event.getSource() == product_button) {
            product_pane.toFront();
        }else  if (event.getSource() == order_button) {
            order_pane.toFront();
        }else  if (event.getSource() == invoice_button) {
        	for (int i=0; i<clientsData.size();i++){
        		clientOptions.add(clientsData.get(i).getId());
        	}
        	incoiceCustomerID_combo.getItems().addAll(clientOptions);
        	
        	for (int i=0; i<productsData.size();i++){
        		productOptions.add(productsData.get(i).getName());
        	}
        	select_product_combo.getItems().addAll(productOptions);

        	productqty_c.setCellValueFactory(new PropertyValueFactory<invoiceProducts,String>("quantity"));
        	invproductName_c.setCellValueFactory(new PropertyValueFactory<invoiceProducts,String>("product"));
        	productValue_c.setCellValueFactory(new PropertyValueFactory<invoiceProducts,Double>("price"));
        	productTotal_c.setCellValueFactory(new PropertyValueFactory<invoiceProducts,Double>("Total"));
        	
        	invoiceId_c.setCellValueFactory(new PropertyValueFactory<Invoice,String>("id"));
        	invoiceName_c.setCellValueFactory(new PropertyValueFactory<Invoice,String>("name"));
        	invoiceDate_c.setCellValueFactory(new PropertyValueFactory<Invoice,String>("date"));
        	invoiceBillTo_c.setCellValueFactory(new PropertyValueFactory<Invoice,String>("billTo"));
        	invoiceTotal_c.setCellValueFactory(new PropertyValueFactory<Invoice,String>("total"));
        	invoicePaid_c.setCellValueFactory(new PropertyValueFactory<Invoice,String>("isPaid"));

            invoice_pane.toFront();
        }else  if (event.getSource() == dashboard_button) {

            dashboard_pane.toFront();
        }else  if (event.getSource() == registerpro_button) {
    		productName_c.setCellValueFactory(new PropertyValueFactory<Product,String>("name"));
    		productDescr_c.setCellValueFactory(new PropertyValueFactory<Product,String>("description"));
    		productPrice_c.setCellValueFactory(new PropertyValueFactory<Product,String>("price"));
    		productName_c1.setCellValueFactory(new PropertyValueFactory<Product,String>("name"));
    		productDescr_c1.setCellValueFactory(new PropertyValueFactory<Product,String>("description"));
    		productPrice_c1.setCellValueFactory(new PropertyValueFactory<Product,String>("price"));
            registerpro_pane.toFront();
        }else  if (event.getSource() == modifypro_button) {
        	for (int i=0; i<productsData.size();i++){
        		productOptions.add(productsData.get(i).getName());
        	}
        	chooseproduct_name_modify.getItems().addAll(productOptions);
        	
            modifypro_pane.toFront();
       
        }else if (event.getSource()==addProduct){
        	productsData.add(new Product(productName.getText(),Double.valueOf(productPrice.getText()),productDescr.getText()));
         productTable.setItems(productsData);
         productTable1.setItems(productsData);

        }else if(event.getSource()==chooseproduct_name_submitbutton){
        	for (int i=0; i<productsData.size(); i++){
        		if (productsData.get(i).getName().equals((String)chooseproduct_name_modify.getValue())){
        			productIndex=i;
        		}	
        	}
        	product_current_price_modify.setText(Double.toString(productsData.get(productIndex).getPrice()));
        }else if(event.getSource()==modify_changebutton){
        	if (productIndex!=-1){
        		productsData.get(productIndex).setPrice(Double.parseDouble(product_new_price_modify.getText()));
        		productTable.refresh();
        		productTable1.refresh();
        		modifysucc_pane.toFront();
        	}
        }else if(event.getSource()==registernewcustomer_button){
        	clientID_c.setCellValueFactory(new PropertyValueFactory<Product,String>("id"));
        	clientID_c1.setCellValueFactory(new PropertyValueFactory<Product,String>("id"));
        	clientName_c.setCellValueFactory(new PropertyValueFactory<Product,String>("name"));
        	clientSurname_c.setCellValueFactory(new PropertyValueFactory<Product,String>("surname"));
        	clientPhone_c.setCellValueFactory(new PropertyValueFactory<Product,String>("phone"));
        	clientEmail_c.setCellValueFactory(new PropertyValueFactory<Product,String>("email"));
        	clientAddress_c.setCellValueFactory(new PropertyValueFactory<Product,String>("address"));
        	clientBalance_c.setCellValueFactory(new PropertyValueFactory<Product,String>("balance"));
        	clientName_c1.setCellValueFactory(new PropertyValueFactory<Product,String>("name"));
        	clientSurname_c1.setCellValueFactory(new PropertyValueFactory<Product,String>("surname"));
        	clientPhone_c1.setCellValueFactory(new PropertyValueFactory<Product,String>("phone"));
        	clientEmail_c1.setCellValueFactory(new PropertyValueFactory<Product,String>("email"));
        	clientAddress_c1.setCellValueFactory(new PropertyValueFactory<Product,String>("address"));
        	clientBalance_c1.setCellValueFactory(new PropertyValueFactory<Product,String>("balance"));
        	registerclient_pane.toFront();
        }else if(event.getSource()==addnewClient_Button){
        	clientsData.add(new Customer(clientId,clientRegisterName.getText(),clientRegisterSurname.getText(),clientRegisterEmail.getText(),clientRegisterPhone.getText(),clientRegisterAddress.getText(),0));
        	clientTable.setItems(clientsData);
        	clientTable1.setItems(clientsData);
        	clientId++;
        }else if(event.getSource()==editexistingcustomer_button){
        	editClient_pane.toFront();
        }else if(event.getSource()==submitEditClient_button){
        	Customer temp=clientsData.get(Integer.parseInt(clientEditID.getText())-1);
        	clientEditName.setText(temp.getName());
        	clientEditSurname.setText(temp.getSurname());
        	clientEditPhone.setText(temp.getPhone());
        	clientEditAddress.setText(temp.getAddress());
        	clientEditEmail.setText(temp.getEmail());
        }else if(event.getSource()==editClient_Button){
        	int index=Integer.parseInt(clientEditID.getText())-1;
        	clientsData.get(index).setName(clientEditName.getText());
        	clientsData.get(index).setSurname(clientEditSurname.getText());
        	clientsData.get(index).setPhone(clientEditPhone.getText());
        	clientsData.get(index).setAddress(clientEditAddress.getText());
        	clientsData.get(index).setEmail(clientEditEmail.getText());
        	clientTable.refresh();
        	clientTable1.refresh();
    		editClientSucc_pane.toFront();
        }else if(event.getSource()==createInvoice_button){
        	String name=clientsData.get((int)incoiceCustomerID_combo.getValue()-1).getName();
        	Invoice temp=new Invoice(name,(int)invoiceId,invoiceBillTo.getText(),invoiceDate.getPromptText());
        	invoiceData.add(temp);
        	invoiceidText.setText(Integer.toString((invoiceId)));
        	invoiceId++;
        }else if(event.getSource()==addproductinvoice_button){
        	int index=-1;
        	for (int i=0; i<productsData.size();i++){
        		
        		if (productsData.get(i).getName().equals(select_product_combo.getValue())){
        			index=i;
        		}
        	}
        	int tempInvoiceId=Integer.parseInt(invoiceidText.getText())-1;
        	Invoice tempInv=invoiceData.get(tempInvoiceId);
        	if (index!=-1){
        	Product temp=productsData.get(index);
        	invoiceProducts ip=new invoiceProducts();
        	if (!value.getText().isEmpty()){
        		 ip=new invoiceProducts(Integer.parseInt(qty_Product.getText()),productsData.get(index).getName(),Double.parseDouble(value.getText()));
        	}else{
        		 ip=new invoiceProducts(Integer.parseInt(qty_Product.getText()),productsData.get(index).getName(),productsData.get(index).getPrice());
        	}
        	tempInv.invoiceProducts.add(ip);
        	invoiceProductsTable.setItems(tempInv.invoiceProducts);
        	double total=0;
        	for (int i=0; i<tempInv.invoiceProducts.size();i++){
        		total=total+tempInv.invoiceProducts.get(i).getTotal();
        	}
        	totalwovat.setText(Double.toString(total));
        	vat.setText(Double.toString((total)*0.19));
        	double totall=total+(total*0.19);
        	ttotal.setText(Double.toString(totall));
        	tempInv.setTotal(totall);
        	}
        }else if(event.getSource()==viewallinvoices_button){
        	allInvoices_pane.toFront();
        }else if(event.getSource()==saveInvoice){
        	String name=clientsData.get((int)incoiceCustomerID_combo.getValue()-1).getName();
        	Invoice inv=invoiceData.get(Integer.parseInt(invoiceidText.getText())-1);
        	Invoice inv2=new Invoice(inv.getName(),inv.getId(),inv.getBillTo(),invoiceDate.getValue().toString(),inv.getTotal(),false);
        	savedInvoice.add(inv2);
        	allInvoices_TableView.setItems(savedInvoice);
        	allInvoices_TableView.refresh();
        }
        
    }
  
    public void handleMouseEntered(Event event) {
    	if (event.getSource() == receipt_button) {
            receipt_button.setStyle("-fx-background-color: rgba(230, 239, 245, .2);");
        }else  if (event.getSource() == credit_button) {
            credit_button.setStyle("-fx-background-color: rgba(230, 239, 245, .2);");
        }else  if (event.getSource() == statement_button) {
            statement_button.setStyle("-fx-background-color: rgba(230, 239, 245, .2);");
        }else  if (event.getSource() == customer_button) {
            customer_button.setStyle("-fx-background-color: rgba(230, 239, 245, .2);");
        }else  if (event.getSource() == product_button) {
            product_button.setStyle("-fx-background-color: rgba(230, 239, 245, .2);");
        }else  if (event.getSource() == order_button) {
            order_button.setStyle("-fx-background-color: rgba(230, 239, 245, .2);");
        }else  if (event.getSource() == invoice_button) {
            invoice_button.setStyle("-fx-background-color: rgba(230, 239, 245, .2);");
        }else  if (event.getSource() == dashboard_button) {
            dashboard_button.setStyle("-fx-background-color: rgba(230, 239, 245, .2);");
        }else  if (event.getSource() == registerpro_button) {
            registerpro_button.setStyle("-fx-background-color: rgba(26, 93, 136, .8);");
        }else  if (event.getSource() == modifypro_button) {
            modifypro_button.setStyle("-fx-background-color: rgba(26, 93, 136, .8);;");
        }
    }
    
    public void handleMouseExited(Event event) {
    	if (event.getSource() == receipt_button) {
            receipt_button.setStyle("-fx-background-color:transparent;");
        }else  if (event.getSource() == credit_button) {
            credit_button.setStyle("-fx-background-color:transparent;");
        }else  if (event.getSource() == statement_button) {
            statement_button.setStyle("-fx-background-color:transparent;");
        }else  if (event.getSource() == customer_button) {
            customer_button.setStyle("-fx-background-color:transparent;");
        }else  if (event.getSource() == product_button) {
            product_button.setStyle("-fx-background-color:transparent;");
        }else  if (event.getSource() == order_button) {
            order_button.setStyle("-fx-background-color:transparent;");
        }else  if (event.getSource() == invoice_button) {
            invoice_button.setStyle("-fx-background-color:transparent;");
        }else  if (event.getSource() == dashboard_button) {
            dashboard_button.setStyle("-fx-background-color:transparent;");
        }else  if (event.getSource() == registerpro_button) {
            registerpro_button.setStyle("-fx-background-color: rgba(26, 93, 136, 1);");
        }else  if (event.getSource() == modifypro_button) {
            modifypro_button.setStyle("-fx-background-color: rgba(26, 93, 136, 1);");
        }
    }
    
    
    
    @Override
    public void initialize(URL location, ResourceBundle resources) {

//defining the axes
		final NumberAxis xAxis = new NumberAxis();
		final NumberAxis yAxis = new NumberAxis();
		//creating the chart
		lineChart_Dashboard.setTitle("Sales, 2018");
		//defining a series
		XYChart.Series series = new XYChart.Series();
		//populating the series with data
		series.getData().add(new XYChart.Data("Jan", 13512));
		series.getData().add(new XYChart.Data("Feb", 11424));
		series.getData().add(new XYChart.Data("Mar", 11556));
		series.getData().add(new XYChart.Data("Apr", 12475));
		series.getData().add(new XYChart.Data("May", 13456));
		series.getData().add(new XYChart.Data("Jun", 13624));
		series.getData().add(new XYChart.Data("Jul", 14047));
		series.getData().add(new XYChart.Data("Aug", 14531));
		series.getData().add(new XYChart.Data("Sep", 14325));
		series.getData().add(new XYChart.Data("Oct", 13078));
		series.getData().add(new XYChart.Data("Nov", 12978));
		series.getData().add(new XYChart.Data("Dec", 12521));
		lineChart_Dashboard.getData().add(series);
		ordersPieChart.setTitle("Orders");

		ObservableList<PieChart.Data> pieChartData =
				FXCollections.observableArrayList(
						new PieChart.Data("Executed", 13),
						new PieChart.Data("Outstanding", 25));
		ordersPieChart.getData().addAll(pieChartData);
		vatfront.setText("€3215.27");orders_front.setText("258");invoicefront.setText("€15328.37");receiptfront.setText("€12896.65");
	}
}
