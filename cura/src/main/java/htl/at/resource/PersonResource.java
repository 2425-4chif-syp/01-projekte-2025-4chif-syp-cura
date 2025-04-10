package htl.at.resource;

import htl.at.model.Person;
import htl.at.repository.PersonRepository;
import jakarta.inject.Inject;
import jakarta.transaction.Transactional;
import jakarta.ws.rs.*;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;
import java.net.URI;
import java.util.List;

/**
 * REST resource for managing Person entities
 */
@Path("/api/persons")
@Produces(MediaType.APPLICATION_JSON)
@Consumes(MediaType.APPLICATION_JSON)
public class PersonResource {

    @Inject
    private PersonRepository personRepository;

    /**
     * Get all persons
     * @return List of all persons
     */
    @GET
    public List<Person> getAllPersons() {
        return personRepository.listAll();
    }

    /**
     * Get person by ID
     * @param id The person ID
     * @return The person with the given ID
     */
    @GET
    @Path("/{id}")
    public Person getPersonById(@PathParam("id") Long id) {
        Person person = personRepository.findById(id);
        if (person == null) {
            throw new NotFoundException("Person with id " + id + " not found");
        }
        return person;
    }

    /**
     * Create a new person
     * @param person The person to create
     * @return Response with the created person
     */
    @POST
    @Transactional
    public Response createPerson(Person person) {
        // Setze die ID auf null, damit Hibernate eine neue ID generiert
        person.id = null;
        personRepository.persist(person);
        return Response.created(URI.create("/api/persons/" + person.id)).entity(person).build();
    }

    /**
     * Update an existing person
     * @param id The ID of the person to update
     * @param updatedPerson The updated person data
     * @return The updated person
     */
    @PUT
    @Path("/{id}")
    @Transactional
    public Person updatePerson(@PathParam("id") Long id, Person updatedPerson) {
        Person person = personRepository.findById(id);
        if (person == null) {
            throw new NotFoundException("Person with id " + id + " not found");
        }
        
        // Update the person properties
        person.setFirstName(updatedPerson.getFirstName());
        person.setAddress(updatedPerson.getAddress());
        person.setPhoneNumber(updatedPerson.getPhoneNumber());
        
        return person;
    }

    /**
     * Delete a person
     * @param id The ID of the person to delete
     * @return Response indicating success
     */
    @DELETE
    @Path("/{id}")
    @Transactional
    public Response deletePerson(@PathParam("id") Long id) {
        Person person = personRepository.findById(id);
        if (person == null) {
            throw new NotFoundException("Person with id " + id + " not found");
        }
        
        personRepository.delete(person);
        return Response.noContent().build();
    }
}
