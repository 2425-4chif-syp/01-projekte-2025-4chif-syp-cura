package htl.at.repository;

import htl.at.model.DayOfWeek;
import htl.at.model.Person;
import htl.at.model.RfidTag;
import io.quarkus.hibernate.orm.panache.PanacheRepository;
import jakarta.enterprise.context.ApplicationScoped;

import java.util.List;

/**
 * Repository for accessing and manipulating RfidTag entities
 */
@ApplicationScoped
public class RfidTagRepository implements PanacheRepository<RfidTag> {
    
    /**
     * Find RFID tags by person ID
     * @param personId The ID of the person whose tags to find
     * @return A list of RFID tags belonging to the person
     */
    public List<RfidTag> findByPersonId(int personId) {
        return list("personId", personId);
    }
    
    /**
     * Find RFID tags by day of week
     * @param day The day of week
     * @return A list of RFID tags linked to the specific day
     */
    public List<RfidTag> findByDay(DayOfWeek day) {
        return list("linkedDay", day);
    }
    
    /**
     * Find an RFID tag by its ID
     * @param id The RFID tag ID
     * @return The RFID tag with the matching ID or null if not found
     */
    public RfidTag findByTagId(String id) {
        return find("id", id).firstResult();
    }
}
