package htl.at.model;

import io.quarkus.hibernate.orm.panache.PanacheEntity;
import jakarta.persistence.*;

import java.util.ArrayList;
import java.util.List;

/**
 * Entity representing a person
 */
@Entity
@Table(name = "person")
@NamedQueries({
    @NamedQuery(name = "Person.findAll", query = "SELECT p FROM Person p ORDER BY p.firstName"),
    @NamedQuery(name = "Person.findByName", query = "SELECT p FROM Person p WHERE p.firstName LIKE :name")
})
public class Person extends PanacheEntity {

    @Column(nullable = false)
    private String firstName;
    
    private String address;
    
    private String phoneNumber;
    
    // Constructors
    public Person() {
    }
    
    public Person(String firstName, String address, String phoneNumber) {
        this.firstName = firstName;
        this.address = address;
        this.phoneNumber = phoneNumber;
    }
    
    // Getters and setters
    public String getFirstName() {
        return firstName;
    }
    
    public void setFirstName(String firstName) {
        this.firstName = firstName;
    }
    
    public String getAddress() {
        return address;
    }
    
    public void setAddress(String address) {
        this.address = address;
    }
    
    public String getPhoneNumber() {
        return phoneNumber;
    }
    
    public void setPhoneNumber(String phoneNumber) {
        this.phoneNumber = phoneNumber;
    }
}
