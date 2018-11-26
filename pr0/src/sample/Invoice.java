package sample;

import java.util.Date;
import java.util.LinkedList;
import java.util.Queue;

import javafx.collections.FXCollections;
import javafx.collections.ObservableList;

public class Invoice {
	private String name;
	private int id;
	private String billTo;
	private String date;
	public ObservableList<invoiceProducts> invoiceProducts=FXCollections.observableArrayList();
	private double total;
	private boolean isPaid;
	public Invoice(String n, int i, String b, String d){
		name=n;
		id=i;
		billTo=b;
		date=d;
	}
	public Invoice(String n, int i, String b, String d, boolean ip){
		name=n;
		id=i;
		billTo=b;
		date=d;
		isPaid=ip;
	}

	public Invoice(String name2, int id2, String billTo2, String date2, double total2, boolean paid) {
		name=name2;
		id=id2;
		billTo=billTo2;
		date=date2;
		total=total2;
		isPaid=paid;
	}
	public String getBillTo() {
		return billTo;
	}public String getDate() {
		return date;
	}public String getName() {
		return name;
	}public int getId() {
		return id;
	}public void setName(String name) {
		this.name = name;
	}public void setId(int id) {
		this.id = id;
	}public void setBillTo(String billTo) {
		this.billTo = billTo;
	}public void setDate(String date) {
		this.date = date;
	}public void addNewProduct(invoiceProducts ip) {
		this.invoiceProducts.add(ip);
	}public void setPaid(boolean isPaid) {
		this.isPaid = isPaid;
	}public boolean isPaid() {
		return isPaid;
	}public double getTotal() {
		return total;
	}
	public void setTotal(double totall) {
		total=totall;
	}
}
