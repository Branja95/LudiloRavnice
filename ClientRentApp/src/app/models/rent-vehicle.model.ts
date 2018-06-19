export class RentVehicle {
    name: string;
    contactEmail: string;
    description: string;

    constructor(name: string, contactEmail: string, description: string) {
        this.name = name;
        this.contactEmail = contactEmail;
        this.description = description;
    }
}