package htl.at.repository;

import htl.at.model.Person;
import io.quarkus.hibernate.orm.panache.PanacheRepository;
import jakarta.enterprise.context.ApplicationScoped;

import java.util.List;

/**
 * Repository for accessing and manipulating Person entities
 */
@ApplicationScoped
public class PersonRepository implements PanacheRepository<Person> {
    
    /**
     * Find a person by their first name
     * @param firstName The first name to search for
     * @return A list of persons with matching first name
     */
    public List<Person> findByFirstName(String firstName) {
        return list("firstName", firstName);
    }
    
    /**
     * Find a person by their phone number
     * @param phoneNumber The phone number to search for
     * @return A person with the matching phone number or null if not found
     */
    public Person findByPhoneNumber(String phoneNumber) {
        return find("phoneNumber", phoneNumber).firstResult();
    }
}
