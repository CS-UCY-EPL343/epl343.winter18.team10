package sample;

import java.net.URL;
import java.util.ResourceBundle;

import com.jfoenix.controls.JFXButton;

import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.event.ActionEvent;
import javafx.event.Event;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.control.TextField;
import javafx.scene.control.cell.PropertyValueFactory;
import javafx.scene.effect.DropShadow;
import javafx.scene.layout.Pane;

public class Controller implements Initializable{

    @FXML
    private Pane receipt_pane,credit_pane,statement_pane,customer_pane,product_pane,order_pane,invoice_pane,dashboard_pane,registerpro_pane,modifypro_pane,modifysucc_pane;

    @FXML
    private JFXButton receipt_button,credit_button,statement_button,customer_button,product_button,order_button,invoice_button,dashboard_button,addProduct,chooseproduct_name_submitbutton;
    @FXML
    private Button registerpro_button,modifypro_button,addproductinvoice_button,modify_changebutton;
    @FXML private ComboBox chooseproduct_name_modify;
    DropShadow shadow = new DropShadow();
    @FXML private TextField qty_Product,select_product,value,productName,productPrice,productDescr,product_current_price_modify,product_new_price_modify;
    @FXML private TableView productTable,productTable1;
    @FXML private TableColumn productName_c,productDescr_c,productPrice_c,productName_c1,productDescr_c1,productPrice_c1;
    ObservableList<Product> data=FXCollections.observableArrayList();
	ObservableList<String> options =FXCollections.observableArrayList();
	int productIndex=-1;
    public void handleButtonAction(ActionEvent event){
        if (event.getSource() == receipt_button) {
            receipt_pane.toFront();
        }else  if (event.getSource() == credit_button) {
            credit_pane.toFront();
        }else  if (event.getSource() == statement_button) {
            statement_pane.toFront();
        }else  if (event.getSource() == customer_button) {
            customer_pane.toFront();
        }else  if (event.getSource() == product_button) {
            product_pane.toFront();
        }else  if (event.getSource() == order_button) {
            order_pane.toFront();
        }else  if (event.getSource() == invoice_button) {
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
        	for (int i=0; i<data.size();i++){
        		options.add(data.get(i).getName());
        	}
        	chooseproduct_name_modify.getItems().addAll(options);
            modifypro_pane.toFront();
       
        }else if (event.getSource()==addProduct){
       	 data.add(new Product(productName.getText(),Double.valueOf(productPrice.getText()),productDescr.getText()));
         productTable.setItems(data);
         productTable1.setItems(data);

        }else if(event.getSource()==chooseproduct_name_submitbutton){
        	for (int i=0; i<data.size(); i++){
        		if (data.get(i).getName().equals((String)chooseproduct_name_modify.getValue())){
        			productIndex=i;
        		}	
        	}
        	product_current_price_modify.setText(Double.toString(data.get(productIndex).getPrice()));
        }else if(event.getSource()==modify_changebutton){
        	if (productIndex!=-1){
        		data.get(productIndex).setPrice(Double.parseDouble(product_new_price_modify.getText()));
        		productTable.refresh();
        		productTable1.refresh();
        		modifysucc_pane.toFront();
        	}
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

    }
}
