package htl.at.model;

import io.quarkus.hibernate.orm.panache.PanacheEntityBase;
import jakarta.persistence.*;

/**
 * Entity representing an RFID tag
 * Each person has 7 RFID tags, one for each day of the week
 */
@Entity
@Table(name = "rfid_tag")
@NamedQueries({
    @NamedQuery(name = "RfidTag.findByPersonId", query = "SELECT r FROM RfidTag r WHERE r.personId = :personId"),
    @NamedQuery(name = "RfidTag.findByDay", query = "SELECT r FROM RfidTag r WHERE r.linkedDay = :day")
})
public class RfidTag extends PanacheEntityBase {

    @Id
    @Column(name = "id")
    private String id;
    
    @Column(name = "person_id", nullable = false)
    private int personId;
    
    @Column(name = "tag", nullable = false)
    private String tag;
    
    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    private DayOfWeek linkedDay;
    
    // Constructors
    public RfidTag() {
    }
    
    public RfidTag(String id, int personId, String tag, DayOfWeek linkedDay) {
        this.id = id;
        this.personId = personId;
        this.tag = tag;
        this.linkedDay = linkedDay;
    }
    
    // Getters and setters
    public String getId() {
        return id;
    }
    
    public void setId(String id) {
        this.id = id;
    }
    
    public int getPersonId() {
        return personId;
    }
    
    public void setPersonId(int personId) {
        this.personId = personId;
    }
    
    public String getTag() {
        return tag;
    }
    
    public void setTag(String tag) {
        this.tag = tag;
    }
    
    public DayOfWeek getLinkedDay() {
        return linkedDay;
    }
    
    public void setLinkedDay(DayOfWeek linkedDay) {
        this.linkedDay = linkedDay;
    }
}
