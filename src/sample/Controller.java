package sample;

import java.net.URL;
import java.util.ResourceBundle;

import com.jfoenix.controls.JFXButton;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Label;
import javafx.scene.layout.Pane;

public class Controller implements Initializable{

    @FXML
    private Pane receipt_pane,credit_pane,statement_pane,customer_pane,product_pane,order_pane,invoice_pane,dashboard_pane;

    @FXML
    private JFXButton receipt_button,credit_button,statement_button,customer_button,product_button,order_button,invoice_button,dashboard_button;


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
        }


    }

    @Override
    public void initialize(URL location, ResourceBundle resources) {

    }
}
