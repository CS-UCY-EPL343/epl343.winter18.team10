package sample;

public class Product {
	private String name;
	private double price;
	private String description;
	public Product(){
		this.name=null;
		this.price=0;
		this.description=null;
	}
	public Product(String n, double p, String d){
		name=n;
		price=p;
		description=d;
	}
	public String getDescription() {
		return description;
	}
	public String getName() {
		return name;
	}
	public double getPrice() {
		return price;
	}
	public void setDescription(String description) {
		this.description = description;
	}
	public void setName(String name) {
		this.name = name;
	}
	public void setPrice(double price) {
		this.price = price;
	}
}
