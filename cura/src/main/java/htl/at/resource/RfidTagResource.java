package htl.at.resource;

import htl.at.model.Person;
import htl.at.model.RfidTag;
import htl.at.repository.PersonRepository;
import htl.at.repository.RfidTagRepository;
import jakarta.inject.Inject;
import jakarta.transaction.Transactional;
import jakarta.ws.rs.*;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;
import java.net.URI;
import java.util.List;

/**
 * REST resource for managing RfidTag entities
 */
@Path("/api/rfidtags")
@Produces(MediaType.APPLICATION_JSON)
@Consumes(MediaType.APPLICATION_JSON)
public class RfidTagResource {

    @Inject
    private RfidTagRepository rfidTagRepository;
    
    @Inject
    private PersonRepository personRepository;

    /**
     * Get all RFID tags
     * @return List of all RFID tags
     */
    @GET
    public List<RfidTag> getAllTags() {
        return rfidTagRepository.listAll();
    }

    /**
     * Get RFID tag by ID
     * @param id The RFID tag ID
     * @return The RFID tag with the given ID
     */
    @GET
    @Path("/{id}")
    public RfidTag getTagById(@PathParam("id") String id) {
        RfidTag tag = rfidTagRepository.findByTagId(id);
        if (tag == null) {
            throw new NotFoundException("RFID tag with id " + id + " not found");
        }
        return tag;
    }
    
    /**
     * Get all RFID tags for a specific person
     * @param personId The person ID
     * @return List of RFID tags belonging to the person
     */
    @GET
    @Path("/person/{personId}")
    public List<RfidTag> getTagsByPerson(@PathParam("personId") Long personId) {
        return rfidTagRepository.findByPersonId(personId.intValue());
    }

    /**
     * Create a new RFID tag
     * @param personId The ID of the person to associate with the tag
     * @param tagRequest The tag information in the request body
     * @return Response with the created tag
     */
    @POST
    @Path("/person/{personId}")
    @Transactional
    public Response createTag(
            @PathParam("personId") Long personId,
            RfidTag tagRequest) {
        
        // Setze die personId für das neue Tag
        tagRequest.setPersonId(personId.intValue());
        
        // Validiere die Eingabedaten
        if (tagRequest.getId() == null || tagRequest.getId().isEmpty()) {
            throw new BadRequestException("Tag ID must not be empty");
        }
        
        if (tagRequest.getTag() == null || tagRequest.getTag().isEmpty()) {
            throw new BadRequestException("Tag value must not be empty");
        }
        
        if (tagRequest.getLinkedDay() == null) {
            throw new BadRequestException("Day must not be empty");
        }
        
        // Speichere das Tag in der Datenbank
        rfidTagRepository.persist(tagRequest);
        
        return Response.created(URI.create("/api/rfidtags/" + tagRequest.getId())).entity(tagRequest).build();
    }

    /**
     * Update an existing RFID tag
     * @param id The ID of the RFID tag to update
     * @param updatedTag The updated RFID tag data
     * @return The updated RFID tag
     */
    @PUT
    @Path("/{id}")
    @Transactional
    public RfidTag updateTag(@PathParam("id") String id, RfidTag updatedTag) {
        RfidTag tag = rfidTagRepository.findByTagId(id);
        if (tag == null) {
            throw new NotFoundException("RFID tag with id " + id + " not found");
        }
        
        // Update the tag properties
        tag.setLinkedDay(updatedTag.getLinkedDay());
        
        return tag;
    }

    /**
     * Delete an RFID tag
     * @param id The ID of the RFID tag to delete
     * @return Response indicating success
     */
    @DELETE
    @Path("/{id}")
    @Transactional
    public Response deleteTag(@PathParam("id") String id) {
        RfidTag tag = rfidTagRepository.findByTagId(id);
        if (tag == null) {
            throw new NotFoundException("RFID tag with id " + id + " not found");
        }
        
        rfidTagRepository.delete(tag);
        return Response.noContent().build();
    }
}
