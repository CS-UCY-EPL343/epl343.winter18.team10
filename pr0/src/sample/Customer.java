package sample;

public class Customer {
	private String name;
	private String surname;
	private double balance;
	private String email;
	private String phone;
	private String address;
	private int id;
	public Customer(){
		name=null;
		surname=null;
		balance=0;
		email=null;
		phone=null;
		address=null;
		id=0;
	}
	public Customer(int i,String n, String s, String e, String p, String a, double b){
		name=n;
		surname=s;
		email=e;
		phone=p;
		address=a;
		balance=b;
		id=i;
	}
	public String getAddress() {
		return address;
	}
	 public double getBalance() {
		return balance;
	}public String getEmail() {
		return email;
	}public String getName() {
		return name;
	}public String getPhone() {
		return phone;
	}public String getSurname() {
		return surname;
	}public void setAddress(String address) {
		this.address = address;
	}public void setBalance(double balance) {
		this.balance = balance;
	}public void setEmail(String email) {
		this.email = email;
	}public void setSurname(String surname) {
		this.surname = surname;
	}public void setName(String name) {
		this.name = name;
	}public void setPhone(String phone) {
		this.phone = phone;
	}public int getId() {
		return id;
	}public void setId(int id) {
		this.id = id;
	}
	
}
