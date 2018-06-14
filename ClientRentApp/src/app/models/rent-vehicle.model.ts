export class RentVehicle {
    name: string;
    logoImage: string;
    contactEmail: string;
    description: string;

    constructor(name: string, logoImage: string, contactEmail: string, description: string) {
        this.name = name;
        this.logoImage = logoImage;
        this.contactEmail = contactEmail;
        this.description = description;
    }
}