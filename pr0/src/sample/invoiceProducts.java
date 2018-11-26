package sample;

public class invoiceProducts {
	private int quantity;
	private String product;
	private double price;
	private double total;
	public invoiceProducts(){
		quantity=0;
		product=null;
		price=0;
		total=0;
	}
	public invoiceProducts(int q, String p, double pr){
		quantity=q;
		product=p;
		price=pr;
		total=q*pr;
	}
	public void setQuantity(int quantity) {
		this.quantity = quantity;
	}public void setProduct(String product) {
		this.product = product;
	}public void setPrice(double price) {
		this.price = price;
	}public int getQuantity() {
		return quantity;
	}public String getProduct() {
		return product;
	}public double getPrice() {
		return price;
	}public double getTotal() {
		return total;
	}
}
